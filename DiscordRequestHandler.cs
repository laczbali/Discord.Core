using Discord.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Discord.Core.Utils.ED25519;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord.Core.Utils;
using System.Net.Http;

namespace Discord.Core
{
    /// <summary>
    /// Use this class to handle the interaction subscription validations and requests from Discord
    /// </summary>
    public class DiscordRequestHandler
    {
        private readonly DiscordClientConfig config;
        private readonly DiscordInteractionsBase interactions;
        private readonly ILogger<DiscordRequestHandler> logger;

        public string ErrorMessage = "Something went wrong :frowning2: Please try again later";

        public DiscordRequestHandler(DiscordClientConfig config, DiscordInteractionsBase interactions, ILogger<DiscordRequestHandler> logger)
        {
            this.config = config;
            this.interactions = interactions;
            this.logger = logger;
        }

		/// <summary>
		/// Handles the HTTP interaction request sent by Discord <br/>
		/// - Performs the subscription validation <br/>
		/// - Calls the appropriate interaction handler
		/// - Returns the result <br/>
		/// </summary>
		public async Task<IActionResult> ImmediateHandlerAsync(HttpRequest request)
        {
			var validationResult = await ValidationAsync(request);
			if (validationResult.validationResponse != null)
			{
				return validationResult.validationResponse;
			}
			var requestData = validationResult.requestData;

			string response;
			try
			{
				response = await this.interactions.HandleInteraction(requestData);
			}
			catch (Exception e)
			{
				logger?.LogError(e, $"Failed to handle interaction. Request:\n{requestData}");
				response = this.ErrorMessage;
			}

			return new OkObjectResult(new
			{
				type = InteractionCallBackType.CHANNEL_MESSAGE_WITH_SOURCE,
				data = new
				{
					content = response
				}
			});
		}

        /// <summary>
        /// Handles the HTTP interaction request sent by Discord <br/>
        /// - Performs the subscription validation <br/>
        /// - Returns a deferred response (bot is thinking)
        /// - Calls the appropriate interaction handler
        /// - Updates the bot message with the result <br/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeferredHandlerAsync(HttpRequest request)
        {
            var validationResult = await ValidationAsync(request);
            if (validationResult.validationResponse != null)
            {
                return validationResult.validationResponse;
            }
            var requestData = validationResult.requestData;

			// deal with interaction in the background
			_ = Task.Run(async () =>
            {
				string response;
				try
				{
					response = await this.interactions.HandleInteraction(requestData);
				}
				catch (Exception e)
				{
					logger?.LogError(e, $"Failed to handle interaction. Request:\n{requestData}");
					response = this.ErrorMessage;
				}

                try
                {
					await this.MakeFollowupResponse(requestData.Token, response);
				}
                catch(Exception e)
                {
                    logger?.LogError(e, "Failed to make followup response");
                }
			});

            // respond immediately with -> I'm thinking, will respond later
            return new OkObjectResult(new
            {
                type = InteractionCallBackType.DEFERRED_CHANNEL_MESSAGE_WITH_SOURCE
			});
        }

        private async Task<(Interaction requestData,IActionResult validationResponse)> ValidationAsync(HttpRequest request)
        {
            // deserialize request
            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            Interaction requestData;
            try
            {
                requestData = JsonConvert.DeserializeObject<Interaction>(requestBody);
            }
            catch (Exception e)
            {
                logger?.LogError(e, $"Deserialization failure. Request:\n{requestBody}");
                return (null, new BadRequestResult());
            }

            // handle verification flow
            try
            {
                if (!this.InteractionRequestIsValid(request.Headers, requestBody))
                {
                    logger?.LogWarning("Invalid interaction request");
                    return (null, new UnauthorizedResult());
                }
            }
            catch (Exception e)
            {
                logger?.LogError(e, $"Validation failure. Request:\n{requestData}\nHeaders:\n{request.Headers}");
                return (null, new UnauthorizedResult());
            }

            // respond to handshake request
            if (requestData.Type == InteractionType.PING)
            {
                return (null, new OkObjectResult(new { type = InteractionCallBackType.PONG }));
            }

            // this was not a validation request
            return (requestData, null);
        }

        private async Task MakeFollowupResponse(string interactionToken, string followupContent)
        {
            var messageContent = new
            {
                content = followupContent,
            };

			var url = $"{this.config.ApiBaseUrl}/webhooks/{this.config.ApplicationId}/{interactionToken}";
			var httpClient = new HttpClientWrapper();
            await httpClient.MakeRequestAsync(url, HttpMethod.Post, messageContent, "Bot", this.config.BotToken);
		}

        private bool InteractionRequestIsValid(IHeaderDictionary requestHeaders, string requestBody)
        {
            var signatureHeader = requestHeaders["X-Signature-Ed25519"].FirstOrDefault();
            var timestampHeader = requestHeaders["X-Signature-Timestamp"].FirstOrDefault();

            if (signatureHeader == null || timestampHeader == null) { return false; }

            var key = HexConverter.HexToByteArray(this.config.PublicKey);
            var signature = HexConverter.HexToByteArray(signatureHeader);
            var timestamp = Encoding.UTF8.GetBytes(timestampHeader);
            var body = Encoding.UTF8.GetBytes(requestBody);

            var message = new List<byte>();
            message.AddRange(timestamp);
            message.AddRange(body);

            return Ed25519.Verify(signature, message.ToArray(), key);
        }
    }
}

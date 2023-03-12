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

namespace Discord.Core
{
    /// <summary>
    /// Use this class to handle the interaction subscription validations and requests from Discord
    /// </summary>
    public class RequestHandler
    {
        private readonly ClientConfig config;
        private readonly InteractionsBase interactions;
        private readonly ILogger<RequestHandler> logger;

        public string ErrorMessage = "Something went wrong :frowning2: Please try again later";

        public RequestHandler(ClientConfig config, InteractionsBase interactions, ILogger<RequestHandler> logger = null)
        {
            this.config = config;
            this.interactions = interactions;
            this.logger = logger;
        }

        /// <summary>
        /// Handles the HTTP interaction request sent by Discord <br/>
        /// - Performs the subscription validation <br/>
        /// - Calls the appropriate interaction handler, and returns the result <br/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IActionResult> HandlerAsync(HttpRequest request)
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
                return new BadRequestResult();
            }

            // handle verification flow
            try
            {
                if (!this.InteractionRequestIsValid(request.Headers, requestBody))
                {
                    logger?.LogWarning("Invalid interaction request");
                    return new UnauthorizedResult();
                }
            }
            catch (Exception e)
            {
                logger?.LogError(e, $"Validation failure. Request:\n{requestData}\nHeaders:\n{request.Headers}");
                return new UnauthorizedResult();
            }

            if (requestData.Type == InteractionType.PING)
            {
                return new OkObjectResult(new { type = InteractionCallBackType.PONG });
            }

            // handle interaction
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

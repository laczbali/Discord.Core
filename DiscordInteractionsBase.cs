﻿using Discord.Core.Models;
using Discord.Core.Utils;
using Blaczko.Core.Wrappers;

namespace Discord.Core
{
	/// <summary>
	/// Register an implementation of this class as a service,<br/>
	/// so that you can use <see cref="DiscordRequestHandler"/><br/>
	/// to deal with Discords interaction requests
	/// </summary>
	public abstract class DiscordInteractionsBase
    {
        private readonly DiscordClientConfig config;

        public DiscordInteractionsBase(DiscordClientConfig config)
        {
            this.config = config;
        }

        /// <summary>
		/// Returns the list of predefined global interactions
		/// </summary>
		/// <returns></returns>
        public abstract ApplicationCommand[] GlobalCommands { get; }

        /// <summary>
		/// Matches the provided interaction to a defined Global Command, and then calls its InteractionHandler function
		/// </summary>
		/// <param name="interaction"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task<string> HandleInteraction(Interaction interaction)
        {
            var registeredCommand = this.GlobalCommands
                .FirstOrDefault(c => c.Name == interaction.Data.Name);

            if (registeredCommand == null)
            {
                throw new Exception($"Failed to find mathcing Global Command to interaction named [{interaction.Data.Name}]");
            }

            if (registeredCommand.InteractionHandler == null)
            {
                throw new Exception($"No interaction handler defined for [{registeredCommand.Name}]");
            }

            return await registeredCommand.InteractionHandler(interaction);
        }

        /// <summary>
        /// Registers all commands specified in <see cref="GlobalCommands"/>
        /// </summary>
        /// <returns></returns>
        public async Task RegisterGlobalCommands()
        {
            foreach (var item in this.GlobalCommands)
            {
                await this.RegisterGlobalCommand(item);
            }
        }

        /// <summary>
        /// Registers the specified command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task RegisterGlobalCommand(ApplicationCommand command)
        {
            var url = $"{this.config.ApiBaseUrl}/applications/{this.config.ClientId}/commands";

            await HttpClientWrapper.MakeRequestAsync(url, HttpMethod.Post, command.ToHttpJsonContent(), "Bot", this.config.BotToken);
        }
    }
}

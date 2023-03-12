using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Discord.Core.Models
{
	/// <summary>
	/// See more at https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-structure
	/// </summary>
	public class ApplicationCommand
	{
		public string Id { get; set; }
		public ApplicationCommandType Type { get; set; }
		public string ApplicationId { get; set; }
		public string GuildId { get; set; }
		public string Name { get; set; }
		public object NameLocalizations { get; set; }
		public string Description { get; set; }
		public object DescriptionLocalizations { get; set; }
		public ApplicationCommandOption[] Options { get; set; }
		public string DefaultMemberPermissions { get; set; }
		public bool DmPermission { get; set; }
		public bool DefaultPermission { get; set; }
		public bool Nsfw { get; set; }
		public string Version { get; set; }

		[JsonIgnore]
		public Func<Interaction, Task<string>> InteractionHandler { get; set; }
	}
}

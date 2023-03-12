namespace Discord.Core.Models
{
	/// <summary>
	/// See more at: https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-application-command-interaction-data-option-structure
	/// </summary>
	public class ApplicationCommandInteractionDataOption
	{
		public string Name { get; set; }
		public ApplicationCommandOptionType Type { get; set; }
		public object Value { get; set; }
		public ApplicationCommandInteractionDataOption[] Options { get; set; }
		public bool Focused { get; set; }
	}
}

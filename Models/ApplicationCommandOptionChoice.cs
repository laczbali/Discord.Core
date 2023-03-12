namespace Discord.Core.Models
{
	/// <summary>
	/// See more at: https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-option-choice-structure
	/// </summary>
	public class ApplicationCommandOptionChoice
	{
		public string Name { get; set; }
		public object NameLocalizations { get; set; }
		public object Value { get; set; }
	}
}

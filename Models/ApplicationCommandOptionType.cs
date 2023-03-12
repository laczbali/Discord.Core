namespace Discord.Core.Models
{
	/// <summary>
	/// See more at: https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-option-type
	/// </summary>
	public enum ApplicationCommandOptionType
	{
		SUB_COMMAND = 1,
		SUB_COMMAND_GROUP = 2,
		STRING = 3,
		INTEGER = 4,
		BOOLEAN = 5,
		USER = 6,
		CHANNEL = 7,
		ROLE = 8,
		MENTIONABLE = 9,
		NUMBER = 10,
		ATTACHMENT = 11
	}
}

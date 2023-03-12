namespace Discord.Core.Models
{
	/// <summary>
	/// See more at: https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-interaction-type
	/// </summary>
	public enum InteractionType
	{
		PING = 1,
		APPLICATION_COMMAND = 2,
		MESSAGE_COMPONENT = 3,
		APPLICATION_COMMAND_AUTOCOMPLETE = 4,
		MODAL_SUBMIT = 5
	}
}

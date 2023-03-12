namespace Discord.Core.Models
{
	/// <summary>
	/// See more at: https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-interaction-callback-type
	/// </summary>
	public enum InteractionCallBackType
	{
		PONG = 1,
		CHANNEL_MESSAGE_WITH_SOURCE = 4,
		DEFERRED_CHANNEL_MESSAGE_WITH_SOURCE = 5,
		DEFERRED_UPDATE_MESSAGE = 6,
		UPDATE_MESSAGE = 7,
		APPLICATION_COMMAND_AUTOCOMPLETE_RESULT = 8,
		MODAL = 9,
	}
}

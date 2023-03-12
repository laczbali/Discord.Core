namespace Discord.Core.Models
{
	/// <summary>
	/// See more at: https://discord.com/developers/docs/resources/channel#message-object
	/// </summary>
	public class Message
	{
		public string Id { get; set; }
		public string ChannelId { get; set; }
		public User Author { get; set; }
		public string Content { get; set; }
		public string Timestamp { get; set; }
		public string EditedTimestamp { get; set; }
		public bool Tts { get; set; }
		public bool MentionEveryone { get; set; }
		public User[] Mentions { get; set; }
		public string[] MentionRoles { get; set; }
		public object[] MentionChannels { get; set; }
		public object[] Attachments { get; set; }
		public object[] Embeds { get; set; }
		public object[] Reactions { get; set; }
		public object Nonce { get; set; }
		public bool Pinned { get; set; }
		public string WebhookId { get; set; }
		public MessageType Type { get; set; }
		public object Activity { get; set; }
		public object Application { get; set; }
		public string ApplicationId { get; set; }
		public object MessageReference { get; set; }
		public int Flags { get; set; }
		public Message ReferencedMessage { get; set; }
		public object Interaction { get; set; }
		public object Thread { get; set; }
		public object[] Components { get; set; }
		public object[] StickerItems { get; set; }
		public object[] Stickers { get; set; }
		public int Position { get; set; }
		public object RoleSubscriptionData { get; set; }
	}
}

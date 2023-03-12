namespace Discord.Core.Models
{
	/// <summary>
	/// See more at: https://discord.com/developers/docs/resources/guild#guild-member-object
	/// </summary>
	public class GuildMember
	{
		public User User { get; set; }
		public string Nick { get; set; }
		public string Avatar { get; set; }
		public string[] Roles { get; set; }
		public string JoinedAt { get; set; }
		public string PremiumSince { get; set; }
		public bool Deaf { get; set; }
		public bool Mute { get; set; }
		public int Flags { get; set; }
		public bool Pending { get; set; }
		public string Permissions { get; set; }
		public string CommunicationDisabledUntil { get; set; }
	}
}

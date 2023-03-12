using System.Linq;
using System;

namespace Discord.Core.Models
{
	/// <summary>
	/// See more at: https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object
	/// </summary>
	public class Interaction
	{
		public string Id { get; set; }
		public string ApplicationId { get; set; }
		public InteractionType Type { get; set; }
		public InteractionData Data { get; set; }
		public string GuildId { get; set; }
		public GuildMember Member { get; set; }
		public User User { get; set; }
		public string Token { get; set; }
		public int Version { get; set; }
		public Message Message { get; set; }
		public string AppPermissions { get; set; }
		public string Locale { get; set; }
		public string GuildLocale { get; set; }

        /// <summary>
        /// Gets the value of the desired interaction option
        /// </summary>
        /// <typeparam name="OptionType"></typeparam>
        /// <param name="optionName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public OptionType GetOptionValue<OptionType>(string optionName)
        {
            var option = this.Data.Options.FirstOrDefault(o => o.Name == optionName);
            if (option == null)
            {
                throw new ArgumentException($"Cannot find [{optionName}]");
            }

            return (OptionType)option.Value;
        }
    }
}

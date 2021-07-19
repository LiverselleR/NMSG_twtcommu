using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NMSGDiscordBot
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
               => ReplyAsync(echo);
    }

	[Group("umamusume")]
    public class UmamusumeModule : ModuleBase<SocketCommandContext>
    {

		[Command("userinfo")]
		[Summary
		("Returns info about the current user, or the user parameter, if one passed.")]
		[Alias("user", "whois")]
		public async Task UserInfoAsync(
			[Summary("The (optional) user to get info from")]
		SocketUser user = null)
		{
			var userInfo = user ?? Context.Client.CurrentUser;
			await ReplyAsync($"{userInfo.Username}#{userInfo.Id}");
		}

		[Command("DerbyTest")]
		[Summary("Make Test File of Derby data")]
		public async Task DerbyTestAsync()
        {
			List<Derby> derbyList = new List<Derby>();
			Derby testDerby = new Derby();
			await testDerby.TestDerby();
			await ReplyAsync("Test Derby Complete") ;
        }

		

	}
}

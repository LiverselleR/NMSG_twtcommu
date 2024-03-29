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
    [Group("Test")]
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
            var userInfo = user ?? Context.User;
            await ReplyAsync($"{userInfo.Username}#{userInfo.Id}");
        }

        [Command("JsonTest")]
        [Summary("Test Json")]
        public async Task JsonTestAsync()
        {
            List<Umamusume> uList = JSONManager.GetUmamusumeList();

            Console.WriteLine(uList[0].ToString());
            await ReplyAsync(uList[0].ToString());
            await ReplyAsync("JObject Test Complete");
        }

        [Command("RegisterTest")]
        [Summary("Umamusume Register Test")]
        public async Task RegisterTestAsync(
            [Summary("Umamusume name to register user")]
        String uName = null)
        {
            List<Umamusume> uList = JSONManager.GetUmamusumeList();

            try
            {
                UmamusumeManager.Register(Context.User.Id, uName, ref uList);
                JSONManager.SetUmamusumeList(uList);
                await ReplyAsync("Register Comlete");
            }
            catch (UmamusumeNameNotFoundException e)
            {
                await ReplyAsync("Register Resigned : Umamusume name Incorrect");
            }
            catch (UmamusumeAlreadyRegisteredException e)
            {
                await ReplyAsync("Register Resigned : Umamusume Already Registered");
            }
            catch (DiscordIdAlreadyRegisteredException e)
            {
                await ReplyAsync("Register Resigned : User Already Registered");
            }

        }

        [Command("DerbyTest")]
        [Summary("Make Test File of Derby data")]
        public async Task DerbyTestAsync()
        {
            List<String> derbyCast = Derby.TestDerby_Process();

            if (derbyCast.Count <= 0)
            {
                await ReplyAsync("derbyCast is Empty. Test End.");
                return;
            }

            await ReplyAsync(derbyCast.Last<String>());

            /*
			foreach(String str in derbyCast)
            {
				await ReplyAsync(str);
				await Task.Delay(2000);
            }
			*/
            await ReplyAsync("Test Derby Complete");
        }
    }
}
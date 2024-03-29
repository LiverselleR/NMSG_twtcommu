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
        [Command("정보")]
        [Summary("정보 조회")]
        public async Task InfoAsync(String name = null)
        {
            List<Umamusume> uList = JSONManager.GetUmamusumeList();
            List<Trainer> tList = JSONManager.GetTrainerList();
            Umamusume u;
            Trainer t;

            if (name == null)
                u = uList.Find(u => u.ownerID == Context.User.Id);
            else
                u = uList.Find(u => u.name == name);
            if (u == null)
            {
                if (name == null)
                    t = tList.Find(t => t.ownerID == Context.User.Id);
                else
                    t = tList.Find(t => t.name == name);
                if (t == null)
                {
                    if (name == null)
                        await ReplyAsync("오류 : 당신은 현재 등록 절차가 진행되지 않았습니다. 우마무스메나 트레이너 등록 이후 정보 조회가 가능합니다.");
                    else
                        await ReplyAsync("오류 : 해당 이름은 우마무스메 / 트레이너 데이터 베이스에 등록되어 있지 않습니다. ");
                    return;
                }
                else
                    await ReplyAsync(t.ToString());
            }
            else
                await ReplyAsync(u.ToString());

        }
    }
}
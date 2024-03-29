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
    [Group("더비")]
    public class DerbyModule : ModuleBase<SocketCommandContext>
    {
        [Command("참가")]
        [Summary
            ("더비 참가 기능")]
        public async Task DerbyEntryAsync(String derbyName, String runningStyleString)
        {
            try
            {
                RaceEntryManager.Entry(Context.User.Id, derbyName, runningStyleString);
                await ReplyAsync("더비 등록 완료! 귀하의 건승을 기원합니다!");
            }
            catch (DiscordIDNotRegisteredException)
            {
                await ReplyAsync("오류 : 우마무스메로 등록되지 않은 유저입니다. 트레이너님은 직접 뛰고 싶으셔도 조금만 참아 주세요.");
            }
            catch (UmamusumeAlreadyRegisteredException)
            {
                await ReplyAsync("오류 : 해당 우마무스메는 이 더비에 이미 등록되어 있습니다. (각질 변경 기능은 추후 업데이트 예정입니다.");
            }
            catch (DerbyNameNotFoundException)
            {
                await ReplyAsync("오류 : 해당 더비를 찾을 수 없습니다. (혹시 언더바 대신 띄어쓰기를 쓰지는 않으셨나요?) ");
            }
            catch (ArgumentException)
            {
                await ReplyAsync("오류 : 각질 이름이 잘못되었습니다. 각질은 [도주 / 선행 / 선입 / 추입] 4종류 입니다.");
            }

        }

        [Command("시작")]
        public async Task DerbyStartAsync(String derbyName)
        {
            if (Context.User.Id != 2)
            {
                await ReplyAsync("오류 : 더비 시작 권한이 없습니다.");
                return;
            }
            try
            {
                List<String> sList = Racemanager.RaceStart(derbyName);

                await ReplyAsync("레이스 시작 30초 전");
                await Task.Delay(25000);
                await ReplyAsync("레이스 시작 5초 전");
                await Task.Delay(5000);

                foreach (String str in sList)
                {
                    await ReplyAsync(str);
                    await Task.Delay(10000);
                }
            }
            catch (DerbyNameNotFoundException)
            {
                await ReplyAsync("오류 : 해당 더비를 찾을 수 없습니다. (혹시 언더바 대신 띄어쓰기를 쓰지는 않으셨나요?) ");
            }
        }
    }
}
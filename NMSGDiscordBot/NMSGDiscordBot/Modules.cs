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

	[Group("등록")]
	[Summary("우마무스메, 트레이너 등록")]
	public class RegisterModule : ModuleBase<SocketCommandContext>
	{
		[Command("우마무스메")]
		[Summary("우마무스메 등록")]
		public async Task UmamusumeRegisterAsync(String uName)
		{
			
			if(Context.Channel.Id != 8)
            {
				await ReplyAsync("오류 : 등록은 관리실에서 진행됩니다. 관리실로 와 주세요.");
				return;
            }
			
			List<Umamusume> uList = JSONManager.GetUmamusumeList();
			Console.WriteLine(uName);
			try
			{
				UmamusumeManager.Register(Context.User.Id, uName, ref uList);
				JSONManager.SetUmamusumeList(uList);
				await ReplyAsync("등록 완료! 호르트 아카데미에 오신 것을 환영합니다!");
			}
			catch (UmamusumeNameNotFoundException e)
			{
				await ReplyAsync("오류 : 해당 우마무스메를 찾을 수 없습니다.");
			}
			catch (UmamusumeAlreadyRegisteredException e)
			{
				await ReplyAsync("오류 : 해당 우마무스메는 이미 등록되었습니다.");
			}
			catch (DiscordIdAlreadyRegisteredException e)
			{
				await ReplyAsync("오류 : 이미 등록이 완료된 유저(우마무스메) 입니다.");
			}

		}

		[Command("트레이너")]
		[Summary("우마무스메 등록")]
		public async Task TrainerRegisterAsync(String tName)
		{
			if (Context.Channel.Id != 8)
			{
				await ReplyAsync("오류 : 등록은 관리실에서 진행됩니다. 관리실로 와 주세요.");
				return;
			}
			List<Trainer> tList = JSONManager.GetTrainerList();
			try
			{
				TrainerManager.Register(Context.User.Id, tName, ref tList);
				JSONManager.SetTrainerList(tList);
				await ReplyAsync("등록 완료! 호르트 아카데미에 오신 것을 환영합니다!");
			}
			catch (TrainerNameNotFoundException e)
			{
				await ReplyAsync("오류 : 해당 트레이너를 찾을 수 없습니다.");
			}
			catch (TrainerAlreadyRegisteredException e)
			{
				await ReplyAsync("오류 : 해당 트레이너는 이미 등록되었습니다.");
			}
			catch (DiscordIdAlreadyRegisteredException e)
			{
				await ReplyAsync("오류 : 이미 등록이 완료된 유저(트레이너) 입니다.");
			}
		}

		[Command("팀")]
		[Summary("팀 등록")]
		public async Task TeamRegister(String tName)
		{
			if (Context.Channel.Id != 8)
			{
				await ReplyAsync("오류 : 등록은 관리실에서 진행됩니다. 관리실로 와 주세요.");
				return;
			}
			List<Team> teamList = JSONManager.GetTeamList();
			List<Trainer> trainerList = JSONManager.GetTrainerList();
			List<Umamusume> uList = JSONManager.GetUmamusumeList();
			try
			{
				TeamManager.Register(Context.User.Id, tName, ref teamList, trainerList, uList);
				JSONManager.SetTeamList(teamList);
				await ReplyAsync("팀 등록 완료!");
			}
			catch (DiscordIDNotRegisteredException e)
			{
				await ReplyAsync("오류 : 해당 디스코드 아이디는 등록되지 않았습니다. 우마무스메나 트레이너 등록 이후 팀 등록을 진행해 주세요.");
			}
			catch (UmamusumeAlreadyRegisteredException e)
			{
				await ReplyAsync("오류 : 해당 우마무스메는 이미 등록되었습니다.");
			}
			catch (TrainerAlreadyRegisteredException e)
			{
				await ReplyAsync("오류 : 해당 트레이너는 이미 등록되었습니다.");
			}
			catch (TeamNameNotFoundException e)
			{
				await ReplyAsync("오류 : 해당 팀을 찾을 수 없습니다.");
			}
		}
	}

	public class TrainingModule : ModuleBase<SocketCommandContext>
	{
		[Command("훈련")]
		[Summary("우마무스메 훈련")]
		public async Task UmamusumeTrainingAsync(String keyword = null, String stat = null)
		{
			if((Context.Channel as ITextChannel).CategoryId != 8)
			{
				await ReplyAsync("오류 : 훈련은 훈련 시설( 체력단련실, 운동장, 연습용 트랙( 잔디, 더트), 도서관 ) 에서만 가능합니다.");
				return;
			}
			else if (keyword == "횟수")
            {
				try
				{
					string result = TrainingManager.GetLeftTraining(Context.User.Id);
					await ReplyAsync(result);
				}
				catch (UserIDNotFoundException e)
				{
					await ReplyAsync("오류 : 디스코드 ID에 해당하는 우마무스메를 찾을 수 없습니다. 등록을 먼저 진행해 주세요.");
				}
			}
			else if (keyword == "진행")
			{
				try
				{
					TrainingManager.UmamusumeTraining(Context.User.Id, stat);
					await ReplyAsync("훈련 완료!");
				}
				catch (NoLeftTrainingCountException e)
				{
					await ReplyAsync("오류 : 오늘의 훈련 가능 횟수를 모두 소진했습니다. 한국 시간 기준 자정에 초기화 됩니다.");
				}
				catch (UserIDNotFoundException e)
				{
					await ReplyAsync("오류 : 디스코드 ID에 해당하는 우마무스메를 찾을 수 없습니다. 등록을 먼저 진행해 주세요.");
				}
				catch (InvalidStatusTypeException e)
				{
					await ReplyAsync("오류 : 훈련 스탯 종류가 잘못 입력되었습니다. (훈련 가능 스탯 : 속도 / 체력 / 근력 / 근성 / 지능)");
				}
			}
			else if (keyword == "예시")
            {
				switch (stat)
                {
					case "속도":
						await ReplyAsync("속도 트레이닝 예시 : 잔디 / 런닝머신 / 피트니스 바이크 / 걸레질 / 샷 건 터치 (버튼을 누른 순간 먼 거리에서 낙하를 시작하는 공을 다이빙 캐치하는 트레이닝.) \n" +
							"< 속도가 상승합니다. 추가로 파워가 소폭 상승합니다. >");
						break;
					case "체력":
						await ReplyAsync("체력 트레이닝 예시 : 평영 / 자유형 / 배영 / 접영 / 고속 개헤엄 \n" +
							"(체력단련실에 수영장이 포함되어 있다는 설정입니다.) \n" +
							"< 체력이 상승합니다. 추가로 근성이 소폭 상승합니다. >");
						break;
					case "근력":
						await ReplyAsync("파워 트레이닝 예시 : 더트 / 스쿼트 / 윗몸 일으키기 / 복싱 / 기왓장 깨기 \n" +
							"< 파워가 상승합니다. 추가로 체력이 소폭 상승합니다. >");
						break;
					case "근성":
						await ReplyAsync("근성 트레이닝 예시 : 더트 언덕길 오르기 / 더트 언덕길 토끼뜀 / 댄스 연습 / 높은 계단 대쉬 / 대형 타이어 끌기 \n" +
							"< 근성이 상승합니다. 추가로 스피드와 파워가 소폭 상승합니다. >");
						break;
					case "지능":
						await ReplyAsync("지능 트레이닝 : 공부 / 독서 / 퀴즈(스피드 퀴즈) / 비디오 연구 / 일본 장기 \n" +
							"< 지능이 상승합니다. 추가로 스피드가 소폭 상승합니다. >");
						break;
					default:
						break;
                }
			}
			else
			{
				await ReplyAsync("훈련 커맨드 입력 방식 : [ !훈련 ] (커맨드 입력 방식) / [ !훈련 횟수 ] (남은 횟수 조회) \n" +
					" / [ !훈련 예시 (속도/체력/근력/근성/지능)] (훈련 예시. 실제 우마무스메 게임판에서의 훈련 내용) \n" +
					" / [ !훈련 진행 (속도/체력/근력/근성/지능)] (훈련 진행, 정해진 스탯 중 택 1) \n");
			}
		}
    }

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
			if(Context.User.Id != 2)
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
			catch(DerbyNameNotFoundException)
			{
				await ReplyAsync("오류 : 해당 더비를 찾을 수 없습니다. (혹시 언더바 대신 띄어쓰기를 쓰지는 않으셨나요?) ");
			}
        }
    }

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
			List<String> derbyCast = new List<string>();

			await ReplyAsync(derbyCast.Last<String>());

			/*
			foreach(String str in derbyCast)
            {
				await ReplyAsync(str);
				await Task.Delay(2000);
            }
			*/
			await ReplyAsync("Test Derby Complete") ;
        }

		

	}
}

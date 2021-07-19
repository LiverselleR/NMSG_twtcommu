using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace NMSGDiscordBot
{
    public class Derby : ModuleBase<SocketCommandContext>
    {
        public String derbyName;
        public FieldType fieldType;
        public int furlong;                             // 200미터 단위 pole 개수 
        public List<CourseType> courseTypeList;         // 200미터 단위로 코스의 휘어짐, 경사 정보
        public int numberParticipants;                  // 참가자 수


        public Derby()
        {
            this.derbyName = "TestDerby";
            this.fieldType = FieldType.soil;
            this.furlong = 1;
            this.courseTypeList = new List<CourseType>();
            this.courseTypeList.Add(CourseType.straight);
            this.numberParticipants = 5;
        }

        public async Task TestDerby()
        {
            Derby test = new Derby();
            List<Umamusume> entryList = GoogleSpreadsheetIO.GetUmamusumeList();
            await RaceManager(entryList);
        }

        public async Task RaceManager(List<Umamusume> entryList)
        {
            int turn = 0;
            double length = furlong * 200;
            List<Umamusume> goalUmamusumes = new List<Umamusume>();


            while (goalUmamusumes.Count < numberParticipants)
            {
                turn = turn + 1;
                List<Umamusume> currGoalUmamusume = new List<Umamusume>();
                
                foreach (Umamusume u in entryList)
                {
                    if(u.TurnProcess(furlong, courseTypeList))
                    {
                        currGoalUmamusume.Add(u);
                    }
                }

                currGoalUmamusume.Sort((u1, u2) => u1.goalTiming.CompareTo(u2.goalTiming));

                foreach(Umamusume u in currGoalUmamusume)
                {
                    goalUmamusumes.Add(u);
                    entryList.Remove(u);
                }

                entryList.Sort((u1, u2) => u1.currLocation.CompareTo(u2.currLocation));

                List<Umamusume> currRanking = new List<Umamusume>();
                currRanking.AddRange(goalUmamusumes);
                currRanking.AddRange(entryList);

                StringBuilder turnResult = new StringBuilder();

                foreach(Umamusume u in currRanking)
                {
                    turnResult.Append(u.name + " " + u.currLocation + " ");
                }
                await ReplyAsync(turnResult.ToString());
            }

        }
            
    }

    

    public enum FieldType
    {
        soil = 1,
        grass = 2
    } 
    public enum CourseType
    {
        curve = 1,
        straight = 2
    }
}

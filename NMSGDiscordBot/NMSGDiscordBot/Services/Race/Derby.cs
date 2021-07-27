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
    public class Derby
    {
        public String derbyName;
        public FieldType fieldType;
        public int furlong;                             // 200미터 단위 pole 개수 
        public List<CourseType> courseTypeList;         // 200미터 단위로 코스의 휘어짐, 경사 정보
        public int numberParticipants;                  // 참가자 수
        public DerbyStatusType statusType;


        public Derby()
        {
            this.derbyName = "TestDerby";
            this.fieldType = FieldType.durt;
            this.furlong = 1;
            this.courseTypeList = new List<CourseType>();
            this.courseTypeList.Add(CourseType.straight);
            this.numberParticipants = 5;
            this.statusType = DerbyStatusType.speed;
        }

        public Derby(String derbyName, FieldType fieldType, int furlong, List<CourseType> courseTypeList, int numberParticipants, DerbyStatusType statusType)
        {
            this.derbyName = derbyName;
            this.fieldType = fieldType;
            this.furlong = furlong;
            this.courseTypeList = courseTypeList;
            this.numberParticipants = numberParticipants;
            this.statusType = statusType;
        }

        public List<String> TestDerby()
        {
            Derby test = new Derby();
            List<Umamusume> entryList = GoogleSpreadsheetIO.GetUmamusumeList();
            List<Participant> entry = new List<Participant>();
            foreach(Umamusume u in entryList)
            {
                entry.Add(new Participant(u, test, RunningStyle.Front, TurfCondition.normal));
            }
            Race r = new Race(test, entry);
            return r.RaceManager();
        }

        
            
    }

    

    public enum FieldType
    {
        durt = 1,
        grass = 2
    } 
    public enum CourseType
    {
        curve = 1,
        straight = 2
    }
    public enum DerbyStatusType
    {
        speed = 1,
        stamina = 2, 
        power = 3,
        toughness = 4,
        intelligence = 5
    }
}

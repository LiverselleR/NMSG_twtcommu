using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace NMSGDiscordBot
{
    public class Turn
    {
        public int currTurn;
        private List<Participant> participants;
        private int furlong;
        private List<CourseType> courseTypeList;
        private List<String> raceLog;
        public Turn(List<Participant> participants, int furlong, List<CourseType> courseTypeList)
        {
            currTurn = 0;
            this.participants = participants;
            this.furlong = furlong;
            this.courseTypeList = courseTypeList;
            raceLog = new List<string>();
        }

        public void Process()
        {
            currTurn++;

            foreach(Participant p in participants)
            {
                p.TurnProcess();
            }

            participants.Sort((p1, p2) => p1.CompareTo(p2));

            for(int i = 0; i < participants.Count(); i++)
            {
                participants[i].RankRenewal(i+1);
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("Turn " + currTurn + " : ");
            foreach(Participant p in participants)
            {
                sb.Append(p.name + " - " + p.currLocation + " / ");
            }
            sb.Remove(sb.Length - 3, 2);
            raceLog.Add(sb.ToString());
            Console.WriteLine(sb.ToString());
            return;
        }

        public List<String> GetLog()
        {
            return raceLog;
        }

        public Boolean IsRaceOver()
        {
            Boolean result = true;

            foreach (Participant p in participants)
            {
                result = result && (p.isGoal || p.isRetired);
            }

            return !result;
        }
    }
}

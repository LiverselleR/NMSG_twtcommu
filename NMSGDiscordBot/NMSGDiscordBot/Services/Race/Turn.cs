﻿using System;
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
        private List<CourseType> courseTypeList;
        private List<String> raceLog;
        private List<String> raceDetailLog;
        public Turn(List<Participant> participants, List<CourseType> courseTypeList)
        {
            currTurn = 0;
            this.participants = participants;
            this.courseTypeList = courseTypeList;
            raceLog = new List<string>();
            raceDetailLog = new List<String>();
        }

        public void Process()
        {
            currTurn++;

            foreach(Participant p in participants)
            {
                p.TurnProcess(participants, currTurn);
            }

            participants.Sort((p1, p2) => p1.CompareTo(p2));

            for(int i = 0; i < participants.Count(); i++)
            {
                participants[i].RankRenewal(i);
            }

            StringBuilder sbInfo = new StringBuilder();
            StringBuilder sbDetail = new StringBuilder();

            sbInfo.Append("◎Turn " + currTurn + "\n");
            sbDetail.Append("◎Turn " + currTurn + "\n");
            foreach(Participant p in participants)
            {
                sbDetail.Append(p.ToString() + "\n");
                sbInfo.Append(p.ToInfoString() + "\n");

                sbDetail.Replace('_', ' ');
                sbInfo.Replace('_', ' ');
            }
            if (currTurn%20 == 0)
            {
                raceLog.Add(sbInfo.ToString());
                raceDetailLog.Add(sbDetail.ToString());
                Console.WriteLine(sbDetail.ToString());
            }
            return;
        }

        public List<String> GetLog()
        {
            return raceLog;
        }
        public List<String> GetDetailLog()
        {
            return raceDetailLog;
        }

        public List<RunningStyle> GetRunningStyles()
        {
            List<RunningStyle> result = new List<RunningStyle>();

            foreach (Participant p in participants)
                result.Add(p.runningStyle);
            return result;
        }

        public String GetResultRank()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("■ 최종 순위\n");
            for (int i = 0; i < participants.Count; i++)
            {
                sb.Append("▷ " + (i + 1) + "위 : " + participants[i].name + "\n");
            }
            sb.Replace('_', ' ');
            return sb.ToString();
        }

        public Boolean IsRaceOver()
        {
            Boolean result = true;

            foreach (Participant p in participants)
            {
                result = result && (p.isGoal);
            }

            return !result;
        }
    }
}

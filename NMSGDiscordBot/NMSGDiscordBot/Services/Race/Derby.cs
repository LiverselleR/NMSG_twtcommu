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
        public int courseLength;                             // 200미터 단위 pole 개수 
        public List<CourseType> courseTypeList;         // 200미터 단위로 코스의 휘어짐, 경사 정보
        public int numberParticipants;                  // 참가자 수
        public StatusType statusType;              // 경기 주요 스테이터스
        public Racetrack racetrack;                     // 경기장 트랙 정보
        public int firstIndex;                          // racetrack 의 초반 시작 인덱스
        public int lastIndex;                           // racetrack 의 후반 시작 인덱스
        public int lastStraightIndex;                   // 최후직선 인덱스
        public TurfCondition turfCondition;             // 마장 상태

        public Derby()
        {
            this.derbyName = "TestDerby";
            this.fieldType = FieldType.durt;
            this.courseLength = 2200;
            this.courseTypeList = new List<CourseType>();
            this.courseTypeList.Add(CourseType.straight);
            this.numberParticipants = 5;
            this.statusType = StatusType.speed;

            this.racetrack = new Racetrack();
            firstIndex = 3;
            lastIndex = 2;
            lastStraightIndex = 3;
            turfCondition = TurfCondition.normal;

        }

        public Derby(String derbyName, FieldType fieldType, int furlong, List<CourseType> courseTypeList, int numberParticipants, StatusType statusType)
        {
            this.derbyName = derbyName;
            this.fieldType = fieldType;
            this.courseLength = furlong;
            this.courseTypeList = courseTypeList;
            this.numberParticipants = numberParticipants;
            this.statusType = statusType;
        }

        public static List<String> TestDerby()
        {
            Derby test = new Derby();
            List<Umamusume> entryList = Umamusume.GetTestUList();
            List<Participant> entry = new List<Participant>();

            entry.Add(new Participant(entryList[0], test, RunningStyle.Stretch, 1));
            entry.Add(new Participant(entryList[1], test, RunningStyle.Runaway, 2));
            entry.Add(new Participant(entryList[2], test, RunningStyle.Runaway, 3));
            entry.Add(new Participant(entryList[4], test, RunningStyle.Front, 5));
            entry.Add(new Participant(entryList[6], test, RunningStyle.FI, 7));
            entry.Add(new Participant(entryList[8], test, RunningStyle.Stretch, 9));

            Race r = new Race(test, entry);
            return r.RaceManager();
        }

        public CoursePhase GetCoursePhase(double currPosition)
        {
            /// 초반 : 스타트 후 직선 코스에서 경쟁
            /// 중반 : 후반 가기 전 컨디션 조절
            /// 후반 : 최후 코너 + 최후 직선

            double leftLength = courseLength - currPosition; 

            if(courseLength < 1600)                     // 단거리
            {
                if (leftLength > courseLength - 400) return CoursePhase.First;
                else if (leftLength > racetrack.straight - 50) return CoursePhase.Middle;
                else return CoursePhase.Last;
            }
            else                                        // 마일, 중거리, 장거리
            {
                if (leftLength > courseLength + 50 - racetrack.GetTrackLength()) return CoursePhase.First;
                else if (leftLength > racetrack.curveLeft + racetrack.straight - 50) return CoursePhase.Middle;
                else return CoursePhase.Last;
            }
        }

        public Boolean IsLastStraight(double currLocation)
        {
            double length = racetrack.partLength[lastStraightIndex] - 50;
            if (currLocation >= courseLength - length) return true;
            else return false;
        }
        
            
    }

    

    public enum CourseType
    {
        curve = 1,
        straight = 2
    }
    public enum StatusType
    {
        speed = 1,
        stamina = 2, 
        power = 3,
        toughness = 4,
        intelligence = 5
    }
    public enum CoursePhase
    {
        First = 1,
        Middle = 2,
        Last = 3
    }
}

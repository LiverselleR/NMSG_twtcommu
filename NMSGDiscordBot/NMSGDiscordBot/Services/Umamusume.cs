using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace NMSGDiscordBot
{
    public class Umamusume
    {
        public string name;         // 우마무스메 이름
        public UInt64 ownerId;      // 오너 아이디 (디스코드 아이디)

        public int speed;           // 속도 - 최고 속도
        public int stamina;         // 최대 체력
        public int power;           // 근력 - 가속력, 코스 잡기, 돌파력
        public int toughness;       // 근성 - 종반 라스트 스퍼트, 달라붙기, 스태미너 보조
        public int intelligence;    // 지능 - 지구력 관리, 스킬 사용 빈도, 디버프 회피

        private double currStamina;    // 더비용 현재 스테미나
        private double currVelocity;   // 더비용 현재 속도
        public double currLocation;    // 더비용 현재 위치
        public double prevLocation;    // 더비용 이전 턴 위치
        private Boolean isGoal;        // 더비용 현재 골 여부 확인
        public double goalTiming;      // 더비용 골 타이밍 (골인한 경우 갱신)

        private double maxVelocity;    // 최고 속도 (speed 영향 받음)
        private double phase1boundary; // 체력 1차 바운더리
        private double phase2boundary; // 체력 2차 바운더리
        private double phase1slope;    // 체력 1차영역 최대속도 기울기 (최대 속도 대비 퍼센트)
        private double phase2slope;    // 체력 2차영역 최대속도 기울기 (최대 속도 대비 퍼센트)



        public Umamusume(string name, UInt64 ownerId, int speed, int stamina, int power, int toughness, int intelligence)
        {
            this.name = name;
            this.ownerId = ownerId;

            this.speed = speed;
            this.stamina = stamina;
            this.power = power;
            this.toughness = toughness;
            this.intelligence = intelligence;

            currStamina = stamina;
            currVelocity = 0;

            currLocation = 0;
            prevLocation = 0;

            isGoal = false;
            goalTiming = -1;

            maxVelocity = speed;
            phase1boundary = 20;
            phase2boundary = 70;
            phase1slope = 90;
            phase2slope = 60;
        }

        public Boolean TurnProcess(int maxFurlong, List<CourseType> courseTypeList)
        {
            Boolean result = false;
            int currFurlong = (int)currLocation / 200;
            int prevFurlong = -1;
            CourseType currCourseType = courseTypeList[currFurlong];

            if(currFurlong < maxFurlong - 1)
            {
                prevFurlong = currFurlong + 1;
            }
            prevLocation = currLocation;
            maxVelocity = GetCurrMaxVelocity();

            if(currVelocity < maxVelocity)
            {
                double accel = GetCurrAccel();
                currVelocity = currVelocity + accel;
                currStamina -= 5;
            }
            else
            {
                currStamina -= 1;
            }
            
            if(currVelocity > maxVelocity)
            {
                currVelocity = maxVelocity;
            }

            currLocation = prevLocation + currVelocity;

            if (currLocation > maxFurlong * 200 && !isGoal)
            {
                result = true;
                isGoal = true;
                goalTiming = GetGoalTiming(maxFurlong * 200);
            }

            return result;
        }

        private double GetCurrMaxVelocity()
        {
            double result = 0;
            double x = currStamina / (double)stamina * 100;
            double y = 0;

            if(x <= phase1boundary)
            {
                y = 100 - (x * (100 - phase1slope)) / phase1boundary;
            }
            else if(x <= phase2boundary)
            {
                y = phase1slope - (x - phase1boundary) * (phase1slope - phase2slope) / (phase2boundary - phase1boundary);
            }
            else
            {
                y = phase2slope - (x - phase2boundary) * phase2slope / (100 - phase2boundary);
            }

            result = speed * (y / 100);
            result = Math.Round(result, 2);
            return result;
        }

        private double GetCurrAccel()
        {
            double result = 50;

            return result;
        }

        public double GetGoalTiming(double length)
        {
            return (length - prevLocation) / (currLocation - prevLocation);    
        }



    }


}

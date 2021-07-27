using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Discord;

namespace NMSGDiscordBot
{
    public class Umamusume
    {
        public string name;         // 우마무스메 이름
        public UInt64 ownerId;      // 오너 아이디 (디스코드 아이디)

        // 기본 스탯
        public int speed;                   // 속도 - 최고 속도
        public int stamina;                 // 최대 체력
        public int power;                   // 근력 - 가속력, 코스 잡기, 돌파력
        public int toughness;               // 근성 - 종반 라스트 스퍼트, 달라붙기, 스태미너 보조
        public int intelligence;            // 지능 - 지구력 관리, 스킬 사용 빈도, 디버프 회피

        // 각질 적성
        public Aptitude runawayAptitude;    // 도주
        public Aptitude frontAptitude;      // 선행
        public Aptitude fiAptitude;         // 선출
        public Aptitude stretchAptitude;    // 추입

        // 경기장 적성
        public Aptitude grassAptitude;      // 잔디
        public Aptitude durtAptitude;       // 더트

        // 거리 적성
        public Aptitude shortAptitude;      // 단거리 적성
        public Aptitude mileAptitude;       // 마일 적성
        public Aptitude middleAptitude;     // 중거리 적성
        public Aptitude longAptitude;       // 장거리 적성



        public Umamusume()
        {
            this.name = "테스트";
            this.ownerId = 0;

            this.speed = 1;
            this.stamina = 1;
            this.power = 1;
            this.toughness = 1;
            this.intelligence = 1;

            this.runawayAptitude = Aptitude.A;
            this.frontAptitude = Aptitude.A;
            this.fiAptitude = Aptitude.A;
            this.stretchAptitude = Aptitude.A;

            this.grassAptitude = Aptitude.A;
            this.durtAptitude = Aptitude.A;

            this.shortAptitude = Aptitude.A;
            this.mileAptitude = Aptitude.A;
            this.middleAptitude = Aptitude.A;
            this.longAptitude = Aptitude.A;

        }

        public Umamusume(string name, UInt64 ownerId,
                        int speed, int stamina, int power, int toughness, int intelligence,
                        Aptitude runawayAptitude, Aptitude frontAptitude, Aptitude fiAptitude, Aptitude stretchAptitude,
                        Aptitude grassAptitude, Aptitude durtAptitude,
                        Aptitude shortAptitude, Aptitude mileAptitude, Aptitude middleAptitude, Aptitude longAptitude)
        {
            this.name = name;
            this.ownerId = ownerId;

            this.speed = speed;
            this.stamina = stamina;
            this.power = power;
            this.toughness = toughness;
            this.intelligence = intelligence;

            this.runawayAptitude = runawayAptitude;
            this.frontAptitude = frontAptitude;
            this.fiAptitude = fiAptitude;
            this.stretchAptitude = stretchAptitude;

            this.grassAptitude = grassAptitude;
            this.durtAptitude = durtAptitude;

            this.shortAptitude = shortAptitude;
            this.mileAptitude = mileAptitude;
            this.middleAptitude = middleAptitude;
            this.longAptitude = longAptitude;
        }


        public override string ToString()
        {
            return "Umamusume Name : " + name + " / owner ID : " + ownerId;
        }


    }

    public enum RunningStyle
    {
        Runaway = 1,        // 도주
        Front = 2,          // 선행  
        FI = 3,             // 선출
        Stretch = 4         // 추입
    }

    public enum Aptitude
    {
        S = 1,
        A = 2,
        B = 3,
        C = 4,
        D = 5,
        E = 6,
        F = 7,
        G = 8
    }



}

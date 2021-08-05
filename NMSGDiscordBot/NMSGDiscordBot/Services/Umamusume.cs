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
        public UInt64 ownerID;      // 오너 아이디 (디스코드 아이디)

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
            this.ownerID = 0;

            this.speed = 600;
            this.stamina = 600;
            this.power = 600;
            this.toughness = 600;
            this.intelligence = 600;

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
            this.ownerID = ownerId;

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
            return "Umamusume Name : " + name + " / owner ID : " + ownerID;
        }

        public static List<Umamusume> GetTestUList()
        {
            List<Umamusume> result = new List<Umamusume>();

            result.Add(
                new Umamusume("스탠다드 스테이트", 0, 1100, 1100, 1100, 600, 600,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.S,
                Aptitude.A, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.A, Aptitude.B));
            result.Add(
                new Umamusume("스탠다드 컨디션스", 0, 1100, 1100, 1100, 600, 600,
                Aptitude.S, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.A, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.A, Aptitude.B));
            result.Add(
                new Umamusume("파란 천둥", 0, 800, 800, 800, 300, 300,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.A, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.A, Aptitude.B));
            result.Add(
                new Umamusume("의성 블랙갈릭", 0, 800, 800, 800, 300, 300,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.A, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.A, Aptitude.B));
            result.Add(
                new Umamusume("해운대 씨걸", 0, 800, 800, 800, 300, 300,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.A, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.A, Aptitude.B));
            result.Add(
                new Umamusume("보라매 레드", 0, 800, 800, 800, 300, 300,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.A, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.A, Aptitude.B));
            result.Add(
                new Umamusume("웨이브 서퍼", 0, 800, 800, 800, 300, 300,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.A, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.A, Aptitude.B));
            result.Add(
                new Umamusume("레인보우 민티", 0, 800, 800, 800, 300, 300,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.A, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.A, Aptitude.B));
            result.Add(
                new Umamusume("썬더 샤베트", 0, 800, 800, 800, 300, 300,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.A, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.A, Aptitude.B));
            result.Add(
                new Umamusume("후유마츠리", 0, 800, 800, 800, 300, 300,
                Aptitude.C, Aptitude.C, Aptitude.C, Aptitude.C,
                Aptitude.A, Aptitude.D,
                Aptitude.C, Aptitude.C, Aptitude.A, Aptitude.B));

            return result;
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

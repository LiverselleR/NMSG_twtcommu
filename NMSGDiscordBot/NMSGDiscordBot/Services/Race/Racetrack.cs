using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMSGDiscordBot
{
    public class Racetrack
    {
        public int straight;               // 직선 주로 거리 (m)
        public int curveLeft;              // 좌측 커브 주로 거리 (m)
        public int curveRight;             // 우측 커브 주로 거리 (m)

        public List<int> partLength;                    // 코스 구간 길이 (m)
        public List<CourseType> partType;               // 코스 타입 (직선, 곡선)
        public List<(int, float)> partHeight;           // 코스 높낮이 (골 기준 현재 위치(m), 골 기준 현재 높이 (m))
        public int width;                               // 주로 폭 (m)
        public FieldType fieldType;                     // 필드 타입 (m)



        public Racetrack()
        {
            this.straight = 450;
            this.curveLeft = 450;
            this.curveRight = 450;
            this.width = 30;
            this.fieldType = FieldType.grass;

            partLength = new List<int>();
            partType = new List<CourseType>();
            partHeight = new List<(int, float)>();

            for(int i = 0; i < 4; i++)
            {
                partLength.Add(450);
                if (i % 2 == 0) partType.Add(CourseType.straight);
                else partType.Add(CourseType.curve);
            }

        }

        public Racetrack(int straight, int curveLeft, int curveRight, int width, FieldType fieldType)
        {
            this.straight = straight;
            this.curveLeft = curveLeft;
            this.curveRight = curveRight;
            this.width = width;
            this.fieldType = fieldType;
        }

        public int GetTrackLength()
        {
            return straight * 2 + curveLeft + curveRight;
        }

    }

    public enum FieldType
    {
        durt = 1,
        grass = 2
    }

}

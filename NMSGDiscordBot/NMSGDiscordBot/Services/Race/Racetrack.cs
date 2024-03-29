﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMSGDiscordBot
{
    public class Racetrack
    {
        public int id;
        public List<int> partLength;                    // 코스 구간 길이 (m)
        public List<CourseType> partType;               // 코스 타입 (직선, 곡선)
        public List<double> partHeight;                  // 코스 높낮이 (시작 포인트 기준 코스 마지막 높이)
        public int width;                               // 주로 폭 (m)
        public int radius;                              // 커브 코스의 반지름 (원의 중심에서 코스 중심까지, m)
        public FieldType fieldType;                     // 필드 타입 (m)


        public Racetrack()
        {
            id = 0;
            this.width = 30;
            this.radius = 150;
            this.fieldType = FieldType.grass;

            partLength = new List<int>();
            partType = new List<CourseType>();
            partHeight = new List<double>();

            for(int i = 0; i < 5; i++)
            {
                partLength.Add(1);
                if (i % 2 == 0) partType.Add(CourseType.straight);
                else partType.Add(CourseType.curve);
                partHeight.Add(0);
            }
        }

        public Racetrack(int id, List<int> partLength, List<CourseType> partType, List<double> partHeight, int width, int radius, FieldType fieldType)
        {
            this.id = id;
            this.partLength = partLength;
            this.partType = partType;
            this.partHeight = partHeight;
            this.width = width;
            this.radius = radius;
            this.fieldType = fieldType;
        }

        public int GetTrackLength()
        {
            int result = 0;
            foreach (int i in partLength)
                result += i;
            return result;
        }

        public int GetLastLength()
        {
            return partLength[partLength.Count - 1] + partLength[partLength.Count - 2] + 50;
        }
        public int GetLastCurve()
        {
            return partLength[partLength.Count - 1] + partLength[partLength.Count - 2];
        }
        public int GetLastStraight()
        {
            return partLength[partLength.Count - 1];
        }
        public int GetLastSpurt()
        {
            return partLength[partLength.Count - 1] / 2;
        }
        public int GetMiddleEndLength()
        {
            return GetTrackLength() - GetLastLength();
        }

        public CourseType GetCourseType(double currLocation)
        {
            CourseType result = CourseType.straight;
            int currPartEnd = 0;
            for(int i = 0; i < partType.Count; i++)
            {
                currPartEnd += partLength[i];
                if(currPartEnd > currLocation)
                {
                    result = partType[i];
                    break;
                }
            }

            return result;
        }

        public double GetCurveMoveLength(double currSpeed, double currY)
        {
            return radius * currSpeed / (radius - ((double)width / 2) + currY);
        }
    }

    public enum FieldType
    {
        durt = 1,
        grass = 2
    }

}

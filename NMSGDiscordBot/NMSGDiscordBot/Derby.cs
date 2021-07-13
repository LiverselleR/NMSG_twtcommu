using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NMSGDiscordBot
{
    public class Derby
    {
        public String derbyName;
        public FieldType fieldType;
        public int Furlong;                             // 200미터 단위 pole 개수 
        public List<CourseType> courseTypeList;         // 200미터 단위로 코스의 휘어짐, 경사 정보
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

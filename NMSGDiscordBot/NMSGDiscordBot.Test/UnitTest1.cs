using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace NMSGDiscordBot.Test
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        
        [Fact]
        public void TestRace()
        {
            List<String> result = Derby.TestDerby_Process();

            foreach(String str in result)
            {
                output.WriteLine(str);
            }
        }

        [Fact]
        public void CheckRaceResult()
        {
            int[,] result = new int[4, 12];
            for(int i = 0; i < 10; i++)
            {
                List<RunningStyle> sList = Derby.TestDerby_RunningStyle();
                for(int j = 0; j < 8; j++)
                {
                    switch(sList[j])
                    {
                        case RunningStyle.Runaway:
                            result[0, j]++;
                            break;
                        case RunningStyle.Front:
                            result[1, j]++;
                            break;
                        case RunningStyle.FI:
                            result[2, j]++;
                            break;
                        case RunningStyle.Stretch:
                            result[3, j]++;
                            break;
                    }
                }
            }
            for(int i = 0; i < 8; i++)
            {
                output.WriteLine((i + 1) + "위 - 도주 : " + result[0, i]
                    + "/ 선행 : " + result[1,i]
                    + "/ 선입 : " + result[2,i]
                    + "/ 추입 : " + result[3,i]);
            }
        }
        
    }
}

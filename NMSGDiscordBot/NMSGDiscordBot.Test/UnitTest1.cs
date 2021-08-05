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
        public void Test1()
        {
            List<String> sList = Derby.TestDerby();
            foreach (string s in sList)
            {
                output.WriteLine(s);
            }
        }
    }
}

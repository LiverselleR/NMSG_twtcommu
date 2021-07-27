using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMSGDiscordBot
{
    public class Race
    {
        public Derby derby;
        public List<Participant> entry;

        public Race(Derby derby, List<Participant> entry)
        {
            this.derby = derby;
            this.entry = entry;
        }

        public List<String> RaceManager()
        {
            int furlong = derby.furlong;
            List<CourseType> courseTypeList = derby.courseTypeList;
            Turn turn = new Turn(entry, furlong, courseTypeList);

            while(turn.IsRaceOver())
            {
                turn.Process();
            }

            return turn.GetLog();
        }

        
    }

    public enum TurfCondition
    {
        normal = 1,
        littleHeavy = 2,
        heavy = 3,
        bad = 4
    }
}

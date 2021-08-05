using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;

namespace NMSGDiscordBot
{ 
    public class TrainingManager
    {
        public static void UmamusumeTraining(UInt64 ownerID, StatusType status) 
        {
            List<Training> tList = JSONManager.GetTrainingInfo();
            List<Umamusume> uList = JSONManager.GetUmamusumeList();

            Training? t = tList.Find(t => t.ownerID == ownerID);

            if (t == null)
            {
                Umamusume? u = uList.Find(u => u.ownerID == ownerID);
                if (u == null) throw new UserIDNotFoundException();
                else
                {
                    tList.Add(new Training(ownerID, 3, DateTime.Now));
                }
            }

            int ti = tList.FindIndex(t => t.ownerID == ownerID);
            int ui = uList.FindIndex(u => u.ownerID == ownerID);

            TimeSpan timeSpan = tList[ti].date - DateTime.Now;
            if (timeSpan.Days > 0)
                tList[ti].leftTraining = 3;
            if (tList[ti].leftTraining > 0)
            {
                tList[ti].leftTraining--;
                switch (status)
                {
                    case StatusType.speed:
                        uList[ui].speed += 10;
                        break;
                    case StatusType.stamina:
                        uList[ui].stamina += 10;
                        break;
                    case StatusType.power:
                        uList[ui].power += 10;
                        break;
                    case StatusType.toughness:
                        uList[ui].toughness += 10;
                        break;
                    case StatusType.intelligence:
                        uList[ui].intelligence += 10;
                        break;
                }
            }
            else throw new NoLeftTrainingCountException();

            JSONManager.SetTrainingInfo(tList);
            JSONManager.SetUmamusumeList(uList);
        }

        public static void GetLeftTraining(UInt64 ownerID)
        {

        }
    }

    public class Training
    {
        public UInt64 ownerID;
        public int leftTraining;
        public DateTime date;

        public Training(UInt64 ownerID, int leftTraining, DateTime date)
        {
            this.ownerID = ownerID;
            this.leftTraining = leftTraining;
            this.date = date;
        }
    }

    public class NoLeftTrainingCountException : Exception
    {
        public NoLeftTrainingCountException()
        {

        }
        public NoLeftTrainingCountException(string message)
            : base(message)
        {

        }
        public NoLeftTrainingCountException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
    public class UserIDNotFoundException : Exception
    {
        public UserIDNotFoundException()
        {
        }

        public UserIDNotFoundException(string message) : base(message)
        {
        }

        public UserIDNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserIDNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }



}

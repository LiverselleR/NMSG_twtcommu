﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NMSGDiscordBot
{
    public class UmamusumeManager
    {
        public static void Register(UInt64 discordID, string uName, ref List<Umamusume> uList)
        {
            int i = uList.FindIndex(x => x.name.Contains(uName));
            if (i == -1) throw new UmamusumeNameNotFoundException();
            else if (uList.FindIndex(x => x.ownerId.CompareTo(discordID) == 0) != -1) throw new DiscordIdAlreadyRegisteredException();
            else
            {
                if (uList[i].ownerId != 0) throw new UmamusumeAlreadyRegisteredException();
                uList[i].ownerId = discordID;
                return;
            }
        }

        public void Withdraw()
        {

        }

        public void Lookup()
        {

        }

        
    }

    public class UmamusumeNameNotFoundException : Exception
    {
        public UmamusumeNameNotFoundException()
        {

        }
        public UmamusumeNameNotFoundException(string message)
            :base(message)
        {

        }
        public UmamusumeNameNotFoundException(string message, Exception inner)
            :base(message, inner)
        {

        }
    }
    public class DiscordIdAlreadyRegisteredException : Exception
    {
        public DiscordIdAlreadyRegisteredException()
        {

        }
        public DiscordIdAlreadyRegisteredException(string message)
            : base(message)
        {

        }
        public DiscordIdAlreadyRegisteredException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
    public class UmamusumeAlreadyRegisteredException : Exception
    {
        public UmamusumeAlreadyRegisteredException()
        {

        }
        public UmamusumeAlreadyRegisteredException(string message)
            : base(message)
        {

        }
        public UmamusumeAlreadyRegisteredException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }

}
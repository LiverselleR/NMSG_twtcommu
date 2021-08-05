using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NMSGDiscordBot
{
    public class JSONManager
    {
        public static List<Umamusume> GetUmamusumeList()
        {
            List<Umamusume> uList = new List<Umamusume>();
            String path = AppDomain.CurrentDomain.BaseDirectory + @"\Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"\Umamusume.json";

            if (!File.Exists(path))
            {   using (FileStream fs = File.Create(path))
                {
                    Umamusume u = new Umamusume();
                    uList.Add(u);
                    uList.Add(u);
                    dataset.Add(JObject.FromObject(u));
                    dataset.Add(JObject.FromObject(u));
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(dataset.ToString());
                    sw.Close();
                }
            }
            else
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    StreamReader sr = new StreamReader(fs);
                    dataset = JArray.Parse(sr.ReadToEnd());
                    uList = dataset.ToObject<List<Umamusume>>();
                }
            }

            return uList;
        }
        public static void SetUmamusumeList(List<Umamusume> uList)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + @"\Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"\Umamusume.json";

            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path))
                {
                    Umamusume u = new Umamusume();
                    uList.Add(u);
                    uList.Add(u);
                    dataset.Add(JObject.FromObject(u));
                    dataset.Add(JObject.FromObject(u));
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(dataset.ToString());
                    sw.Close();
                }
            }
            else
            {
                using (FileStream fs = File.OpenWrite(path))
                {
                    dataset = JArray.FromObject(uList);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(dataset.ToString());
                    sw.Close();
                }
            }
        }

        public static List<Training> GetTrainingInfo()
        {
            List<Training> trainList = new List<Training>();
            List<Umamusume> uList = GetUmamusumeList();
            String path = AppDomain.CurrentDomain.BaseDirectory + @"\Data";
            JArray dataset = new JArray();

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"\Training.json";

            if (!File.Exists(path))
            {
                File.Create(path);
            }
            else
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    StreamReader sr = new StreamReader(fs);
                    dataset = JArray.Parse(sr.ReadToEnd());
                    trainList = dataset.ToObject<List<Training>>();
                }
            }

            return trainList;
        }

        public static void SetTrainingInfo(List<Training> tList)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory + @"\Data";
            JArray dataset = new JArray();
            FileStream fs;

            DirectoryInfo dl = new DirectoryInfo(path);
            if (dl.Exists == false) dl.Create();

            path = path + @"\Training.json";

            if (!File.Exists(path))
                fs = File.Create(path);
            else
                fs = File.OpenWrite(path);   

            dataset = JArray.FromObject(tList);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(dataset.ToString());
            sw.Close();
        }
    }
}

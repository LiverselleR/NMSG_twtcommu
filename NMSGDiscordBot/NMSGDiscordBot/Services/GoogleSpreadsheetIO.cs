using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NMSGDiscordBot
{
    public class GoogleSpreadsheetIO
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Manage FantasticDerby DB with Google Sheets";

        public static List<Umamusume> GetUmamusumeList()
        {
            List<Umamusume> result = new List<Umamusume>();
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = "14AsOgzK1L0FP_SpJ6ATJVI2r12pc_zUp1la1_3LzJ-U";
            String range = "Umamusume!A2:G";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the data of Umamusumes in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/14AsOgzK1L0FP_SpJ6ATJVI2r12pc_zUp1la1_3LzJ-U/edit#gid=0
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Name, UID, Speed, Intelligence, Power, Toughness, Stamina");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", row[0], row[1], row[2], row[3], row[4], row[5], row[6]);
                    result.Add(new Umamusume((string)row[0], Convert.ToUInt64(row[1]), int.Parse((string)row[2]), int.Parse((string)row[3]), int.Parse((string)row[4]), int.Parse((string)row[5]), int.Parse((string)row[6])));
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            return result;
        }
    }
}

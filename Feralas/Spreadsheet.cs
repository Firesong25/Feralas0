using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Generic
{
    class Spreadsheet
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName =
            Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
        static readonly string SpreadsheetId = "1ezQFy7w_Zmtla-n5lMJZAskUNQTXTcIrmEFGyndF6dE";
        static SheetsService service;

        static Spreadsheet()
        {

            GoogleCredential credential;
            //Reading Credentials File...
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            // Creating Google Sheets API service...
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,

            });
        }

        public async static Task<bool> AppendRows(string worksheet, IList<IList<object>> grid)
        {
            bool success = false;
            var _range = $"'{worksheet}'!A1:Z1";
            var valueRange = new ValueRange();
            valueRange.Values = grid;
            try
            {
                var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, _range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendReponse = await appendRequest.ExecuteAsync();
                success = true;
            }
            catch
            {
                // wtf
            }
            return success;
        }

        public static async Task<List<List<string>>> GetData(string worksheet, string range)
        {
            var _range = $"'{worksheet}'!{range}";
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(SpreadsheetId, _range);
            var response = await request.ExecuteAsync();
            List<List<string>> rows = new List<List<string>>();
            foreach (IList<object> line in response.Values)
            {
                List<string> row = new List<string>();
                string field = string.Empty;
                foreach (object o in line)
                {
                    field = o.ToString();
                    row.Add(field);
                }
                rows.Add(row);
            }
            return rows;
        }


        async static Task<bool> UpdateCells(string worksheet, string range, IList<IList<object>> grid)
        {
            bool success = false;
            var _range = $"'{worksheet}'!{range}";
            var valueRange = new ValueRange();
            valueRange.Values = grid;

            try
            {
                var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, _range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                var appendReponse = await updateRequest.ExecuteAsync();
                success = true;
            }
            catch
            {
                // wtf
            }
            return success;
        }



        public async static Task<bool> UpdateCells(string worksheet, string range, List<List<string>> string_grid)
        {

            IList<IList<object>> grid = new List<IList<object>>();

            foreach (List<string> tmp in string_grid)
            {
                if (tmp.Count > 0)
                {
                    IList<object> tmp_object_list = new List<object>();
                    tmp_object_list.Add(tmp[0]);
                    tmp_object_list.Add(tmp[1]);
                    grid.Add(tmp_object_list);
                }
            }


            bool success = false;
            var _range = $"'{worksheet}'!{range}";
            var valueRange = new ValueRange();
            valueRange.Values = grid;

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, _range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = await updateRequest.ExecuteAsync();
            success = true;
            return success;
        }
    }
}



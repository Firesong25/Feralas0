using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Generic
{
    public class Logmaker
    {
        static string _msg = string.Empty;
        
        public async static Task RecordAsync(string msg)
        {
            if (msg != _msg)
            {
                string _name =
                    Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
                _msg = msg;
                IList<object> line = new List<object>();
                IList<IList<object>> grid = new List<IList<object>>();
                line.Add(DateTime.Now.ToString("dMMMyy H:mm:ss"));
                line.Add($"{_name}: {msg}");
                line.Add(System.Environment.MachineName);
                grid.Add(line);
                Console.Clear();
                Console.WriteLine(msg);
                bool success = await Spreadsheet.AppendRows("Logging", grid);
                if (!success)
                {
                    string boom = $"{DateTime.Now.ToString("dMMMyy H:mm:ss")}: {msg}";
                    Console.WriteLine(boom);
                }
            }
        }
    }
}

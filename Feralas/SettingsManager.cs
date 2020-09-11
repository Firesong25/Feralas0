using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Generic
{
    public class SettingsManager
    {
        public string db_host = string.Empty;
        public string db_name = string.Empty;
        public string db_user = string.Empty;
        public string db_password = string.Empty;
        public string pg_user_passwd = string.Empty;
        public string url = string.Empty;
        public string userName = string.Empty;
        public string fmn = string.Empty;
        public string timezone = string.Empty;
        public static DateTime nowTime = DateTime.Now;
        public DateTime startTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 9, 30, 0, 0);
        public DateTime endTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 9, 30, 0, 0);
        public DateTime scanOneTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 9, 30, 0, 0);
        public DateTime stopLossTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 9, 30, 0, 0);
        public double wodge = 0;
        public double stopLossPercent = 0;
        public double stopLossTotal = 0;
        public double stopLossPerShare = 0;
        public bool runRightNow = false;
        public bool scanOneDone = false;
        public double snapshot_total = 0;
        public string polygonIoKey = string.Empty;

        

        public async Task<bool> Refresh()
        {
            try
            {
                var grid = await Spreadsheet.GetData("Settings", "A1:B100");

                foreach (List<string> pair in grid)
                {
                    string key = pair[0].ToString();
                    object value = pair[1];

                    if (key.Contains("db_host"))
                    {
                        db_host = value.ToString();
                        continue;
                    }

                    if (key.Contains("db_name"))
                    {
                        db_name = value.ToString();
                        continue;
                    }

                    if (key.Contains("db_user"))
                    {
                        db_user = value.ToString();
                        continue;
                    }

                    if (key.Contains("db_password"))
                    {
                        db_password = value.ToString();
                        continue;
                    }

                    if (key.Contains("pg_user_passwd"))
                    {
                        pg_user_passwd = value.ToString();
                        continue;
                    }

                    if (key.Contains("url"))
                    {
                        url = value.ToString();
                        continue;
                    }

                    if (key.Contains("userName"))
                    {
                        userName = value.ToString();
                        continue;
                    }

                    if (key.Contains("timezone"))
                    {
                        timezone = value.ToString();
                        continue;
                    }

                    if (key.Contains("polygonIoKey"))
                    {
                        polygonIoKey = value.ToString();
                        continue;
                    }

                    if (key.Contains("wodge"))
                    {
                        wodge = Convert.ToDouble(value);
                        continue;
                    }

                    if (key.Contains("stop_loss_percent"))
                    {
                        stopLossPercent = Convert.ToDouble(value);
                        continue;
                    }

                    if (key.Contains("stop_loss_total"))
                    {
                        stopLossTotal = Convert.ToDouble(value);
                        continue;
                    }

                    if (key.Contains("stop_loss_per_share"))
                    {
                        stopLossPerShare = Convert.ToDouble(value);
                        continue;
                    }

                    if (key.Contains("runRightNow"))
                    {
                        runRightNow = (bool)value;
                        continue;
                    }

                    if (key.Contains("scanOneDone"))
                    {
                        scanOneDone = (bool)value;
                        continue;
                    }

                    if (key.Contains("startTime"))
                    {
                        DateTime.TryParse(value.ToString(), out startTime);
                        continue;
                    }

                    if (key.Contains("endTime"))
                    {
                        DateTime.TryParse(value.ToString(), out endTime);
                        continue;
                    }

                    if (key.Contains("scanOneTime"))
                    {
                        DateTime.TryParse(value.ToString(), out scanOneTime);
                        continue;
                    }

                    if (key.Contains("stopLossTime"))
                    {
                        DateTime.TryParse(value.ToString(), out stopLossTime);
                        continue;
                    }

                    if (key.Contains("snapshot_total"))
                    {
                        snapshot_total = Convert.ToDouble(value);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                string boom = $"Database.Refresh(): {ex.Message}";
                await Logmaker.RecordAsync(boom);
            }

            // has it worked?
            if (db_password.Length > 1)
                return true;

            return false;

        }
    }
}

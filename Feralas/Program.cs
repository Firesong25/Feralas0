using Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Feralas
{
    class Program
    {
        public static async Task ShowTime(SettingsManager settings)
        {
            DateTime tradingDayStart = new DateTime(2020, 8, 13, 9, 30, 15);
            TradeManager trader = new TradeManager(settings, tradingDayStart);
            trader.JournalOfPrices = await trader.CreateJournal();
            StockQuote sq = trader.JournalOfPrices[0];
            Holding h = new Holding();
            Console.WriteLine(h.EpochConvertor(sq.EpochTime));
        }
        public static async Task<int> TestParameters(SettingsManager settings,
            short month,
            short day,
            double startDelay,
            double checkPointDelay,
            double buyDelay,
            double stopLossDelay,
            double sellTimeDelay,
            double stopLoss,
            int total)
        {
            settings.stopLossPerShare = stopLoss;
            DateTime tradingDayStart = new DateTime(2020, month, day, 9, 30, 15);
            TradeManager trader = new TradeManager(settings, tradingDayStart);
            trader.JournalOfPrices = await trader.CreateJournal();
            if (trader.JournalOfPrices.Count == 0)
                return total;
            DateTime startTime = tradingDayStart.AddMinutes(startDelay);
            DateTime checkPoint = tradingDayStart.AddMinutes(checkPointDelay);
            DateTime buyTime = tradingDayStart.AddMinutes(buyDelay);
            DateTime stopLossTime = tradingDayStart.AddMinutes(stopLossDelay);
            DateTime sellTime = tradingDayStart.AddMinutes(sellTimeDelay);
            Portfolio portfolio = await trader.CreatePortfolio(startTime, checkPoint, buyTime);
            await trader.StopLoss(portfolio, stopLossTime, sellTime);
            double profit = await trader.ValuePortfolio(sellTime);
            total += (int)Math.Round(profit);
            if (profit > 0)
            {
                Console.WriteLine($"Profit is ${Math.Round(profit)} on {day}th for total to date ${total}.");
                // Console.WriteLine($"Peak potential profit of ${Math.Round(portfolio.PeakValue - portfolio.Cost)}.");
            }
            else
            {
                Console.WriteLine($"LOSS of ${Math.Round(profit)} on {day}th for total to date ${total}.");
            }
            return total;
        }
    
        public static async Task Main(string[] args)
        {            
            Console.Clear();
            string name =
                    Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"{name} starting at {DateTime.Now.ToLongTimeString()}.");
            SettingsManager settings = new SettingsManager();                        
            await settings.Refresh();
            //await ShowTime(settings);

            DateTime nowUtc = DateTime.UtcNow;
            string easternZoneId = "America/New_York";
            // Retrieve the time zone for Eastern Standard Time (New York time).
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
            DateTime targetTime = TimeZoneInfo.ConvertTime(nowUtc, est);

            int year = targetTime.Year;
            int month = targetTime.Month;
            int day = targetTime.Day;

            // DateTime tradingDayStart = new DateTime(year, month, day, 9, 30, 15);


            double startDelay = 5;
            double checkPointDelay = 10;
            double buyDelay = 15;
            double stopLossDelay = 20;
            double sellTimeDelay = 385;
            double stopLoss = 0;
            int total = 0;

            total = await TestParameters(settings,
                9,
                4,
                startDelay,
                checkPointDelay,
                buyDelay,
                stopLossDelay,
                sellTimeDelay,
                stopLoss,
                total);


            sw.Stop();

            Console.WriteLine($"For investment of ${settings.wodge * 10} result is ${total}.");
            Console.WriteLine($"{name} ending at {DateTime.Now.ToLongTimeString()}");
            Console.WriteLine($"All operations took {sw.ElapsedMilliseconds / 1000} seconds.");
        }
    }
}

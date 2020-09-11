using Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace Feralas
{
    public class TradeManager
    {
        SettingsManager settings = new SettingsManager();
        public List<StockQuote> JournalOfPrices = new List<StockQuote>();
        short year = 0;
        short month = 0;
        short day = 0;
        public Portfolio Investment;
        string connectionString = string.Empty;
        int numberOfHoldings = 10;
        double transactionCosts = 0;
        sagorneContext context = new sagorneContext();

        public TradeManager(SettingsManager settingsManager, DateTime tradingDay)
        {
            settings = settingsManager;
            year = (short)tradingDay.Year;
            month = (short)tradingDay.Month;
            day = (short)tradingDay.Day;
            transactionCosts = 25 * numberOfHoldings;            
            context.ConnectionString = $"Host={settings.db_host};" +
                                    $"Username={settings.db_user};" +
                                    $"Password={settings.db_password};" +
                                    $"Database={settings.db_name}";

        }

        public async Task<Portfolio> StopLoss(Portfolio portfolio, DateTime stopLossTime, DateTime sellTime)
        {
            await CreateJournal();
            Holding ho = new Holding();
            long startStopLoss = await GetEpochTime(stopLossTime);
            long timeToSell = await GetEpochTime(sellTime);
            List<long> epochs = await GetEpochsList();
            portfolio.Holdings = await context.Holdings.Where(o => o.Day == day).ToListAsync();

            foreach (long timeSlot in epochs)
            {
                
                // Console.WriteLine(ho.EpochConvertor(timeSlot));
                if (timeSlot <= startStopLoss || timeSlot >= timeToSell)
                    continue;
                
                List<string> looper = (from s in portfolio.Holdings select s.Epic).ToList();
                foreach (string epic in looper)
                {
                    Holding holding = (from h in portfolio.Holdings
                                       where (h.Epic == epic)
                                       select h).FirstOrDefault();

                    if (holding.TimeSold > 0)
                        continue;

                    StockQuote sq = (from s in JournalOfPrices
                                     where (s.EpochTime == timeSlot && s.Epic == epic)
                                     select s).FirstOrDefault();

                    if (holding.PeakPrice == 0 || holding.PeakPrice < sq.Ask)
                    {
                        holding.PeakPrice = sq.Ask;
                    }

                    if (sq.Ask < holding.PricePaid)
                    {
                        int nextTimeSlot = epochs.IndexOf(timeSlot) + 1;
                        holding.TimeToSell = epochs[nextTimeSlot];
                        double invested = holding.PricePaid * holding.Quantity;
                        double worth = sq.Ask * holding.Quantity;
                        double lostValue = worth - invested;
                        if (lostValue < settings.stopLossPerShare)
                        {
                            int nxtTime = epochs.IndexOf((long)holding.TimeToSell);
                            StockQuote stockToSell = (from s in JournalOfPrices
                                             where (s.EpochTime == epochs[nxtTime] &&
                                             s.Epic == holding.Epic)
                                             select s).FirstOrDefault();
                            holding.SoldPrice = stockToSell.Ask;
                            holding.TimeSold = timeSlot;
                            Console.WriteLine($"Selling {holding.Epic} at {holding.EpochConvertor(timeSlot)}.");
                            portfolio.Holdings.Remove(holding);
                            context.Holdings.Update(holding);
                            await context.SaveChangesAsync();
                        }
                    }
                }

            }            
            return portfolio;
        }

        
        public async Task<double> ValuePortfolio(DateTime timeToSell)
        {
            List<Holding> portfolio = await context.Holdings.Where(o => o.Day == day).ToListAsync();
            double portfolioCost = 0;
            double totalValue = 0;
            List<StockQuote> PurchaseTime = await GetSnapshot(timeToSell);
            await Task.Run(() =>
            {
                foreach (Holding shareBought in portfolio)
                {
                    string epic = shareBought.Epic;
                    int quantity = shareBought.Quantity;
                    portfolioCost += shareBought.PricePaid * shareBought.Quantity;
                    if (shareBought.TimeSold == 0)
                    {
                        StockQuote snap = (from s in PurchaseTime
                                           where
                                           s.Epic == epic
                                           select s).FirstOrDefault();
                        totalValue += snap.Bid * quantity;
                    }
                    else
                    {
                    totalValue += shareBought.SoldPrice * shareBought.Quantity;
                    }
                }
            });
            return System.Math.Round(totalValue - portfolioCost, 2) - transactionCosts;
        }

        public async Task<Portfolio> CreatePortfolio(DateTime timeToStartScan,
            DateTime timeCheckpoint,
            DateTime timeToBuy)
        {
            List<string> epics = await GetTopTenGainers(timeToStartScan, timeCheckpoint, timeToBuy);
            long time = await GetEpochTime(timeToBuy);
            StringBuilder sb = new StringBuilder();
             
            List<StockQuote> PurchaseTime = await GetSnapshot(timeToBuy);
            double wodge = settings.wodge;
            double totalSpent = 0;
            Portfolio shares = new Portfolio(new List<Holding>(), totalSpent);
            shares.Holdings = await context.Holdings.Where(o => o.Day == day).ToListAsync();

            foreach (Holding h in shares.Holdings)
            {
                h.TimeSold = 0;
            }


            if (shares.Holdings.Count < numberOfHoldings)
            {
                foreach (string epic in epics)
                {
                    if (shares.Holdings.Count >= numberOfHoldings)
                        break;

                    StockQuote snap = (from s in PurchaseTime where s.Epic == epic select s).FirstOrDefault();
                    double cost = snap.Ask;
                    short quantity = Convert.ToInt16(System.Math.Floor(wodge / cost));
                    Holding shareBought = new Holding(epic, time, cost, quantity);
                    if (!shares.Holdings.Contains(shareBought))
                    {
                        shares.Holdings.Add(shareBought);
                        totalSpent += quantity * cost;
                        sb.Append(shareBought.Epic);
                        sb.Append(", ");
                    }
                }
                await context.Holdings.AddRangeAsync(shares.Holdings);
                await context.SaveChangesAsync();
            }
            //Console.WriteLine(sb.ToString());
            return shares;
        }


        public async Task<List<string>> GetTopTenGainers(DateTime startTime,
                DateTime checkPointTime,
                DateTime endTime)
        {
            List<string> epicsGainers = new List<string>();
            List<StockQuote> firstSlice = await GetSnapshot(startTime);
            List<StockQuote> checkPointSlice = await GetSnapshot(checkPointTime);
            List<StockQuote> secondSlice = await GetSnapshot(endTime);
            Dictionary<string, double> gainers = new Dictionary<string, double>();

            foreach (StockQuote p in secondSlice)
            {
                var firstSnap = (from company in firstSlice
                                 where company.Epic == p.Epic
                                 select company).
                                 FirstOrDefault();
                var checkPointSnap = (from company in checkPointSlice
                                      where company.Epic == p.Epic
                                      select company).
                                 FirstOrDefault();

                if (firstSnap == null || checkPointSnap == null)
                    continue;

                if (p.Ask > firstSnap.Ask && p.Ask > checkPointSnap.Ask)
                {
                    gainers[p.Epic] =
                        System.Math.Round((p.Ask - checkPointSnap.Ask) / checkPointSnap.Ask * 100, 2);
                }
            }

            int count = 0;
            foreach (KeyValuePair<string, double> pair in gainers.
                OrderByDescending(key => key.Value))
            {
                if (count >= numberOfHoldings)
                {
                    break;
                }
                epicsGainers.Add(pair.Key);
                count++;
            }
            return epicsGainers;
        }

        public async Task<List<StockQuote>> GetSnapshot(DateTime time)
        {
            if (JournalOfPrices.Count == 0)
                throw new JournalNotFoundException("No data in journal of prices.");

            long epoch = await GetEpochTime(time);

            List<StockQuote> snap = new List<StockQuote>();
            await Task.Run(() =>
            {
                snap = JournalOfPrices.Where(p => p.EpochTime == epoch).OrderBy(s => s.Epic).ToList();
            });

            return snap;
        }

        public async Task<long> GetEpochTime(DateTime when)
        {
            if (JournalOfPrices.Count == 0)
                throw new JournalNotFoundException("No data in journal of prices.");

            DateTimeOffset time2epoch = new DateTimeOffset(when,
                  TimeZoneInfo.FindSystemTimeZoneById("America/New_York").GetUtcOffset(when));
            long unixWhen = time2epoch.ToUnixTimeSeconds();
            List<long> epochs = await GetEpochsList();
            long lastEpoch = epochs[epochs.Count - 1];

            if (unixWhen > lastEpoch)
            {
                // wtf do I do?
                return lastEpoch;
            }

            long epochTime = 0;  // this is in case 
            long upperEpoch = 0;

            foreach (long epoch in epochs)
            {
                if (epoch < unixWhen && epochs.IndexOf(epoch) < epochs.Count)
                {
                    upperEpoch = epochs[epochs.IndexOf(epoch) + 1];
                    if (upperEpoch - unixWhen < unixWhen - epoch)
                        epochTime = upperEpoch;
                    else
                        epochTime = epoch;
                }
            }
            return epochTime;
        }

           
        public async Task<List<StockQuote>> CreateJournal()
        {
            if (JournalOfPrices.Count > 0)
                return JournalOfPrices;

            JournalOfPrices = await context.Stockquotes.Where(o => o.Day == day).ToListAsync();

            return JournalOfPrices;
        }

        async Task<List<long>> GetEpochsList()
        {
            List<long> unixTimes = new List<long>();
            await Task.Run(() =>
            {
                foreach (StockQuote snap in JournalOfPrices)
                {
                    if (!unixTimes.Contains(snap.EpochTime))
                    {
                        unixTimes.Add(snap.EpochTime);
                    }
                }
                unixTimes.Sort();
            });

            return unixTimes;
        }
    }
}

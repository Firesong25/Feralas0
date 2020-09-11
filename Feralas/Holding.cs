using System;

namespace Feralas
{
    public partial class Holding
    {
        public Holding() { }
        public Holding(string epicName, long purchaseTime, double price, short amount)
        {
            Epic = epicName;
            TimeBought = purchaseTime;
            PricePaid = price;
            Quantity = amount;
            DateTime tradeTime = EpochConvertor(purchaseTime);
            Year = (short)tradeTime.Year;
            Month = (short)tradeTime.Month;
            Day = (short)tradeTime.Day;
            ProcessId = (short)System.Diagnostics.Process.GetCurrentProcess().Id;

        }
        public int Id { get; set; }
        public string Epic { get; set; }
        public long TimeBought { get; set; }
        public long TimeToSell { get; set; }
        public long TimeSold { get; set; }
        public double PricePaid { get; set; }
        public double PeakPrice { get; set; }
        public double SoldPrice { get; set; }
        public short Quantity { get; set; }
        public short ProcessId { get; set; }
        public short Year { get; set; }
        public short Month { get; set; }
        public short Day { get; set; }

        public DateTime EpochConvertor(long epoch)
        {
            string easternZoneId = "America/New_York";
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
            DateTimeOffset value =
               DateTimeOffset.FromUnixTimeSeconds(epoch);
            DateTime scanTime = value.DateTime;
            DateTime targetTime = TimeZoneInfo.ConvertTime(scanTime, easternZone);
            return targetTime;
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feralas
{
    public partial class StockQuote
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("epic")]
        [StringLength(15)]
        public string Epic { get; set; }
        // If you are selling a stock, you are going to get the bid price.
        [Column("bid")]
        public double Bid { get; set; }
        // If you are buying a stock you are going to get the ask price. 
        [Column("ask")]
        public double Ask { get; set; }
        [Column("epoch_time")]
        public long EpochTime { get; set; }
        [Column("timestamp")]
        public long Timestamp { get; set; }
        [Column("year")]
        public short Year { get; set; }
        [Column("month")]
        public short Month { get; set; }
        [Column("day")]
        public short Day { get; set; }
    }
}

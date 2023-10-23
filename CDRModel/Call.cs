using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDRModel
{
    public enum CallType
    {
        Domestic = 1,
        International = 2
    }

    public class Call
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        [Column("caller_id")]
        public string? CallerId { get; set; }
        [Required]
        [MaxLength(20)]
        public string? Recipient { get; set; }
        [Column("call_date")]
        public DateTime CallDate { get; set; }
        [Column("end_time")]
        public TimeSpan EndTime { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        [Precision(18, 3)]
        public decimal Cost { get; set; }
        [Required]
        [MaxLength(33)]
        public string? Reference { get; set; }
        [Required]
        [MaxLength(3)]
        public string? Currency { get; set; }
        public CallType Type { get; set; }
    }
}

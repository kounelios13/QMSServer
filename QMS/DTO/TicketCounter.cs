using System.ComponentModel.DataAnnotations;

namespace QMS.DTO
{
    public class TicketCounter
    {
        [Key]
        public int Id { get; set; } = 1;
        public long CurrentNumber { get; set; } = 0;
        public DateTime LastUpdated { get; set; }
    }
}

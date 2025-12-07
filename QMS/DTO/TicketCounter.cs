using System.ComponentModel.DataAnnotations;

namespace QMS.DTO
{
    public class TicketCounter
    {
        [Key]
        public int Id { get; set; }
        public long CurrentNumber { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

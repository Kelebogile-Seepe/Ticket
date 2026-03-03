
namespace BeyonceConcert.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string? BuyerName { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string SeatNumber { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal Price { get; set; }
        public bool IsBooked { get; set; }
    }
}

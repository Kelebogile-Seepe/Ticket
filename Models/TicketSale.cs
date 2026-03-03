using System;
namespace BeyonceConcert.Models
{
    public class TicketSale
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public string UserId { get; set; }

        public string BuyerName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }

        public string SeatNumber { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }

        public DateTime SaleDate { get; set; }
    }

}

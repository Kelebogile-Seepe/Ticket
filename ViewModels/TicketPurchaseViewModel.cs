using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using BeyonceConcert.Models;

namespace BeyonceConcert.ViewModels
{
    public class TicketPurchaseViewModel
    {
        // Purchase form
        [Required(ErrorMessage = "Buyer name is required")]
        public string BuyerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } = string.Empty;

        [Range(1, 120, ErrorMessage = "Enter a valid age")]
        public int Age { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "Select a seat")]
        public int SelectedTicketId { get; set; }

        // Seats listing for the form
        public List<Ticket> AvailableTickets { get; set; } = new();

        // Recent sales table
        public List<TicketSale> RecentSales { get; set; } = new();

        // Optional: selected ticket metadata (helps views)
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }

        // Note: The following SQL query is not C# code and should not be included in the C# file.
        /*
        SELECT TOP (0) * FROM dbo.TicketSales;
        */
    }
}

   

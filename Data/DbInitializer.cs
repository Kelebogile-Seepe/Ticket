using System.Linq;
using BeyonceConcert.Models;

namespace BeyonceConcert.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Tickets.Any())
                return;

            for (char row = 'A'; row <= 'G'; row++)
            {
                for (int seat = 1; seat <= 20; seat++)
                {
                    context.Tickets.Add(new Ticket
                    {
                        SeatNumber = $"{row}{seat}",
                        Category = row <= 'C' ? "VIP" : "General",
                        Price = row <= 'C' ? 3000 : 500,
                        IsBooked = false
                    });
                }
            }

            context.SaveChanges();
        }
    }
}

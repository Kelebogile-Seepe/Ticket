using BeyonceConcert.Data;
using BeyonceConcert.Models;
using BeyonceConcert.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace BeyonceConcert.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<TicketsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET
        public async Task<IActionResult> SelectSeat()
        {
            var model = new TicketPurchaseViewModel();
            await PopulateListsAsync(model);
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectSeat(TicketPurchaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateListsAsync(model);
                return View(model);
            }

            // Use a transaction so the update + insert are atomic
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Try to mark the ticket as booked in a single conditional UPDATE.
                // ExecuteUpdateAsync returns the number of affected rows.
                var updatedCount = await _context.Tickets
                    .Where(t => t.Id == model.SelectedTicketId && !t.IsBooked)
                    .ExecuteUpdateAsync(set => set.SetProperty(t => t.IsBooked, true));

                if (updatedCount == 0)
                {
                    // Seat already booked (or doesn't exist)
                    ModelState.AddModelError(string.Empty, "Seat already booked.");
                    await PopulateListsAsync(model);
                    return View(model);
                }

                // Fetch ticket details (after the successful conditional update)
                var ticket = await _context.Tickets
                    .AsNoTracking()
                    .Where(t => t.Id == model.SelectedTicketId)
                    .Select(t => new { t.SeatNumber, t.Category, t.Price })
                    .SingleOrDefaultAsync();

                if (ticket == null)
                {
                    // Unlikely because update succeeded, but guard anyway
                    ModelState.AddModelError(string.Empty, "Selected seat could not be found.");
                    await PopulateListsAsync(model);
                    return View(model);
                }

                _context.TicketSales.Add(new TicketSale
                {
                    TicketId = model.SelectedTicketId,
                    UserId = _userManager.GetUserId(User),
                    BuyerName = model.BuyerName,
                    Gender = model.Gender,
                    Age = model.Age,
                    SeatNumber = ticket.SeatNumber,
                    Category = ticket.Category,
                    Price = ticket.Price,
                    SaleDate = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(SelectSeat));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while booking ticket {TicketId}", model.SelectedTicketId);
                // Attempt to rollback if transaction still open
                try { await transaction.RollbackAsync(); } catch { /* swallow */ }

                ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again.");
                await PopulateListsAsync(model);
                return View(model);
            }
        }

        // Helper to reduce duplication and use async queries
        private async Task PopulateListsAsync(TicketPurchaseViewModel model)
        {
            model.AvailableTickets = await _context.Tickets
                .Where(t => !t.IsBooked)
                .OrderBy(t => t.SeatNumber)
                .ToListAsync();

            model.RecentSales = await _context.TicketSales
                .OrderByDescending(s => s.SaleDate)
                .Take(7)
                .ToListAsync();
        }
    }
}

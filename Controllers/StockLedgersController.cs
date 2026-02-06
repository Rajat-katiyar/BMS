using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessManagementSystem.Data;

namespace BusinessManagementSystem.Controllers
{
    public class StockLedgersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockLedgersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StockLedgers
        public async Task<IActionResult> Index()
        {
            var stockLedgers = await _context.StockLedgers
                .Include(s => s.Product)
                .OrderByDescending(s => s.TransactionDate)
                .ToListAsync();
            return View(stockLedgers);
        }
    }
}

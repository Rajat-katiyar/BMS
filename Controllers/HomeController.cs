using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessManagementSystem.Models;
using BusinessManagementSystem.Data;
using BusinessManagementSystem.ViewModels;
using System.Diagnostics;

namespace BusinessManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalVendors = await _context.Vendors.CountAsync(),
                TotalEmployees = await _context.Employees.CountAsync(),
                TotalPurchases = await _context.Purchases.CountAsync(),
                TotalStock = await _context.Products.SumAsync(x => x.CurrentStock),
                TodayPresent = await _context.EmployeeAttendances.CountAsync(x => x.AttendanceDate == DateTime.Today && x.Status == "Present"),
                IsTodayHoliday = await _context.Holidays.AnyAsync(h => h.HolidayDate == DateTime.Today && h.IsActive),

                RecentPurchases = await _context.Purchases
                    .Include(p => p.Vendor)
                    .OrderByDescending(x => x.PurchaseDate)
                    .Take(5)
                    .Select(x => new RecentPurchaseVM
                    {
                        PurchaseId = x.PurchaseId,
                        VendorName = x.Vendor != null ? x.Vendor.VendorName : "Unknown",
                        PurchaseDate = x.PurchaseDate,
                        TotalAmount = x.TotalAmount
                    }).ToListAsync(),

                LowStockProducts = await _context.Products
                    .Where(x => x.CurrentStock <= x.MinimumStockLevel && x.IsActive)
                    .OrderBy(x => x.CurrentStock)
                    .Select(x => new LowStockProductVM
                    {
                        ProductName = x.ProductName,
                        Quantity = x.CurrentStock,
                        MinStock = x.MinimumStockLevel,
                        Unit = x.Unit
                    }).ToListAsync()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

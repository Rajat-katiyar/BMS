using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessManagementSystem.Data;
using BusinessManagementSystem.Models;

namespace BusinessManagementSystem.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.Vendor)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();
            return View(purchases);
        }

        // GET: Purchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.Vendor)
                .FirstOrDefaultAsync(m => m.PurchaseId == id);
            
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // GET: Purchases/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.IsActive), "ProductId", "ProductName");
            ViewData["VendorId"] = new SelectList(_context.Vendors.Where(v => v.IsActive), "VendorId", "VendorName");
            return View();
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PurchaseId,VendorId,ProductId,Quantity,UnitPrice,InvoiceNumber,Notes,PurchaseDate")] Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                // Calculate total amount
                purchase.TotalAmount = purchase.Quantity * purchase.UnitPrice;
                purchase.CreatedDate = DateTime.Now;

                _context.Add(purchase);
                
                // Update Product Stock
                var product = await _context.Products.FindAsync(purchase.ProductId);
                if (product != null)
                {
                    product.CurrentStock += purchase.Quantity;
                    _context.Update(product);

                    // Add Stock Ledger Entry
                    var ledger = new StockLedger
                    {
                        ProductId = purchase.ProductId,
                        TransactionType = "IN",
                        Quantity = purchase.Quantity,
                        BalanceQuantity = product.CurrentStock,
                        TransactionDate = purchase.PurchaseDate,
                        Reference = $"Purchase #{purchase.InvoiceNumber}",
                        Remarks = "Purchase Entry",
                        CreatedDate = DateTime.Now
                    };
                    _context.Add(ledger);
                }

                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Purchase recorded and stock updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products.Where(p => p.IsActive), "ProductId", "ProductName", purchase.ProductId);
            ViewData["VendorId"] = new SelectList(_context.Vendors.Where(v => v.IsActive), "VendorId", "VendorName", purchase.VendorId);
            return View(purchase);
        }

        // GET: Purchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.Vendor)
                .FirstOrDefaultAsync(m => m.PurchaseId == id);
            
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase != null)
            {
                // Revert Stock
                var product = await _context.Products.FindAsync(purchase.ProductId);
                if (product != null)
                {
                    product.CurrentStock -= purchase.Quantity;
                    _context.Update(product);

                    // Add Stock Ledger Entry (Reversal)
                    var ledger = new StockLedger
                    {
                        ProductId = purchase.ProductId,
                        TransactionType = "OUT",
                        Quantity = purchase.Quantity,
                        BalanceQuantity = product.CurrentStock,
                        TransactionDate = DateTime.Now,
                        Reference = $"Revert Purchase #{purchase.InvoiceNumber}",
                        Remarks = "Purchase Deleted/Reverted",
                        CreatedDate = DateTime.Now
                    };
                    _context.Add(ledger);
                }

                _context.Purchases.Remove(purchase);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Purchase deleted and stock reverted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

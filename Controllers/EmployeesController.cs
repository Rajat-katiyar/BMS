using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessManagementSystem.Data;
using BusinessManagementSystem.Models;

namespace BusinessManagementSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper to check session
        private bool IsSessionActive()
        {
            return HttpContext.Session.GetString("UserId") != null;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");

            var employees = await _context.Employees
                .OrderByDescending(e => e.CreatedOn)
                .ToListAsync();
            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");

            if (id == null) return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            
            if (employee == null) return NotFound();

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,EmployeeCode,EmployeeName,Email,Phone,Designation,Salary")] Employee employee)
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Check if code exists
                if (_context.Employees.Any(e => e.EmployeeCode == employee.EmployeeCode))
                {
                    ModelState.AddModelError("EmployeeCode", "Employee Code already exists.");
                    return View(employee);
                }

                employee.CreatedOn = DateTime.Now;
                employee.IsActive = true;
                _context.Add(employee);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Employee added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");

            if (id == null) return NotFound();

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,EmployeeCode,EmployeeName,Email,Phone,Designation,Salary,IsActive,CreatedOn")] Employee employee)
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");

            if (id != employee.EmployeeId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Employee updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");

            if (id == null) return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            
            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");

            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                // Hard delete or Soft delete based on preference. 
                // User requirement implied list display, let's stick to Soft Delete for consistency with other modules
                // Or user example code didn't specify. Let's do Soft Delete to be safe, but code implies "Active" flag exists.
                // Wait, user SQL has IsActive BIT DEFAULT 1.
                // Let's do Soft Delete.
                
                employee.IsActive = false;
                _context.Update(employee);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Employee deactivated successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}

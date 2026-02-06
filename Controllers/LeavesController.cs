using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessManagementSystem.Data;
using BusinessManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusinessManagementSystem.Controllers
{
    public class LeavesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeavesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper
        private bool IsSessionActive()
        {
            return HttpContext.Session.GetString("UserId") != null;
        }

        private int? GetLoggedInEmployeeId()
        {
            var empId = HttpContext.Session.GetString("EmployeeId");
            return string.IsNullOrEmpty(empId) ? null : int.Parse(empId);
        }

        // GET: Leaves (My Leaves)
        public async Task<IActionResult> Index()
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");
            
            var empId = GetLoggedInEmployeeId();
            if (empId == null)
            {
                TempData["Error"] = "You must be mapped to an employee.";
                return RedirectToAction("Index", "Home");
            }

            var leaves = await _context.EmployeeLeaves
                .Include(l => l.LeaveType)
                .Where(l => l.EmployeeId == empId)
                .OrderByDescending(l => l.FromDate)
                .ToListAsync();

            return View(leaves);
        }

        // GET: Leaves/Create
        public IActionResult Create()
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");
            
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "LeaveTypeId", "LeaveName");
            return View();
        }

        // POST: Leaves/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeaveTypeId,FromDate,ToDate,Reason")] EmployeeLeave employeeLeave)
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");

            var empId = GetLoggedInEmployeeId();
            if (empId == null) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                // Logic
                employeeLeave.EmployeeId = empId.Value;
                employeeLeave.Status = "Pending";
                employeeLeave.AppliedOn = DateTime.Now;
                
                // Calculate Total Days
                employeeLeave.TotalDays = (employeeLeave.ToDate - employeeLeave.FromDate).Days + 1;

                if (employeeLeave.TotalDays <= 0)
                {
                    ModelState.AddModelError("ToDate", "To Date must be after From Date");
                }
                else
                {
                    _context.Add(employeeLeave);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Leave Applied Successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "LeaveTypeId", "LeaveName", employeeLeave.LeaveTypeId);
            return View(employeeLeave);
        }

        // GET: Leaves/Manage (For Managers/Admins)
        public async Task<IActionResult> Manage()
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");
            
            // In a real app, check role. Here assuming everyone can see for demo or implement role check.
            // Let's assume RoleId 1 is Admin.
            // if (HttpContext.Session.GetString("Role") != "Admin") ... 

            var leaves = await _context.EmployeeLeaves
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .OrderByDescending(l => l.AppliedOn)
                .ToListAsync();

            return View(leaves);
        }

        // POST: Leaves/Approve/5
        public async Task<IActionResult> Approve(int id)
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");

            var leave = await _context.EmployeeLeaves.FindAsync(id);
            if (leave != null)
            {
                leave.Status = "Approved";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Leave Approved!";
            }
            return RedirectToAction(nameof(Manage));
        }

        // POST: Leaves/Reject/5
        public async Task<IActionResult> Reject(int id)
        {
            if (!IsSessionActive()) return RedirectToAction("Login", "Account");

            var leave = await _context.EmployeeLeaves.FindAsync(id);
            if (leave != null)
            {
                leave.Status = "Rejected";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Leave Rejected!";
            }
            return RedirectToAction(nameof(Manage));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessManagementSystem.Data;
using BusinessManagementSystem.Models;

namespace BusinessManagementSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Attendance
        public async Task<IActionResult> Index()
        {
            var employeeIdString = HttpContext.Session.GetString("EmployeeId");
            if (string.IsNullOrEmpty(employeeIdString))
            {
                // If user doesn't have an employee mapped, maybe show error or redirect
                 TempData["Error"] = "You must be mapped to an employee to view attendance.";
                 return RedirectToAction("Index", "Home");
            }

            int employeeId = int.Parse(employeeIdString);
            var today = DateTime.Today;

            // Get today's attendance for status
            var todayAttendance = await _context.EmployeeAttendances
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.AttendanceDate == today);
            
            ViewBag.TodayAttendance = todayAttendance;

            // Get history
            var history = await _context.EmployeeAttendances
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.AttendanceDate)
                .ToListAsync();

            return View(history);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn()
        {
            var employeeIdString = HttpContext.Session.GetString("EmployeeId");
            if (string.IsNullOrEmpty(employeeIdString)) return RedirectToAction("Login", "Account");

            int empId = int.Parse(employeeIdString);
            var today = DateTime.Today;

            // Check for Holiday
            bool isHoliday = await _context.Holidays.AnyAsync(x => x.HolidayDate == today && x.IsActive);
            if (isHoliday)
            {
                 TempData["InfoMessage"] = "Today is a Holiday. Attendance is not required.";
                 return RedirectToAction(nameof(Index));
            }

            var attendance = await _context.EmployeeAttendances
                .FirstOrDefaultAsync(x => x.EmployeeId == empId && x.AttendanceDate == today);

            if (attendance == null)
            {
                attendance = new EmployeeAttendance
                {
                    EmployeeId = empId,
                    AttendanceDate = today,
                    CheckInTime = DateTime.Now,
                    Status = "Present"
                };

                _context.EmployeeAttendances.Add(attendance);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Checked In successfully!";
            }
            else
            {
                TempData["InfoMessage"] = "You have already checked in today.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            var employeeIdString = HttpContext.Session.GetString("EmployeeId");
            if (string.IsNullOrEmpty(employeeIdString)) return RedirectToAction("Login", "Account");

            int empId = int.Parse(employeeIdString);
            var today = DateTime.Today;

            // Check for Holiday
            bool isHoliday = await _context.Holidays.AnyAsync(x => x.HolidayDate == today && x.IsActive);
            if (isHoliday)
            {
                 TempData["InfoMessage"] = "Today is a Holiday. Operations are disabled.";
                 return RedirectToAction(nameof(Index));
            }

            var attendance = await _context.EmployeeAttendances
                .FirstOrDefaultAsync(x => x.EmployeeId == empId && x.AttendanceDate == today);

            if (attendance != null && attendance.CheckOutTime == null)
            {
                attendance.CheckOutTime = DateTime.Now;

                var duration = attendance.CheckOutTime.Value - attendance.CheckInTime.Value;
                var hours = duration.TotalHours;
                
                attendance.WorkingHours = (decimal)hours;
                
                // Logic: < 4 hours = Absent? Or just HalfDay? 
                // User logic: hours >= 9 ? "Present" : "HalfDay"
                // But initially set to "Present" on CheckIn. Let's update it.
                if (hours >= 9) 
                {
                    attendance.Status = "Present";
                }
                else
                {
                    attendance.Status = "HalfDay";
                }

                _context.Update(attendance);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Checked Out successfully!";
            }
            else
            {
                 TempData["ErrorMessage"] = "Cannot check out. Either not checked in or already checked out.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

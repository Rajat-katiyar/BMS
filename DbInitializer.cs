using BusinessManagementSystem.Data;
using BusinessManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessManagementSystem
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Seed Leave Types
            if (!context.LeaveTypes.Any())
            {
                var leaveTypes = new LeaveType[]
                {
                    new LeaveType { LeaveName = "Casual Leave", MaxDays = 12 },
                    new LeaveType { LeaveName = "Sick Leave", MaxDays = 10 },
                    new LeaveType { LeaveName = "Earned Leave", MaxDays = 15 },
                    new LeaveType { LeaveName = "Unpaid Leave", MaxDays = 0 }
                };
                context.LeaveTypes.AddRange(leaveTypes);
                context.SaveChanges();
            }
        }
    }
}

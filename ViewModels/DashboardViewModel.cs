using System;
using System.Collections.Generic;

namespace BusinessManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalVendors { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalPurchases { get; set; }
        public int TotalStock { get; set; }
        
        public int TodayPresent { get; set; }
        public bool IsTodayHoliday { get; set; }

        public List<RecentPurchaseVM> RecentPurchases { get; set; } = new List<RecentPurchaseVM>();
        public List<LowStockProductVM> LowStockProducts { get; set; } = new List<LowStockProductVM>();
    }

    public class RecentPurchaseVM
    {
        public int PurchaseId { get; set; }
        public string? VendorName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class LowStockProductVM
    {
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public int MinStock { get; set; }
        public string? Unit { get; set; }
    }
}

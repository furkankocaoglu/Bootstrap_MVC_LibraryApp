using bootstrapmvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bootstrapmvc.Areas.ManagerPanel.Data
{
    public class StudentBorrowHistoryViewModel
    {
        public Student Student { get; set; }
        public List<Borrow> BorrowHistory { get; set; }

        public decimal TotalPenalty { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bootstrapmvc.Areas.ManagerPanel.Data
{
    public class BorrowDetailViewModel
    {
        public string BookName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal Penalty { get; set; }
        public bool IsReturned { get; set; }
    }
}
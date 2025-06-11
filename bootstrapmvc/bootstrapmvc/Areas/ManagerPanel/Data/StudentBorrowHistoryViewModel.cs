using bootstrapmvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bootstrapmvc.Areas.ManagerPanel.Data
{
    public class StudentBorrowHistoryViewModel
    {
        public int StudentID { get; set; }   
        public string StudentName { get; set; }
        public string StudentSurname { get; set; }
        public List<BorrowDetailViewModel> BorrowHistory { get; set; }
        public decimal TotalPenalty { get; set; }
    }
}
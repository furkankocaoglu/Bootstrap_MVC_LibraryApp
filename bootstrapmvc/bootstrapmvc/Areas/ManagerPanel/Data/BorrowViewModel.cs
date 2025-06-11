using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bootstrapmvc.Areas.ManagerPanel.Data
{
    public class BorrowViewModel
    {
        public int ID { get; set; }
        public string StudentName { get; set; }
        public string StudentSurname { get; set; }
        public string StudentNumber { get; set; }
        public string BookName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }  
        public bool IsReturned { get; set; }
    }
}
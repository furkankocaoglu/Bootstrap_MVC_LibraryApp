using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bootstrapmvc.Areas.ManagerPanel.Data
{
    public class BlackListPenaltyViewModel
    {
        public decimal Penalty { get; set; }
        public string StudentName { get; set; }
        public string StudentSurname { get; set; }
        public string StudentNumber { get; set; }
        public string BookTitle { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int StudentID { get; set; }
    }
}
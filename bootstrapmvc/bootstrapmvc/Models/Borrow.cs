using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace bootstrapmvc.Models
{
    public class Borrow : Entity
    {
        public int StudentID { get; set; }
        [ForeignKey("StudentID")]
        public virtual Student Student { get; set; }

        public int BookID { get; set; }
        [ForeignKey("BookID")]
        public virtual Book Book { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.Now; //Kitabın ödünç alındığı tarih

        public DateTime? ReturnDate { get; set; } // Öğrencinin gerçekten kitabı teslim ettiği geri getirdiği tarih

        [Display(Name = "Teslim Edilmesi Gereken Tarih")]
        [Required(ErrorMessage = "Teslim tarihi seçilmelidir")]
        public DateTime DueDate { get; set; } // Kitabın en geç iade edilmesi gereken tarih
        public bool IsReturned { get; set; } = false; //Kitap iade edildi mi?

    }
}
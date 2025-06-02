using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bootstrapmvc.Models
{
    public class Student:Entity
    {
        public Student()
        {
            IsDeleted = false;
        }
        [Display(Name = "İsim")]
        [Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        [StringLength(75, ErrorMessage = "Bu alan en fazla 75 karakter olabilir")]
        public string Name { get; set; }

        [Display(Name = "Soyisim")]
        [StringLength(75, ErrorMessage = "Bu alan en fazla 75 karakter olabilir")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Öğrenci numarası zorunludur")]
        [StringLength(20, ErrorMessage = "Öğrenci numarası en fazla 20 karakter olabilir")]
        public string StudentNumber { get; set; }

        [StringLength(100, ErrorMessage = "Bölüm adı en fazla 100 karakter olabilir")]
        public string Department { get; set; }

        [StringLength(50, ErrorMessage = "Sınıf bilgisi en fazla 50 karakter olabilir")]
        public string Class { get; set; }

        [Phone(ErrorMessage = "Başında 0 olmadan geçerli bir telefon numarası giriniz")]
        [StringLength(10, ErrorMessage = "Telefon numarası en fazla 10 karakter olabilir")]
        public string PhoneNumber { get; set; }

        [StringLength(250, ErrorMessage = "Adres en fazla 250 karakter olabilir")]
        public string Address { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now; //kayıt tarihi

        [Display(Name = "E-Posta")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Bu alan 5 - 200 karakter arasında olabilir")]
        public string Mail { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
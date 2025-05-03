using System.ComponentModel.DataAnnotations;

namespace AMC_Generator.Models
{
    public class Owner
    {
        public int Id { get; set; }

        /* ---------- Identity ---------- */
        [Required, Display(Name = "الاسم الكامل")]
        [StringLength(100)]
        public string? FullName { get; set; }

        [Display(Name = "الجنسية")]
        [StringLength(50)]
        public string? Nationality { get; set; }


        /* ---------- Contact ---------- */
        [Display(Name = "مفتاح الدولة")]
        public string? PhoneCode { get; set; }     // e.g. "+971"
        [Display(Name = "رقم الهاتف")]
        [Phone, StringLength(25)]
        public string? Phone { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        /* ---------- Address ---------- */
        [Display(Name = "عنوان المراسلة")]
        [StringLength(200)]
        public string? MailingAddress { get; set; }

        [Display(Name = "المدينة")]
        [StringLength(60)]
        public string? City { get; set; }

        [Display(Name = "الدولة / الإمارة")]
        [StringLength(60)]
        public string? StateOrEmirate { get; set; }

        /* ---------- Navigation ---------- */
        public ICollection<Building>? Buildings { get; set; }
    }
}

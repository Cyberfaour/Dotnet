using System.ComponentModel.DataAnnotations;

namespace AMC_Generator.ModelViews
{
    public class BuildingWithOwnerVM
    {
        /* ---------- Owner ---------- */

        [Required, Display(Name = "اسم المالك")]
        [StringLength(100)]
        public string OwnerFullName { get; set; }

        [Display(Name = "مفتاح الهاتف")]
        [StringLength(6)]
        public string OwnerPhoneCode { get; set; } = "+971";

        [Display(Name = "هاتف المالك")]
        [StringLength(25)]
        public string OwnerPhone { get; set; }

        [Display(Name = "المدينة")]
        [StringLength(60)]
        public string OwnerCity { get; set; } = string.Empty;   // avoids NULL

        [Display(Name = "الإمارة / الدولة")]
        [StringLength(60)]
        public string OwnerState { get; set; } = string.Empty;

        [Display(Name = "البريد الإلكتروني")]
        [EmailAddress, StringLength(100)]
        public string OwnerEmail { get; set; }

        /* ---------- Building ---------- */

        [Required, Display(Name = "اسم المبنى")]
        [StringLength(100)]
        public string BuildingName { get; set; }

        [Display(Name = "العنوان")]
        [StringLength(200)]
        public string BuildingAddress { get; set; } = string.Empty;
    }
}

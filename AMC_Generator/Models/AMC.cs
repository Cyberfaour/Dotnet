using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMC_Generator.Models
{
    public enum AmcStatus { Active, Expiring, Expired, Terminated }

    public class AMC
    {
        public int Id { get; set; }

        [Display(Name = "رقم المشروع")]
        public string? ProjectNumber { get; set; }

        [Display(Name = "قيمة العقد"), Column(TypeName = "decimal(18,2)")]
        public decimal AMCValue { get; set; }

        [Display(Name = "تاريخ البدء"), DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "تاريخ الانتهاء"), DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        /* ---------- Tracking ---------- */

        [Display(Name = "تاريخ الإنهاء المبكر"), DataType(DataType.Date)]
        public DateTime? TerminatedOn { get; set; }

        [NotMapped]
        public AmcStatus Status =>
            TerminatedOn.HasValue ? AmcStatus.Terminated :
            EndDate.Date < DateTime.Today ? AmcStatus.Expired :
            EndDate.Date <= DateTime.Today.AddDays(30) ? AmcStatus.Expiring :
                                                        AmcStatus.Active;

        [NotMapped]
        public int DaysLeft =>
            (TerminatedOn ?? EndDate).Date.Subtract(DateTime.Today).Days;

        /* ---------- Relations ---------- */

        public int BuildingId { get; set; }
        public Building? Building { get; set; }

        /* ---------- Generated PDF ---------- */
        public string? PdfPath { get; set; }
    }
}

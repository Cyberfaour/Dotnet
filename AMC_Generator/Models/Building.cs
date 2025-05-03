using System.ComponentModel.DataAnnotations;

namespace AMC_Generator.Models
{
    public class Building
    {
        public int Id { get; set; }
        [Required, Display(Name = "اسم المبنى")]
        public string? Name { get; set; }
        [Display(Name = "العنوان")]
        public string? Address { get; set; }

        public int OwnerId { get; set; }
        public Owner? Owner { get; set; }

        public ICollection<AMC>? AMCs { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class CategoryFormViewModel
    {
        public int EventId { get; set; }

        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";
    }
}
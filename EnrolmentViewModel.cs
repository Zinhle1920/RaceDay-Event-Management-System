using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class EnrolmentViewModel
    {
        public int EventId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string EventName { get; set; } = "";

        public List<CategoryViewModel> Categories { get; set; } = new();
    }
}
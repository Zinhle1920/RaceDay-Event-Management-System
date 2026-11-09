namespace RaceDay.Mvc.Models
{
    public class EventTypeViewModel
    {
        public int EventTypeId { get; set; }
        public string TypeName { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class EventFormViewModel
    {
        public string EventName { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public double Distance { get; set; }
        public int EventTypeId { get; set; }

        public List<EventTypeViewModel> EventTypes { get; set; } = new();
    }

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string CategoryName { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
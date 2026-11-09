namespace RaceDay.Mvc.Models
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public double Distance { get; set; }
        public int EventTypeId { get; set; }
        public string EventType { get; set; } = "";
        public int OrganiserId { get; set; }
    }

    public class DashboardViewModel
    {
        public string FirstName { get; set; } = "";
        public List<EventViewModel> Events { get; set; } = new();
    }
}
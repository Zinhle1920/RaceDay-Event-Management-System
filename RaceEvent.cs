namespace RaceDay.Api.Models;

public class RaceEvent
{
    public int EventId { get; set; }

    public string EventName { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = "";
    public double Distance { get; set; }

    public int EventTypeId { get; set; }
    public EventType? EventType { get; set; }

    public int OrganiserId { get; set; }
    public AppUser? Organiser { get; set; }

    public ICollection<Category> Categories { get; set; }
        = new List<Category>();

    public ICollection<Enrolment> Enrolments { get; set; }
        = new List<Enrolment>();
}
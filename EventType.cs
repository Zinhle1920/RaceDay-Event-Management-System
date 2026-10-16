namespace RaceDay.Api.Models;

public class EventType
{
    public int EventTypeId { get; set; }

    public string TypeName { get; set; } = "";
    public string Description { get; set; } = "";

    public ICollection<RaceEvent> Events { get; set; }
        = new List<RaceEvent>();
}
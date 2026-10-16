namespace RaceDay.Api.Models;

public class RaceResult
{
    public int ResultId { get; set; }

    public int EnrolmentId { get; set; }
    public Enrolment? Enrolment { get; set; }

    public TimeSpan? FinishTime { get; set; }
    public int? FinishPosition { get; set; }
}
namespace RaceDay.Mvc.Models
{
    public class ParticipantEnrolmentViewModel
    {
        public int EnrolmentId { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = "";
    }
}
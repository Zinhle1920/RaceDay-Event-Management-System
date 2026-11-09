namespace RaceDay.Mvc.Models
{
    public class ResultViewModel
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public string EventName { get; set; } = "";
        public TimeSpan? FinishTime { get; set; }
        public int? FinishPosition { get; set; }
    }

    public class CreateResultViewModel
    {
        public int EnrolmentId { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? FinishPosition { get; set; }
    }
}
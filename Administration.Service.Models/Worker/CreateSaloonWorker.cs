namespace Administration.Service.Models.Worker
{
    public class CreateSaloonWorker
    {
        public Guid WorkerId { get; set; }
        public Guid SaloonId { get; set; }
        public IEnumerable<DayOfWeek> WorkingDays { get; set; } = new List<DayOfWeek>();
    }
}

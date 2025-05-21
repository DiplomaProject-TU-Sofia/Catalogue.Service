namespace Catalogue.Service.Models.Worker
{
    public class CreateSaloonWorkerModel
    {
        public Guid WorkerId { get; set; }
        public Guid SaloonId { get; set; }
        public IEnumerable<DayOfWeek> WorkingDays { get; set; } = new List<DayOfWeek>();
    }
}

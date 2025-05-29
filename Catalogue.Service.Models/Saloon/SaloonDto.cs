using Catalogue.Service.Models.Worker;

namespace Catalogue.Service.Models.Saloon
{
    public class SaloonDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

		public Dictionary<DayOfWeek, WorkingHourRange> WorkHours { get; set; }

		public IEnumerable<WorkerDto> Workers { get; set; }
    }
}

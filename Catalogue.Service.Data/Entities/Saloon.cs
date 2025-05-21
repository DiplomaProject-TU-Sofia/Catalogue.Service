using Catalogue.Service.Models.Saloon;

namespace Catalogue.Service.Data.Entities
{
    public class Saloon
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string Location { get; set; } = null!;
		public Dictionary<DayOfWeek, WorkingHourRange> WorkHours { get; set; }

		//Navigation properties
		public ICollection<SaloonWorker> SaloonWorkers { get; set; } = new List<SaloonWorker>();
	}
}

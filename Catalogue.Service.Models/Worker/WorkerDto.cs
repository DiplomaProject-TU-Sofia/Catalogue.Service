using Catalogue.Service.Models.Saloon;

namespace Catalogue.Service.Models.Worker
{
	public class WorkerDto
	{
		public Guid Id { get; set; }

		public string? FirstName { get; set; }

		public string? LastName { get; set; }

		public List<DayOfWeek> WorkingDays { get; set; }

		public IEnumerable<SaloonDto> Saloons { get; set; } 
	}
}

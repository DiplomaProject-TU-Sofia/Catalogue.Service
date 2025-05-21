using Catalogue.Service.Models.Saloon;
using Catalogue.Service.Models.Service;

namespace Catalogue.Service.Models.Worker
{
	public class WorkerDetailsDto
	{
		public Guid Id { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }

		public IEnumerable<SaloonDto> Saloons { get; set; } = new List<SaloonDto>();

		public IEnumerable<ServiceDto> Services { get; set; } = new List<ServiceDto>();
	}
}

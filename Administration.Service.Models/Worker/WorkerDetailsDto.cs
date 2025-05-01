using Administration.Service.Models.Saloon;
using Administration.Service.Models.Service;

namespace Administration.Service.Models.Worker
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

using Administration.Service.Models.Worker;

namespace Administration.Service.Models.Service
{
	public  class ServiceDetailsDto
	{
		public Guid ServiceId { get; set; }
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public TimeSpan Duration { get; set; }

		public IEnumerable<WorkerDto> Worker { get; set; } = new List<WorkerDto>();
	}
}

namespace Catalogue.Service.Models.Service
{
	public class UpdateService
	{
		public Guid ServiceId { get; set; }
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public string? ImageName { get; set; }
		public TimeSpan Duration { get; set; }
	}
}

namespace Catalogue.Service.Models.Service
{
	public class ServiceDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public string? ImageName { get; set; }
	}
}

namespace Catalogue.Service.Models.Saloon
{
	public class UpdateSaloon
	{
		public Guid SaloonId { get; set; }
		public string Name { get; set; } = null!;
		public string Location { get; set; } = null!;
		public Dictionary<DayOfWeek, WorkingHourRange> WorkHours { get; set; }
	}
}

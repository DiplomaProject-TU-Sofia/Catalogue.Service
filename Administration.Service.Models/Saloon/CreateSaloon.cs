namespace Administration.Service.Models.Saloon
{
    public class CreateSaloon
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public Dictionary<DayOfWeek, WorkingHourRange> WorkHours { get; set; }
    }
}

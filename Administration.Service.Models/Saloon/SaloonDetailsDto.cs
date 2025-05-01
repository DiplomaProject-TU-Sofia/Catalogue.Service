using Administration.Service.Models.Worker;

namespace Administration.Service.Models.Saloon
{
    public class SaloonDetailsDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public Dictionary<DayOfWeek, WorkingHourRange> WorkHours { get; set; }

        public List<WorkerDto> Workers { get; set; }
    }
}

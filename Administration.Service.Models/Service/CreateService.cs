namespace Administration.Service.Models.Service
{
    public class CreateService
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
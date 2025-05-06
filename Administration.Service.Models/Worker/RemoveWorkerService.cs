namespace Administration.Service.Models.Worker
{
    public class RemoveWorkerService
    {
        public Guid WorkerId { get; set; }

        public Guid ServiceId { get; set; }
    }
}
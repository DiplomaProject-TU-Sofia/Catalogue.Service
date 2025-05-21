namespace Catalogue.Service.Models.Worker
{
    public class RemoveWorkerServiceModel
    {
        public Guid WorkerId { get; set; }

        public Guid ServiceId { get; set; }
    }
}
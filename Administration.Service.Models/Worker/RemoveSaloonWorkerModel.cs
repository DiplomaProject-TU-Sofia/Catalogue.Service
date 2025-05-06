namespace Administration.Service.Models.Worker
{
    public class RemoveSaloonWorkerModel
    {
        public Guid WorkerId { get; set; }

        public Guid SaloonId { get; set; }
    }
}
namespace Administration.Service.Data.Entities
{
	public class User
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public ICollection<UserRole> UserRoles { get; set; }

		public ICollection<SaloonWorker> SaloonWorkers { get; set; } = new List<SaloonWorker>();
		public ICollection<WorkerService> WorkerServices { get; set; } = new List<WorkerService>();

	}

}

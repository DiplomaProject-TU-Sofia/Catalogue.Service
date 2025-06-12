using Catalogue.Service.Data.Entities;
using Catalogue.Service.Models.Saloon;
using Catalogue.Service.Models.Service;
using Catalogue.Service.Models.Worker;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace Catalogue.Service.Data.Repositories
{
	public class WorkersRepository
	{
		private readonly AdministrationServiceDbContext _dbContext;
		public WorkersRepository(AdministrationServiceDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<bool> AssignWorkerToSaloonAsync(Guid saloonId, Guid workerId, IEnumerable<DayOfWeek> workingDays)
		{
			if (!_dbContext.Saloons.Any(s => s.Id == saloonId) || !_dbContext.Users.Any(u => u.Id == workerId && u.UserRoles.Any(ur => ur.Role.Name == Entities.Enumerations.Role.Worker.ToString())))
				return false;

			_dbContext.SaloonWorkers.Add(new SaloonWorker
			{
				SaloonId = saloonId,
				UserId = workerId,
				WorkingDays = workingDays
			});

			await _dbContext.SaveChangesAsync();
			return true;

		}

		public async Task<bool> AssignServiceToWorkerAsync(Guid serviceId, Guid workerId)
		{
			if (!_dbContext.Services.Any(s => s.Id == serviceId) || !_dbContext.Users.Any(u => u.Id == workerId && u.UserRoles.Any(ur => ur.Role.Name == Entities.Enumerations.Role.Worker.ToString())))
				return false;

			_dbContext.WorkerServices.Add(new WorkerService { ServiceId = serviceId, UserId = workerId });
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<User>> GetWorkers(ODataQueryOptions<User> queryOptions)
		{
			return await queryOptions
				.ApplyTo(_dbContext.Users
				.Include(u => u.SaloonWorkers)
					.ThenInclude(sw => sw.Saloon)
				.Include(u => u.WorkerServices)
					.ThenInclude(ws => ws.Service)
				.Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Worker")))
				.Cast<User>()
				.ToListAsync();
		}

		public async Task<WorkerDetailsDto> GetWorkerDetails(Guid workerId)
		{
			var worker = await _dbContext.Users
				.Include(u => u.WorkerServices)
				.ThenInclude(ws => ws.Service)
				.Include(u => u.SaloonWorkers)
				.ThenInclude(sw => sw.Saloon)
				.FirstOrDefaultAsync(u => u.Id == workerId && u.UserRoles.Any(ur => ur.Role.Name == "Worker"));

			if (worker == null)
				return null;

			return new WorkerDetailsDto
			{
				Id = worker.Id,
				FirstName = worker.FirstName,
				LastName = worker.LastName,
				Saloons = worker.SaloonWorkers.Select(sw => new SaloonDto
				{
					Id = sw.Saloon.Id,
					Name = sw.Saloon.Name,
					Location = sw.Saloon.Location,
					WorkHours = sw.Saloon.WorkHours
				}),
				Services = worker.WorkerServices.Select(ws => new ServiceDto
				{
					Id = ws.Service.Id,
					Name = ws.Service.Name,
					Description = ws.Service.Description
				})
			};
		}

		public async Task RemoveWorkerServiceAsync(Guid serviceId, Guid workerId)
		{
			var workerServiceToDelete = await _dbContext.WorkerServices.FirstOrDefaultAsync(ws => ws.ServiceId == serviceId && ws.UserId == workerId);

			if (workerServiceToDelete != null)
			{
				_dbContext.WorkerServices.Remove(workerServiceToDelete);
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task RemoveSaloonWorkerAsync(Guid saloonId, Guid workerId)
		{
			var saloonWorkerToDelete = await _dbContext.SaloonWorkers.FirstOrDefaultAsync(sw => sw.SaloonId == saloonId && sw.UserId == workerId);

			if (saloonWorkerToDelete != null)
			{
				_dbContext.SaloonWorkers.Remove(saloonWorkerToDelete);
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task DeleteWorkerAsync(Guid workerId)
		{
			var worker = await _dbContext.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == workerId && u.UserRoles.Any(ur => ur.Role.Name == "Worker"));
			if(worker == null)
				return;

			_dbContext.Users.Remove(worker);
			await _dbContext.SaveChangesAsync();
		}

		public async Task UpdateWorkerImage(Guid workerId, string imageName)
		{
			var worker = await _dbContext.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == workerId && u.UserRoles.Any(ur => ur.Role.Name == "Worker"));
			if (worker == null)
				return;

			worker.ImageName = imageName;
			await _dbContext.SaveChangesAsync();
		}
	}
}

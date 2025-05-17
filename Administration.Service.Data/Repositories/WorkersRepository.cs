using Administration.Service.Data.Entities;
using Administration.Service.Models.Saloon;
using Administration.Service.Models.Service;
using Administration.Service.Models.Worker;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Administration.Service.Data.Repositories
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

		public async Task<IEnumerable<WorkerDto>> GetWorkers(ODataQueryOptions<WorkerDto> queryOptions)
		{
			return await queryOptions
				.ApplyTo(_dbContext.Users
				.Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Worker"))
				.Select(u => new WorkerDto
				{
					Id = u.Id,
					FirstName = u.FirstName,
					LastName = u.LastName
				})).Cast<WorkerDto>().ToListAsync();
		}

		public async Task<WorkerDetailsDto> GetWorkerDetails(Guid workerId)
		{
			var worker = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == workerId && u.UserRoles.Any(ur => ur.Role.Name == "Worker"));

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
					Location = sw.Saloon.Location
				}),
				Services = worker.WorkerServices.Select(ws => new ServiceDto
				{
					Id = ws.Service.Id,
					Name = ws.Service.Name,
					Description = ws.Service.Description
				})
			};
		}

		public async Task<bool> RemoveWorkerServiceAsync(Guid serviceId, Guid workerId)
		{
			var workerServiceToDelete = new WorkerService() { ServiceId = serviceId, UserId = workerId };

			// Attach the entity to the context and mark it as deleted - this would be less resource consuming than reading it from the DB
			_dbContext.WorkerServices.Attach(workerServiceToDelete);
			_dbContext.WorkerServices.Remove(workerServiceToDelete);

			await _dbContext.SaveChangesAsync();

			return true;
		}

		public async Task<bool> RemoveSaloonWorkerAsync(Guid saloonId, Guid workerId)
		{
			var saloonWorkerToDelete = new SaloonWorker() { SaloonId = saloonId, UserId = workerId };

			// Attach the entity to the context and mark it as deleted - this would be less resource consuming than reading it from the DB
			_dbContext.SaloonWorkers.Attach(saloonWorkerToDelete);
			_dbContext.SaloonWorkers.Remove(saloonWorkerToDelete);

			await _dbContext.SaveChangesAsync();

			return true;
		}

		public async Task DeleteWorkerAsync(Guid workerId)
		{
			var worker = await _dbContext.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == workerId && u.UserRoles.Any(ur => ur.Role.Name == "Worker"));
			if(worker == null)
				return;

			_dbContext.Users.Remove(worker);
			await _dbContext.SaveChangesAsync();
		}
	}
}

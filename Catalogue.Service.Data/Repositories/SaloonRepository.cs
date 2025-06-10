using Catalogue.Service.Data.Entities;
using Catalogue.Service.Models.Saloon;
using Catalogue.Service.Models.Worker;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Catalogue.Service.Data.Repositories
{
	public class SaloonRepository
	{
		private readonly AdministrationServiceDbContext _dbContext;
		public SaloonRepository(AdministrationServiceDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task CreateSaloonAsync(CreateSaloon createSaloon)
		{
			var saloon = new Saloon
			{
				Id = Guid.NewGuid(),
				Name = createSaloon.Name,
				Location = createSaloon.Location,
				WorkHours = createSaloon.WorkHours,
				ImageName = createSaloon.ImageName
			};
			_dbContext.Saloons.Add(saloon);
			await _dbContext.SaveChangesAsync();
		}

		public async Task UpdateSaloonAsync(UpdateSaloon updateSaloon)
		{
			var existingSaloon = await _dbContext.Saloons.FirstOrDefaultAsync(s => s.Id == updateSaloon.SaloonId);

			if (existingSaloon == null)
			{
				throw new Exception($"Couldn't find saloon with id: {updateSaloon.SaloonId}");
			}

			existingSaloon.Id = updateSaloon.SaloonId;
			existingSaloon.Name = updateSaloon.Name;
			existingSaloon.Location = updateSaloon.Location;
			existingSaloon.WorkHours = updateSaloon.WorkHours;
			existingSaloon.ImageName = updateSaloon.ImageName.IsNullOrEmpty() ? existingSaloon.ImageName : updateSaloon.ImageName;

			await _dbContext.SaveChangesAsync();
		}

		public async Task DeleteSaloonAsync(Guid saloonId)
		{
			var saloonToDelete = new Saloon() { Id = saloonId };

			// Attach the entity to the context and mark it as deleted - this would be less resource consuming than reading it from the DB
			_dbContext.Saloons.Attach(saloonToDelete);
			_dbContext.Saloons.Remove(saloonToDelete);

			await _dbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<SaloonDto>> GetSaloonsAsync(ODataQueryOptions<SaloonDto> queryOptions)
		{
			var query = _dbContext.Saloons
				.Select(s => new SaloonDto { Id = s.Id, Name =  s.Name , Location = s.Location})
				.AsQueryable();

			var saloons = await queryOptions.ApplyTo(query).Cast<SaloonDto>().ToListAsync();

			return saloons;
		}

		public async Task<IEnumerable<Saloon>> GetSaloonsAsync(ODataQueryOptions<Saloon> queryOptions)
		{
			var saloons = _dbContext.Saloons
					.Include(s => s.SaloonWorkers)
						.ThenInclude(sw => sw.Worker)
							.ThenInclude(w => w.WorkerServices) // go through the actual chain
								.ThenInclude(ws => ws.Service);

			var filtered = (IQueryable<Saloon>)queryOptions.ApplyTo(saloons);

			return await filtered.ToListAsync();
		}

		public async Task<SaloonDetailsDto> GetSaloonDetails(Guid saloonId)
		{
			var saloon = await _dbContext.Saloons.Include(s => s.SaloonWorkers).ThenInclude(sw => sw.Worker).FirstOrDefaultAsync(s => s.Id == saloonId);

			if (saloon == null)
				return null;

			var saloonDto = new SaloonDetailsDto
			{
				Id = saloon.Id,
				Location = saloon.Location,
				WorkHours = saloon.WorkHours,
				Name = saloon.Name,
				Workers = saloon.SaloonWorkers.Select(sw => new WorkerDto
				{
					Id = sw.UserId,
					FirstName = sw.Worker.FirstName,
					LastName = sw.Worker.LastName,
					WorkingDays = sw.WorkingDays.ToList()
				}).ToList()
			};

			return saloonDto;
		}
	}
}

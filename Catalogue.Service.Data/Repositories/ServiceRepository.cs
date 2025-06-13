using Catalogue.Service.Models.Service;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Catalogue.Service.Data.Repositories
{
	public class ServiceRepository
	{
		private readonly CatalogueServiceDbContext _dbContext;
		public ServiceRepository(CatalogueServiceDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task CreateServiceAsync(CreateService createService)
		{
			var service = new Entities.Service
			{
				Id = Guid.NewGuid(),
				Name = createService.Name,
				Description = createService.Description,
				Duration = createService.Duration,
				Price = createService.Price,
				ImageName = createService.ImageName,
			};
			_dbContext.Services.Add(service);
			await _dbContext.SaveChangesAsync();
		}

		public async Task UpdateServiceAsync(UpdateService updateDto)
		{
			var existingService = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == updateDto.ServiceId);

			if (existingService == null)
				throw new Exception($"Couldn't find service with id: {updateDto.ServiceId}");

			// Мапни стойностите ръчно
			existingService.Name = updateDto.Name;
			existingService.Description = updateDto.Description;
			existingService.Price = updateDto.Price;
			existingService.Duration = updateDto.Duration;
			existingService.ImageName = updateDto.ImageName.IsNullOrEmpty() ? existingService.ImageName : updateDto.ImageName;

			await _dbContext.SaveChangesAsync();
		}
		public async Task DeleteServiceAsync(Guid serviceId)
		{
			var serviceToDelete = new Entities.Service() { Id = serviceId };

			// Attach the entity to the context and mark it as deleted - this would be less resource consuming than reading it from the DB
			_dbContext.Services.Attach(serviceToDelete);
			_dbContext.Services.Remove(serviceToDelete);

			await _dbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<ServiceDto>> GetServicesAsync(ODataQueryOptions<ServiceDto> queryOptions)
		{
			return await queryOptions
				.ApplyTo(_dbContext.Services.Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Description = s.Description, Price = s.Price, ImageName = s.ImageName}))
				.Cast<ServiceDto>()
				.ToListAsync();
		}

		public async Task<ServiceDetailsDto> GetServiceDetails(Guid serviceId)
		{
			var service = await _dbContext.Services.Include(s => s.WorkerServices).ThenInclude(sw => sw.Worker).FirstOrDefaultAsync(s => s.Id == serviceId);

			if (service == null)
				return null;

			return new ServiceDetailsDto
			{
				ServiceId = service.Id,
				Name = service.Name,
				Description = service.Description,
				Duration = service.Duration,
				Price = service.Price,
				ImageName = service.ImageName,
				Workers = service.WorkerServices.Select(ws => new Models.Worker.WorkerDto
				{
					Id = ws.Worker.Id,
					FirstName = ws.Worker.FirstName,
					LastName = ws.Worker.LastName
				})
			};
		}
	}
}

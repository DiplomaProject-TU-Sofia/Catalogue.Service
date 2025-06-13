using Catalogue.Service.Models.Statistics;
using Microsoft.EntityFrameworkCore;

namespace Catalogue.Service.Data.Repositories
{
	public class StatisticsRepository(CatalogueServiceDbContext _dbContext)
    {
		public async Task<StatisticsModel> GetStatistics()
		{
			return new StatisticsModel
			{
				UsersCount = await _dbContext.Users.Where(u => u.UserRoles.Any(ur => ur.Role.Name == "User")).CountAsync(),
				WorkersCount = await _dbContext.Users.Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Workers")).CountAsync(),
				ServicesCount = await _dbContext.Services.CountAsync(),
				SaloonsCount = await _dbContext.Saloons.CountAsync()
			};
		}
	}
}

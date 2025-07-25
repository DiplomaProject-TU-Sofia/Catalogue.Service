﻿using Microsoft.AspNetCore.Mvc;
using Catalogue.Service.Data.Repositories;

namespace Catalogue.Service.API.Controllers
{
	[Route("api/statistics")]
	[ApiController]
	public class StatisticsController(ILogger<StatisticsController> log, StatisticsRepository statisticsRepository) : BaseController(log)
	{

		/// <summary>
		/// Get statistics
		/// </summary>
		/// <returns></returns>
		[HttpGet()]
		public async Task<IActionResult> GetStatistics()
		{
			return Ok(await statisticsRepository.GetStatistics());
		}
	}
}

using Catalogue.Service.Data.Entities;
using Catalogue.Service.Data.Repositories;
using Catalogue.Service.Models.Saloon;
using Catalogue.Service.Models.Worker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Catalogue.Service.API.Controllers
{
	[AllowAnonymous]
	[Route("api/saloons")]
	[ApiController]
	public class SaloonController : BaseController
	{
		private readonly SaloonRepository _saloonRepository;
		public SaloonController(SaloonRepository saloonRepository, ILogger<SaloonController> log) : base(log)
		{
			_saloonRepository = saloonRepository;
		}

		/// <summary>
		/// Get saloon's details
		/// </summary>
		/// <param name="saloonId"></param>
		/// <returns></returns>
		[HttpGet("{saloonId:guid}")]
		public async Task<IActionResult> GetSaloonDetails(Guid saloonId)
		{
			//Validate saloonId
			if (saloonId == default)
				return BadRequest();

			//Get saloon details
			var saloonDetails = await _saloonRepository.GetSaloonDetails(saloonId);

			if (saloonDetails == null)
				return NotFound();

			return Ok(saloonDetails);
		}

		/// <summary>
		/// Get saloons using oDataQuery
		/// </summary>
		/// <param name="queryOptions"></param>
		/// <returns></returns>
		[HttpGet()]
		public async Task<IActionResult> GetSaloons(ODataQueryOptions<Saloon> queryOptions)
		{
			//Validate oDataQuery options
			ValidationSettings.AllowedQueryOptions = AllowedQueryOptions.Filter | AllowedQueryOptions.OrderBy | AllowedQueryOptions.Top | AllowedQueryOptions.Skip | AllowedQueryOptions.Count;
			ValidationSettings.AllowedLogicalOperators = AllowedLogicalOperators.Equal | AllowedLogicalOperators.NotEqual | AllowedLogicalOperators.And | AllowedLogicalOperators.Or;
			ValidationSettings.AllowedFunctions = AllowedFunctions.All | AllowedFunctions.Any;
			ValidationSettings.MaxAnyAllExpressionDepth = 100;

			if (!ValidateODataQuery(queryOptions, out string validationMessage))
				return BadRequest(validationMessage);

			//Get saloons and apply oData
			var saloons = await _saloonRepository.GetSaloonsAsync(queryOptions);

			var response = saloons.Select(s => new SaloonDto
			{
				Id = s.Id,
				Location = s.Location,
				Name = s.Name,
				Workers = s.SaloonWorkers.Select(sw => new WorkerDto
				{
					Id = sw.Worker.Id,
					FirstName = sw.Worker.FirstName,
					LastName = sw.Worker.LastName,
					WorkingDays = sw.WorkingDays.ToList()
				})
			});

			return Ok(response);
		}

		/// <summary>
		/// Creates saloon
		/// </summary>
		/// <param name="createSaloon"></param>
		/// <returns></returns>
		[HttpPost()]
		public async Task<IActionResult> CreateSaloon([FromBody] CreateSaloon createSaloon)
		{
			if (createSaloon == null || string.IsNullOrWhiteSpace(createSaloon.Name) || string.IsNullOrWhiteSpace(createSaloon.Location))
				return BadRequest("Cannot create saloon with empty name or location");


			await _saloonRepository.CreateSaloonAsync(createSaloon);

			return Ok();
		}

		/// <summary>
		/// Updates saloon
		/// </summary>
		/// <param name="updateSaloon"></param>
		/// <returns></returns>
		[HttpPut()]
		public async Task<IActionResult> UpdateSaloon([FromBody] UpdateSaloon updateSaloon)
		{
			if (updateSaloon == null || string.IsNullOrWhiteSpace(updateSaloon.Name) || string.IsNullOrWhiteSpace(updateSaloon.Location) || updateSaloon.SaloonId == default)
				return BadRequest("Cannot update saloon with empty name, location or id");

			await _saloonRepository.UpdateSaloonAsync(updateSaloon);

			return Ok();
		}

		/// <summary>
		/// Deletes saloon
		/// </summary>
		/// <param name="saloonId"></param>
		/// <returns></returns>
		[HttpDelete("{saloonId}")]
		public async Task<IActionResult> DeleteSaloon(Guid saloonId)
		{
			if (saloonId == default)
				return BadRequest("saloonId shouldn't be null");

			await _saloonRepository.DeleteSaloonAsync(saloonId);

			return Ok();
		}
	}
}

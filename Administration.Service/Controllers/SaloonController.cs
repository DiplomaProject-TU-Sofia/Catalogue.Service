using Administration.Service.Data.Entities;
using Administration.Service.Data.Repositories;
using Administration.Service.Models.Saloon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Administration.Service.API.Controllers
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

			if(saloonDetails == null)
				return NotFound();

			return Ok(saloonDetails);
		}

		/// <summary>
		/// Get saloons using oDataQuery
		/// </summary>
		/// <param name="queryOptions"></param>
		/// <returns></returns>
		[HttpGet()]
		public async Task<IActionResult> GetSaloons(ODataQueryOptions<SaloonDto> queryOptions)
		{
			//Validate oDataQuery options
			ValidationSettings.AllowedQueryOptions = AllowedQueryOptions.Filter | AllowedQueryOptions.OrderBy | AllowedQueryOptions.Top | AllowedQueryOptions.Skip | AllowedQueryOptions.Count;
			ValidationSettings.AllowedLogicalOperators = AllowedLogicalOperators.Equal | AllowedLogicalOperators.NotEqual | AllowedLogicalOperators.And | AllowedLogicalOperators.Or;
			ValidationSettings.AllowedFunctions = AllowedFunctions.All;
			if (!ValidateODataQuery(queryOptions, out string validationMessage))
				return BadRequest(validationMessage);

			//Get saloons and apply oData
			var saloons = await _saloonRepository.GetSaloonsAsync(queryOptions);

			return Ok(saloons);
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

			return Created();
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

			return NoContent();
		}
	}
}

using Administration.Service.Data.Repositories;
using Administration.Service.Models.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Administration.Service.API.Controllers
{
	[AllowAnonymous]
	[Route("api/service")]
	[ApiController]
	public class ServiceController : BaseController
	{
		private readonly ServiceRepository _serviceRepository;
		public ServiceController(ServiceRepository serviceRepository, ILogger<ServiceController> log) : base(log)
		{
			_serviceRepository = serviceRepository;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryOptions"></param>
		/// <returns></returns>
		[HttpGet()]
		public async Task<IActionResult> GetServices(ODataQueryOptions<ServiceDto> queryOptions)
		{
			//Validate oDataQuery options
			ValidationSettings.AllowedQueryOptions = AllowedQueryOptions.Filter | AllowedQueryOptions.OrderBy | AllowedQueryOptions.Top | AllowedQueryOptions.Skip | AllowedQueryOptions.Count;
			ValidationSettings.AllowedLogicalOperators = AllowedLogicalOperators.Equal | AllowedLogicalOperators.NotEqual | AllowedLogicalOperators.And | AllowedLogicalOperators.Or;
			ValidationSettings.AllowedFunctions = AllowedFunctions.All;
			if (!ValidateODataQuery(queryOptions, out string validationMessage))
				return BadRequest(validationMessage);

			//Get services and apply oData
			var services = await _serviceRepository.GetServicesAsync(queryOptions);

			return Ok(services);
		}

		/// <summary>
		/// Get service's details
		/// </summary>
		/// <param name="serviceId"></param>
		/// <returns></returns>
		[HttpGet("{serviceId:guid}")]
		public async Task<IActionResult> GetServiceDetails(Guid serviceId)
		{
			//Validate serviceId
			if (serviceId == default)
				return BadRequest();

			//Get service
			var service = await _serviceRepository.GetServiceDetails(serviceId);

			if(service == null)
				return NotFound();

			return Ok(service);
		}

		[HttpPost()]
		public async Task<IActionResult> CreateService([FromBody] CreateService createService)
		{
			if (createService == null || string.IsNullOrWhiteSpace(createService.Name) || createService.Duration == default || createService.Price == default)
				return BadRequest("Cannot create service with empty name, duration or price");


			await _serviceRepository.CreateServiceAsync(createService);

			return Created();
		}

		[HttpPut()]
		public async Task<IActionResult> UpdateService([FromBody] UpdateService updateService)
		{
			if (updateService == null || string.IsNullOrWhiteSpace(updateService.Name) || updateService.Duration != default || updateService.Price == default || updateService.ServiceId == default)
				return BadRequest("Cannot create service with empty name, duration, price or id");

			await _serviceRepository.UpdateServiceAsync(updateService);

			return NoContent();
		}
	}
}

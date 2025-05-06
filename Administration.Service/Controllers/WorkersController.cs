using Administration.Service.Data.Repositories;
using Administration.Service.Models.Worker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Administration.Service.API.Controllers
{
	[AllowAnonymous]
	[Route("api/workers")]
	[ApiController]
	public class WorkersController : BaseController
	{
		private readonly WorkersRepository _workersRepository;

		public WorkersController(ILogger<WorkersController> log, WorkersRepository workersRepository) : base(log)
		{
			_workersRepository = workersRepository;
		}

		/// <summary>
		/// Get workers
		/// </summary>
		/// <param name="queryOptions"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> GetWorkers(ODataQueryOptions<WorkerDto> queryOptions)
		{
			//Validate oDataQuery options
			ValidationSettings.AllowedQueryOptions = AllowedQueryOptions.Filter | AllowedQueryOptions.OrderBy | AllowedQueryOptions.Top | AllowedQueryOptions.Skip | AllowedQueryOptions.Count;
			ValidationSettings.AllowedLogicalOperators = AllowedLogicalOperators.Equal | AllowedLogicalOperators.NotEqual | AllowedLogicalOperators.And | AllowedLogicalOperators.Or;
			ValidationSettings.AllowedFunctions = AllowedFunctions.All;
			if (!ValidateODataQuery(queryOptions, out string validationMessage))
				return BadRequest(validationMessage);

			//Get workers and apply oData
			var workers = await _workersRepository.GetWorkers(queryOptions);

			return Ok(workers);
		}

		/// <summary>
		/// Get worker's details
		/// </summary>
		/// <param name="workerId"></param>
		/// <returns></returns>
		[HttpGet("{serviceId:guid}")]
		public async Task<IActionResult> GetWorkerDetails(Guid workerId)
		{
			//Validate workerId
			if (workerId == default)
				return BadRequest();

			//Get worker
			var worker = await _workersRepository.GetWorkerDetails(workerId);

			if (worker == null)
				return NotFound();

			return Ok(worker);
		}

		/// <summary>
		/// Assign service to worker
		/// </summary>
		/// <param name="workerService"></param>
		/// <returns></returns>
		[HttpPost("service")]
		public async Task<IActionResult> AssignWorkerToService(CreateWorkerService workerService)
		{
			if (workerService.ServiceId == default || workerService.UserId == default)
				return BadRequest();

			if (!await _workersRepository.AssignServiceToWorkerAsync(workerService.ServiceId, workerService.UserId))
				return BadRequest();

			return NoContent();
		}

		/// <summary>
		/// Assign worker to saloon
		/// </summary>
		/// <param name="saloonWorker"></param>
		/// <returns></returns>
		[HttpPost("saloon")]
		public async Task<IActionResult> AssignWorkerToSaloon([FromBody] CreateSaloonWorker saloonWorker)
		{
			if (saloonWorker.SaloonId == default || saloonWorker.WorkerId == default)
				return BadRequest();

			if (!await _workersRepository.AssignWorkerToSaloonAsync(saloonWorker.SaloonId, saloonWorker.WorkerId, saloonWorker.WorkingDays))
				return BadRequest();

			return NoContent();
		}
		
		/// <summary>
		/// Remove service from worker
		/// </summary>
		/// <param name="workerService"></param>
		/// <returns></returns>
		[HttpPost("service-remove")]
		public async Task<IActionResult> RemoveWorkerFromService(RemoveWorkerService workerService)
		{
			if (workerService.ServiceId == default || workerService.WorkerId == default)
				return BadRequest();

			await _workersRepository.RemoveWorkerServiceAsync(workerService.ServiceId, workerService.WorkerId);

			return NoContent();
		}

		/// <summary>
		/// Remove worker from saloon
		/// </summary>
		/// <param name="workerService"></param>
		/// <returns></returns>
		[HttpPost("saloon-remove")]
		public async Task<IActionResult> RemoveWorkerFromService(RemoveSaloonWorker workerService)
		{
			if (workerService.SaloonId == default || workerService.WorkerId == default)
				return BadRequest();

			await _workersRepository.RemoveSaloonWorkerAsync(workerService.SaloonId, workerService.WorkerId);

			return NoContent();
		}
	}
}

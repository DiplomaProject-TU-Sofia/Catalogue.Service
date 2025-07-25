﻿using Catalogue.Service.Data.Entities;
using Catalogue.Service.Data.Repositories;
using Catalogue.Service.Models.Saloon;
using Catalogue.Service.Models.Worker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.IdentityModel.Tokens;

namespace Catalogue.Service.API.Controllers
{
	[AllowAnonymous]
	[Route("api")]
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
		[HttpGet("workers")]
		public async Task<IActionResult> GetWorkers(ODataQueryOptions<User> queryOptions)
		{
			//Validate oDataQuery options
			ValidationSettings.AllowedQueryOptions = AllowedQueryOptions.Filter | AllowedQueryOptions.OrderBy | AllowedQueryOptions.Top | AllowedQueryOptions.Skip | AllowedQueryOptions.Count;
			ValidationSettings.AllowedLogicalOperators = AllowedLogicalOperators.Equal | AllowedLogicalOperators.NotEqual | AllowedLogicalOperators.And | AllowedLogicalOperators.Or;
			ValidationSettings.AllowedFunctions = AllowedFunctions.All | AllowedFunctions.Any;
			if (!ValidateODataQuery(queryOptions, out string validationMessage))
				return BadRequest(validationMessage);

			//Get workers and apply oData
			var workers = await _workersRepository.GetWorkers(queryOptions);

			var response = workers.Select(w => new WorkerDto
			{
				Id = w.Id,
				FirstName = w.FirstName,
				LastName = w.LastName,
				ImageName = w.ImageName,
				Saloons = w.SaloonWorkers.Select(sw =>
				new SaloonDto
				{
					Id = sw.Saloon.Id,
					Location = sw.Saloon.Location,
					Name = sw.Saloon.Name,
					WorkHours = sw.Saloon.WorkHours
				})
			});

			return Ok(response);
		}

		/// <summary>
		/// Get worker's details
		/// </summary>
		/// <param name="workerId"></param>
		/// <returns></returns>
		[HttpGet("workers/{workerId:guid}")]
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
		[HttpPost("worker-service")]
		public async Task<IActionResult> CreateWorkerService(CreateWorkerServiceModel workerService)
		{
			if (workerService.ServiceId == default || workerService.WorkerId == default)
				return BadRequest();

			if (!await _workersRepository.AssignServiceToWorkerAsync(workerService.ServiceId, workerService.WorkerId))
				return BadRequest();

			return NoContent();
		}

		/// <summary>
		/// Assign worker to saloon
		/// </summary>
		/// <param name="saloonWorker"></param>
		/// <returns></returns>
		[HttpPost("worker-saloon")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> CreateSaloonWorker([FromBody] CreateSaloonWorkerModel saloonWorker)
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
		[HttpDelete("worker-service")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> RemoveWorkerService(RemoveWorkerServiceModel workerService)
		{
			if (workerService.ServiceId == default || workerService.WorkerId == default)
				return BadRequest();

			await _workersRepository.RemoveWorkerServiceAsync(workerService.ServiceId, workerService.WorkerId);

			return NoContent();
		}

		/// <summary>
		/// Remove worker from saloon
		/// </summary>
		/// <param name="saloonWorker"></param>
		/// <returns></returns>
		[HttpDelete("worker-saloon")]
		public async Task<IActionResult> RemoveSaloonWorker(RemoveSaloonWorkerModel saloonWorker)
		{
			if (saloonWorker.SaloonId == default || saloonWorker.WorkerId == default)
				return BadRequest();

			await _workersRepository.RemoveSaloonWorkerAsync(saloonWorker.SaloonId, saloonWorker.WorkerId);

			return NoContent();
		}

		/// <summary>
		/// Deletes worker
		/// </summary>
		/// <param name="workerId"></param>
		/// <returns></returns>
		[HttpDelete("workers/{workerId}")]
		public async Task<IActionResult> DeleteService(Guid workerId)
		{
			if (workerId == default)
				return BadRequest("workerId shouldn't be null");

			await _workersRepository.DeleteWorkerAsync(workerId);

			return Ok();
		}

		/// <summary>
		/// Update worker image
		/// </summary>
		/// <param name="workerId"></param>
		/// <param name="imageName"></param>
		/// <returns></returns>
		[HttpPut("workers/{workerId}/{imageName}")]
		public async Task<IActionResult> UpdateWorkerImage(Guid workerId, string imageName)
		{
			if (workerId == default || imageName.IsNullOrEmpty())
				return BadRequest("workerId and imageName shouldn't be null");

			await _workersRepository.UpdateWorkerImage(workerId, imageName);

			return Ok();
		}
	}
}

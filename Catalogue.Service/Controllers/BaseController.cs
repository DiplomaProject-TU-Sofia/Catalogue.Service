using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;
using Microsoft.OData;

namespace Catalogue.Service.API.Controllers
{

	[ApiController]
	public class BaseController : ControllerBase
	{
		protected ILogger Log { get; set; }

		public BaseController(ILogger log)
		{
			Log = log;
			Log.LogInformation("Logging initialised");
		}

		private ODataValidationSettings _validationSettings;
		/// <summary>
		/// Gets an <see cref="ODataValidationSettings" /> object for validation of OData queries.
		/// All OData options are turned off by default, otherwise they are all turned on by default.
		/// </summary>
		protected ODataValidationSettings ValidationSettings
			=> _validationSettings ??= new ODataValidationSettings
			{
				AllowedQueryOptions = AllowedQueryOptions.None,
				AllowedArithmeticOperators = AllowedArithmeticOperators.None,
				AllowedFunctions = AllowedFunctions.None,
				AllowedLogicalOperators = AllowedLogicalOperators.None
			};

		/// <summary>
		/// Validates an OData query according to the allowed OData query options set
		/// on the <see cref="ValidationSettings"/> object.
		/// </summary>
		/// <param name="options">An <see cref="ODataQueryOptions"/> object to validate</param>
		/// <param name="message">The validation message for failed validation</param>
		/// <returns>True if successful</returns>
		protected bool ValidateODataQuery(ODataQueryOptions options, out string message)
		{
			try
			{
				options.Validate(ValidationSettings);
			}
			catch (ODataException ex)
			{
				message = ex.Message;
				Log.LogError(ex, "Error Validating QueryOptions");
				return false;
			}

			message = null;
			return true;
		}
	}

}

using Catalogue.Service.Data;
using Catalogue.Service.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{

		services.AddDbContext<AdministrationServiceDbContext>(options =>
			options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

		// Enable JWT authentication
		// Add Authentication using JWT Bearer
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = _configuration["JwtSettings:Issuer"],
				ValidateAudience = true,
				ValidAudience = _configuration["JwtSettings:Audience"],
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])
				),
				RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
			};
		});

		services.AddAuthorization(); // Enables [Authorize] attribute

		services.AddTransient<SaloonRepository>();
		services.AddTransient<ServiceRepository>();
		services.AddTransient<WorkersRepository>();
		
		services.AddControllers()
			.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
			})
			.AddOData(opt =>
			{
				opt.Filter().Expand().Select().OrderBy().SetMaxTop(100).Count();
				opt.EnableQueryFeatures(maxTopValue: 100);
			});
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
			{
				Title = "Catalogue.Service",
				Version = "v1"
			});
		});
		services.AddCors(options =>
		{
			options.AddPolicy("AllowAll", builder =>
			{
				builder
					.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
			});
		});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseRouting();
		app.UseCors("AllowAll");
		// Enable Authentication & Authorization Middleware
		app.UseAuthentication();
		app.UseAuthorization();

		// Enable Swagger
		app.UseSwagger();

		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API");
			c.RoutePrefix = "swagger"; // or "" to serve Swagger at root URL
		});

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
}

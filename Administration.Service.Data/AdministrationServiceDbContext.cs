using Administration.Service.Data.Entities;
using Administration.Service.Models.Saloon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Administration.Service.Data
{
    public class AdministrationServiceDbContext : DbContext
	{
		public AdministrationServiceDbContext(DbContextOptions<AdministrationServiceDbContext> options) : base(options)
		{
		}
		public DbSet<Saloon> Saloons { get; set; }
		public DbSet<Entities.Service> Services { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }

		public DbSet<SaloonWorker> SaloonWorkers { get; set; }
		public DbSet<WorkerService> WorkerServices { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>(entity =>
			{
				entity.ToTable("AspNetUsers");
				entity.HasKey(u => u.Id);

				entity.HasMany(u => u.UserRoles)
					  .WithOne(ur => ur.User)
					  .HasForeignKey(ur => ur.UserId);

				entity.Metadata.SetIsTableExcludedFromMigrations(true);
			});

			modelBuilder.Entity<Role>(entity =>
			{
				entity.ToTable("AspNetRoles");
				entity.HasKey(r => r.Id);

				entity.Metadata.SetIsTableExcludedFromMigrations(true);
			});

			modelBuilder.Entity<UserRole>(entity =>
			{
				entity.ToTable("AspNetUserRoles");
				entity.HasKey(ur => new { ur.UserId, ur.RoleId });

				entity.HasOne(ur => ur.User)
					  .WithMany(u => u.UserRoles)
					  .HasForeignKey(ur => ur.UserId);

				entity.HasOne(ur => ur.Role)
					  .WithMany()
					  .HasForeignKey(ur => ur.RoleId);

				entity.Metadata.SetIsTableExcludedFromMigrations(true);
			});


			// Many-to-many relationship between Worker and Saloon
			modelBuilder.Entity<SaloonWorker>()
				.HasKey(sw => new { sw.UserId, sw.SaloonId });

			modelBuilder.Entity<SaloonWorker>()
				.HasOne(sw => sw.Worker)
				.WithMany(w => w.SaloonWorkers)
				.HasForeignKey(sw => sw.UserId);

			modelBuilder.Entity<SaloonWorker>()
				.HasOne(sw => sw.Saloon)
				.WithMany(s => s.SaloonWorkers)
				.HasForeignKey(sw => sw.SaloonId);

			// Many-to-many relationship between Worker and Service
			modelBuilder.Entity<WorkerService>()
				.HasKey(ws => new { ws.UserId, ws.ServiceId });

			modelBuilder.Entity<WorkerService>()
				.HasOne(ws => ws.Worker)
				.WithMany(w => w.WorkerServices)
				.HasForeignKey(ws => ws.UserId);

			modelBuilder.Entity<WorkerService>()
				.HasOne(ws => ws.Service)
				.WithMany(s => s.WorkerServices)
				.HasForeignKey(ws => ws.ServiceId);

			var daysOfWeekConverter = new ValueConverter<IEnumerable<DayOfWeek>, string>(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
				v => JsonSerializer.Deserialize<IEnumerable<DayOfWeek>>(v, (JsonSerializerOptions?)null) ?? new List<DayOfWeek>());

			modelBuilder.Entity<SaloonWorker>()
				.Property(sw => sw.WorkingDays)
				.HasConversion(daysOfWeekConverter);

			var workHoursConverter = new ValueConverter<Dictionary<DayOfWeek, WorkingHourRange>, string>(
				v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
				v => JsonSerializer.Deserialize<Dictionary<DayOfWeek, WorkingHourRange>>(v ?? "{}", (JsonSerializerOptions?)null) ?? new());

			var workHoursComparer = new ValueComparer<Dictionary<DayOfWeek, WorkingHourRange>>(
				(c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null),
				c => JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),
				c => JsonSerializer.Deserialize<Dictionary<DayOfWeek, WorkingHourRange>>(JsonSerializer.Serialize(c, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null) ?? new()
			);

			modelBuilder.Entity<Saloon>()
				.Property(s => s.WorkHours)
				.HasConversion(workHoursConverter)
				.Metadata.SetValueComparer(workHoursComparer);
		}
	}
}
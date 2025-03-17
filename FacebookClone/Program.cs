
using CloudinaryDotNet;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using FacebookClone.Core.Repository.Auhtentication;
using FacebookClone.Core.Services;
using FacebookClone.EF;
using FacebookClone.EF.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace FacebookClone
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);


			builder.Configuration
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();

			builder.Services.AddControllers();

			builder.Services.AddOpenApi();
			builder.Services.AddTransient<IEmailService, EmailService>();
			builder.Services.AddSingleton<RedisOTPService>();
			builder.Services.AddScoped<IPostRepository,PostRepository>();
			builder.Services.AddScoped<IGenericRepository<SavedPosts>, GenericRepository<SavedPosts>>();
			builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<FacebookContext>().AddDefaultTokenProviders();
			builder.Services.AddDbContext<FacebookContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("OmarConnection")));

			builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
			builder.Services.AddAuthentication();
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(options =>
					{
						options.RequireHttpsMetadata = false;
						options.SaveToken = true;
						options.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
							ValidateIssuer = true,
							ValidateAudience = true,
							ValidIssuer = builder.Configuration["JWT:Issuer"],
							ValidAudience = builder.Configuration["JWT:Audience"],
							ValidateLifetime = true
						};
					});
			builder.Services.AddAuthorization();

			var cloudinaryConfig = builder.Configuration.GetSection("Cloudinary");
			var cloudinary = new Cloudinary(new Account(
				cloudinaryConfig["CloudName"],
				cloudinaryConfig["ApiKey"],
				cloudinaryConfig["ApiSecret"]
			));
			builder.Services.AddSingleton(cloudinary);
			builder.Services.AddScoped<IGenericRepository<Media>, GenericRepository<Media>>();
			builder.Services.AddScoped<IGenericRepository<Post>, GenericRepository<Post>>();
			builder.Services.AddScoped<IMediaRepository, MediaRepository>();
			var app = builder.Build();


			if (app.Environment.IsDevelopment())
			{
				app.MapOpenApi();
				app.MapScalarApiReference();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/error"); 
			}
			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();


				await SeedRolesAndAdminAsync(roleManager, userManager);
			}
			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
		private static async Task SeedRolesAndAdminAsync(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
		{
			var roles = new List<string> { "Admin", "User" };

			foreach (var role in roles)
			{
				if (!await roleManager.RoleExistsAsync(role))
				{
					await roleManager.CreateAsync(new AppRole(role));
				}
			}

			var adminEmail = "admin@admin.com";
			var adminUser = await userManager.FindByEmailAsync(adminEmail);
			if (adminUser == null)
			{
				var newAdmin = new AppUser
				{
					UserName = "admin",
					Email = adminEmail,
					EmailConfirmed = true
				};

				var result = await userManager.CreateAsync(newAdmin, "Admin@123");
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(newAdmin, "Admin");
				}
				else
				{
					Console.WriteLine("Failed to create admin user:");
					foreach (var error in result.Errors)
					{
						Console.WriteLine($"- {error.Description}");
					}
				}
			}
		}
	}
}

using CloudinaryDotNet;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using FacebookClone.Core.Repository.Auhtentication;
using FacebookClone.Core.Services;
using FacebookClone.EF;
using FacebookClone.EF.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using FacebookClone.Hubs;
using FacebookClone.API.Hubs;

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
			builder.Services.AddScoped<IPostRepository, PostRepository>();
			builder.Services.AddScoped<IGenericRepository<SavedPosts>, GenericRepository<SavedPosts>>();
			builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<FacebookContext>().AddDefaultTokenProviders();
			builder.Services.AddDbContext<FacebookContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("OmarConnection")));
			builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
			builder.Services.AddScoped<IGenericRepository<Media>, GenericRepository<Media>>();
			builder.Services.AddScoped<IGenericRepository<AppUser>, GenericRepository<AppUser>>();
			builder.Services.AddScoped<IGenericRepository<Post>, GenericRepository<Post>>();
			builder.Services.AddScoped<IGenericRepository<Reactions>, GenericRepository<Reactions>>();
			builder.Services.AddScoped<IGenericRepository<Comment>, GenericRepository<Comment>>();
			builder.Services.AddScoped<IReactionRepository, ReactionRepository>();
			builder.Services.AddScoped<ICommentRepository, CommentRepository>();
			builder.Services.AddScoped<IFriendShipRepository, FriendShipRepository>();
			builder.Services.AddScoped<IMediaRepository, MediaRepository>();
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IChatRepository, ChatRepository>();
			builder.Services.AddAuthentication();
			builder.Services.AddSignalR();
			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(policy =>
				{
					policy.AllowAnyHeader()
						  .AllowAnyMethod()
						  .AllowCredentials()
						  .SetIsOriginAllowed(origin => true);
				});
			});

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = true; 
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					ValidAudience = builder.Configuration["Jwt:Audience"],
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero 
				};
				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						// 34an akra2 el JWT token mn el "jwt" cookie
						var token = context.Request.Cookies["jwt"];
						if (!string.IsNullOrEmpty(token))
						{
							context.Token = token;
						}
						return Task.CompletedTask;
					}
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
			builder.Services.Configure<JsonOptions>(options =>
			{
				options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
				options.SerializerOptions.PropertyNameCaseInsensitive = true;
			});

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

			app.UseCors();
			app.UseStaticFiles();
			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();
			app.MapHub<PostHub>("/posthub");
			app.MapHub<ChatHub>("/chatHub");

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
					EmailConfirmed = true,
					Verify = true
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
using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace FacebookClone.EF
{
	public class FacebookContext : IdentityDbContext<AppUser, AppRole, Guid>
	{
		public FacebookContext(DbContextOptions<FacebookContext> options) : base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<SavedPosts>().HasKey(op => new { op.PostId, op.AppUserId });
			modelBuilder.Entity<Comment>().Property(c => c.Text).HasMaxLength(500);

			modelBuilder.Entity<Post>()
			.HasMany(p => p.Media)
			.WithOne(m => m.Post)
			.HasForeignKey(m => m.PostId)
			.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Post>()
			.HasMany(p => p.Comments)
			.WithOne(c => c.Post)
			.HasForeignKey(c => c.PostId)
			.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Post>()
			.HasMany(p => p.Reactions)
			.WithOne(r => r.Post)
			.HasForeignKey(r => r.PostId)
			.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SavedPosts>()
			.HasOne(sp => sp.Post)
			.WithMany()  
			.HasForeignKey(sp => sp.PostId)
			.OnDelete(DeleteBehavior.Cascade);
		}
		public DbSet<Post> Post { get; set; }
		public DbSet<Comment> Comment { get; set; }
		public DbSet<Reactions> Reactions { get; set; }
		public DbSet<Media> Media { get; set; }
		public DbSet<SavedPosts> SavedPosts { get; set; }
		public DbSet<Friendship> Friendship { get; set; }
		public DbSet<Chat> Chat { get; set; }
		public DbSet<Message> Message { get; set; }


	}
}

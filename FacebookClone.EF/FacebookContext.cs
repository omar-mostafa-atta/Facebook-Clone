using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace FacebookClone.EF
{
	public class FacebookContext: IdentityDbContext<AppUser,AppRole,Guid>
	{
		public FacebookContext(DbContextOptions<FacebookContext> options):base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<SavedPosts>().HasKey(op => new { op.PostId, op.AppUserId });
			modelBuilder.Entity<Comment>().Property(c => c.Text).HasMaxLength(500);

		 
		}
		public DbSet<Post> Post {  get; set; }
		public DbSet<Comment> Comment {  get; set; }
		public DbSet<Reactions> Reactions {  get; set; }
		public DbSet<Media> Media {  get; set; }
		public DbSet<SavedPosts> SavedPosts {  get; set; }
		public DbSet<Friendship> Friendship {  get; set; }
		public DbSet<Chat> Chat {  get; set; }
		public DbSet<Message> Message {  get; set; }


	}
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacebookClone.EF.Migrations
{
	public partial class init : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "AspNetRoles",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoles", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUsers",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					BackgroundPictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ProfilePicturePublicId = table.Column<string>(type: "nvarchar(max)", nullable: true),
					BackgroundPicturePublicId = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Verify = table.Column<bool>(type: "bit", nullable: false),
					Activated = table.Column<bool>(type: "bit", nullable: false),
					OTP = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
					NumberOfFriends = table.Column<int>(type: "int", nullable: true),
					UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
					PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
					SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
					TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
					LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
					AccessFailedCount = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUsers", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AspNetRoleClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetUserClaims_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserLogins",
				columns: table => new
				{
					LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
					table.ForeignKey(
						name: "FK_AspNetUserLogins_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserRoles",
				columns: table => new
				{
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserTokens",
				columns: table => new
				{
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
					table.ForeignKey(
						name: "FK_AspNetUserTokens_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Chat",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Chat", x => x.Id);
					table.ForeignKey(
						name: "FK_Chat_AspNetUsers_ReceiverId",
						column: x => x.ReceiverId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.NoAction);
					table.ForeignKey(
						name: "FK_Chat_AspNetUsers_SenderId",
						column: x => x.SenderId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.NoAction);
				});

			migrationBuilder.CreateTable(
				name: "Friendship",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ReciverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Status = table.Column<int>(type: "int", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Friendship", x => x.Id);
					table.ForeignKey(
						name: "FK_Friendship_AspNetUsers_ReciverId",
						column: x => x.ReciverId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.NoAction);
					table.ForeignKey(
						name: "FK_Friendship_AspNetUsers_SenderId",
						column: x => x.SenderId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.NoAction);
				});

			migrationBuilder.CreateTable(
				name: "Post",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					TotalReactions = table.Column<int>(type: "int", nullable: false),
					TotalComments = table.Column<int>(type: "int", nullable: false),
					AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					SharedPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Post", x => x.Id);
					table.ForeignKey(
						name: "FK_Post_AspNetUsers_AppUserId",
						column: x => x.AppUserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Post_Post_SharedPostId",
						column: x => x.SharedPostId,
						principalTable: "Post",
						principalColumn: "Id",
						onDelete: ReferentialAction.NoAction);
				});

			migrationBuilder.CreateTable(
				name: "Message",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
					SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Message", x => x.Id);
					table.ForeignKey(
						name: "FK_Message_AspNetUsers_SenderId",
						column: x => x.SenderId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.NoAction);
					table.ForeignKey(
						name: "FK_Message_Chat_ChatId",
						column: x => x.ChatId,
						principalTable: "Chat",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Comment",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Comment", x => x.Id);
					table.ForeignKey(
						name: "FK_Comment_AspNetUsers_AppUserId",
						column: x => x.AppUserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.NoAction);
					table.ForeignKey(
						name: "FK_Comment_Post_PostId",
						column: x => x.PostId,
						principalTable: "Post",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Media",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
					PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
					PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Media", x => x.Id);
					table.ForeignKey(
						name: "FK_Media_Post_PostId",
						column: x => x.PostId,
						principalTable: "Post",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Reactions",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ReactionType = table.Column<int>(type: "int", nullable: false),
					AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Reactions", x => x.Id);
					table.ForeignKey(
						name: "FK_Reactions_AspNetUsers_AppUserId",
						column: x => x.AppUserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.NoAction);
					table.ForeignKey(
						name: "FK_Reactions_Post_PostId",
						column: x => x.PostId,
						principalTable: "Post",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "SavedPosts",
				columns: table => new
				{
					AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_SavedPosts", x => new { x.PostId, x.AppUserId });
					table.ForeignKey(
						name: "FK_SavedPosts_AspNetUsers_AppUserId",
						column: x => x.AppUserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.NoAction);
					table.ForeignKey(
						name: "FK_SavedPosts_Post_PostId",
						column: x => x.PostId,
						principalTable: "Post",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_AspNetRoleClaims_RoleId",
				table: "AspNetRoleClaims",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "RoleNameIndex",
				table: "AspNetRoles",
				column: "NormalizedName",
				unique: true,
				filter: "[NormalizedName] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserClaims_UserId",
				table: "AspNetUserClaims",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserLogins_UserId",
				table: "AspNetUserLogins",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserRoles_RoleId",
				table: "AspNetUserRoles",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "EmailIndex",
				table: "AspNetUsers",
				column: "NormalizedEmail");

			migrationBuilder.CreateIndex(
				name: "UserNameIndex",
				table: "AspNetUsers",
				column: "NormalizedUserName",
				unique: true,
				filter: "[NormalizedUserName] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_Chat_ReceiverId",
				table: "Chat",
				column: "ReceiverId");

			migrationBuilder.CreateIndex(
				name: "IX_Chat_SenderId",
				table: "Chat",
				column: "SenderId");

			migrationBuilder.CreateIndex(
				name: "IX_Comment_AppUserId",
				table: "Comment",
				column: "AppUserId");

			migrationBuilder.CreateIndex(
				name: "IX_Comment_PostId",
				table: "Comment",
				column: "PostId");

			migrationBuilder.CreateIndex(
				name: "IX_Friendship_ReciverId",
				table: "Friendship",
				column: "ReciverId");

			migrationBuilder.CreateIndex(
				name: "IX_Friendship_SenderId",
				table: "Friendship",
				column: "SenderId");

			migrationBuilder.CreateIndex(
				name: "IX_Media_PostId",
				table: "Media",
				column: "PostId");

			migrationBuilder.CreateIndex(
				name: "IX_Message_ChatId",
				table: "Message",
				column: "ChatId");

			migrationBuilder.CreateIndex(
				name: "IX_Message_SenderId",
				table: "Message",
				column: "SenderId");

			migrationBuilder.CreateIndex(
				name: "IX_Post_AppUserId",
				table: "Post",
				column: "AppUserId");

			migrationBuilder.CreateIndex(
				name: "IX_Post_SharedPostId",
				table: "Post",
				column: "SharedPostId");

			migrationBuilder.CreateIndex(
				name: "IX_Reactions_AppUserId",
				table: "Reactions",
				column: "AppUserId");

			migrationBuilder.CreateIndex(
				name: "IX_Reactions_PostId",
				table: "Reactions",
				column: "PostId");

			migrationBuilder.CreateIndex(
				name: "IX_SavedPosts_AppUserId",
				table: "SavedPosts",
				column: "AppUserId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AspNetRoleClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserLogins");

			migrationBuilder.DropTable(
				name: "AspNetUserRoles");

			migrationBuilder.DropTable(
				name: "AspNetUserTokens");

			migrationBuilder.DropTable(
				name: "Comment");

			migrationBuilder.DropTable(
				name: "Friendship");

			migrationBuilder.DropTable(
				name: "Media");

			migrationBuilder.DropTable(
				name: "Message");

			migrationBuilder.DropTable(
				name: "Reactions");

			migrationBuilder.DropTable(
				name: "SavedPosts");

			migrationBuilder.DropTable(
				name: "AspNetRoles");

			migrationBuilder.DropTable(
				name: "Chat");

			migrationBuilder.DropTable(
				name: "Post");

			migrationBuilder.DropTable(
				name: "AspNetUsers");
		}
	}
}
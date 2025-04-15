using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalChallengePlatform.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_AspNetUsers_UserId",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Categories_CategoryId",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Progress_AspNetUsers_UserId",
                table: "Progress");

            migrationBuilder.DropForeignKey(
                name: "FK_Progress_Challenges_ChallengeId",
                table: "Progress");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievements_AspNetUsers_UserId",
                table: "UserAchievements");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStatistics_AspNetUsers_UserId",
                table: "UserStatistics");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_AspNetUsers_UserId",
                table: "Challenges",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Categories_CategoryId",
                table: "Challenges",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_AspNetUsers_UserId",
                table: "Progress",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_Challenges_ChallengeId",
                table: "Progress",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievements_AspNetUsers_UserId",
                table: "UserAchievements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStatistics_AspNetUsers_UserId",
                table: "UserStatistics",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_AspNetUsers_UserId",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Categories_CategoryId",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Progress_AspNetUsers_UserId",
                table: "Progress");

            migrationBuilder.DropForeignKey(
                name: "FK_Progress_Challenges_ChallengeId",
                table: "Progress");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievements_AspNetUsers_UserId",
                table: "UserAchievements");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStatistics_AspNetUsers_UserId",
                table: "UserStatistics");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_AspNetUsers_UserId",
                table: "Challenges",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Categories_CategoryId",
                table: "Challenges",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_AspNetUsers_UserId",
                table: "Progress",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_Challenges_ChallengeId",
                table: "Progress",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievements_AspNetUsers_UserId",
                table: "UserAchievements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStatistics_AspNetUsers_UserId",
                table: "UserStatistics",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

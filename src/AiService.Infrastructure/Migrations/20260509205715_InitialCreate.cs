using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ai_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Instructions = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "generated_resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    JobRequisitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExperienceProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    AiProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    GeneratedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_generated_resumes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ai_profiles_UserId",
                table: "ai_profiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_generated_resumes_JobRequisitionId_UserId",
                table: "generated_resumes",
                columns: new[] { "JobRequisitionId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_profiles");

            migrationBuilder.DropTable(
                name: "generated_resumes");
        }
    }
}

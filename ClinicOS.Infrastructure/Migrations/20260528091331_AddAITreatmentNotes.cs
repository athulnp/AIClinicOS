using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAITreatmentNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiUsageLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClinicId = table.Column<int>(type: "int", nullable: false),
                    FeatureName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PromptTokens = table.Column<int>(type: "int", nullable: true),
                    CompletionTokens = table.Column<int>(type: "int", nullable: true),
                    TotalTokens = table.Column<int>(type: "int", nullable: true),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiUsageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiUsageLogs_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AiUsageLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClinicId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    ProcedureType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ToothNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Symptoms = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Diagnosis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TreatmentPerformed = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AiGeneratedNote = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    FinalNote = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    GeneratedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentNotes_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreatmentNotes_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreatmentNotes_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreatmentNotes_Users_GeneratedByUserId",
                        column: x => x.GeneratedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiUsageLogs_ClinicId_CreatedAt",
                table: "AiUsageLogs",
                columns: new[] { "ClinicId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AiUsageLogs_ClinicId_UserId",
                table: "AiUsageLogs",
                columns: new[] { "ClinicId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AiUsageLogs_FeatureName",
                table: "AiUsageLogs",
                column: "FeatureName");

            migrationBuilder.CreateIndex(
                name: "IX_AiUsageLogs_UserId",
                table: "AiUsageLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentNotes_AppointmentId",
                table: "TreatmentNotes",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentNotes_ClinicId_AppointmentId",
                table: "TreatmentNotes",
                columns: new[] { "ClinicId", "AppointmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentNotes_ClinicId_PatientId",
                table: "TreatmentNotes",
                columns: new[] { "ClinicId", "PatientId" });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentNotes_GeneratedByUserId",
                table: "TreatmentNotes",
                column: "GeneratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentNotes_PatientId",
                table: "TreatmentNotes",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiUsageLogs");

            migrationBuilder.DropTable(
                name: "TreatmentNotes");
        }
    }
}

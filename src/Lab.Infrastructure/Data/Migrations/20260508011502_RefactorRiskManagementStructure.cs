using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorRiskManagementStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Incidents_Risks_RelatedRiskId'
                )
                BEGIN
                    ALTER TABLE Incidents
                    DROP CONSTRAINT FK_Incidents_Risks_RelatedRiskId
                END
            """);

            migrationBuilder.DropIndex(
                name: "IX_Incidents_RelatedRiskId",
                table: "Incidents");

            // NOVA COLUNA NULLABLE
            migrationBuilder.AddColumn<Guid>(
                name: "RiskId",
                table: "Incidents",
                type: "uniqueidentifier",
                nullable: true);

            // MIGRA DADOS ANTIGOS
            migrationBuilder.Sql("""
                UPDATE Incidents
                SET RiskId = RelatedRiskId
                WHERE RelatedRiskId IS NOT NULL
            """);

            // REMOVE COLUNA ANTIGA
            migrationBuilder.DropColumn(
                name: "RelatedRiskId",
                table: "Incidents"
            );

            // VALIDA SE EXISTEM NULOS
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM Incidents
                    WHERE RiskId IS NULL
                )
                BEGIN
                    THROW 50000, 'Existem incidents sem RiskId.', 1;
                END
            """);

            // TORNA OBRIGATÓRIO
            migrationBuilder.AlterColumn<Guid>(
                name: "RiskId",
                table: "Incidents",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_RiskId",
                table: "Incidents",
                column: "RiskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Risks_RiskId",
                table: "Incidents",
                column: "RiskId",
                principalTable: "Risks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Controls_TenantId",
                table: "Controls");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "IncidentImpacts");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEvaluatedAt",
                table: "Risks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonForClose",
                table: "Risks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewFixedDate",
                table: "Risks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ReviewInterval",
                table: "Risks",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Incidents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "IncidentId",
                table: "IncidentImpacts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "IncidentImpacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeverityScore",
                table: "IncidentImpacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RiskSnapshot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThreatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VulnerabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Probability = table.Column<int>(type: "int", nullable: false),
                    Impact = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Treatment = table.Column<int>(type: "int", nullable: true),
                    TreatmentDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawScore = table.Column<int>(type: "int", nullable: false),
                    ResidualScore = table.Column<double>(type: "float", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    EffectivenessOnProbability = table.Column<double>(type: "float", nullable: false),
                    EffectivenessOnImpact = table.Column<double>(type: "float", nullable: false),
                    ReviewFixedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewInterval = table.Column<TimeSpan>(type: "time", nullable: true),
                    LastEvaluatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskSnapshot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiskHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Event = table.Column<int>(type: "int", nullable: false),
                    RiskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiskHistory_RiskSnapshot_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "RiskSnapshot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RiskHistory_Risks_RiskId",
                        column: x => x.RiskId,
                        principalTable: "Risks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RiskHistory_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Controls_TenantId_Name",
                table: "Controls",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RiskHistory_RiskId",
                table: "RiskHistory",
                column: "RiskId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskHistory_SnapshotId",
                table: "RiskHistory",
                column: "SnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskHistory_TenantId",
                table: "RiskHistory",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Risks_RiskId",
                table: "Incidents");

            migrationBuilder.DropTable(
                name: "RiskHistory");

            migrationBuilder.DropTable(
                name: "RiskSnapshot");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_RiskId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Controls_TenantId_Name",
                table: "Controls");

            migrationBuilder.DropColumn(
                name: "LastEvaluatedAt",
                table: "Risks");

            migrationBuilder.DropColumn(
                name: "ReasonForClose",
                table: "Risks");

            migrationBuilder.DropColumn(
                name: "ReviewFixedDate",
                table: "Risks");

            migrationBuilder.DropColumn(
                name: "ReviewInterval",
                table: "Risks");

            migrationBuilder.DropColumn(
                name: "RiskId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "IncidentImpacts");

            migrationBuilder.DropColumn(
                name: "SeverityScore",
                table: "IncidentImpacts");

            migrationBuilder.AddColumn<Guid>(
                name: "RelatedRiskId",
                table: "Incidents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IncidentId",
                table: "IncidentImpacts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "IncidentImpacts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CpfCnpj = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Offerings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offerings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offerings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Appointments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Offerings_OfferingId",
                        column: x => x.OfferingId,
                        principalTable: "Offerings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Appointments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_RelatedRiskId",
                table: "Incidents",
                column: "RelatedRiskId");

            migrationBuilder.CreateIndex(
                name: "IX_Controls_TenantId",
                table: "Controls",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CreatedBy",
                table: "Appointments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CustomerId",
                table: "Appointments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_OfferingId",
                table: "Appointments",
                column: "OfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TenantId",
                table: "Appointments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CpfCnpj",
                table: "Customers",
                column: "CpfCnpj",
                unique: true,
                filter: "[CpfCnpj] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId",
                table: "Customers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Offerings_TenantId",
                table: "Offerings",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Risks_RelatedRiskId",
                table: "Incidents",
                column: "RelatedRiskId",
                principalTable: "Risks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

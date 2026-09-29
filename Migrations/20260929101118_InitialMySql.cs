using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace KISMApplicationManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialMySql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastLoginAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    CountyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CountyNumber = table.Column<int>(type: "int", nullable: false),
                    CountyName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counties", x => x.CountyId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Intakes",
                columns: table => new
                {
                    IntakeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    IntakeName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    AcademicYear = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    IntakePeriod = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    ApplicationOpeningDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ApplicationClosingDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AdmissionStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AdmissionEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ApplicationsOpen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intakes", x => x.IntakeId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Programmes",
                columns: table => new
                {
                    ProgrammeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ProgrammeCode = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    ProgrammeName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    AwardType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    DurationYears = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programmes", x => x.ProgrammeId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicantQualifications",
                columns: table => new
                {
                    ApplicantQualificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    QualificationRoute = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    QualificationYear = table.Column<int>(type: "int", nullable: false),
                    ExaminationIndexNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    MeanGrade = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    QualificationType = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    InstitutionName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    FieldOfStudy = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CertificateNumber = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    VerificationStatus = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    VerifiedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    VerifiedByUserId = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantQualifications", x => x.ApplicantQualificationId);
                    table.ForeignKey(
                        name: "FK_ApplicantQualifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantQualifications_AspNetUsers_VerifiedByUserId",
                        column: x => x.VerifiedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Subcounties",
                columns: table => new
                {
                    SubcountyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CountyId = table.Column<int>(type: "int", nullable: false),
                    SubcountyName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcounties", x => x.SubcountyId);
                    table.ForeignKey(
                        name: "FK_Subcounties_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "CountyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CommitteeSittings",
                columns: table => new
                {
                    CommitteeSittingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    IntakeId = table.Column<int>(type: "int", nullable: false),
                    SittingDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Remarks = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeSittings", x => x.CommitteeSittingId);
                    table.ForeignKey(
                        name: "FK_CommitteeSittings_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommitteeSittings_Intakes_IntakeId",
                        column: x => x.IntakeId,
                        principalTable: "Intakes",
                        principalColumn: "IntakeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProgrammeRequirementGroups",
                columns: table => new
                {
                    ProgrammeRequirementGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ProgrammeId = table.Column<int>(type: "int", nullable: false),
                    ParentGroupId = table.Column<int>(type: "int", nullable: true),
                    GroupName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    SelectionRule = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    MinimumRequired = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeRequirementGroups", x => x.ProgrammeRequirementGroupId);
                    table.ForeignKey(
                        name: "FK_ProgrammeRequirementGroups_ProgrammeRequirementGroups_Parent~",
                        column: x => x.ParentGroupId,
                        principalTable: "ProgrammeRequirementGroups",
                        principalColumn: "ProgrammeRequirementGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgrammeRequirementGroups_Programmes_ProgrammeId",
                        column: x => x.ProgrammeId,
                        principalTable: "Programmes",
                        principalColumn: "ProgrammeId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicantQualificationSubjects",
                columns: table => new
                {
                    ApplicantQualificationSubjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ApplicantQualificationId = table.Column<int>(type: "int", nullable: false),
                    SubjectName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    SubjectCode = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    Grade = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantQualificationSubjects", x => x.ApplicantQualificationSubjectId);
                    table.ForeignKey(
                        name: "FK_ApplicantQualificationSubjects_ApplicantQualifications_Appli~",
                        column: x => x.ApplicantQualificationId,
                        principalTable: "ApplicantQualifications",
                        principalColumn: "ApplicantQualificationId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    DivisionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SubcountyId = table.Column<int>(type: "int", nullable: false),
                    DivisionName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.DivisionId);
                    table.ForeignKey(
                        name: "FK_Divisions_Subcounties_SubcountyId",
                        column: x => x.SubcountyId,
                        principalTable: "Subcounties",
                        principalColumn: "SubcountyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProgrammeRequirements",
                columns: table => new
                {
                    ProgrammeRequirementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ProgrammeRequirementGroupId = table.Column<int>(type: "int", nullable: false),
                    RequirementType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    RequirementName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    SubjectName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ComparisonOperator = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    RequiredValue = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeRequirements", x => x.ProgrammeRequirementId);
                    table.ForeignKey(
                        name: "FK_ProgrammeRequirements_ProgrammeRequirementGroups_ProgrammeRe~",
                        column: x => x.ProgrammeRequirementGroupId,
                        principalTable: "ProgrammeRequirementGroups",
                        principalColumn: "ProgrammeRequirementGroupId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    DivisionId = table.Column<int>(type: "int", nullable: false),
                    LocationName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Locations_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "DivisionId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sublocations",
                columns: table => new
                {
                    SublocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    SublocationName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sublocations", x => x.SublocationId);
                    table.ForeignKey(
                        name: "FK_Sublocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicantProfiles",
                columns: table => new
                {
                    ApplicantProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    FullName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    IdPassportBirthCertificateNo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    MobileNumber = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    ContactAddress = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    AddressCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Town = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ContactEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PlaceOfBirth = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    CountyId = table.Column<int>(type: "int", nullable: true),
                    SubcountyId = table.Column<int>(type: "int", nullable: true),
                    DivisionId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    SublocationId = table.Column<int>(type: "int", nullable: true),
                    MaritalStatus = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    ParentGuardianName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    ParentGuardianMobile = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    AlternativeContact = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantProfiles", x => x.ApplicantProfileId);
                    table.ForeignKey(
                        name: "FK_ApplicantProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantProfiles_Counties_CountyId",
                        column: x => x.CountyId,
                        principalTable: "Counties",
                        principalColumn: "CountyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicantProfiles_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "DivisionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicantProfiles_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicantProfiles_Subcounties_SubcountyId",
                        column: x => x.SubcountyId,
                        principalTable: "Subcounties",
                        principalColumn: "SubcountyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicantProfiles_Sublocations_SublocationId",
                        column: x => x.SublocationId,
                        principalTable: "Sublocations",
                        principalColumn: "SublocationId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ApplicationNumber = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    ApplicantProfileId = table.Column<int>(type: "int", nullable: false),
                    IntakeId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SubmittedByUserId = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_Applications_ApplicantProfiles_ApplicantProfileId",
                        column: x => x.ApplicantProfileId,
                        principalTable: "ApplicantProfiles",
                        principalColumn: "ApplicantProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_AspNetUsers_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_Intakes_IntakeId",
                        column: x => x.IntakeId,
                        principalTable: "Intakes",
                        principalColumn: "IntakeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicationProgrammeChoices",
                columns: table => new
                {
                    ApplicationProgrammeChoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ProgrammeId = table.Column<int>(type: "int", nullable: false),
                    ChoiceNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationProgrammeChoices", x => x.ApplicationProgrammeChoiceId);
                    table.ForeignKey(
                        name: "FK_ApplicationProgrammeChoices_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationProgrammeChoices_Programmes_ProgrammeId",
                        column: x => x.ProgrammeId,
                        principalTable: "Programmes",
                        principalColumn: "ProgrammeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CommitteeSittingApplications",
                columns: table => new
                {
                    CommitteeSittingApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CommitteeSittingId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    AddedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AddedByUserId = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeSittingApplications", x => x.CommitteeSittingApplicationId);
                    table.ForeignKey(
                        name: "FK_CommitteeSittingApplications_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommitteeSittingApplications_AspNetUsers_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommitteeSittingApplications_CommitteeSittings_CommitteeSitt~",
                        column: x => x.CommitteeSittingId,
                        principalTable: "CommitteeSittings",
                        principalColumn: "CommitteeSittingId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CommitteeDecisions",
                columns: table => new
                {
                    CommitteeDecisionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CommitteeSittingApplicationId = table.Column<int>(type: "int", nullable: false),
                    Decision = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    AssignedProgrammeId = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    DecisionDateUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RecordedByUserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeDecisions", x => x.CommitteeDecisionId);
                    table.ForeignKey(
                        name: "FK_CommitteeDecisions_AspNetUsers_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommitteeDecisions_CommitteeSittingApplications_CommitteeSit~",
                        column: x => x.CommitteeSittingApplicationId,
                        principalTable: "CommitteeSittingApplications",
                        principalColumn: "CommitteeSittingApplicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommitteeDecisions_Programmes_AssignedProgrammeId",
                        column: x => x.AssignedProgrammeId,
                        principalTable: "Programmes",
                        principalColumn: "ProgrammeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfiles_CountyId",
                table: "ApplicantProfiles",
                column: "CountyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfiles_DivisionId",
                table: "ApplicantProfiles",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfiles_LocationId",
                table: "ApplicantProfiles",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfiles_SubcountyId",
                table: "ApplicantProfiles",
                column: "SubcountyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfiles_SublocationId",
                table: "ApplicantProfiles",
                column: "SublocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantProfiles_UserId",
                table: "ApplicantProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantQualifications_UserId_QualificationRoute",
                table: "ApplicantQualifications",
                columns: new[] { "UserId", "QualificationRoute" });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantQualifications_VerifiedByUserId",
                table: "ApplicantQualifications",
                column: "VerifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantQualificationSubjects_ApplicantQualificationId_Subj~",
                table: "ApplicantQualificationSubjects",
                columns: new[] { "ApplicantQualificationId", "SubjectName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationProgrammeChoices_ApplicationId_ChoiceNumber",
                table: "ApplicationProgrammeChoices",
                columns: new[] { "ApplicationId", "ChoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationProgrammeChoices_ApplicationId_ProgrammeId",
                table: "ApplicationProgrammeChoices",
                columns: new[] { "ApplicationId", "ProgrammeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationProgrammeChoices_ProgrammeId",
                table: "ApplicationProgrammeChoices",
                column: "ProgrammeId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantProfileId_IntakeId",
                table: "Applications",
                columns: new[] { "ApplicantProfileId", "IntakeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicationNumber",
                table: "Applications",
                column: "ApplicationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_IntakeId",
                table: "Applications",
                column: "IntakeId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_SubmittedByUserId",
                table: "Applications",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeDecisions_AssignedProgrammeId",
                table: "CommitteeDecisions",
                column: "AssignedProgrammeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeDecisions_CommitteeSittingApplicationId",
                table: "CommitteeDecisions",
                column: "CommitteeSittingApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeDecisions_RecordedByUserId",
                table: "CommitteeDecisions",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeSittingApplications_AddedByUserId",
                table: "CommitteeSittingApplications",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeSittingApplications_ApplicationId",
                table: "CommitteeSittingApplications",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeSittingApplications_CommitteeSittingId_ApplicationId",
                table: "CommitteeSittingApplications",
                columns: new[] { "CommitteeSittingId", "ApplicationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeSittingApplications_CommitteeSittingId_DisplayOrder",
                table: "CommitteeSittingApplications",
                columns: new[] { "CommitteeSittingId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeSittings_CreatedByUserId",
                table: "CommitteeSittings",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeSittings_IntakeId_SittingDate",
                table: "CommitteeSittings",
                columns: new[] { "IntakeId", "SittingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CommitteeSittings_ReferenceNumber",
                table: "CommitteeSittings",
                column: "ReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Counties_CountyName",
                table: "Counties",
                column: "CountyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Counties_CountyNumber",
                table: "Counties",
                column: "CountyNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_SubcountyId_DivisionName",
                table: "Divisions",
                columns: new[] { "SubcountyId", "DivisionName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_DivisionId_LocationName",
                table: "Locations",
                columns: new[] { "DivisionId", "LocationName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeRequirementGroups_ParentGroupId",
                table: "ProgrammeRequirementGroups",
                column: "ParentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeRequirementGroups_ProgrammeId_DisplayOrder",
                table: "ProgrammeRequirementGroups",
                columns: new[] { "ProgrammeId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeRequirementGroups_ProgrammeId_ParentGroupId_Display~",
                table: "ProgrammeRequirementGroups",
                columns: new[] { "ProgrammeId", "ParentGroupId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeRequirements_ProgrammeRequirementGroupId_DisplayOrd~",
                table: "ProgrammeRequirements",
                columns: new[] { "ProgrammeRequirementGroupId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_ProgrammeCode",
                table: "Programmes",
                column: "ProgrammeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subcounties_CountyId_SubcountyName",
                table: "Subcounties",
                columns: new[] { "CountyId", "SubcountyName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sublocations_LocationId_SublocationName",
                table: "Sublocations",
                columns: new[] { "LocationId", "SublocationName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicantQualificationSubjects");

            migrationBuilder.DropTable(
                name: "ApplicationProgrammeChoices");

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
                name: "CommitteeDecisions");

            migrationBuilder.DropTable(
                name: "ProgrammeRequirements");

            migrationBuilder.DropTable(
                name: "ApplicantQualifications");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CommitteeSittingApplications");

            migrationBuilder.DropTable(
                name: "ProgrammeRequirementGroups");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "CommitteeSittings");

            migrationBuilder.DropTable(
                name: "Programmes");

            migrationBuilder.DropTable(
                name: "ApplicantProfiles");

            migrationBuilder.DropTable(
                name: "Intakes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Sublocations");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropTable(
                name: "Subcounties");

            migrationBuilder.DropTable(
                name: "Counties");
        }
    }
}

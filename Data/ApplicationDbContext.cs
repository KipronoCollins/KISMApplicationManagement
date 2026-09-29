using KISMApplicationManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =========================================================
        // Location Hierarchy
        // =========================================================

        public DbSet<County> Counties { get; set; }

        public DbSet<Subcounty> Subcounties { get; set; }

        public DbSet<Division> Divisions { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<Sublocation> Sublocations { get; set; }

        // =========================================================
        // Applicant
        // =========================================================

        public DbSet<ApplicantProfile> ApplicantProfiles { get; set; }

        public DbSet<ApplicantQualification>
            ApplicantQualifications { get; set; }

        public DbSet<ApplicantQualificationSubject>
            ApplicantQualificationSubjects { get; set; }

        // =========================================================
        // Programmes
        // =========================================================

        public DbSet<Programme> Programmes { get; set; }

        public DbSet<Intake> Intakes { get; set; }

        // =========================================================
        // Applications
        // =========================================================

        public DbSet<Application> Applications { get; set; }

        public DbSet<ApplicationProgrammeChoice>
            ApplicationProgrammeChoices { get; set; }

        // =========================================================
        // Programme Requirements
        // =========================================================

        public DbSet<ProgrammeRequirementGroup>
            ProgrammeRequirementGroups { get; set; }

        public DbSet<ProgrammeRequirement>
            ProgrammeRequirements { get; set; }

        // =========================================================
        // Committee
        // =========================================================

        public DbSet<CommitteeSitting>
            CommitteeSittings { get; set; }

        public DbSet<CommitteeSittingApplication>
            CommitteeSittingApplications { get; set; }

        public DbSet<CommitteeDecision>
            CommitteeDecisions { get; set; }

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =====================================================
            // County
            // =====================================================

            builder.Entity<County>()
                .HasIndex(county => county.CountyNumber)
                .IsUnique();

            builder.Entity<County>()
                .HasIndex(county => county.CountyName)
                .IsUnique();

            builder.Entity<County>()
                .Property(county => county.CountyName)
                .HasMaxLength(100);

            // =====================================================
            // Subcounty
            // =====================================================

            builder.Entity<Subcounty>()
                .HasOne(subcounty => subcounty.County)
                .WithMany(county => county.Subcounties)
                .HasForeignKey(subcounty => subcounty.CountyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Subcounty>()
                .HasIndex(subcounty => new
                {
                    subcounty.CountyId,
                    subcounty.SubcountyName
                })
                .IsUnique();

            builder.Entity<Subcounty>()
                .Property(subcounty => subcounty.SubcountyName)
                .HasMaxLength(100);

            // =====================================================
            // Division
            // =====================================================

            builder.Entity<Division>()
                .HasOne(division => division.Subcounty)
                .WithMany(subcounty => subcounty.Divisions)
                .HasForeignKey(division => division.SubcountyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Division>()
                .HasIndex(division => new
                {
                    division.SubcountyId,
                    division.DivisionName
                })
                .IsUnique();

            builder.Entity<Division>()
                .Property(division => division.DivisionName)
                .HasMaxLength(100);

            // =====================================================
            // Location
            // =====================================================

            builder.Entity<Location>()
                .HasOne(location => location.Division)
                .WithMany(division => division.Locations)
                .HasForeignKey(location => location.DivisionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Location>()
                .HasIndex(location => new
                {
                    location.DivisionId,
                    location.LocationName
                })
                .IsUnique();

            builder.Entity<Location>()
                .Property(location => location.LocationName)
                .HasMaxLength(100);

            // =====================================================
            // Sublocation
            // =====================================================

            builder.Entity<Sublocation>()
                .HasOne(sublocation => sublocation.Location)
                .WithMany(location => location.Sublocations)
                .HasForeignKey(sublocation => sublocation.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Sublocation>()
                .HasIndex(sublocation => new
                {
                    sublocation.LocationId,
                    sublocation.SublocationName
                })
                .IsUnique();

            builder.Entity<Sublocation>()
                .Property(sublocation => sublocation.SublocationName)
                .HasMaxLength(100);

            // =====================================================
            // Applicant Profile
            // =====================================================

            builder.Entity<ApplicantProfile>()
                .HasOne(profile => profile.User)
                .WithOne()
                .HasForeignKey<ApplicantProfile>(
                    profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicantProfile>()
                .HasIndex(profile => profile.UserId)
                .IsUnique();

            builder.Entity<ApplicantProfile>()
                .HasOne(profile => profile.County)
                .WithMany()
                .HasForeignKey(profile => profile.CountyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicantProfile>()
                .HasOne(profile => profile.Subcounty)
                .WithMany()
                .HasForeignKey(profile => profile.SubcountyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicantProfile>()
                .HasOne(profile => profile.Division)
                .WithMany()
                .HasForeignKey(profile => profile.DivisionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicantProfile>()
                .HasOne(profile => profile.Location)
                .WithMany()
                .HasForeignKey(profile => profile.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicantProfile>()
                .HasOne(profile => profile.Sublocation)
                .WithMany()
                .HasForeignKey(profile => profile.SublocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // Applicant Qualification
            // =====================================================

            builder.Entity<ApplicantQualification>()
                .HasOne(qualification => qualification.User)
                .WithMany()
                .HasForeignKey(qualification => qualification.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicantQualification>()
                .HasIndex(qualification => new
                {
                    qualification.UserId,
                    qualification.QualificationRoute
                });

            builder.Entity<ApplicantQualification>()
                .Property(qualification =>
                    qualification.QualificationRoute)
                .HasMaxLength(20);

            builder.Entity<ApplicantQualification>()
                .Property(qualification =>
                    qualification.ExaminationIndexNumber)
                .HasMaxLength(100);

            builder.Entity<ApplicantQualification>()
                .Property(qualification =>
                    qualification.MeanGrade)
                .HasMaxLength(100);

            builder.Entity<ApplicantQualification>()
                .Property(qualification =>
                    qualification.QualificationType)
                .HasMaxLength(150);

            builder.Entity<ApplicantQualification>()
                .Property(qualification =>
                    qualification.InstitutionName)
                .HasMaxLength(200);

            builder.Entity<ApplicantQualification>()
                .Property(qualification =>
                    qualification.FieldOfStudy)
                .HasMaxLength(200);

            builder.Entity<ApplicantQualification>()
                .Property(qualification =>
                    qualification.CertificateNumber)
                .HasMaxLength(150);

            builder.Entity<ApplicantQualification>()
                .Property(qualification =>
                    qualification.Description)
                .HasMaxLength(2000);

            builder.Entity<ApplicantQualification>()
                .Property(qualification =>
                    qualification.VerificationStatus)
                .HasMaxLength(30);

            // -----------------------------------------------------
            // Qualification Verification User
            // -----------------------------------------------------

            builder.Entity<ApplicantQualification>()
                .HasOne(qualification =>
                    qualification.VerifiedByUser)
                .WithMany()
                .HasForeignKey(qualification =>
                    qualification.VerifiedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // =====================================================
            // Applicant Qualification Subject
            // =====================================================

            builder.Entity<ApplicantQualificationSubject>()
                .HasOne(subject => subject.Qualification)
                .WithMany(qualification => qualification.Subjects)
                .HasForeignKey(subject =>
                    subject.ApplicantQualificationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicantQualificationSubject>()
                .HasIndex(subject => new
                {
                    subject.ApplicantQualificationId,
                    subject.SubjectName
                })
                .IsUnique();

            builder.Entity<ApplicantQualificationSubject>()
                .Property(subject => subject.SubjectName)
                .HasMaxLength(150);

            builder.Entity<ApplicantQualificationSubject>()
                .Property(subject => subject.SubjectCode)
                .HasMaxLength(30);

            builder.Entity<ApplicantQualificationSubject>()
                .Property(subject => subject.Grade)
                .HasMaxLength(10);

            // =====================================================
            // Programme
            // =====================================================

            builder.Entity<Programme>()
                .HasIndex(programme => programme.ProgrammeCode)
                .IsUnique();

            builder.Entity<Programme>()
                .Property(programme => programme.ProgrammeCode)
                .HasMaxLength(30);

            builder.Entity<Programme>()
                .Property(programme => programme.ProgrammeName)
                .HasMaxLength(200);

            builder.Entity<Programme>()
                .Property(programme => programme.AwardType)
                .HasMaxLength(50);

            builder.Entity<Programme>()
                .Property(programme => programme.Description)
                .HasMaxLength(2000);

            // =====================================================
            // Programme Requirement Group
            // =====================================================

            builder.Entity<ProgrammeRequirementGroup>()
                .HasOne(group => group.Programme)
                .WithMany(programme => programme.RequirementGroups)
                .HasForeignKey(group => group.ProgrammeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProgrammeRequirementGroup>()
                .HasIndex(group => new
                {
                    group.ProgrammeId,
                    group.DisplayOrder
                });

            builder.Entity<ProgrammeRequirementGroup>()
                .HasOne(group => group.ParentGroup)
                .WithMany(group => group.ChildGroups)
                .HasForeignKey(group => group.ParentGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProgrammeRequirementGroup>()
                .HasIndex(group => new
                {
                    group.ProgrammeId,
                    group.ParentGroupId,
                    group.DisplayOrder
                });

            builder.Entity<ProgrammeRequirementGroup>()
                .Property(group => group.GroupName)
                .HasMaxLength(150);

            builder.Entity<ProgrammeRequirementGroup>()
                .Property(group => group.Description)
                .HasMaxLength(500);

            builder.Entity<ProgrammeRequirementGroup>()
                .Property(group => group.SelectionRule)
                .HasMaxLength(20);

            // =====================================================
            // Programme Requirement
            // =====================================================

            builder.Entity<ProgrammeRequirement>()
                .HasOne(requirement =>
                    requirement.RequirementGroup)
                .WithMany(group =>
                    group.Requirements)
                .HasForeignKey(requirement =>
                    requirement.ProgrammeRequirementGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProgrammeRequirement>()
                .HasIndex(requirement => new
                {
                    requirement.ProgrammeRequirementGroupId,
                    requirement.DisplayOrder
                });

            builder.Entity<ProgrammeRequirement>()
                .Property(requirement =>
                    requirement.RequirementType)
                .HasMaxLength(50);

            builder.Entity<ProgrammeRequirement>()
                .Property(requirement =>
                    requirement.RequirementName)
                .HasMaxLength(150);

            builder.Entity<ProgrammeRequirement>()
                .Property(requirement =>
                    requirement.SubjectName)
                .HasMaxLength(100);

            builder.Entity<ProgrammeRequirement>()
                .Property(requirement =>
                    requirement.ComparisonOperator)
                .HasMaxLength(20);

            builder.Entity<ProgrammeRequirement>()
                .Property(requirement =>
                    requirement.RequiredValue)
                .HasMaxLength(50);

            builder.Entity<ProgrammeRequirement>()
                .Property(requirement =>
                    requirement.Description)
                .HasMaxLength(500);

            // =====================================================
            // Application
            // =====================================================

            builder.Entity<Application>()
                .HasKey(application => application.ApplicationId);

            builder.Entity<Application>()
                .Property(application =>
                    application.ApplicationNumber)
                .HasMaxLength(30)
                .IsRequired();

            builder.Entity<Application>()
                .Property(application =>
                    application.Status)
                .HasMaxLength(30)
                .IsRequired();

            builder.Entity<Application>()
                .HasIndex(application =>
                    application.ApplicationNumber)
                .IsUnique();

            builder.Entity<Application>()
                .HasIndex(application => new
                {
                    application.ApplicantProfileId,
                    application.IntakeId
                })
                .IsUnique();

            builder.Entity<Application>()
                .HasOne(application =>
                    application.ApplicantProfile)
                .WithMany()
                .HasForeignKey(application =>
                    application.ApplicantProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Application>()
                .HasOne(application =>
                    application.Intake)
                .WithMany()
                .HasForeignKey(application =>
                    application.IntakeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Application>()
                .HasOne(application =>
                    application.SubmittedByUser)
                .WithMany()
                .HasForeignKey(application =>
                    application.SubmittedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // Application Programme Choice
            // =====================================================

            builder.Entity<ApplicationProgrammeChoice>()
                .HasKey(choice =>
                    choice.ApplicationProgrammeChoiceId);

            builder.Entity<ApplicationProgrammeChoice>()
                .Property(choice =>
                    choice.ChoiceNumber)
                .IsRequired();

            builder.Entity<ApplicationProgrammeChoice>()
                .HasIndex(choice => new
                {
                    choice.ApplicationId,
                    choice.ChoiceNumber
                })
                .IsUnique();

            builder.Entity<ApplicationProgrammeChoice>()
                .HasIndex(choice => new
                {
                    choice.ApplicationId,
                    choice.ProgrammeId
                })
                .IsUnique();

            builder.Entity<ApplicationProgrammeChoice>()
                .HasOne(choice =>
                    choice.Application)
                .WithMany(application =>
                    application.ProgrammeChoices)
                .HasForeignKey(choice =>
                    choice.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationProgrammeChoice>()
                .HasOne(choice =>
                    choice.Programme)
                .WithMany()
                .HasForeignKey(choice =>
                    choice.ProgrammeId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // Committee Sitting
            // =====================================================

            builder.Entity<CommitteeSitting>()
                .HasKey(sitting =>
                    sitting.CommitteeSittingId);

            builder.Entity<CommitteeSitting>()
                .Property(sitting =>
                    sitting.ReferenceNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<CommitteeSitting>()
                .Property(sitting =>
                    sitting.Remarks)
                .HasMaxLength(2000);

            builder.Entity<CommitteeSitting>()
                .Property(sitting =>
                    sitting.Status)
                .HasMaxLength(30)
                .IsRequired();

            builder.Entity<CommitteeSitting>()
                .HasIndex(sitting =>
                    sitting.ReferenceNumber)
                .IsUnique();

            builder.Entity<CommitteeSitting>()
                .HasIndex(sitting => new
                {
                    sitting.IntakeId,
                    sitting.SittingDate
                });

            builder.Entity<CommitteeSitting>()
                .HasOne(sitting =>
                    sitting.Intake)
                .WithMany()
                .HasForeignKey(sitting =>
                    sitting.IntakeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CommitteeSitting>()
                .HasOne(sitting =>
                    sitting.CreatedByUser)
                .WithMany()
                .HasForeignKey(sitting =>
                    sitting.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // Committee Sitting Application
            // =====================================================

            builder.Entity<CommitteeSittingApplication>()
                .HasKey(sittingApplication =>
                    sittingApplication.CommitteeSittingApplicationId);

            builder.Entity<CommitteeSittingApplication>()
                .Property(sittingApplication =>
                    sittingApplication.DisplayOrder)
                .IsRequired();

            builder.Entity<CommitteeSittingApplication>()
                .HasIndex(sittingApplication => new
                {
                    sittingApplication.CommitteeSittingId,
                    sittingApplication.ApplicationId
                })
                .IsUnique();

            builder.Entity<CommitteeSittingApplication>()
                .HasIndex(sittingApplication => new
                {
                    sittingApplication.CommitteeSittingId,
                    sittingApplication.DisplayOrder
                });

            builder.Entity<CommitteeSittingApplication>()
                .HasOne(sittingApplication =>
                    sittingApplication.CommitteeSitting)
                .WithMany(sitting =>
                    sitting.Applications)
                .HasForeignKey(sittingApplication =>
                    sittingApplication.CommitteeSittingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CommitteeSittingApplication>()
                .HasOne(sittingApplication =>
                    sittingApplication.Application)
                .WithMany()
                .HasForeignKey(sittingApplication =>
                    sittingApplication.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CommitteeSittingApplication>()
                .HasOne(sittingApplication =>
                    sittingApplication.AddedByUser)
                .WithMany()
                .HasForeignKey(sittingApplication =>
                    sittingApplication.AddedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // Committee Decision
            // =====================================================

            builder.Entity<CommitteeDecision>()
                .HasKey(decision =>
                    decision.CommitteeDecisionId);

            builder.Entity<CommitteeDecision>()
                .Property(decision =>
                    decision.Decision)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<CommitteeDecision>()
                .Property(decision =>
                    decision.Remarks)
                .HasMaxLength(2000);

            builder.Entity<CommitteeDecision>()
                .HasIndex(decision =>
                    decision.CommitteeSittingApplicationId)
                .IsUnique();

            builder.Entity<CommitteeDecision>()
                .HasOne(decision =>
                    decision.CommitteeSittingApplication)
                .WithOne(sittingApplication =>
                    sittingApplication.Decision)
                .HasForeignKey<CommitteeDecision>(
                    decision =>
                        decision.CommitteeSittingApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CommitteeDecision>()
                .HasOne(decision =>
                    decision.AssignedProgramme)
                .WithMany()
                .HasForeignKey(decision =>
                    decision.AssignedProgrammeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CommitteeDecision>()
                .HasOne(decision =>
                    decision.RecordedByUser)
                .WithMany()
                .HasForeignKey(decision =>
                    decision.RecordedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
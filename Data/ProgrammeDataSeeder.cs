using KISMApplicationManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Data
{
    public static class ProgrammeDataSeeder
    {
        public static async Task SeedProgrammesAsync(
            ApplicationDbContext context)
        {
            var programmes = new[]
            {
                new Programme
                {
                    ProgrammeCode = "DLS",
                    ProgrammeName = "Diploma in Land Survey",
                    AwardType = "Diploma",
                    DurationYears = 3,
                    DisplayOrder = 1,
                    IsActive = true,
                    Description =
                        "Diploma programme in Land Survey.",
                    CreatedAtUtc = DateTime.UtcNow
                },

                new Programme
                {
                    ProgrammeCode = "DPRS",
                    ProgrammeName =
                        "Diploma in Photogrammetry & Remote Sensing",
                    AwardType = "Diploma",
                    DurationYears = 3,
                    DisplayOrder = 2,
                    IsActive = true,
                    Description =
                        "Diploma programme in Photogrammetry and Remote Sensing.",
                    CreatedAtUtc = DateTime.UtcNow
                },

                new Programme
                {
                    ProgrammeCode = "DCART",
                    ProgrammeName = "Diploma in Cartography",
                    AwardType = "Diploma",
                    DurationYears = 3,
                    DisplayOrder = 3,
                    IsActive = true,
                    Description =
                        "Diploma programme in Cartography.",
                    CreatedAtUtc = DateTime.UtcNow
                },

                new Programme
                {
                    ProgrammeCode = "DMRP",
                    ProgrammeName =
                        "Diploma in Map Reproduction (Printing)",
                    AwardType = "Diploma",
                    DurationYears = 3,
                    DisplayOrder = 4,
                    IsActive = true,
                    Description =
                        "Diploma programme in Map Reproduction and Printing.",
                    CreatedAtUtc = DateTime.UtcNow
                },

                new Programme
                {
                    ProgrammeCode = "DIT",
                    ProgrammeName =
                        "Diploma in Information Technology",
                    AwardType = "Diploma",
                    DurationYears = 3,
                    DisplayOrder = 5,
                    IsActive = true,
                    Description =
                        "Diploma programme in Information Technology.",
                    CreatedAtUtc = DateTime.UtcNow
                },

                new Programme
                {
                    ProgrammeCode = "CLS",
                    ProgrammeName =
                        "Certificate in Land Survey",
                    AwardType = "Certificate",
                    DurationYears = 2,
                    DisplayOrder = 6,
                    IsActive = true,
                    Description =
                        "Certificate programme in Land Survey.",
                    CreatedAtUtc = DateTime.UtcNow
                }
            };

            foreach (var programme in programmes)
            {
                var exists =
                    await context.Programmes
                        .AnyAsync(p =>
                            p.ProgrammeCode ==
                            programme.ProgrammeCode);

                if (!exists)
                {
                    context.Programmes.Add(programme);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
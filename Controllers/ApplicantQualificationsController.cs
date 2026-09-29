using System.Security.Claims;
using KISMApplicationManagement.Data;
using KISMApplicationManagement.Models;
using KISMApplicationManagement.ViewModels.ApplicantQualifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KISMApplicationManagement.Controllers
{
    [Authorize(Roles = "Applicant")]
    public class ApplicantQualificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicantQualificationsController> _logger;

        public ApplicantQualificationsController(
            ApplicationDbContext context,
            ILogger<ApplicantQualificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =========================================================
        // Index
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var qualifications = await _context.ApplicantQualifications
                .AsNoTracking()
                .Include(qualification => qualification.Subjects)
                .Where(qualification => qualification.UserId == userId)
                .OrderByDescending(qualification =>
                    qualification.QualificationYear)
                .ThenBy(qualification =>
                    qualification.QualificationRoute)
                .ToListAsync();

            var viewModels = qualifications
                .Select(MapToDetailsViewModel)
                .ToList();

            return View(viewModels);
        }

        // =========================================================
        // Create - GET
        // =========================================================

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ApplicantQualificationViewModel
            {
                QualificationYear = DateTime.UtcNow.Year,
                QualificationRoute = string.Empty
            };

            return PartialView("_CreatePartial", model);
        }

        // =========================================================
        // Create - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ApplicantQualificationViewModel model)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Your session has expired. Please log in again."
                });
            }

            NormalizeModel(model);

            if (!IsSupportedRoute(model.QualificationRoute))
            {
                ModelState.AddModelError(
                    nameof(model.QualificationRoute),
                    "Only KCSE and KNEC qualification routes are supported.");
            }

            ValidateQualification(model);

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Please correct the highlighted validation errors.",
                    errors = GetModelStateErrors()
                });
            }

            try
            {
                var qualification = new ApplicantQualification
                {
                    UserId = userId,
                    QualificationRoute = model.QualificationRoute,
                    QualificationYear = model.QualificationYear,
                    ExaminationIndexNumber =
                        model.ExaminationIndexNumber,
                    MeanGrade = model.MeanGrade,
                    QualificationType =
                        model.QualificationType,
                    InstitutionName =
                        model.InstitutionName,
                    FieldOfStudy =
                        model.FieldOfStudy,
                    CertificateNumber =
                        model.CertificateNumber,
                    Description =
                        model.Description,
                    VerificationStatus = "Pending",
                    CreatedAtUtc = DateTime.UtcNow
                };

                _context.ApplicantQualifications.Add(qualification);

                if (IsKcse(model.QualificationRoute))
                {
                    AddSubjects(
                        qualification,
                        model.Subjects);
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Qualification added successfully.",
                    qualificationId =
                        qualification.ApplicantQualificationId
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating applicant qualification for user {UserId}.",
                    userId);

                return Json(new
                {
                    success = false,
                    message =
                        "The qualification could not be saved. Please try again."
                });
            }
        }

        // =========================================================
        // Details - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var qualification =
                await _context.ApplicantQualifications
                    .AsNoTracking()
                    .Include(item => item.Subjects)
                    .FirstOrDefaultAsync(item =>
                        item.ApplicantQualificationId == id &&
                        item.UserId == userId);

            if (qualification == null)
            {
                return NotFound();
            }

            var model = MapToDetailsViewModel(qualification);

            return PartialView("_DetailsPartial", model);
        }

        // =========================================================
        // Edit - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var qualification =
                await _context.ApplicantQualifications
                    .AsNoTracking()
                    .Include(item => item.Subjects)
                    .FirstOrDefaultAsync(item =>
                        item.ApplicantQualificationId == id &&
                        item.UserId == userId);

            if (qualification == null)
            {
                return NotFound();
            }

            if (IsVerified(qualification))
            {
                return Json(new
                {
                    success = false,
                    message =
                        "This qualification has already been verified and cannot be edited."
                });
            }

            var model = new ApplicantQualificationViewModel
            {
                ApplicantQualificationId =
                    qualification.ApplicantQualificationId,

                QualificationRoute =
                    qualification.QualificationRoute,

                QualificationYear =
                    qualification.QualificationYear,

                ExaminationIndexNumber =
                    qualification.ExaminationIndexNumber,

                MeanGrade =
                    qualification.MeanGrade,

                QualificationType =
                    qualification.QualificationType,

                InstitutionName =
                    qualification.InstitutionName,

                FieldOfStudy =
                    qualification.FieldOfStudy,

                CertificateNumber =
                    qualification.CertificateNumber,

                Description =
                    qualification.Description,

                VerificationStatus =
                    qualification.VerificationStatus,

                Subjects = qualification.Subjects
                    .OrderBy(subject => subject.DisplayOrder)
                    .ThenBy(subject => subject.SubjectName)
                    .Select(subject =>
                        new ApplicantQualificationSubjectViewModel
                        {
                            ApplicantQualificationSubjectId =
                                subject.ApplicantQualificationSubjectId,

                            ApplicantQualificationId =
                                subject.ApplicantQualificationId,

                            SubjectName =
                                subject.SubjectName,

                            SubjectCode =
                                subject.SubjectCode,

                            Grade =
                                subject.Grade,

                            DisplayOrder =
                                subject.DisplayOrder
                        })
                    .ToList()
            };

            return PartialView("_EditPartial", model);
        }

        // =========================================================
        // Edit - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            ApplicantQualificationViewModel model)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Your session has expired. Please log in again."
                });
            }

            NormalizeModel(model);

            var qualification =
                await _context.ApplicantQualifications
                    .Include(item => item.Subjects)
                    .FirstOrDefaultAsync(item =>
                        item.ApplicantQualificationId ==
                            model.ApplicantQualificationId &&
                        item.UserId == userId);

            if (qualification == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Qualification not found."
                });
            }

            if (IsVerified(qualification))
            {
                return Json(new
                {
                    success = false,
                    message =
                        "This qualification has already been verified and cannot be edited."
                });
            }

            if (!IsSupportedRoute(model.QualificationRoute))
            {
                ModelState.AddModelError(
                    nameof(model.QualificationRoute),
                    "Only KCSE and KNEC qualification routes are supported.");
            }

            ValidateQualification(model);

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Please correct the highlighted validation errors.",
                    errors = GetModelStateErrors()
                });
            }

            try
            {
                qualification.QualificationRoute =
                    model.QualificationRoute;

                qualification.QualificationYear =
                    model.QualificationYear;

                qualification.ExaminationIndexNumber =
                    model.ExaminationIndexNumber;

                qualification.MeanGrade =
                    model.MeanGrade;

                qualification.QualificationType =
                    model.QualificationType;

                qualification.InstitutionName =
                    model.InstitutionName;

                qualification.FieldOfStudy =
                    model.FieldOfStudy;

                qualification.CertificateNumber =
                    model.CertificateNumber;

                qualification.Description =
                    model.Description;

                qualification.UpdatedAtUtc =
                    DateTime.UtcNow;

                // -------------------------------------------------
                // Replace KCSE subject collection
                // -------------------------------------------------

                foreach (var subject in qualification.Subjects.ToList())
                {
                    _context.ApplicantQualificationSubjects.Remove(subject);
                }

                if (IsKcse(model.QualificationRoute))
                {
                    AddSubjects(
                        qualification,
                        model.Subjects);
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Qualification updated successfully.",
                    qualificationId =
                        qualification.ApplicantQualificationId
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Concurrency conflict editing qualification {QualificationId}.",
                    model.ApplicantQualificationId);

                return Json(new
                {
                    success = false,
                    message =
                        "The qualification was changed by another request. Please refresh and try again."
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating qualification {QualificationId} for user {UserId}.",
                    model.ApplicantQualificationId,
                    userId);

                return Json(new
                {
                    success = false,
                    message =
                        "The qualification could not be updated. Please try again."
                });
            }
        }

        // =========================================================
        // Delete - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Your session has expired. Please log in again."
                });
            }

            var qualification =
                await _context.ApplicantQualifications
                    .Include(item => item.Subjects)
                    .FirstOrDefaultAsync(item =>
                        item.ApplicantQualificationId == id &&
                        item.UserId == userId);

            if (qualification == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Qualification not found."
                });
            }

            if (IsVerified(qualification))
            {
                return Json(new
                {
                    success = false,
                    message =
                        "This qualification has already been verified and cannot be deleted."
                });
            }

            try
            {
                _context.ApplicantQualifications.Remove(qualification);

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Qualification deleted successfully."
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Error deleting qualification {QualificationId} for user {UserId}.",
                    id,
                    userId);

                return Json(new
                {
                    success = false,
                    message =
                        "The qualification could not be deleted. Please try again."
                });
            }
        }

        // =========================================================
        // Helpers
        // =========================================================

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(
                ClaimTypes.NameIdentifier);
        }

        private static bool IsSupportedRoute(
            string? qualificationRoute)
        {
            if (string.IsNullOrWhiteSpace(
                    qualificationRoute))
            {
                return false;
            }

            return qualificationRoute.Equals(
                       "KCSE",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   qualificationRoute.Equals(
                       "KNEC",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKcse(
            string? qualificationRoute)
        {
            return string.Equals(
                qualificationRoute,
                "KCSE",
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsKnec(
            string? qualificationRoute)
        {
            return string.Equals(
                qualificationRoute,
                "KNEC",
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsVerified(
            ApplicantQualification qualification)
        {
            return string.Equals(
                qualification.VerificationStatus,
                "Verified",
                StringComparison.OrdinalIgnoreCase);
        }

        private static void NormalizeModel(
            ApplicantQualificationViewModel model)
        {
            model.QualificationRoute =
                model.QualificationRoute?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            model.ExaminationIndexNumber =
                NormalizeNullable(
                    model.ExaminationIndexNumber);

            model.MeanGrade =
                NormalizeNullable(
                    model.MeanGrade);

            model.QualificationType =
                NormalizeNullable(
                    model.QualificationType);

            model.InstitutionName =
                NormalizeNullable(
                    model.InstitutionName);

            model.FieldOfStudy =
                NormalizeNullable(
                    model.FieldOfStudy);

            model.CertificateNumber =
                NormalizeNullable(
                    model.CertificateNumber);

            model.Description =
                NormalizeNullable(
                    model.Description);

            if (model.Subjects == null)
            {
                model.Subjects =
                    new List<ApplicantQualificationSubjectViewModel>();
            }

            foreach (var subject in model.Subjects)
            {
                subject.SubjectName =
                    subject.SubjectName?.Trim()
                    ?? string.Empty;

                subject.SubjectCode =
                    NormalizeNullable(
                        subject.SubjectCode);

                subject.Grade =
                    subject.Grade?.Trim()
                    .ToUpperInvariant()
                    ?? string.Empty;
            }
        }

        private static string? NormalizeNullable(
            string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }

        // =========================================================
        // Qualification Validation
        // =========================================================

        private void ValidateQualification(
            ApplicantQualificationViewModel model)
        {
            if (!IsSupportedRoute(model.QualificationRoute))
            {
                return;
            }

            if (IsKcse(model.QualificationRoute))
            {
                if (string.IsNullOrWhiteSpace(model.MeanGrade))
                {
                    ModelState.AddModelError(
                        nameof(model.MeanGrade),
                        "Please enter your KCSE mean grade.");
                }

                if (string.IsNullOrWhiteSpace(
                        model.ExaminationIndexNumber))
                {
                    ModelState.AddModelError(
                        nameof(model.ExaminationIndexNumber),
                        "Please enter your KCSE examination index number.");
                }

                if (model.Subjects == null ||
                    model.Subjects.Count == 0)
                {
                    ModelState.AddModelError(
                        nameof(model.Subjects),
                        "Please add at least one KCSE subject.");
                }
                else
                {
                    ValidateSubjects(model.Subjects);
                }
            }

            if (IsKnec(model.QualificationRoute))
            {
                if (string.IsNullOrWhiteSpace(
                        model.QualificationType))
                {
                    ModelState.AddModelError(
                        nameof(model.QualificationType),
                        "Please enter the KNEC qualification type.");
                }

                if (string.IsNullOrWhiteSpace(
                        model.InstitutionName))
                {
                    ModelState.AddModelError(
                        nameof(model.InstitutionName),
                        "Please enter the institution name.");
                }

                if (string.IsNullOrWhiteSpace(
                        model.FieldOfStudy))
                {
                    ModelState.AddModelError(
                        nameof(model.FieldOfStudy),
                        "Please enter the field of study.");
                }
            }
        }

        private void ValidateSubjects(
            IEnumerable<ApplicantQualificationSubjectViewModel>
                subjects)
        {
            var subjectList = subjects.ToList();

            var duplicateSubjects =
                subjectList
                    .Where(subject =>
                        !string.IsNullOrWhiteSpace(
                            subject.SubjectName))
                    .GroupBy(subject =>
                        subject.SubjectName.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key)
                    .ToList();

            if (duplicateSubjects.Count > 0)
            {
                ModelState.AddModelError(
                    nameof(ApplicantQualificationViewModel.Subjects),
                    "A KCSE subject cannot be entered more than once.");
            }

            for (var index = 0;
                 index < subjectList.Count;
                 index++)
            {
                var subject = subjectList[index];

                if (string.IsNullOrWhiteSpace(
                        subject.SubjectName))
                {
                    ModelState.AddModelError(
                        $"Subjects[{index}].SubjectName",
                        "Please enter the subject name.");
                }

                if (string.IsNullOrWhiteSpace(
                        subject.Grade))
                {
                    ModelState.AddModelError(
                        $"Subjects[{index}].Grade",
                        "Please enter the subject grade.");
                }
            }
        }

        private static void AddSubjects(
            ApplicantQualification qualification,
            IEnumerable<ApplicantQualificationSubjectViewModel> subjects)
        {
            var order = 1;

            foreach (var subject in subjects)
            {
                if (string.IsNullOrWhiteSpace(
                        subject.SubjectName))
                {
                    continue;
                }

                qualification.Subjects.Add(
                    new ApplicantQualificationSubject
                    {
                        SubjectName =
                            subject.SubjectName.Trim(),

                        SubjectCode =
                            string.IsNullOrWhiteSpace(
                                subject.SubjectCode)
                                ? null
                                : subject.SubjectCode.Trim(),

                        Grade =
                            subject.Grade.Trim()
                                .ToUpperInvariant(),

                        DisplayOrder = order++,

                        CreatedAtUtc =
                            DateTime.UtcNow
                    });
            }
        }

        // =========================================================
        // Mapping
        // =========================================================

        private static ApplicantQualificationDetailsViewModel
            MapToDetailsViewModel(
                ApplicantQualification qualification)
        {
            return new ApplicantQualificationDetailsViewModel
            {
                ApplicantQualificationId =
                    qualification.ApplicantQualificationId,

                QualificationRoute =
                    qualification.QualificationRoute,

                QualificationYear =
                    qualification.QualificationYear,

                ExaminationIndexNumber =
                    qualification.ExaminationIndexNumber,

                MeanGrade =
                    qualification.MeanGrade,

                QualificationType =
                    qualification.QualificationType,

                InstitutionName =
                    qualification.InstitutionName,

                FieldOfStudy =
                    qualification.FieldOfStudy,

                CertificateNumber =
                    qualification.CertificateNumber,

                Description =
                    qualification.Description,

                VerificationStatus =
                    qualification.VerificationStatus,

                VerifiedAtUtc =
                    qualification.VerifiedAtUtc,

                CreatedAtUtc =
                    qualification.CreatedAtUtc,

                UpdatedAtUtc =
                    qualification.UpdatedAtUtc,

                Subjects = qualification.Subjects
                    .OrderBy(subject =>
                        subject.DisplayOrder)
                    .ThenBy(subject =>
                        subject.SubjectName)
                    .Select(subject =>
                        new ApplicantQualificationSubjectViewModel
                        {
                            ApplicantQualificationSubjectId =
                                subject.ApplicantQualificationSubjectId,

                            ApplicantQualificationId =
                                subject.ApplicantQualificationId,

                            SubjectName =
                                subject.SubjectName,

                            SubjectCode =
                                subject.SubjectCode,

                            Grade =
                                subject.Grade,

                            DisplayOrder =
                                subject.DisplayOrder
                        })
                    .ToList()
            };
        }

        // =========================================================
        // ModelState Error Helper
        // =========================================================

        private Dictionary<string, string[]> GetModelStateErrors()
        {
            return ModelState
                .Where(item =>
                    item.Value != null &&
                    item.Value.Errors.Count > 0)
                .ToDictionary(
                    item => item.Key,
                    item => item.Value!.Errors
                        .Select(error =>
                            string.IsNullOrWhiteSpace(
                                error.ErrorMessage)
                                ? "Invalid value."
                                : error.ErrorMessage)
                        .ToArray());
        }
    }
}
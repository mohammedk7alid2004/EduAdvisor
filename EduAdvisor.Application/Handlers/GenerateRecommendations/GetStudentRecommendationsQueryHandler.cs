using EduAdvisor.Application.Commands.GenerateRecommendations;
using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.DTO.AiRecommendation;
using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Queries.CourseModules;
using EduAdvisor.Domain.Enums.University;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduAdvisor.Application.Handlers.GenerateRecommendations;

public sealed class GetStudentRecommendationsQueryHandler(
    IApplicationDbContext context,
    IGetCurrentUserRepository currentUser,
    ISender sender)
    : IRequestHandler<
        GetStudentRecommendationsQuery,
        Result<StudentRecommendationsResultDto>>
{
    private const int GraduationRequiredHours = 144;

    public async Task<Result<StudentRecommendationsResultDto>> Handle(
        GetStudentRecommendationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<StudentRecommendationsResultDto>
                .Unauthorized("Unauthorized.");
        }

        var student = await context.Students
            .AsNoTracking()
            .Where(student => student.UserId == userId)
            .Select(student => new StudentData(
                student.Id,
                student.Department.Name,
                student.GPA,
                (int)student.AcademicYear,
                student.CompletedHours))
            .SingleOrDefaultAsync(cancellationToken);

        if (student is null)
        {
            return Result<StudentRecommendationsResultDto>
                .NotFound("Student profile not found.");
        }

        var semester = await context.Semesters
            .AsNoTracking()
            .Where(semester => semester.IsActive)
            .Select(semester => new SemesterData(
                semester.Id,
                semester.StandardSemesterNumber))
            .SingleOrDefaultAsync(cancellationToken);

        if (semester is null)
        {
            return Result<StudentRecommendationsResultDto>
                .NotFound("No active semester was found.");
        }

        /*
         * First-level students do not have enough academic history
         * for AI recommendations. Return all offered semester courses,
         * including courses that the student has already registered.
         */
        if (student.Level == 1)
        {
            var availableCourses =
                await GetFirstLevelCoursesAsync(
                    student.Id,
                    semester.Id,
                    cancellationToken);

            if (availableCourses.Count == 0)
            {
                return Result<StudentRecommendationsResultDto>
                    .NotFound("No available courses were found.");
            }

            var response = new StudentRecommendationsResultDto(
                HasAiRecommendations: false,
                Recommendations: [],
                AvailableCourses: availableCourses);

            return Result<StudentRecommendationsResultDto>.Success(
                response,
                "Available courses retrieved for first-level student.");
        }

        var recommendations = await GetRecommendationsAsync(
            student.Id,
            semester.Id,
            cancellationToken);

        if (recommendations.Count > 0)
        {
            var response = new StudentRecommendationsResultDto(
                HasAiRecommendations: true,
                Recommendations: recommendations,
                AvailableCourses: []);

            return Result<StudentRecommendationsResultDto>.Success(
                response,
                "Student recommendations retrieved successfully.");
        }

        var registeredHours = await GetRegisteredHoursAsync(
            student.Id,
            semester.Id,
            cancellationToken);

        var coursesForAi = await GetCoursesForAiAsync(
            cancellationToken);

        if (coursesForAi.Count == 0)
        {
            return Result<StudentRecommendationsResultDto>
                .NotFound("No available courses were found.");
        }

        var isGraduationSemester = IsGraduationSemester(
            student.CompletedHours,
            registeredHours);

        var command = new GenerateRecommendationsCommand(
            StudentId: student.Id,
            SemesterId: semester.Id,
            StudentMajor: student.Major,
            CurrentGpa: student.CurrentGpa,
            Level: student.Level,
            CompletedHours: student.CompletedHours,
            RegisteredHours: registeredHours,
            Semester: semester.Number,
            IsGraduationSemester: isGraduationSemester,
            AvailableCourses: coursesForAi);

        var generationResult = await sender.Send(
            command,
            cancellationToken);

        if (!generationResult.IsSuccess)
        {
            return Result<StudentRecommendationsResultDto>.Error(
                generationResult.Message);
        }

        recommendations = await GetRecommendationsAsync(
            student.Id,
            semester.Id,
            cancellationToken);

        if (recommendations.Count == 0)
        {
            return Result<StudentRecommendationsResultDto>.Error(
                "Recommendations were generated but could not be retrieved.");
        }

        var generatedResponse = new StudentRecommendationsResultDto(
            HasAiRecommendations: true,
            Recommendations: recommendations,
            AvailableCourses: []);

        return Result<StudentRecommendationsResultDto>.Success(
            generatedResponse,
            "Student recommendations generated successfully.");
    }

    private Task<List<AvailableCourseDetailsDto>>
        GetFirstLevelCoursesAsync(
            Guid studentId,
            Guid semesterId,
            CancellationToken cancellationToken)
    {
        return context.SemesterCourses
            .AsNoTracking()
            .Where(semesterCourse =>
                semesterCourse.SemesterId == semesterId)
            .Select(semesterCourse =>
                new AvailableCourseDetailsDto
                {
                    SemesterCourseId = semesterCourse.Id,

                    CourseId = semesterCourse
                        .CourseAcademicPlan.CourseId,

                    SemesterId = semesterCourse.SemesterId,

                    CourseCode = semesterCourse
                        .CourseAcademicPlan
                        .Course
                        .CourseCode,

                    CourseName = semesterCourse
                        .CourseAcademicPlan
                        .Course
                        .CourseName,

                    CreditHours = semesterCourse
                        .CourseAcademicPlan
                        .Course
                        .CreditHours,

                    IsRegistered = context.Enrollments.Any(
                        enrollment =>
                            enrollment.StudentId == studentId &&
                            enrollment.SemesterCourseId ==
                            semesterCourse.Id &&
                            (enrollment.Status ==
                             EnrollmentStatus.Pending ||
                             enrollment.Status ==
                             EnrollmentStatus.Approved))
                })
            .OrderBy(course => course.CourseCode)
            .ToListAsync(cancellationToken);
    }

    private async Task<List<AvailableCourseDto>>
        GetCoursesForAiAsync(
            CancellationToken cancellationToken)
    {
        var courseCodes = await context.Courses
            .AsNoTracking()
            .Where(course =>
                !string.IsNullOrWhiteSpace(course.CourseCode))
            .OrderBy(course => course.CourseCode)
            .Select(course => course.CourseCode)
            .Distinct()
            .ToListAsync(cancellationToken);

        return courseCodes
            .Select(courseCode => new AvailableCourseDto(
                CourseCode: courseCode.Trim(),
                PrereqGrades: []))
            .ToList();
    }

    private async Task<int> GetRegisteredHoursAsync(
        Guid studentId,
        Guid semesterId,
        CancellationToken cancellationToken)
    {
        return await context.Enrollments
            .AsNoTracking()
            .Where(enrollment =>
                enrollment.StudentId == studentId &&
                enrollment.SemesterCourse.SemesterId == semesterId &&
                (enrollment.Status == EnrollmentStatus.Pending ||
                 enrollment.Status == EnrollmentStatus.Approved))
            .SumAsync(
                enrollment => (int?)enrollment
                    .SemesterCourse
                    .CourseAcademicPlan
                    .Course
                    .CreditHours,
                cancellationToken) ?? 0;
    }

    private Task<List<StudentRecommendationDto>>
        GetRecommendationsAsync(
            Guid studentId,
            Guid semesterId,
            CancellationToken cancellationToken)
    {
        return context.CourseRecommendations
            .AsNoTracking()
            .Where(recommendation =>
                recommendation.StudentId == studentId &&
                recommendation.SemesterId == semesterId)
            .Join(
                context.Courses.AsNoTracking(),
                recommendation => recommendation.CourseId,
                course => course.Id,
                (recommendation, course) =>
                    new StudentRecommendationDto
                    {
                        Id = recommendation.Id,
                        CourseId = course.Id,
                        CourseCode = course.CourseCode,
                        CourseName = course.CourseName,
                        CreditHours = course.CreditHours,
                        SemesterId = recommendation.SemesterId,
                        Difficulty =
                            recommendation.Difficulty.ToString(),
                        Description = recommendation.Description,
                        Reasoning = recommendation.Reasoning,
                        ExpectedGpaImpact =
                            recommendation.ExpectedGpaImpact
                    })
            .OrderByDescending(recommendation =>
                recommendation.ExpectedGpaImpact)
            .ThenBy(recommendation =>
                recommendation.CourseCode)
            .ToListAsync(cancellationToken);
    }

    private static bool IsGraduationSemester(
        int completedHours,
        int registeredHours)
    {
        var remainingHours =
            GraduationRequiredHours - completedHours;

        return remainingHours > 0 &&
               remainingHours <= registeredHours;
    }

    private sealed record StudentData(
        Guid Id,
        string Major,
        decimal CurrentGpa,
        int Level,
        int CompletedHours);

    private sealed record SemesterData(
        Guid Id,
        int Number);
}
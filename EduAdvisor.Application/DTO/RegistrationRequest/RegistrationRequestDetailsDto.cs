using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Application.DTO.RegistrationRequest
{
    public sealed record RegistrationRequestDetailsDto
    {
        public Guid RegistrationRequestId { get; init; }

        public string StudentName { get; init; } = string.Empty;

        public string StudentCode { get; init; } = string.Empty;

        public string? StudentPhotoUrl { get; init; }

        public string DepartmentName { get; init; } = string.Empty;

        public int AcademicYear { get; init; }

        public decimal GPA { get; init; }

        public int CompletedHours { get; init; }

        public int FailedCoursesCount { get; init; }

        public string Status { get; init; }

        public List<RequestedCourseDto> Courses { get; init; } = [];
    }
}

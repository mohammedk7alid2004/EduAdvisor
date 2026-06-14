using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduAdvisor.Application.DTO.RegistrationRequest
{
    public sealed record RequestedCourseDto
    {
        public Guid EnrollmentId { get; init; }

        public Guid CourseId { get; init; }

        public string CourseCode { get; init; } = string.Empty;

        public string CourseName { get; init; } = string.Empty;

        public int CreditHours { get; init; }

        public bool IsRetake { get; init; }

        public bool HasMissingPrerequisites { get; init; }

        public List<string> MissingPrerequisites { get; init; } = [];
    }
}

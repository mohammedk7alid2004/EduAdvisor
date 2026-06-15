using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduAdvisor.Domain.Enums.University;

namespace EduAdvisor.Application.DTO.User
{
    public sealed record RegistrationRequestListDto
    {
        public Guid Id { get; init; }

        public string SemesterName { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;

        public DateTime SubmittedAt { get; init; }

        public string? Notes { get; init; }

        public int CoursesCount { get; init; }
    }
}

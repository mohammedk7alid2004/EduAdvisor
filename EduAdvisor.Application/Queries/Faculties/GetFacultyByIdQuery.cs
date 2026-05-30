using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduAdvisor.Application.DTO.Faculties;

namespace EduAdvisor.Application.Queries.Faculties;

public sealed record GetFacultyByIdQuery(Guid Id)
    : IRequest<Result<FacultyResponse>>;

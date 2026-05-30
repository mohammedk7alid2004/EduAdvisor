using FluentValidation.Results;

namespace EduAdvisor.Application.Exceptions
{
    public class ValidationExceptions : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationExceptions(IEnumerable<ValidationFailure> failures)
            : base("Validation errors occurred.")
        {
            Errors = failures
                .GroupBy(e => e.PropertyName)
                .ToDictionary(k => k.Key, v => v.Select(x => x.ErrorMessage).ToArray());
        }
    }
}

using FluentValidation.Results;

namespace SiaInteractive.Application.Dtos.Common
{
    public class ResponseGeneric<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public IEnumerable<ValidationFailure>? ValidationErrors { get; set; }
    }
}

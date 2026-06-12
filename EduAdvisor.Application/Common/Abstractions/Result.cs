namespace EduAdvisor.Application.Common.Abstractions
{
    public class Result<TEntity>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public TEntity? Data { get; set; }

        public static Result<TEntity> Success(TEntity data, string message = "", int statusCode = 200)
            => new() { IsSuccess = true, Data = data, Message = message, StatusCode = statusCode };

        public static Result<TEntity> Failure(string message, int statusCode = 400)
            => new() { IsSuccess = false, Message = message, StatusCode = statusCode };

        public static Result<TEntity> Unauthorized(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = 401 };

        public static Result<TEntity> Forbidden(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = 403 };

        public static Result<TEntity> NotFound(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = 404 };

        public static Result<TEntity> Conflict(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = 409 };

        public static Result<TEntity> Error(string message, int statusCode = 500)
            => new() { IsSuccess = false, Message = message, StatusCode = statusCode };
    }
}
namespace EduAdvisor.Application.Common.Abstractions
{
    public class Result<TEntity>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public TEntity? Data { get; set; }

        public static Result<TEntity> Success(TEntity data, string message = null, int statusCode = 200)
        {
            return new Result<TEntity> { IsSuccess = true, Data = data, Message = message, StatusCode = statusCode };
        }

        public static Result<TEntity> Failure(string message, int statusCode = 400)
        {
            return new Result<TEntity> { IsSuccess = false, Message = message, StatusCode = statusCode };
        }

        public static Result<TEntity> Error(string message, int statusCode = 500)
        {
            return new Result<TEntity> { IsSuccess = false, Message = message, StatusCode = statusCode };
        }
        public static Result<TEntity> NotFound(string message)
        {
            return new Result<TEntity> { IsSuccess = false, Message = message, StatusCode = 404 };
        }

        public static Result<TEntity> Conflict(string message)
        {
            return new Result<TEntity> { IsSuccess = false, Message = message, StatusCode = 409 };
        }
    }
}

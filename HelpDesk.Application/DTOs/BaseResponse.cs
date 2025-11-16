namespace HelpDesk.Application.DTOs
{
    public class BaseResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }
        public T? Result { get; set; }

        public static BaseResponse<T> Success(T data, string message = "Operation completed success")
        {
            return new()
            {
                IsSuccess = true,
                Message = message,
                Result = data
            };
        }

        public static BaseResponse<T> Failure(string message, string? errorCode = null)
        {
            return new()
            {
                IsSuccess = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

    }
}

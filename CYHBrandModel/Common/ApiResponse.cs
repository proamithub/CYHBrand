namespace CYHBrandModel.Common
{
    /// <summary>
    /// Generic API / service-layer response used everywhere instead of bare FormResponseDTO.
    /// SP must return: Id, IsSuccess (bit), Message, ErrorCode (optional).
    /// </summary>
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int? Id { get; set; }             // inserted / updated record id
        public string? ErrorCode { get; set; }   // e.g. "DUPLICATE_EMAIL", "NOT_FOUND"
        public List<string> Errors { get; set; } = new();

        // ── factory helpers ──────────────────────────────────────────────
        public static ApiResponse<T> Ok(T? data = default, string message = "Success", int? id = null)
            => new() { IsSuccess = true, Data = data, Message = message, Id = id };

        public static ApiResponse<T> Fail(string message, string? errorCode = null)
            => new() { IsSuccess = false, Message = message, ErrorCode = errorCode };

        public static ApiResponse<T> Fail(List<string> errors, string message = "Validation failed")
            => new() { IsSuccess = false, Message = message, Errors = errors };

        public static ApiResponse<T> ServerError(string? detail = null)
            => new() { IsSuccess = false, Message = "An unexpected error occurred.", ErrorCode = "SERVER_ERROR",
                       Errors = detail is null ? new() : new() { detail } };
    }

    /// <summary>Non-generic shorthand for void operations (save / delete / toggle).</summary>
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse OkResult(string message = "Success", int? id = null)
            => new() { IsSuccess = true, Message = message, Id = id };

        public static ApiResponse FailResult(string message, string? errorCode = null)
            => new() { IsSuccess = false, Message = message, ErrorCode = errorCode };

        public static ApiResponse ServerErrorResult(string? detail = null)
            => new() { IsSuccess = false, Message = "An unexpected error occurred.", ErrorCode = "SERVER_ERROR",
                       Errors = detail is null ? new() : new() { detail } };
    }

    /// <summary>
    /// Thin SP result model — what stored procedures actually return.
    /// Map columns: Id, IsSuccess, Message, ErrorCode.
    /// </summary>
    public class SpResult
    {
        public int? Id { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }

        public ApiResponse ToApiResponse()
            => IsSuccess
                ? ApiResponse.OkResult(Message ?? "Success", Id)
                : ApiResponse.FailResult(Message ?? "Operation failed", ErrorCode);
    }
}

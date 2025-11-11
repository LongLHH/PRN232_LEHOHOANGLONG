namespace WebAPI.Models;

/// <summary>
/// Cấu trúc chuẩn cho API Response
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu của data</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Thông báo kết quả (success message hoặc error message)
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// HTTP Status Code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Dữ liệu trả về (có thể là object, array, hoặc null)
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Constructor mặc định
    /// </summary>
    public ApiResponse()
    {
    }

    /// <summary>
    /// Constructor đầy đủ
    /// </summary>
    public ApiResponse(int statusCode, string message, T? data = default)
    {
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    /// <summary>
    /// Tạo response thành công
    /// </summary>
    public static ApiResponse<T> Success(T? data, string message = "Success")
    {
        return new ApiResponse<T>(200, message, data);
    }

    /// <summary>
    /// Tạo response thành công với status code tùy chỉnh
    /// </summary>
    public static ApiResponse<T> Success(int statusCode, T? data, string message = "Success")
    {
        return new ApiResponse<T>(statusCode, message, data);
    }

    /// <summary>
    /// Tạo response lỗi
    /// </summary>
    public static ApiResponse<T> Error(int statusCode, string message)
    {
        return new ApiResponse<T>(statusCode, message, default);
    }

    /// <summary>
    /// Tạo response Bad Request (400)
    /// </summary>
    public static ApiResponse<T> BadRequest(string message = "Bad Request")
    {
        return new ApiResponse<T>(400, message, default);
    }

    /// <summary>
    /// Tạo response Not Found (404)
    /// </summary>
    public static ApiResponse<T> NotFound(string message = "Not Found")
    {
        return new ApiResponse<T>(404, message, default);
    }

    /// <summary>
    /// Tạo response Unauthorized (401)
    /// </summary>
    public static ApiResponse<T> Unauthorized(string message = "Unauthorized")
    {
        return new ApiResponse<T>(401, message, default);
    }

    /// <summary>
    /// Tạo response Forbidden (403)
    /// </summary>
    public static ApiResponse<T> Forbidden(string message = "Forbidden")
    {
        return new ApiResponse<T>(403, message, default);
    }

    /// <summary>
    /// Tạo response Internal Server Error (500)
    /// </summary>
    public static ApiResponse<T> InternalServerError(string message = "Internal Server Error")
    {
        return new ApiResponse<T>(500, message, default);
    }
}

/// <summary>
/// ApiResponse không có data (chỉ có message và statusCode)
/// </summary>
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required
public class ApiResponse : ApiResponse<object>
{
    public ApiResponse() : base()
    {
    }

    public ApiResponse(int statusCode, string message) : base(statusCode, message, null)
    {
    }

    /// <summary>
    /// Tạo response thành công không có data
    /// </summary>
    public static new ApiResponse Success(string message = "Success")
    {
        return new ApiResponse(200, message);
    }

    /// <summary>
    /// Tạo response lỗi không có data
    /// </summary>
    public static new ApiResponse Error(int statusCode, string message)
    {
        return new ApiResponse(statusCode, message);
    }

    /// <summary>
    /// Tạo response Bad Request (400)
    /// </summary>
    public static new ApiResponse BadRequest(string message = "Bad Request")
    {
        return new ApiResponse(400, message);
    }

    /// <summary>
    /// Tạo response Not Found (404)
    /// </summary>
    public static new ApiResponse NotFound(string message = "Not Found")
    {
        return new ApiResponse(404, message);
    }

    /// <summary>
    /// Tạo response Unauthorized (401)
    /// </summary>
    public static new ApiResponse Unauthorized(string message = "Unauthorized")
    {
        return new ApiResponse(401, message);
    }

    /// <summary>
    /// Tạo response Forbidden (403)
    /// </summary>
    public static new ApiResponse Forbidden(string message = "Forbidden")
    {
        return new ApiResponse(403, message);
    }

    /// <summary>
    /// Tạo response Internal Server Error (500)
    /// </summary>
    public static new ApiResponse InternalServerError(string message = "Internal Server Error")
    {
        return new ApiResponse(500, message);
    }
}
#pragma warning restore CS0109

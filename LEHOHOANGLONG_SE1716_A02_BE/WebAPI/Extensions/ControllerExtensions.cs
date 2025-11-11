using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Extensions;

/// <summary>
/// Extension methods cho ControllerBase để tạo ApiResponse dễ dàng
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Trả về response thành công với data
    /// </summary>
    public static IActionResult SuccessResponse<T>(this ControllerBase controller, T data, string message = "Success")
    {
        var response = ApiResponse<T>.Success(data, message);
        return controller.Ok(response);
    }

    /// <summary>
    /// Trả về response thành công với status code tùy chỉnh
    /// </summary>
    public static IActionResult SuccessResponse<T>(this ControllerBase controller, int statusCode, T data, string message = "Success")
    {
        var response = ApiResponse<T>.Success(statusCode, data, message);
        return controller.StatusCode(statusCode, response);
    }

    /// <summary>
    /// Trả về response Created (201) với data
    /// </summary>
    public static IActionResult CreatedResponse<T>(this ControllerBase controller, T data, string message = "Created successfully")
    {
        var response = ApiResponse<T>.Success(201, data, message);
        return controller.StatusCode(201, response);
    }

    /// <summary>
    /// Trả về response Bad Request (400)
    /// </summary>
    public static IActionResult BadRequestResponse(this ControllerBase controller, string message = "Bad Request")
    {
        var response = ApiResponse.BadRequest(message);
        return controller.BadRequest(response);
    }

    /// <summary>
    /// Trả về response Not Found (404)
    /// </summary>
    public static IActionResult NotFoundResponse(this ControllerBase controller, string message = "Not Found")
    {
        var response = ApiResponse.NotFound(message);
        return controller.NotFound(response);
    }

    /// <summary>
    /// Trả về response Unauthorized (401)
    /// </summary>
    public static IActionResult UnauthorizedResponse(this ControllerBase controller, string message = "Unauthorized")
    {
        var response = ApiResponse.Unauthorized(message);
        return controller.Unauthorized(response);
    }

    /// <summary>
    /// Trả về response Forbidden (403)
    /// </summary>
    public static IActionResult ForbiddenResponse(this ControllerBase controller, string message = "Forbidden")
    {
        var response = ApiResponse.Forbidden(message);
        return controller.StatusCode(403, response);
    }

    /// <summary>
    /// Trả về response No Content (204)
    /// </summary>
    public static IActionResult NoContentResponse(this ControllerBase controller, string message = "No Content")
    {
        var response = new ApiResponse(204, message);
        return controller.StatusCode(204, response);
    }

    /// <summary>
    /// Trả về response Internal Server Error (500)
    /// </summary>
    public static IActionResult InternalServerErrorResponse(this ControllerBase controller, string message = "Internal Server Error")
    {
        var response = ApiResponse.InternalServerError(message);
        return controller.StatusCode(500, response);
    }
}

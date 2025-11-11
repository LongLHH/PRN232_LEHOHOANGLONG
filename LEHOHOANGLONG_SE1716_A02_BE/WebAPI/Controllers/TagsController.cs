using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;
using WebAPI.Extensions;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "1,2")] // Staff and Lecturer
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    // GET: api/Tags
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _tagService.GetAllTagsAsync();
        return this.SuccessResponse(tags, "Retrieved all tags successfully");
    }

    // GET: api/Tags/5
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag == null)
            return this.NotFoundResponse($"Tag with ID {id} not found");

        return this.SuccessResponse(tag, "Tag retrieved successfully");
    }

    // POST: api/Tags
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagDto dto)
    {
        if (!ModelState.IsValid)
            return this.BadRequestResponse("Invalid tag data");

        try
        {
            var tag = await _tagService.CreateTagAsync(dto);
            return this.CreatedResponse(tag, "Tag created successfully");
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequestResponse(ex.Message);
        }
    }

    // PUT: api/Tags/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTagDto dto)
    {
        if (!ModelState.IsValid)
            return this.BadRequestResponse("Invalid tag data");

        try
        {
            var tag = await _tagService.UpdateTagAsync(id, dto);
            return this.SuccessResponse(tag, "Tag updated successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return this.NotFoundResponse(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequestResponse(ex.Message);
        }
    }

    // DELETE: api/Tags/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _tagService.DeleteTagAsync(id);
            return this.SuccessResponse<object>(null, "Tag deleted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

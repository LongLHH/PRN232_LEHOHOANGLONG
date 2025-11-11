using Repository.Entities;
using Repository.Interfaces;
using Service.DTOs;
using Service.Interfaces;

namespace Service.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await _tagRepository.GetAllAsync();
        return tags.Select(MapToDto);
    }

    public async Task<TagDto?> GetTagByIdAsync(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        return tag != null ? MapToDto(tag) : null;
    }

    public async Task<TagDto> CreateTagAsync(CreateTagDto dto)
    {
        // Check for duplicate tag name
        var allTags = await _tagRepository.GetAllAsync();
        if (allTags.Any(t => t.TagName.Equals(dto.TagName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Tag with name '{dto.TagName}' already exists");
        }

        var tag = new Tag
        {
            TagName = dto.TagName,
            Note = dto.Note
        };

        var created = await _tagRepository.AddAsync(tag);
        return MapToDto(created);
    }

    public async Task<TagDto> UpdateTagAsync(int id, UpdateTagDto dto)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag == null)
            throw new KeyNotFoundException($"Tag with ID {id} not found");

        // Check for duplicate tag name (excluding current tag)
        var allTags = await _tagRepository.GetAllAsync();
        if (allTags.Any(t => t.TagId != id && t.TagName.Equals(dto.TagName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Tag with name '{dto.TagName}' already exists");
        }

        tag.TagName = dto.TagName;
        tag.Note = dto.Note;

        await _tagRepository.UpdateAsync(tag);
        return MapToDto(tag);
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        // Check if tag is being used by any news articles
        var hasArticles = await _tagRepository.HasNewsArticlesAsync(id);
        if (hasArticles)
        {
            throw new InvalidOperationException("Cannot delete tag that is being used by news articles");
        }

        await _tagRepository.DeleteAsync(id);
        return true;
    }

    private static TagDto MapToDto(Tag tag)
    {
        return new TagDto
        {
            TagId = tag.TagId,
            TagName = tag.TagName,
            Note = tag.Note
        };
    }
}

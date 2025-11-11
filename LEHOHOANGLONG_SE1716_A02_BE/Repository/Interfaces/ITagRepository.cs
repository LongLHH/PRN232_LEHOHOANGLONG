using Repository.Entities;

namespace Repository.Interfaces;

public interface ITagRepository : IGenericRepository<Tag>
{
    Task<IEnumerable<Tag>> GetTagsByIdsAsync(IEnumerable<int> tagIds);
    Task<bool> HasNewsArticlesAsync(int tagId);
}

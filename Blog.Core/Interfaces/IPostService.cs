namespace Blog.Core.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostResponseDto>> GetPostsAsync(int pageNumber, int pageSize);
        Task<PostResponseDto> CreatePostAsync(PostRequestDto postRequest);
        Task<PostResponseDto> UpdatePostAsync(int id, PostRequestDto postRequest);
        Task<bool> DeletePostAsync(int id);
    }
}

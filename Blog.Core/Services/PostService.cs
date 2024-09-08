using Blog.Core.Interfaces;
using Blog.Domain.Entities;
using Blog.Infra.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PostService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<PostResponseDto>> GetPostsAsync(int pageNumber, int pageSize)
    {
        var posts = await _context.Posts
            .OrderBy(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return posts.Select(p => new PostResponseDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });
    }

    public async Task<PostResponseDto> CreatePostAsync(PostRequestDto postRequest)
    {
        var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = userIdClaim?.Value;

        if (userId == null)
        {
            throw new UnauthorizedAccessException("Usuário não autenticado.");
        }

        var post = new Post
        {
            Title = postRequest.Title,
            Content = postRequest.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = int.Parse(userId)
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }

    public async Task<PostResponseDto> UpdatePostAsync(int id, PostRequestDto postRequest)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post == null)
            throw new KeyNotFoundException("Post not found");

        post.Title = postRequest.Title;
        post.Content = postRequest.Content;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post == null)
            return false;

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return true;
    }
}

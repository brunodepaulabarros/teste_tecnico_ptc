using System.Security.Claims;
using Blog.Infra.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Blog.Tests
{
    public class PostServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly PostService _postService;

        public PostServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _postService = new PostService(_context, _mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task CriarPostAsync_DeveRetornarPostResponseDto_QuandoPostForCriado()
        {
            var userId = "1";
            var postRequest = new PostRequestDto
            {
                Title = "Título de Teste",
                Content = "Conteúdo de Teste"
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User.FindFirst(ClaimTypes.NameIdentifier))
                .Returns(new Claim(ClaimTypes.NameIdentifier, userId));

            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

            // Act
            var result = await _postService.CreatePostAsync(postRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(postRequest.Title, result.Title);
            Assert.Equal(postRequest.Content, result.Content);
            Assert.Equal(DateTime.UtcNow.Date, result.CreatedAt.Date);
            Assert.Equal(DateTime.UtcNow.Date, result.UpdatedAt.Date);

            var createdPost = await _context.Posts.FindAsync(result.Id);
            Assert.NotNull(createdPost);
            Assert.Equal(postRequest.Title, createdPost.Title);
            Assert.Equal(postRequest.Content, createdPost.Content);
            Assert.Equal(int.Parse(userId), createdPost.UserId);
        }
    }
}

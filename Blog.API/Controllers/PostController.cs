using Blog.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.Json;

namespace Blog.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IHubContext<PostHub> _hubContext;

        public PostController(IPostService postService, IHubContext<PostHub> hubContext)
        {
            _postService = postService;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Route("getPostList")]
        [Produces("application/json")]
        public async Task<IActionResult> GetPostList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var posts = await _postService.GetPostsAsync(pageNumber, pageSize);
            return Ok(posts);
        }

        [HttpPost]
        [Route("createPost")]
        [Produces("application/json")]
        public async Task<IActionResult> CreatePost([FromBody] PostRequestDto postRequest)
        {
            var post = await _postService.CreatePostAsync(postRequest);

            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            await _hubContext.Clients.All.SendAsync("ReceivePostUpdate", JsonSerializer.Serialize(post));

            return Ok(post);
        }

        [HttpPut]
        [Route("updatePost/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostRequestDto postRequest)
        {
            try
            {
                var post = await _postService.UpdatePostAsync(id, postRequest);
                return Ok(post);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [Route("deletePost/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var result = await _postService.DeletePostAsync(id);
            if (result)
                return NoContent();
            else
                return NotFound();
        }
    }
}

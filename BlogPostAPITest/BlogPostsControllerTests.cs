using BlogPostAPI.Controllers;
using BlogPostAPI.DTO_s;
using BlogPostAPI.Repositories;
using BlogPostService.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogPostAPITest
{
    public class BlogPostsControllerTests
    {
        private readonly Mock<IBlogPostRepository> _mockRepo;
        private readonly BlogPostsController _controller;

        public BlogPostsControllerTests()
        {
            _mockRepo = new Mock<IBlogPostRepository>();
            _controller = new BlogPostsController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfBlogPosts()
        {
            // Arrange
            var mockPosts = new List<BlogPost> { new BlogPost { Id = 1, Username = "user1", Text = "text1" } };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockPosts);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<BlogPost>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithBlogPost()
        {
            // Arrange
            var mockPost = new BlogPost { Id = 1, Username = "user1", Text = "text1" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(mockPost);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<BlogPost>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenBlogPostDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((BlogPost)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithNewBlogPost()
        {
            // Arrange
            var postDto = new BlogPostDto { Username = "user1", Text = "text1" };
            var mockPost = new BlogPost { Id = 1, Username = "user1", Text = "text1" };
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<BlogPost>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(postDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<BlogPost>(createdAtActionResult.Value);
            Assert.Equal("user1", returnValue.Username);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenBlogPostIsUpdated()
        {
            // Arrange
            var postDto = new BlogPostDto { Username = "user1", Text = "updated text" };
            var mockPost = new BlogPost { Id = 1, Username = "user1", Text = "text1" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(mockPost);
            _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<BlogPost>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(1, postDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenBlogPostDoesNotExist()
        {
            // Arrange
            var postDto = new BlogPostDto { Username = "user1", Text = "updated text" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((BlogPost)null);

            // Act
            var result = await _controller.Update(1, postDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenBlogPostIsDeleted()
        {
            // Arrange
            var mockPost = new BlogPost { Id = 1, Username = "user1", Text = "text1" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(mockPost);
            _mockRepo.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenBlogPostDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((BlogPost)null);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}


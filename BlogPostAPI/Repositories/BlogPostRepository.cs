using BlogPostAPI.Models;
using System.Text.Json;

namespace BlogPostAPI.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly string _filePath;
        public BlogPostRepository(IWebHostEnvironment env)
        {
            // Ensure the file path points to the `Data` folder
            _filePath = Path.Combine(env.ContentRootPath, "Data", "blogposts.json");
            EnsureFileExists();
        }
        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await ReadFromFileAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            var posts = await ReadFromFileAsync();
            return posts.Find(p => p.Id == id);
        }

        public async Task AddAsync(BlogPost post)
        {
            var posts = await ReadFromFileAsync();
            post.Id = posts.Any() ? posts.Max(p => p.Id) + 1 : 1;
            post.DateCreated = DateTime.UtcNow;
            posts.Add(post);
            await WriteToFileAsync(posts);
        }

        public async Task UpdateAsync(BlogPost post)
        {
            var posts = await ReadFromFileAsync();
            var index = posts.FindIndex(p => p.Id == post.Id);
            if (index >= 0)
            {
                posts[index] = post;
                await WriteToFileAsync(posts);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var posts = await ReadFromFileAsync();
            posts.RemoveAll(p => p.Id == id);
            await WriteToFileAsync(posts);
        }

        private async Task<List<BlogPost>> ReadFromFileAsync()
        {
            if (!File.Exists(_filePath))
                return new List<BlogPost>();

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<BlogPost>>(json) ?? new List<BlogPost>();
        }

        private async Task WriteToFileAsync(IEnumerable<BlogPost> posts)
        {
            var json = JsonSerializer.Serialize(posts);
            await File.WriteAllTextAsync(_filePath, json);
        }
        private void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                var initialData = JsonSerializer.Serialize(new List<BlogPost>());
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath) ?? string.Empty);
                File.WriteAllText(_filePath, initialData);
            }
        }
    }
}

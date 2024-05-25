using TheBlogProject.Data;
using TheBlogProject.Enums;
using TheBlogProject.Models;

namespace TheBlogProject.Services;

public class BlogSearchService(ApplicationDbContext context)
{
    public IQueryable<Post> Search(string searchTerm)
    {
        var posts = context.Posts
            .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
            .AsQueryable();
        if (searchTerm != null)
        {
            searchTerm = searchTerm.ToLower();
                
            posts = posts.Where(
                p => p.Title.ToLower().Contains(searchTerm) ||
                     p.Abstract.ToLower().Contains(searchTerm) ||
                     p.Content.ToLower().Contains(searchTerm) ||
                     p.Comments.Any(
                         c => c.Body.ToLower().Contains(searchTerm) ||
                              c.ModeratedBody.ToLower().Contains(searchTerm) ||
                              c.BlogUser.FirstName.ToLower().Contains(searchTerm) ||
                              c.BlogUser.LastName.ToLower().Contains(searchTerm) ||
                              c.BlogUser.Email.ToLower().Contains(searchTerm)));
        }

        return posts.OrderByDescending(p => p.Created);
    }
}
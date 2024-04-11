using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TheBlogProject.Models;

public class Post
{
    public int Id { get; set; }

    [Display(Name = "Blog Name")] 
    public int BlogId { get; set; }
    public string AuthorId { get; set; }

    [Required]
    [StringLength(
        50,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 2
    )]
    public string Title { get; set; }

    [Required]
    [StringLength(
        200,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 2
    )]
    public string Abstract { get; set; }

    [Required]
    public string Content { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Created Date")]
    public DateTime? Created { get; set; }
    
    [DataType(DataType.Date)]
    [Display(Name = "Updated Date")]
    public DateTime? Updated { get; set; }

    public string Slug { get; set; }

    public byte[] ImageData { get; set; }
    public string ImageType { get; set; }

    [NotMapped]
    public IFormFile Image { get; set; }
    
    // Navigation Properties
    public virtual Blog Blog { get; set; }
    public virtual IdentityUser Author { get; set; }
    public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
}
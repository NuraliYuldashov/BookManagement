namespace BookManagement.Domain;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int ViewsCount { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public double CalculatePopularityScore()
    {
        var yearsSincePublished = DateTime.Now.Year - PublicationYear;
        return ViewsCount * 0.5 + yearsSincePublished * 2;
    }
}

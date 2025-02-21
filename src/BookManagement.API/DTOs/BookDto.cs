namespace BookManagement.API.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int ViewsCount { get; set; }
    public double PopularityScore { get; set; }
}

public class CreateBookDto
{
    public string Title { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string AuthorName { get; set; } = string.Empty;
}

public class UpdateBookDto
{
    public string Title { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string AuthorName { get; set; } = string.Empty;
}

public class BookListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ViewsCount { get; set; }
}

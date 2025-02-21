namespace BookManagement.Domain;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllBooksAsync(int page, int pageSize);
    Task<Book?> GetBookByIdAsync(int id);
    Task<bool> BookExistsAsync(string title);
    Task<Book> AddBookAsync(Book book);
    Task<IEnumerable<Book>> AddBooksAsync(IEnumerable<Book> books);
    Task UpdateBookAsync(Book book);
    Task DeleteBookAsync(int id);
    Task DeleteBooksAsync(IEnumerable<int> ids);
    Task IncrementViewCountAsync(int id);
}

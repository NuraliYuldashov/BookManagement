using BookManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Infrastructure;

public class BookRepository : IBookRepository
{
    private readonly BookManagementDbContext _context;

    public BookRepository(BookManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync(int page, int pageSize)
    {
        return await _context.Books
            .Where(b => !b.IsDeleted)
            .OrderByDescending(b => b.ViewsCount)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
    }

    public async Task<bool> BookExistsAsync(string title)
    {
        return await _context.Books
            .AnyAsync(b => b.Title.ToLower() == title.ToLower() && !b.IsDeleted);
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        book.CreatedAt = DateTime.UtcNow;
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<IEnumerable<Book>> AddBooksAsync(IEnumerable<Book> books)
    {
        var now = DateTime.UtcNow;
        foreach (var book in books)
        {
            book.CreatedAt = now;
        }
        await _context.Books.AddRangeAsync(books);
        await _context.SaveChangesAsync();
        return books;
    }

    public async Task UpdateBookAsync(Book book)
    {
        book.UpdatedAt = DateTime.UtcNow;
        _context.Entry(book).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            book.IsDeleted = true;
            book.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteBooksAsync(IEnumerable<int> ids)
    {
        var books = await _context.Books
            .Where(b => ids.Contains(b.Id))
            .ToListAsync();

        var now = DateTime.UtcNow;
        foreach (var book in books)
        {
            book.IsDeleted = true;
            book.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();
    }

    public async Task IncrementViewCountAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            book.ViewsCount++;
            await _context.SaveChangesAsync();
        }
    }
}

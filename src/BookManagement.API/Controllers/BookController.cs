using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookManagement.API.DTOs;
using BookManagement.Domain;

namespace BookManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepository;

    public BookController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookListDto>>> GetBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var books = await _bookRepository.GetAllBooksAsync(page, pageSize);
        return Ok(books.Select(b => new BookListDto
        {
            Id = b.Id,
            Title = b.Title,
            ViewsCount = b.ViewsCount
        }));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        var book = await _bookRepository.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        await _bookRepository.IncrementViewCountAsync(id);

        return Ok(new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            PublicationYear = book.PublicationYear,
            AuthorName = book.AuthorName,
            ViewsCount = book.ViewsCount + 1,
            PopularityScore = book.CalculatePopularityScore()
        });
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
    {
        if (await _bookRepository.BookExistsAsync(createBookDto.Title))
        {
            return BadRequest("A book with this title already exists");
        }

        var book = new Book
        {
            Title = createBookDto.Title,
            PublicationYear = createBookDto.PublicationYear,
            AuthorName = createBookDto.AuthorName
        };

        await _bookRepository.AddBookAsync(book);

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            PublicationYear = book.PublicationYear,
            AuthorName = book.AuthorName,
            ViewsCount = book.ViewsCount,
            PopularityScore = book.CalculatePopularityScore()
        });
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<BookDto>>> CreateBooks(IEnumerable<CreateBookDto> createBookDtos)
    {
        foreach (var dto in createBookDtos)
        {
            if (await _bookRepository.BookExistsAsync(dto.Title))
            {
                return BadRequest($"A book with title '{dto.Title}' already exists");
            }
        }

        var books = createBookDtos.Select(dto => new Book
        {
            Title = dto.Title,
            PublicationYear = dto.PublicationYear,
            AuthorName = dto.AuthorName
        });

        var createdBooks = await _bookRepository.AddBooksAsync(books);

        return Ok(createdBooks.Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            PublicationYear = b.PublicationYear,
            AuthorName = b.AuthorName,
            ViewsCount = b.ViewsCount,
            PopularityScore = b.CalculatePopularityScore()
        }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updateBookDto)
    {
        var book = await _bookRepository.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        if (book.Title != updateBookDto.Title && await _bookRepository.BookExistsAsync(updateBookDto.Title))
        {
            return BadRequest("A book with this title already exists");
        }

        book.Title = updateBookDto.Title;
        book.PublicationYear = updateBookDto.PublicationYear;
        book.AuthorName = updateBookDto.AuthorName;

        await _bookRepository.UpdateBookAsync(book);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _bookRepository.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        await _bookRepository.DeleteBookAsync(id);
        return NoContent();
    }

    [HttpDelete("bulk")]
    public async Task<IActionResult> DeleteBooks([FromBody] int[] ids)
    {
        await _bookRepository.DeleteBooksAsync(ids);
        return NoContent();
    }
}

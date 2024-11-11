using GigKompassen.Data;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Test.Helpers.DbContextHelper;
using static GigKompassen.Test.Helpers.BogusRepositories;

namespace GigKompassen.Test.Tests.Services
{
  public class GenreServiceTests
  {

    private readonly GenreService _genreService;
    private readonly ApplicationDbContext _context;

    private readonly FakeDataHelper f;

    public GenreServiceTests()
    {
      _context = GetInMemoryDbContext();
      _genreService = new GenreService(_context);

      f = new FakeDataHelper(_context);
    }

    [Fact]
    public async Task CanCheckIfGenreExists()
    {
      var genre = f.AddGenre();

      var resName = await _genreService.HasGenreAsync(genre.Name);
      var resId = await _genreService.HasGenreAsync(genre.Id);

      var resNameFalse = await _genreService.HasGenreAsync("NotAGenre");
      var resIdFalse = await _genreService.HasGenreAsync(Guid.NewGuid());

      Assert.True(resName);
      Assert.True(resId);
      Assert.False(resNameFalse);
      Assert.False(resIdFalse);
    }

    [Fact]
    public async Task CanGetGenres()
    {
      var genre1 = f.AddGenre();
      var genre2 = f.AddGenre();
      var genre3 = f.AddGenre();
      var genre4 = f.AddGenre();

      var genresById = await _genreService.GetGenresAsync(new List<Guid> { genre1.Id, genre2.Id, genre3.Id, genre4.Id });
      var genreById = await _genreService.GetGenreAsync(genre1.Id);

      var genresByName = await _genreService.GetGenresAsync(new List<string> { genre1.Name, genre2.Name, genre3.Name, genre4.Name });
      var genreByName = await _genreService.GetGenreAsync(genre1.Name);

      var notFoundById = await _genreService.GetGenreAsync(Guid.NewGuid());
      var notFoundByName = await _genreService.GetGenreAsync("NotAGenre");

      Assert.Equal(4, genresById.Count);
      Assert.Equal(4, genresByName.Count);

      Assert.NotNull(genreById);
      Assert.NotNull(genreByName);

      Assert.Null(notFoundById);
      Assert.Null(notFoundByName);
    }

    [Fact]
    public async Task CanCreateGenre()
    {
      var genre = await _genreService.CreateGenreAsync("TestGenre");
      var genres = await _genreService.CreateGenresAsync(new List<string> { "TestGenre1", "TestGenre2", "TestGenre3" });

      var genreFromDb = await _context.Genres.FirstOrDefaultAsync(g => g.Id == genre.Id);
      var genresFromDb = await _context.Genres.Where(g => genres.Select(g => g.Id).Contains(g.Id)).ToListAsync();

      Assert.NotNull(genre);
      Assert.NotNull(genreFromDb);

      Assert.Equal(genre.Id, genreFromDb.Id);
      Assert.Equal(genre.Name, genreFromDb.Name);

      Assert.Equal(3, genres.Count);
      Assert.Equal(3, genresFromDb.Count);
    }

    [Fact]
    public async Task CanGetOrCreateGenres()
    {
      var genre1 = f.AddGenre();
      var genre2 = f.AddGenre();
      var genre3 = f.AddGenre();

      var createdGenre1 = GenreFaker.Generate();
      var createdGenre2 = GenreFaker.Generate();

      var genres = await _genreService.GetOrCreateGenresAsync(new List<string> { genre1.Name, genre2.Name, createdGenre1.Name });
      var firstGenre = await _genreService.GetOrCreateGenreAsync(genre3.Name);
      var secodGenre = await _genreService.GetOrCreateGenreAsync(createdGenre2.Name);

      var genresFromDb = await _context.Genres.ToListAsync();

      Assert.Equal(5, genresFromDb.Count());
      Assert.Equal(3, genres.Count);
      Assert.Equal(genre3.Id, firstGenre.Id);
      Assert.Equal(genre3.Name, firstGenre.Name);
      Assert.Equal(createdGenre2.Name, secodGenre.Name);

    }

    [Fact]
    public async Task CanRemoveGenres()
    {
      var genre1 = f.AddGenre();
      var genre2 = f.AddGenre();
      var genre3 = f.AddGenre();
      var genre4 = f.AddGenre();
      var genre5 = f.AddGenre();
      var genre6 = f.AddGenre();
      var genre7 = f.AddGenre();
      var genre8 = f.AddGenre();
      var genre9 = f.AddGenre();
      var genre10 = f.AddGenre();

      var remove1and2 = await _genreService.RemoveGenresAsync(new List<Guid> { genre1.Id, genre2.Id });
      var remove3 = await _genreService.RemoveGenreAsync(genre3.Id);
      var remove4and5 = await _genreService.RemoveGenresAsync(new List<string> { genre4.Name, genre5.Name });
      var remove6 = await _genreService.RemoveGenreAsync(genre6.Name);
      var remove7and8 = await _genreService.RemoveGenresAsync(new List<Genre> { genre7, genre8 });
      var remove9 = await _genreService.RemoveGenreAsync(genre9);

      var genresFromDb = await _context.Genres.ToListAsync();

      Assert.Equal(2, remove1and2);
      Assert.Equal(2, remove4and5);
      Assert.Equal(2, remove7and8);
      Assert.True(remove3);
      Assert.True(remove6);
      Assert.True(remove9);
      Assert.Single(genresFromDb);
      Assert.Contains(genresFromDb, g => g.Id == genre10!.Id);
    }
  }
}

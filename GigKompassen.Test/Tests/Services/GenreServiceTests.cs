using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Test.Helpers.DbContextHelper;

namespace GigKompassen.Test.Tests.Services
{
  public class GenreServiceTests
  {
    [Fact]
    public async Task CanCheckIfGenreExists()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var genre4 = repo.NewGenre();

      await context.Genres.AddRangeAsync(genre1, genre2); //Not Genre3 or Genre4

      await context.SaveChangesAsync();

      // Act
      var result1 = await genreService.HasGenreAsync(genre1.Id);
      var result2 = await genreService.HasGenreAsync(genre2.Name);
      var result3 = await genreService.HasGenreAsync(genre3.Id);
      var result4 = await genreService.HasGenreAsync(genre4.Name);
      var result5 = await genreService.HasGenreAsync("Fake Genre");
      var result6 = await genreService.HasGenreAsync(Guid.NewGuid());

      // Assert
      Assert.True(result1);
      Assert.True(result2);
      Assert.False(result3);
      Assert.False(result4);
      Assert.False(result5);
      Assert.False(result6);
    }

    [Fact]
    public async Task CanGetGenres()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var genre4 = repo.NewGenre();

      await context.Genres.AddRangeAsync(genre1, genre2, genre3);
      await context.SaveChangesAsync();
      // Act
      var result1 = await genreService.GetGenreAsync(genre1.Id);
      var result2 = await genreService.GetGenreAsync(genre2.Name);
      var result3 = await genreService.GetGenresAsync(new List<string>() { genre2.Name, genre3.Name, genre4.Name, "Fake Genre"});
      var result4 = await genreService.GetGenreAsync("Fake Genre");
      var result5 = await genreService.GetGenreAsync(Guid.NewGuid());
      // Assert
      Assert.Equal(genre1, result1);
      Assert.Equal(genre2, result2);
      Assert.Equal(new List<Genre>() { genre2, genre3 }, result3);;
      Assert.Null(result4);
      Assert.Null(result5);

      await context.SaveChangesAsync();
    }

    [Fact]
    public async Task CanAddOrGetGenres()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var genre4 = repo.NewGenre();

      await context.Genres.AddRangeAsync(genre1, genre2);
      await context.SaveChangesAsync();
      // Act
      var result1 = await genreService.AddOrGetGenreAsync(genre2.Name);
      var result2 = await genreService.AddOrGetGenreAsync(genre3.Name);
      var result3 = await genreService.AddOrGetGenresAsync(new List<string>() { genre3.Name, genre4.Name});

      var genres = await context.Genres.ToListAsync();
      // Assert
      Assert.Equal(4, genres.Count);
      Assert.Equal(genre2, result1);
      Assert.Equal(genre3.Name, result2.Name);
      Assert.Equal(new List<Genre>() { genre3, genre4 }.Select(p => p.Name), result3.Select(p => p.Name));
    }

    [Fact]
    public async Task CanRemoveGenres()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var genre4 = repo.NewGenre();
      var genre5 = repo.NewGenre();
      var genre6 = repo.NewGenre();
      var genre7 = repo.NewGenre();
      var genre8 = repo.NewGenre();

      await context.Genres.AddRangeAsync(genre1, genre2, genre3, genre4, genre5, genre6, genre7);
      await context.SaveChangesAsync();
      // Act
      await genreService.RemoveGenreAsync(genre2.Id);
      await genreService.RemoveGenreAsync(genre3.Name);
      await genreService.RemoveGenresAsync(new List<Guid>() { genre4.Id, genre5.Id });
      await genreService.RemoveGenresAsync(new List<string>() { genre6.Name, genre8.Name });

      var genres = await context.Genres.ToListAsync();

      // Assert
      Assert.Equal(2, genres.Count);
      Assert.DoesNotContain(genre2, genres);
      Assert.DoesNotContain(genre3, genres);
      Assert.DoesNotContain(genre4, genres);
      Assert.DoesNotContain(genre5, genres);
      Assert.DoesNotContain(genre6, genres);
      Assert.Contains(genre1, genres);
      Assert.Contains(genre7, genres);

    }
  }
}

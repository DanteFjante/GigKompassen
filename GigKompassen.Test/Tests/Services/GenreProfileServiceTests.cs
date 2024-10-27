using System;
using System.Threading.Tasks;
using Xunit;
using GigKompassen.Services;
using GigKompassen.Data;
using GigKompassen.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using static GigKompassen.Test.Helpers.DbContextHelper;
using GigKompassen.Models.Profiles;
using GigKompassen.Test.Helpers;
using static System.Formats.Asn1.AsnWriter;

namespace GigKompassen.Test.Tests.Services
{
  public class GenreProfileServiceTests
  {
    [Fact]
    public async Task CanCheckArtistHasGenre()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var genreProfileService = new GenreProfileService(context, genreService);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var artist = repo.GetArtistProfile(genres: new() { genre1, genre2});

      await context.Genres.AddRangeAsync(genre1, genre2, genre3);
      await context.ArtistProfiles.AddAsync(artist);
      await context.SaveChangesAsync();

      // Act
      var result1 = await genreProfileService.ArtistHasGenreAsync(artist.Id, genre1.Name);
      var result2 = await genreProfileService.ArtistHasGenreAsync(artist.Id, genre1.Name);
      var result3 = await genreProfileService.ArtistHasGenreAsync(artist.Id, genre3.Name);
      var result4 = await genreProfileService.ArtistHasGenreAsync(artist.Id, genre1.Id);
      var result5 = await genreProfileService.ArtistHasGenreAsync(artist.Id, genre2.Id);
      var result6 = await genreProfileService.ArtistHasGenreAsync(artist.Id, genre3.Id);

      // Assert
      Assert.True(result1);
      Assert.True(result2);
      Assert.False(result3);
      Assert.True(result4);
      Assert.True(result5);
      Assert.False(result6);
    }

    [Fact]
    public async Task CanCheckSceneHasGenre()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var genreProfileService = new GenreProfileService(context, genreService);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var scene = repo.GetSceneProfile(genres: new() { genre1, genre2 });

      await context.Genres.AddRangeAsync(genre1, genre2, genre3);
      await context.SceneProfiles.AddAsync(scene);
      await context.SaveChangesAsync();

      // Act
      var result1 = await genreProfileService.SceneHasGenreAsync(scene.Id, genre1.Name);
      var result2 = await genreProfileService.SceneHasGenreAsync(scene.Id, genre1.Name);
      var result3 = await genreProfileService.SceneHasGenreAsync(scene.Id, genre3.Name);
      var result4 = await genreProfileService.SceneHasGenreAsync(scene.Id, genre1.Id);
      var result5 = await genreProfileService.SceneHasGenreAsync(scene.Id, genre2.Id);
      var result6 = await genreProfileService.SceneHasGenreAsync(scene.Id, genre3.Id);

      // Assert
      Assert.True(result1);
      Assert.True(result2);
      Assert.False(result3);
      Assert.True(result4);
      Assert.True(result5);
      Assert.False(result6);
    }

    [Fact]
    public async Task CanCheckGenreHasArtist()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var genreProfileService = new GenreProfileService(context, genreService);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var artist = repo.GetArtistProfile(genres: new() { genre1, genre2 });

      await context.Genres.AddRangeAsync(genre1, genre2, genre3);
      await context.ArtistProfiles.AddAsync(artist);
      await context.SaveChangesAsync();

      // Act
      var result1 = await genreProfileService.GenreHasArtistAsync(genre1.Name, artist.Id);
      var result2 = await genreProfileService.GenreHasArtistAsync(genre1.Name, artist.Id);
      var result3 = await genreProfileService.GenreHasArtistAsync(genre3.Name, artist.Id);
      var result4 = await genreProfileService.GenreHasArtistAsync(genre1.Id, artist.Id);
      var result5 = await genreProfileService.GenreHasArtistAsync(genre2.Id, artist.Id);
      var result6 = await genreProfileService.GenreHasArtistAsync(genre3.Id, artist.Id);

      // Assert
      Assert.True(result1);
      Assert.True(result2);
      Assert.False(result3);
      Assert.True(result4);
      Assert.True(result5);
      Assert.False(result6);
    }

    [Fact]
    public async Task CanCheckGenreHasScene()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var genreProfileService = new GenreProfileService(context, genreService);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var scene = repo.GetSceneProfile(genres: new() { genre1, genre2 });

      await context.Genres.AddRangeAsync(genre1, genre2, genre3);
      await context.SceneProfiles.AddAsync(scene);
      await context.SaveChangesAsync();

      // Act
      var result1 = await genreProfileService.GenreHasSceneAsync(genre1.Name, scene.Id);
      var result2 = await genreProfileService.GenreHasSceneAsync(genre1.Name, scene.Id);
      var result3 = await genreProfileService.GenreHasSceneAsync(genre3.Name, scene.Id);
      var result4 = await genreProfileService.GenreHasSceneAsync(genre1.Id, scene.Id);
      var result5 = await genreProfileService.GenreHasSceneAsync(genre2.Id, scene.Id);
      var result6 = await genreProfileService.GenreHasSceneAsync(genre3.Id, scene.Id);

      // Assert
      Assert.True(result1);
      Assert.True(result2);
      Assert.False(result3);
      Assert.True(result4);
      Assert.True(result5);
      Assert.False(result6);
    }

    [Fact]
    public async Task CanAddGenreToArtist()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var genreProfileService = new GenreProfileService(context, genreService);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var artist = repo.GetArtistProfile(genres: new() { genre1, genre2 });

      await context.Genres.AddRangeAsync(genre1, genre2, genre3);
      await context.ArtistProfiles.AddAsync(artist);
      await context.SaveChangesAsync();

      // Act
      var result = await genreProfileService.AddGenreToArtist(artist.Id, genre3.Name);
      var retrievedArtist = await context.ArtistProfiles.Include(s => s.Genres).FirstOrDefaultAsync(s => s.Id == artist.Id);

      // Assert
      Assert.True(result);
      Assert.Contains(genre3, retrievedArtist.Genres);
      Assert.Equal(3, retrievedArtist.Genres.Count);
    }

    [Fact]
    public async Task CanAddGenreToScene()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var genreProfileService = new GenreProfileService(context, genreService);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var scene = repo.GetSceneProfile(genres: new() { genre1, genre2 });

      await context.Genres.AddRangeAsync(genre1, genre2, genre3);
      await context.SceneProfiles.AddAsync(scene);
      await context.SaveChangesAsync();

      // Act
      var result = await genreProfileService.AddGenreToScene(scene.Id, genre3.Name);
      var retrievedScene = await context.SceneProfiles.Include(s => s.Genres).FirstOrDefaultAsync(s => s.Id == scene.Id);

      // Assert
      Assert.True(result);
      Assert.Contains(genre3, retrievedScene.Genres);
      Assert.Equal(3, retrievedScene.Genres.Count);
    }

    [Fact]
    public async Task CanRemoveGenreFromArtist()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var genreProfileService = new GenreProfileService(context, genreService);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var artist = repo.GetArtistProfile(genres: new() { genre1, genre2 });

      await context.Genres.AddRangeAsync(genre1, genre2, genre3);
      await context.ArtistProfiles.AddAsync(artist);
      await context.SaveChangesAsync();

      // Act
      var result = await genreProfileService.RemoveGenreFromArtist(artist.Id, genre2.Name);
      var retrievedArtist = await context.ArtistProfiles.Include(s => s.Genres).FirstOrDefaultAsync(s => s.Id == artist.Id);

      // Assert
      Assert.True(result);
      Assert.Contains(genre1, retrievedArtist.Genres);
      Assert.DoesNotContain(genre3, retrievedArtist.Genres);
      Assert.DoesNotContain(genre2, retrievedArtist.Genres);
      Assert.Equal(1, retrievedArtist.Genres.Count);
    }

    [Fact]
    public async Task CanRemoveGenreFromScene()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var genreService = new GenreService(context);
      var genreProfileService = new GenreProfileService(context, genreService);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      var scene = repo.GetSceneProfile(genres: new() { genre1, genre2 });

      await context.Genres.AddRangeAsync(genre1, genre2, genre3);
      await context.SceneProfiles.AddAsync(scene);
      await context.SaveChangesAsync();

      // Act
      var result = await genreProfileService.RemoveGenreFromScene(scene.Id, genre2.Name);
      var retrievedScene = await context.SceneProfiles.Include(s => s.Genres).FirstOrDefaultAsync(s => s.Id == scene.Id);

      // Assert
      Assert.True(result);
      Assert.Contains(genre1, retrievedScene.Genres);
      Assert.DoesNotContain(genre3, retrievedScene.Genres);
      Assert.DoesNotContain(genre2, retrievedScene.Genres);
      Assert.Equal(1, retrievedScene.Genres.Count);
    }

  }

}
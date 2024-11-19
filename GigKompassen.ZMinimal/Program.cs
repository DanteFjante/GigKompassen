using GigKompassen.Data;
using GigKompassen.Models.Accounts;
using GigKompassen.Misc;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GigKompassen.ZMinimal.Helpers;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<FakeDataHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}


app.MapGet("/addUser", (FakeDataHelper fakeDataHelper) =>
{
  return fakeDataHelper.AddFakeUser();
})
.WithName("Add User")
.WithOpenApi();

app.MapGet("/removeUser", (ApplicationDbContext context, Guid userId) =>
{
  var user = context.Users.FirstOrDefault(u => u.Id == userId);
  context.Users.Remove(user);
  context.SaveChanges();
  return $"Removed user: {user.UserName}";
})
  .WithName("Remove User")
  .WithOpenApi();


app.MapGet("/addArtist", (FakeDataHelper fakeDataHelper, Guid userId) =>
{
  return fakeDataHelper.AddFakeArtistProfile(userId);
})
.WithName("Add Artist")
.WithOpenApi();

app.MapGet("/removeArtist", (ApplicationDbContext context, Guid artistId) =>
{
  var artist = context.ArtistProfiles.FirstOrDefault(a => a.Id == artistId);
  context.ArtistProfiles.Remove(artist);
  context.SaveChanges();
  return $"Removed artist: {artist.Name}";
})
  .WithName("Remove Artist")
  .WithOpenApi();

app.MapGet("/addMember", (FakeDataHelper fakeDataHelper, Guid artistId) =>
{
  return fakeDataHelper.AddFakeArtistMember(artistId);
})
  .WithName("Add Member")
  .WithOpenApi();

app.MapGet("/removeMember", (ApplicationDbContext context, Guid memberId) =>
{
  var member = context.ArtistMembers.FirstOrDefault(a => a.Id == memberId);
  context.ArtistMembers.Remove(member);
  context.SaveChanges();
  return $"Removed member: {member.Name}";
})
  .WithName("Remove Member")
  .WithOpenApi();

app.MapGet("/addGenreToArtist", (FakeDataHelper fakeDataHelper, Guid artistId) =>
{
  return fakeDataHelper.AddFakeGenreToArtist(artistId);
})
  .WithName("Add Genre To Artist")
  .WithOpenApi();

app.MapGet("/removeGenreFromArtist", (ApplicationDbContext context, Guid genreId) =>
{
  var genre = context.Genres.FirstOrDefault(a => a.Id == genreId);
  context.Genres.Remove(genre);
  context.SaveChanges();
  return $"Removed genre: {genre.Name}";
})
  .WithName("Remove Genre From Artist")
  .WithOpenApi();

app.MapGet("/addGenre", (FakeDataHelper fakeDataHelper) =>
{
  return fakeDataHelper.AddFakeGenre();
})
  .WithName("Add Genre")
  .WithOpenApi();

app.MapGet("/addScene", (FakeDataHelper fakeDataHelper, Guid userId) =>
{
  return fakeDataHelper.AddFakeSceneProfile(userId);
})
  .WithName("Add Scene")
  .WithOpenApi();

app.MapGet("/removeScene", (ApplicationDbContext context, Guid sceneId) =>
{
  var scene = context.SceneProfiles.FirstOrDefault(a => a.Id == sceneId);
  context.SceneProfiles.Remove(scene);
  context.SaveChanges();
  return $"Removed scene: {scene.Name}";
})
 .WithName("Remove Scene")
 .WithOpenApi();

app.MapGet("/addManager", (FakeDataHelper fakeDataHelper, Guid userId) =>
{
  return fakeDataHelper.AddFakeManagerProfile(userId);
})
  .WithName("Add Manager")
  .WithOpenApi();

app.MapGet("/removeScene", (ApplicationDbContext context, Guid managerId) =>
{
  var manager = context.ManagerProfiles.FirstOrDefault(a => a.Id == managerId);
  context.ManagerProfiles.Remove(manager);
  context.SaveChanges();
  return $"Removed manager: {manager.Name}";
})
 .WithName("Remove manager")
 .WithOpenApi();

app.Run();
using AutoMapper.Execution;

using GigKompassen.Enums;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;

using Microsoft.AspNetCore.Components;

namespace GigKompassen.Blazor.Components.Profile
{
  public partial class CreateArtistProfileComponent : ComponentBase
  {
    [Inject]
    protected UserService UserService { get; set; }

    [Inject]
    protected ArtistService ArtistService { get; set; }

    [Parameter]
    public Guid UserId { get; set; }

    [Parameter]
    public EventCallback<ArtistProfile> OnProfileCreated { get; set; }


    public ArtistProfileViewModel ArtistProfile { get; set; } = new ArtistProfileViewModel();
    public List<ArtistMemberViewModel> Members { get; set; } = new List<ArtistMemberViewModel>();
    public List<string> Genres { get; set; } = new List<string>();


    protected override async Task OnInitializedAsync()
    {
      // Initialization logic if needed
    }

    protected void AddGenre()
    {
      Genres.Add(string.Empty);
    }

    protected void RemoveGenre(int index)
    {
      if (index >= 0 && index < Genres.Count)
      {
        Genres.RemoveAt(index);
      }
    }

    protected void AddMember()
    {
      Members.Add(new ArtistMemberViewModel());
    }

    protected void RemoveMember(int index)
    {
      if (index >= 0 && index < Members.Count)
      {
        Members.RemoveAt(index);
      }
    }


    protected async Task HandleSubmit()
    {
      var user = await UserService.GetUserByIdAsync(UserId);
      if (user == null)
      {
        // Handle user not found, e.g., display an error message
        return;
      }

      if (!ValidateViewModel())
      {
        // Handle validation failure, e.g., display an error message
        return;
      }

      // Call the service to create the artist profile
      var createdProfile = await ArtistService.CreateAsync(user.Id, FromViewModel(ArtistProfile), Genres, FromViewModel(Members));

      // Invoke the callback to notify the parent component or page
      await OnProfileCreated.InvokeAsync(createdProfile);

    }

    private bool ValidateViewModel()
    {
      if (ArtistProfile == null) return false;
      if (Members == null) return false;
      if (string.IsNullOrWhiteSpace(ArtistProfile.Name)) return false;
      if (Members.Any(m => string.IsNullOrWhiteSpace(m.Name))) return false;
      return true;
    }

    private CreateArtistDto FromViewModel(ArtistProfileViewModel viewModel)
    {
      return new CreateArtistDto(viewModel.Name, viewModel.Location, viewModel.Bio, viewModel.Description, viewModel.Availability, true);
    }

    private List<ArtistMemberDto> FromViewModel(List<ArtistMemberViewModel> viewModel)
    {
      return viewModel.Select(m => new ArtistMemberDto(null, m.Name, m.Role)).ToList();
    }
  }

  public class ArtistProfileViewModel
  {
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.Open;
  }

  public class ArtistMemberViewModel
  {
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
  }
}

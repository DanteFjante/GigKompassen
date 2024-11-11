using GigKompassen.Enums;
using GigKompassen.Models.Profiles;

using Microsoft.AspNetCore.Components;

namespace GigKompassen.Web.Components
{
  public class CreateProfileComponentBase : ComponentBase
  {
    [Parameter]
    public ProfileTypes SelectedProfileType { get; set; }

    [Parameter]
    public EventCallback<BaseProfile> OnProfileCreated { get; set; }

    protected async Task HandleProfileCreated(BaseProfile profile)
    {
      await OnProfileCreated.InvokeAsync(profile);
    }
  }
}

using GigKompassen.Models.Accounts;

namespace GigKompassen.Web.Models
{
  public class HomeViewModel
  {

    public string StatusMessage { get; set; }

    public bool IsLoggedIn { get; set; }
    public ApplicationUser? User { get; set; }

  }
}

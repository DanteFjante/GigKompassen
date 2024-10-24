using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Enums
{
  public enum AvailabilityStatus
  {
    [Display(Name = "Open For Gigs")]
    Open,
    [Display(Name = "Not Accepting Invites")]
    Closed
  }
}

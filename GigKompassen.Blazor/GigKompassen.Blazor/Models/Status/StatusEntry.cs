namespace GigKompassen.Blazor.Models.Status
{
  public class StatusEntry
  {
    public StatusType Type { get; set; }
    public required string Subject { get; set; }
    public required string Message { get; set; }
    public string? Link { get; set; }
    public int Instances { get; set; } = 1;

    public string GetIcon()
    {
      return Type switch
      {
        StatusType.Success => "check",
        StatusType.Info => "info",
        StatusType.Warning => "exclamation",
        StatusType.Danger => "x-circle",
        StatusType.Primary => "star",
        StatusType.Dark => "moon",
        StatusType.Light => "sun",
        StatusType.Secondary => "heart",
        StatusType.Question => "question",
        _ => "none"
      };
    }

    public string GetColor()
    {
      return Type switch
      {
        StatusType.Success => "success",
        StatusType.Info => "info",
        StatusType.Warning => "warning",
        StatusType.Danger => "danger",
        StatusType.Primary => "primary",
        StatusType.Dark => "dark",
        StatusType.Light => "light",
        StatusType.Secondary => "secondary",
        StatusType.Question => "info", // Setting question to a neutral color
        _ => "black"
      };
    }

    public override string ToString()
    {
      return $"[{Type}]{Subject}: {Message}";
    }
  }
}

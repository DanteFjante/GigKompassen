namespace GigKompassen.Blazor.Models.Status
{
  public class StatusCollection
  {
    public List<StatusEntry> StatusEntries { get; } = new List<StatusEntry>();

    public event Action<StatusEntry>? OnStatusAdded;
    public event Action<StatusEntry>? OnStatusRemoved;
    public event Action<string>? OnSubjectCleared;
    public event Action? OnChange;

    public List<string> GetSubjects()
    {
      return StatusEntries.Where(x => x.Subject != null).Select(x => x.Subject!).Distinct().ToList();
    }

    public bool HasSubject<T>()
    {
      return HasSubject(GetName<T>());
    }

    public bool HasType<T>(StatusType type)
    {
      return HasType(type, GetName<T>());
    }

    public List<StatusEntry> GetEntries<T>()
    {
      return GetEntries(GetName<T>());
    }

    public void AddEntry<T>(StatusType type, string message, string? link = null)
    {
      AddEntry(type, message, GetName<T>(), link);
    }

    public void ClearEntries<T>()
    {
      ClearEntries(GetName<T>());
    }

    public void RemoveEntry(StatusEntry entry)
    {
      StatusEntries.Remove(entry);
      OnStatusRemoved?.Invoke(entry);
      OnChange?.Invoke();
    }

    public void AddEntry(StatusType type, string message, string subject, string? link = null)
    {
      if (StatusEntries.Any(x => x.Type == type && x.Message == message && x.Subject == subject && x.Link == link))
      {
        var entry = StatusEntries.First(x => x.Type == type && x.Message == message && x.Subject == subject && x.Link == link);
        entry.Instances++;
        OnChange?.Invoke();
        return;
      }

      var statusEntry = new StatusEntry
      {
        Type = type,
        Message = message,
        Subject = subject,
        Link = link
      };
      StatusEntries.Add(statusEntry);
      OnStatusAdded?.Invoke(statusEntry);
      OnChange?.Invoke();
    }

    public void ClearEntries(string? subject = null)
    {
      if (subject == null)
      {
        var subjects = GetSubjects();
        StatusEntries.Clear();
        foreach (var s in subjects)
          OnSubjectCleared?.Invoke(s);
      }
      else
      {
        StatusEntries.RemoveAll(x => x.Subject == subject);
        OnSubjectCleared?.Invoke(subject);
      }
      OnChange?.Invoke();
    }

    public bool HasSubject(string subject)
    {
      return StatusEntries.Any(x => x.Subject == subject);
    }

    public bool HasType(StatusType type, string? subject = null)
    {
      if (subject == null)
        return StatusEntries.Any(x => x.Type == type);
      else
        return StatusEntries.Any(x => x.Type == type && x.Subject == subject);
    }

    public List<StatusEntry> GetEntries(string? subject = null)
    {
      if (subject == null)
        return StatusEntries.ToList();
      else
        return StatusEntries.Where(x => x.Subject == subject).ToList();
    }

    private string GetName<T>()
    {
      return typeof(T).Name;
    }
  }
}


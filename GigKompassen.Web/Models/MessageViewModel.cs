namespace GigKompassen.Web.Models
{
  public class MessageViewModel
  {

    public MessageViewModel(string message)
    {
      Message = message;
    }

    public MessageViewModel()
    {
    }

    public string Message { get; set; }
  }
}

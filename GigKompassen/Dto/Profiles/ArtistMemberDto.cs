namespace GigKompassen.Dto.Profiles
{
  public class ArtistMemberDto
  {

    public ArtistMemberDto() { }

    public ArtistMemberDto(Guid id, string name, string role)
    {
      Id = id;
      Name = name;
      Role = role;
    }

    public ArtistMemberDto(string name, string role)
    {
      Name = name;
      Role = role;
    }

    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
  }
}

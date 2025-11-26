namespace HelpDesk.Application.DTOs
{
    public record UserDto(int Id, string Email, string FullName, List<string> Roles);
}

using MediatR;

namespace HelpDesk.Application.Commands
{
    public class CreateRoleCommand : IRequest<bool>
    {
        public string RoleName { get; set; }

        public CreateRoleCommand(string roleName)
        {
            RoleName = roleName;
        }
    }
}

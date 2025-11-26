using MediatR;

namespace HelpDesk.Application.Commands
{
    public class DeleteRoleCommand : IRequest<bool>
    {
        public string RoleName { get; set; }

        public DeleteRoleCommand(string roleName)
        {
            RoleName = roleName;
        }
    }
}

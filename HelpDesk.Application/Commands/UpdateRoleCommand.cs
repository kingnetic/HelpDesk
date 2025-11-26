using MediatR;

namespace HelpDesk.Application.Commands
{
    public class UpdateRoleCommand : IRequest<bool>
    {
        public string RoleName { get; set; }
        public string NewRoleName { get; set; }

        public UpdateRoleCommand(string roleName, string newRoleName)
        {
            RoleName = roleName;
            NewRoleName = newRoleName;
        }
    }
}

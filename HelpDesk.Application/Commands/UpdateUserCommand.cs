using MediatR;
using System.Collections.Generic;

namespace HelpDesk.Application.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }

        public UpdateUserCommand(int id, string email, string fullName, List<string> roles)
        {
            Id = id;
            Email = email;
            FullName = fullName;
            Roles = roles;
        }
    }
}

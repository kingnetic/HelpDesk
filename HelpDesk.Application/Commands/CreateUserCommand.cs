using MediatR;
using System.Collections.Generic;

namespace HelpDesk.Application.Commands
{
    public class CreateUserCommand : IRequest<int>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }

        public CreateUserCommand(string email, string password, string fullName, List<string> roles)
        {
            Email = email;
            Password = password;
            FullName = fullName;
            Roles = roles;
        }
    }
}

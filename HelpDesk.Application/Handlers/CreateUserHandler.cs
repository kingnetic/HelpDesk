using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Commands;

namespace HelpDesk.Application.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, int>
    {
        private readonly IUserService _userService;

        public CreateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.CreateUserAsync(request.Email, request.Email, request.Password, request.FullName, request.Roles);
        }
    }
}

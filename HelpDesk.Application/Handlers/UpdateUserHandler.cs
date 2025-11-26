using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Commands;

namespace HelpDesk.Application.Handlers
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserService _userService;

        public UpdateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateUserAsync(request.Id, request.Email, request.FullName, request.Roles);
        }
    }
}

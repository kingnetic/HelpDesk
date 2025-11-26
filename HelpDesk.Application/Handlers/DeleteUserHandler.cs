using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Commands;

namespace HelpDesk.Application.Handlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserService _userService;

        public DeleteUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.DeleteUserAsync(request.Id);
        }
    }
}

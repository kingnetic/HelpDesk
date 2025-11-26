using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Queries;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.Application.Handlers
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        private readonly IUserService _userService;

        public GetUsersHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetAllUsersAsync(cancellationToken);
        }
    }
}

using HelpDesk.Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HelpDesk.Application.Queries
{
    public class GetUsersQuery : IRequest<List<UserDto>>
    {
    }
}

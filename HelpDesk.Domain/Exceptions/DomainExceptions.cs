namespace HelpDesk.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message)
    {
    }

    public Dictionary<string, string[]> Errors { get; set; } = new();
}

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.")
    {
    }
}

public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message)
    {
    }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message) : base(message)
    {
    }
}

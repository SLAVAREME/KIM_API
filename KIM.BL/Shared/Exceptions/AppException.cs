namespace KIM.BL.Shared.Exceptions;

public abstract class AppException(string message) : Exception(message)
{
}

public sealed class NotFoundException(string message) : AppException(message)
{
}

public sealed class ConflictException(string message) : AppException(message)
{
}

public sealed class ForbiddenOperationException(string message) : AppException(message)
{
}
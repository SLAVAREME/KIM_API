namespace KIM.BL.Shared.Responses;

public class ApiResponse<T>
{
    public int Code { get; init; }

    public bool Success { get; init; }

    public T? Data { get; init; }

    public string? Message { get; init; }

    public IReadOnlyCollection<ValidationErrorItem>? Errors { get; init; }

    public static ApiResponse<T> SuccessResult(T? data, string? message = null)
    {
        var normalizedData = NormalizeData(data);

        return new ApiResponse<T>
        {
            Code = (int)AppCode.Ok,
            Success = true,
            Data = normalizedData,
            Message = message
        };
    }

    private static T? NormalizeData(T? data)
    {
        if (data is not null)
        {
            return data;
        }

        var type = typeof(T);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PagedResult<>))
        {
            return (T?)Activator.CreateInstance(type);
        }

        if (!type.IsInterface || !type.IsGenericType)
        {
            return data;
        }

        var genericType = type.GetGenericTypeDefinition();
        if (genericType != typeof(IEnumerable<>)
            && genericType != typeof(IReadOnlyCollection<>)
            && genericType != typeof(ICollection<>)
            && genericType != typeof(IList<>))
        {
            return data;
        }

        var itemType = type.GetGenericArguments()[0];
        var emptyArray = Array.CreateInstance(itemType, 0);
        return (T?)(object)emptyArray;
    }

    public static ApiResponse<T> Failure(AppCode code, string message, IReadOnlyCollection<ValidationErrorItem>? errors = null)
    {
        return new ApiResponse<T>
        {
            Code = (int)code,
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

public class ValidationErrorItem
{
    public string Field { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
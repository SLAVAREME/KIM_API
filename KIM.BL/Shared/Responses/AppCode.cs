namespace KIM.BL.Shared.Responses;

public enum AppCode
{
    Ok = 200,
    ValidationError = 1001,
    NotFound = 1002,
    Conflict = 1003,
    ForbiddenOperation = 1004,
    InternalError = 1500
}
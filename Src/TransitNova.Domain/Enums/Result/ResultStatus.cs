
namespace TransitNova.Domain.Enums.Result
{
    public enum ResultStatus
    {
        Success = 200,
        Created = 201,
        NoContent = 204,
        ValidationError = 422,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        Failure = 400,
        UnExpected = 500,
    }
}

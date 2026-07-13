using Microsoft.AspNetCore.Mvc;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.Domain.Enums.Result; 
namespace TransitNova.Api.Controllers
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this BaseResult result)
        {
            if (result.IsSuccess)
            {
                return result.Status switch
                {
                    ResultStatus.NoContent => new NoContentResult(),
                    ResultStatus.Created => new ObjectResult(BuildSuccess(result))
                    {
                        StatusCode = StatusCodes.Status201Created
                    },
                    _ => new OkObjectResult(BuildSuccess(result))
                };
            }

            return result.Status switch
            {
                ResultStatus.NotFound => new NotFoundObjectResult(BuildError(result)),
                ResultStatus.Unauthorized => new UnauthorizedObjectResult(BuildError(result)),
                ResultStatus.Forbidden => new ObjectResult(BuildError(result))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                },
                ResultStatus.Conflict => new ConflictObjectResult(BuildError(result)),
                ResultStatus.ValidationError => new UnprocessableEntityObjectResult(BuildValidation(result)),
                ResultStatus.UnExpected => new ObjectResult(BuildError(result))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                },
                _ => new BadRequestObjectResult(BuildError(result))
            };
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return result.Status switch
                {
                    ResultStatus.Created => new ObjectResult(BuildSuccess(result, result.Data))
                    {
                        StatusCode = StatusCodes.Status201Created
                    },
                    ResultStatus.NoContent => new NoContentResult(),
                   _ => new OkObjectResult(BuildSuccess(result, result.Data))
                };
            }

            return ((BaseResult)result).ToActionResult();
        }

        private static object BuildSuccess(BaseResult result, object? data = null) => new
        {
            IsSuccess = true,
            Message = result.Message,
            StatusCode = result.StatusCode,
            Data = data
        };

        private static object BuildError(BaseResult result) => new
        {
            IsSuccess = false,
            StatusCode = result.StatusCode,
            ErrorCode = result.Error?.Code.ToString(),
            Message = result.Error?.Message
        };

        private static object BuildValidation(BaseResult result) => new
        {
            IsSuccess = false,
            StatusCode = result.StatusCode,
            Errors = result.Errors.Select(e => new
            {
                code = e.Code.ToString(),
                message = e.Message
            })
        };
    }
}

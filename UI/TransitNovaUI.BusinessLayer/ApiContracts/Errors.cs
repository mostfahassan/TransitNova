

using TransitNova.Domain.Enums.Result;

namespace TransitNovaUI.BusinessLayer.ApiContracts
{
    internal class Errors
    {
        public static ApiError Failure(string message) => 
            
            new ApiError(ErrorCode.FAILED, message);
    }
}

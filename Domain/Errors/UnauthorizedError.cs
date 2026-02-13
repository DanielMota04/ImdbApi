using FluentResults;

namespace Domain.Errors
{
    public class UnauthorizedError : Error
    {
        public UnauthorizedError(string message) : base(message)
        {
            Metadata.Add("StatusCode", 401);
        }
    }
}

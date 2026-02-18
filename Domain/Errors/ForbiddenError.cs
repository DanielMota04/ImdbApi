using FluentResults;

namespace Domain.Errors
{
    public class ForbiddenError : Error
    {
        public ForbiddenError(string message) : base(message)
        {
            
        }
    }
}

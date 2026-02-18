using FluentResults;

namespace Domain.Errors
{
    public class NotFoundError : Error
    {
        public NotFoundError(string message) : base(message)
        {
            Metadata.Add("StatusCode", 404);
        }
    }
}

namespace Application.Exceptions
{
    public class ExternalServiceException : Exception
    {
        public int? ExternalErrorCode { get; }

        public ExternalServiceException(string message, int? errorCode = null)
            : base(message)
        {
            ExternalErrorCode = errorCode;
        }
    }
}

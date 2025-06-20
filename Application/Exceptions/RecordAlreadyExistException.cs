namespace Application.Exceptions
{
    public class RecordAlreadyExistException : Exception
    {
        public RecordAlreadyExistException(string message = "Este registro ya existe") : base(message)
        {
        }
    }
}

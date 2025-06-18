namespace Application.Exceptions
{
    public class NullFileException : Exception
    {
        public NullFileException(string message = "La solicitud requiere que seleccione un archivo") : base(message)
        {
        }
    }
}

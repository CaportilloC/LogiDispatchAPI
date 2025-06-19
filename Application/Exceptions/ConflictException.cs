﻿namespace Application.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException() : base("Conflict occurred.") { }

        public ConflictException(string message) : base(message) { }

        public ConflictException(string message, Exception inner) : base(message, inner) { }
    }
}

namespace BookingSystem.Application.Exceptions;

public class BookingConflictException : Exception
{
    public BookingConflictException()
    {
    }

    public BookingConflictException(string message) : base(message)
    {
    }

    public BookingConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
namespace Booking.Entities.Exceptions;

// levée quand une règle métier n'est pas respectée -> 409
public class BusinessRuleException(string message) : Exception(message);

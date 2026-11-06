namespace Booking.Entities.Exceptions;

// levée quand un user, une salle ou une réservation n'existe pas -> 404
public class NotFoundException(string message) : Exception(message);

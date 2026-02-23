// Custom exceptions for invalid appointment times and double bookings

public class DoubleBookingException : Exception
{
	public DoubleBookingException() : base("Provider is already booked at that time.") {}

	public DoubleBookingException(string message) : base(message) {}
}

public class InvalidAppointmentTimeException : Exception
{
	public InvalidAppointmentTimeException() : base("Appointment start and end times are invalid.") {}
	public InvalidAppointmentTimeException(string message) : base(message) {}
}
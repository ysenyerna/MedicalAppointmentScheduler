// Appointment Scheduler Class

class AppointmentScheduler {
	
	// Constant values
	static readonly TimeSpan OpeningTime = new(8, 0, 0);
	static readonly TimeSpan ClosingTime = new(17, 0, 0);
	const int MinimumAppointmentDuration = 15; // in minutes

	// Properties
	readonly List<Appointment> _appointments = [];

	// Methods

	// Adds an appointment, throws exceptions if the appointment time is invalid
	public void Add(Appointment appt)
	{
		if (!ValidateAppointmentHours(appt))
		{
			throw new InvalidAppointmentTimeException();
		}
		if (!HasDoubleBookings(appt))
		{
			throw new DoubleBookingException();
		}

		// Add the appointment
		_appointments.Add(appt);
	}

	// Attempts to remove an appointment by ID, returns false if the appointment is not found
	public bool Cancel(string id)
	{
		Appointment? appt = _appointments.Find(a => a.Id == id);
		return appt == null ? false : _appointments.Remove(appt);
	}

	// Attempts to reschedule an appointment by ID, returns false if the appointment ID is not found
	public bool Reschedule(string id, DateTime newStart, DateTime newEnd)
	{
		Appointment? appt = _appointments.Find(a => a.Id == id);
		if (appt == null)
			return false;

		if (!ValidateAppointmentHours(newStart, newEnd))
		{
			throw new InvalidAppointmentTimeException();
		}
		if (!HasDoubleBookings(newStart, newEnd, appt.ProviderName, appt.Id))
		{
			throw new DoubleBookingException();
		}

		appt.Reschedule(newStart, newEnd);
		return true;
	}

	public List<Appointment> ListAppointments()
		=> [.. _appointments];

	public List<Appointment> ListByDay(DateTime day)
		=> [.. _appointments.Where(a => a.StartTime.Date == day.Date)];


	public List<Appointment> ListByProvider(string provider)
		=> [.. _appointments.Where(a => string.Equals(a.ProviderName, provider, StringComparison.OrdinalIgnoreCase))];


	public Appointment? FindAppointment(string id) 
		=> _appointments.Find(a => a.Id == id);
	

	// Returns true if appointment hours are valid
	static bool ValidateAppointmentHours(Appointment appt)
		=> ValidateAppointmentHours(appt.StartTime, appt.EndTime);

	static bool ValidateAppointmentHours(DateTime start, DateTime end)
	{
		// Check if the start and end times are on the same day
		if (start.Date != end.Date)
		{
			return false;
		}

		// Check if start time is before end time
		if (start >= end)
		{
			return false;
		}

		TimeSpan startTime = start.TimeOfDay;
		TimeSpan endTime = end.TimeOfDay;
	
		// Check if the start and end times are within valid hours
		if (startTime < OpeningTime || startTime > ClosingTime
			|| endTime < OpeningTime || endTime > ClosingTime)
		{
			return false;
		}

		// Check if the appointment is longer than the minimum appointment time
		if ((endTime - startTime) < TimeSpan.FromMinutes(MinimumAppointmentDuration))
		{
			return false;
		}

		return true;
	}

	// Checks if the appointment time overlaps with an existing appointment
	public bool HasDoubleBookings(Appointment appt)
		=> HasDoubleBookings(appt.StartTime, appt.EndTime, appt.ProviderName, appt.Id);
	
	public bool HasDoubleBookings(DateTime startTime, DateTime endTime, string provider, string? ignoreId = null)
	{
		List<Appointment> appts = ListByProvider(provider);

		foreach (Appointment otherAppt in appts)
		{
			// Ignore specified appointment (used to ignore self when rescheduling)
			if (otherAppt.Id == ignoreId)
				continue; 

			// Check if the datetimes overlap
			if (startTime < otherAppt.EndTime 
				&& otherAppt.StartTime < endTime)
			{
				return false;
			}
		}
		return true;
	}

}
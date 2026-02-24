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
		if (!ValidateAppointmentHours(appt, out string timeError))
		{
			throw new InvalidAppointmentTimeException(timeError);
		}
		if (!HasDoubleBookings(appt, out string doubleBookError))
		{
			throw new DoubleBookingException(doubleBookError);
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

		if (!ValidateAppointmentHours(newStart, newEnd, out string timeError))
		{
			throw new InvalidAppointmentTimeException(timeError);
		}
		if (!HasDoubleBookings(newStart, newEnd, appt.ProviderName, out string doubleBookError, appt.Id))
		{
			throw new DoubleBookingException(doubleBookError);
		}

		appt.Reschedule(newStart, newEnd);
		return true;
	}




	// Returns a deep copy of the _appointments list
	List<Appointment> CloneAppointmentList()
	{
		List<Appointment> newlist = [];
		_appointments.ForEach(a => newlist.Add(new(a))); // clone each appointment and add to new list
		return newlist;
	}


	// Methods for getting appointment info, all return copies of the appointment objects so they can't be modified
	public List<Appointment> ListAppointments() 
		=> [.. CloneAppointmentList()];

	public List<Appointment> ListByDay(DateTime day)
		=> [.. CloneAppointmentList().Where(a => a.StartTime.Date == day.Date)];

	public List<Appointment> ListByProvider(string provider)
		=> [.. CloneAppointmentList().Where(a => string.Equals(a.ProviderName, provider, StringComparison.OrdinalIgnoreCase))];

	public Appointment? FindAppointment(string id) 
		=> CloneAppointmentList().Find(a => a.Id == id);
	



	// Validation methods


	// Returns true if appointment hours are valid, otherwise outputs an error message and returns false
	static bool ValidateAppointmentHours(Appointment appt, out string errorMessage)
		=> ValidateAppointmentHours(appt.StartTime, appt.EndTime, out errorMessage);

	static bool ValidateAppointmentHours(DateTime start, DateTime end, out string errorMessage)
	{
		errorMessage = "";
		// Check if the start and end times are on the same day
		if (start.Date != end.Date)
		{
			errorMessage = "Appointment times are not on the same day";
			return false;
		}

		// Check if start time is before end time
		if (start >= end)
		{
			errorMessage = "Appointment start time is after appointment end time";
			return false;
		}

		TimeSpan startTime = start.TimeOfDay;
		TimeSpan endTime = end.TimeOfDay;
	
		// Check if the start and end times are within valid hours
		if (startTime < OpeningTime || startTime > ClosingTime
			|| endTime < OpeningTime || endTime > ClosingTime)
		{
			errorMessage = $"Appointment time is not within valid hours. Appointments must be between {OpeningTime:hh\\:mm} and {ClosingTime:hh\\:mm}";
			return false;
		}

		// Check if the appointment is longer than the minimum appointment time
		if ((endTime - startTime) < TimeSpan.FromMinutes(MinimumAppointmentDuration))
		{
			errorMessage = $"Appointment is too short. Appointments must be {MinimumAppointmentDuration} minutes or longer";
			return false;
		}

		return true;
	}


	// Checks if the appointment time overlaps with an existing appointment
	public bool HasDoubleBookings(Appointment appt, out string errorMessage)
		=> HasDoubleBookings(appt.StartTime, appt.EndTime, appt.ProviderName, out errorMessage, appt.Id);
	
	public bool HasDoubleBookings(DateTime startTime, DateTime endTime, string provider, out string errorMessage, string? ignoreId = null)
	{
		errorMessage = "";
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
				errorMessage = $"Appointment [{otherAppt.Id}] is already booked at that time with that provider";
				return false;
			}
		}
		return true;
	}

}
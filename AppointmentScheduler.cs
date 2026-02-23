// Appointment Scheduler Class

class AppointmentScheduler {
	
	// Constant values
	static readonly TimeSpan OpeningTime = new(8, 0, 0);
	static readonly TimeSpan ClosingTime = new(17, 0, 0);
	const int MinimumAppointmentDuration = 15; // in minutes

	// Properties
	List<Appointment> _appointments = [];

	// Methods

	// Adds an appointment
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

	// Attempts to remove an appointment by ID, returns false if the appointment was not found
	public bool Cancel(string id)
	{
		Appointment? appt = _appointments.Find(a => a.Id == id);
		return appt == null ? false : _appointments.Remove(appt);
	}


	public List<Appointment> ListByDay(DateTime day)
	{
		return [.. _appointments.Where(a => a.StartTime.Date == day.Date)];
	}

	public List<Appointment> ListByProvider(string provider)
	{
		return [.. _appointments.Where(a => a.ProviderName == provider)];
	}


	// Returns true if appointment hours are valid
	static bool ValidateAppointmentHours(Appointment appt)
	{
		// Check if the start and end times are on the same day
		if (appt.StartTime.Date != appt.EndTime.Date)
		{
			return false;
		}

		// Check if start time is before end time
		if (appt.StartTime >= appt.EndTime)
		{
			return false;
		}

		TimeSpan startTime = appt.StartTime.TimeOfDay;
		TimeSpan endTime = appt.EndTime.TimeOfDay;
	
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
	{
		List<Appointment> appts = ListByProvider(appt.ProviderName);

		foreach (Appointment otherAppt in appts)
		{
			// Check if the datetimes overlap
			if (appt.StartTime < otherAppt.EndTime 
				&& otherAppt.StartTime < appt.EndTime)
			{
				return false;
			}
		}
		return true;
	}




}
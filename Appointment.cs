// Appointment class


class Appointment
{
	// Appointment ID 
	string _id = null!;
	public string Id { 
		get { return _id; }
		private set { 
			ArgumentException.ThrowIfNullOrWhiteSpace(value);
			_id = value.Trim();
			}
		}
	// Patient Name
	string _patientName = null!;
	public string PatientName { 
		get { return _patientName; }
		private set { 
			ArgumentException.ThrowIfNullOrWhiteSpace(value);
			_patientName = value.Trim();
			}
		}
	// Provider Name
	string _providerName = null!;
	public string ProviderName { 
		get { return _providerName; }
		private set { 
			ArgumentException.ThrowIfNullOrWhiteSpace(value);
			_providerName = value.Trim();
			}
		}
	// Room name
	string _room = null!;
	public string Room { 
		get { return _room; }
		private set { 
			ArgumentException.ThrowIfNullOrWhiteSpace(value);
			_room = value.Trim();
			}
		}
	// Start and end time
	public DateTime StartTime {get; private set; }
	public DateTime EndTime {get; private set;}


	// Constructor
	public Appointment(string id, string patientName, string providerName, DateTime startTime, DateTime endTime, string room) {
		Id = id;
		PatientName = patientName;
		ProviderName = providerName;
		Room = room;
		StartTime = startTime;
		EndTime = endTime;

	}

	// Methods
	public void Reschedule(DateTime newStart, DateTime newEnd)
	{
		StartTime = newStart;
		EndTime = newEnd;
	}

	public override string ToString()
	{
		return $"[ {Id} ] {StartTime}–{EndTime}, Provider: {ProviderName}, Patient: {PatientName}, {Room}";
	}

}
// Medical Appointment Scheduler Application //

AppointmentScheduler scheduler = new();


// Show a menu
Console.WriteLine("- Medical Appointment Scheduler -");
Console.WriteLine("1. Add Appointment\n2. Cancel Appointment\n3. Reschedule Appointment\n4. List All Appointments\n5. List by Provider\n6. List by Day\n7. Exit");

bool continueRunning = true;
do
{

	// Get user choice
	int choice = GetInput<int>("Select an option: ", "Choice must be a number!");

	switch (choice)
	{
		// Add Appointment
		case 1:
			Console.WriteLine("- Adding an Appointment -");
			AddAppointment();
			break;

		// Cancel Appointment
		case 2:
			Console.WriteLine("- Cancelling an Appointment -");
			string cancelId = GetInput<string>("Enter appointment ID: ");
			Appointment? cancelAppt = scheduler.FindAppointment(cancelId);
			if (cancelAppt != null)
			{
				scheduler.Cancel(cancelId);
				WriteColoredLine("Appointment Canceled!", ConsoleColor.Green);
				Logger.Info("Canceled " + cancelAppt.ToString());
			}
			else
				WriteColoredLine($"Could not find appointment with ID '{cancelId}'.", ConsoleColor.Red);
			break;
		
		// Reschedule Appointment
		case 3:
			Console.WriteLine("- Rescheduling an Appointment -");
			RescheduleAppointment();
			break;

		// List Appointments
		case 4:
			Console.WriteLine("- Listing all appointments -");
			var appts = scheduler.ListAppointments();
			if (appts.Count == 0)
				WriteColoredLine("There are currently no appointments in the system.", ConsoleColor.Green);
			else
				appts.ForEach(a => Console.WriteLine(a.ToString()));
			break;
		
		// List by Provider
		case 5:
			Console.WriteLine("- Listing appointments by provider -");
			string provider = GetInput<string>("Enter provider name: ");
			var apptsByProvider = scheduler.ListByProvider(provider);
			if (apptsByProvider.Count == 0)
				WriteColoredLine("There are no appointments scheduled with that provider.", ConsoleColor.Green);
			else
				apptsByProvider.ForEach(a => Console.WriteLine(a.ToString()));
			break;

		// List by Day
		case 6:
			Console.WriteLine("- Listing appointments by day -");
			DateTime date = GetInput<DateTime>("Enter date: ", "Input must be a date!");
			var apptsByDay = scheduler.ListByDay(date);
			if (apptsByDay.Count == 0)
				WriteColoredLine("There are no appointments scheduled for that date.", ConsoleColor.Green);
			else
				apptsByDay.ForEach(a => Console.WriteLine(a.ToString()));
			break;
		
		// Exit
		case 7:
			Console.WriteLine("- Exiting! -");
			continueRunning = false;
			break;
		
		default:
			WriteColoredLine("Input must be between 1 and 7!", ConsoleColor.Red);
			break;
	}



} while (continueRunning);



void AddAppointment()
{
	// Get appointment information from user
	string id;
	while (true)
	{
		id = GetInput<string>("Enter appointment ID: ");

		// Check that the ID is valid and unique
		if (string.IsNullOrWhiteSpace(id)) {
			WriteColoredLine("ID cannot be empty!", ConsoleColor.Red);
			continue; }
		if (scheduler.FindAppointment(id) != null) {
			WriteColoredLine($"There is already an appointment with the ID '{id}'! ID must be unique.", ConsoleColor.Red);
			continue; }
		break;
	}

	string patient;
	while (string.IsNullOrWhiteSpace(patient = GetInput<string>("Enter patient name: ")))
		WriteColoredLine("Patient name cannot be empty!", ConsoleColor.Red);

	string provider;
	while (string.IsNullOrWhiteSpace(provider = GetInput<string>("Enter provider name: ")))
		WriteColoredLine("Provider name cannot be empty!", ConsoleColor.Red);

	string room;
	while (string.IsNullOrWhiteSpace(room = GetInput<string>("Enter room: ")))
		WriteColoredLine("Room cannot be empty!", ConsoleColor.Red);

	DateTime start = GetInput<DateTime>("Enter start time: ", "Input must be a date!");
	DateTime end = GetInput<DateTime>("Enter end time: ", "Input must be a date!");

	// Add the appointment
	Appointment appt = new(id, patient, provider, start, end, room);
	try {
		scheduler.Add(appt);
		Logger.Info("Added " + appt.ToString());
		WriteColoredLine("Appointment Added!", ConsoleColor.Green);
	}
	catch (InvalidAppointmentTimeException ex)
	{
		Logger.Warn($"Attempted to schedule an appointment for {start.TimeOfDay:hh\\:mm}–{end.TimeOfDay:hh\\:mm} on {start:MM-dd-yyyy}: " + ex.Message);
		WriteColoredLine("Appointment times are invalid!", ConsoleColor.Red);
	}
	catch (DoubleBookingException ex)
	{
		Logger.Warn($"Attempted to double book an appointment for {appt.ProviderName} at {start.TimeOfDay:hh\\:mm} on {start:MM-dd-yyyy}: " + ex.Message);
		WriteColoredLine("There is already an appointment at that time!", ConsoleColor.Red);
	}
	catch (Exception ex)
	{
		Logger.Error("Unexpected error when adding an appointment: " + ex.Message);
	}
}

void RescheduleAppointment()
{
	string rescheduleId = GetInput<string>("Enter appointment ID: ");
	var rescheduleAppt = scheduler.FindAppointment(rescheduleId);
	if (rescheduleAppt == null)
	{
		WriteColoredLine($"Could not find appointment with ID '{rescheduleId}'.", ConsoleColor.Red);
		return;
	}

	// Get new times from the user
	DateTime start = GetInput<DateTime>("Enter start time: ", "Input must be a date!");
	DateTime end = GetInput<DateTime>("Enter end time: ", "Input must be a date!");

	try {
		DateTime oldStart = rescheduleAppt.StartTime;
		DateTime oldEnd = rescheduleAppt.EndTime;
		scheduler.Reschedule(rescheduleId, start, end);
		Logger.Info($"Rescheduled [{rescheduleAppt.Id}] {oldStart:HH:mm}-{oldEnd:HH:mm} -> {start:HH:mm}-{end:HH:mm}");
		WriteColoredLine("Appointment Rescheduled!", ConsoleColor.Green);
	}
	catch (InvalidAppointmentTimeException)
	{
		Logger.Warn($"Attempted to reschedule appointment [{rescheduleAppt.Id}] for {start.TimeOfDay:hh\\:mm}–{end.TimeOfDay:hh\\:mm} on {start:MM-dd-yyyy}");
		WriteColoredLine("New appointment times are invalid!", ConsoleColor.Red);
	}
	catch (DoubleBookingException)
	{
		Logger.Warn($"Attempted to double book an appointment for {rescheduleAppt.ProviderName} at {start.TimeOfDay:hh\\:mm} on {start:MM-dd-yyyy}");
		WriteColoredLine("There is already an appointment at that time!", ConsoleColor.Red);
	}
	catch (Exception ex)
	{
		Logger.Error("Unexpected error when rescheduling an appointment: " + ex.Message);
	}
}

// Prompts the user to enter a value and returns the input as the specified type
static T GetInput<T>(string message, string? errorMessage = null) 
{
	while (true)
	{
		// Get input from user
		Console.Write(message);
		Console.ForegroundColor = ConsoleColor.Yellow;
		string input = Console.ReadLine() ?? "";
		Console.ResetColor();
		

		// Validate input and return
		if (typeof(T) == typeof(string))
			return (T)(object)input;
		if (typeof(T) == typeof(int) && int.TryParse(input, out int intValue))
			return (T)(object)intValue;
		if (typeof(T) == typeof(double) && double.TryParse(input, out double doubleValue))
			return (T)(object)doubleValue;
		if (typeof(T) == typeof(DateTime) && DateTime.TryParse(input, out DateTime dateValue))
			return (T)(object)dateValue;

		// Print an error message
		WriteColoredLine(errorMessage ?? "Invalid Input! Please try again.", ConsoleColor.Red);
	}
}

// Writes a line to the console in the specified color
static void WriteColoredLine(string message, ConsoleColor color)
{
	Console.ForegroundColor = color;
	Console.WriteLine(message);
	Console.ResetColor();
}
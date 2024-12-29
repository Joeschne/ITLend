using Microsoft.AspNetCore.Components;
using Frontend.Interfaces;
using Shared.DTOs;
using Microsoft.JSInterop;

namespace Frontend.Pages;

public partial class Home : ComponentBase
{
    [Inject]
    private ILaptopService LaptopService { get; set; }

    [Inject]
    private IStudentService StudentService { get; set; }

    [Inject]
    private IBookingService BookingService { get; set; }

    private int availableLaptops;
    private string searchQuery = string.Empty;
    private List<StudentDTO> autocompleteResults = new();
    private List<BookingResponseDTO> openBookings;
    private bool showStudentModal = false;
    private StudentDetailDTO? selectedStudent;
    private bool showNewLendingModal = false;
    private BookingRequestDTO newBooking = new BookingRequestDTO();
    private List<LaptopDTO> availableLaptopList = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadAvailableLaptopsAsync();
        await LoadOpenBookingsAsync();
    }

    private async Task LoadAvailableLaptopsAsync()
    {
        try
        {
            var laptops = await LaptopService.GetLaptopsAsync();
            availableLaptops = laptops.Count(l => l.IsAvailable);
            availableLaptopList = laptops.Where(l => l.IsAvailable).ToList(); // Store only available laptops for the dropdown
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading laptops: {ex.Message}");
        }
    }

    private async Task LoadOpenBookingsAsync()
    {
        try
        {
            openBookings = (await BookingService.GetNotReturnedBookingsAsync()).ToList();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading open bookings: {ex.Message}");
            openBookings = new List<BookingResponseDTO>();
        }
    }

    private async Task SearchStudentsAsync(ChangeEventArgs e)
    {
        searchQuery = e.Value.ToString();
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            autocompleteResults.Clear();
            return;
        }

        try
        {
            autocompleteResults = (await StudentService.SearchStudentsAsync(searchQuery)).ToList();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error searching students: {ex.Message}");
        }
    }

    private async Task SelectStudent(StudentDTO student)
    {
        try
        {
            // Try to fetch student bookings
            selectedStudent = await BookingService.GetStudentBookingsAsync(student.Username);
        }
        catch (Exception ex)
        {
            // Initialize StudentDetailDTO with the entered username and empty bookings
            selectedStudent = new StudentDetailDTO
            {
                Username = student.Username,
                Bookings = new List<BookingResponseDTO>()
            };
        }
        finally
        {
            showStudentModal = true;
        }
    }

    private void OpenNewStudentModal()
    {
        // Initialize modal for a new student with no bookings
        selectedStudent = new StudentDetailDTO
        {
            Username = searchQuery, // Use current search query for convenience
            Bookings = new List<BookingResponseDTO>()
        };
        showStudentModal = true;
    }


    private void CloseStudentModal()
    {
        showStudentModal = false;
        selectedStudent = null;
    }

    private async Task OpenNewLendingModal()
    {

        if (availableLaptopList == null || !availableLaptopList.Any())
        {
            CloseStudentModal();
            showNoLaptopsAlert = true;
            return;
        }
        try
        {
            if (selectedStudent != null)
            {
                // Check if student exists before creating a new one
                var existingStudents = await StudentService.SearchStudentsAsync(selectedStudent.Username);

                if (!existingStudents.Any(s => s.Username.Equals(selectedStudent.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    // Create a new student
                    await StudentService.AddStudentAsync(new StudentDTO { Username = selectedStudent.Username });
                    Console.WriteLine($"New student '{selectedStudent.Username}' created.");
                }
            }

            // Initialize NewBooking object with pre-filled student username
            newBooking = new BookingRequestDTO
            {
                StudentUsername = selectedStudent?.Username ?? string.Empty,
                BookingDateTime = DateTime.Now,
                PlannedReturn = DateTime.Today.AddHours(16) // Default planned return: today at 16:00
            };

            // Open New Lending Modal
            showStudentModal = false; // Close the student modal
            showNewLendingModal = true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating new student or opening lending modal: {ex.Message}");
        }
    }


    private void CloseNewLendingModal()
    {
        showNewLendingModal = false;
    }

    private async Task SaveNewBooking(BookingRequestDTO booking)
    {
        try
        {
            await BookingService.CreateBookingAsync(booking);
            Console.WriteLine("Booking successfully created.");
            showNewLendingModal = false;

            // Reload open bookings
            await LoadOpenBookingsAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating booking: {ex.Message}");
        }
    }

    private bool showNoLaptopsAlert = false;

    private void DismissNoLaptopsAlert()
    {
        showNoLaptopsAlert = false;
    }
}

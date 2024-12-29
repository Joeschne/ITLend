using Microsoft.AspNetCore.Components;
using Frontend.Interfaces;
using Shared.DTOs;

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


    private async Task OpenNewLendingModal()
    {
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

            // Placeholder for opening lending modal logic
            Console.WriteLine("Opening new lending modal...");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating new student or opening lending modal: {ex.Message}");
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
}

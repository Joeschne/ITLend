using Microsoft.AspNetCore.Components;
using Frontend.Interfaces;
using Shared.DTOs;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;

namespace Frontend.Pages;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
public partial class Home : ComponentBase
{
    /// <summary>
    /// Gets or sets the laptop service.
    /// </summary>
    /// <value>
    /// The laptop service.
    /// </value>
    [Inject]
    private ILaptopService LaptopService { get; set; }

    /// <summary>
    /// Gets or sets the student service.
    /// </summary>
    /// <value>
    /// The student service.
    /// </value>
    [Inject]
    private IStudentService StudentService { get; set; }

    /// <summary>
    /// Gets or sets the booking service.
    /// </summary>
    /// <value>
    /// The booking service.
    /// </value>
    [Inject]
    private IBookingService BookingService { get; set; }

    /// <summary>
    /// The available laptops
    /// </summary>
    private int availableLaptops;
    /// <summary>
    /// The search query
    /// </summary>
    private string searchQuery = string.Empty;
    /// <summary>
    /// The index of the highlighted item in the dropdown
    /// </summary>
    private int highlightedIndex = -1; // Tracks the currently highlighted item in the dropdown
    /// <summary>
    /// The autocomplete results
    /// </summary>
    private List<StudentDTO> autocompleteResults = new();
    /// <summary>
    /// The open bookings
    /// </summary>
    private List<BookingResponseDTO> openBookings;
    /// <summary>
    /// The selected student
    /// </summary>
    private StudentDetailDTO? selectedStudent;
    /// <summary>
    /// The new booking
    /// </summary>
    private BookingRequestDTO newBooking = new BookingRequestDTO();
    /// <summary>
    /// The available laptop list
    /// </summary>
    private List<LaptopDTO> availableLaptopList = new();
    /// <summary>
    /// The selected laptop
    /// </summary>
    private LaptopDTO selectedLaptop;
    /// <summary>
    /// The selected laptop bookings
    /// </summary>
    private List<BookingResponseDTO> selectedLaptopBookings = new();
    /// <summary>
    /// The selected booking
    /// </summary>
    private BookingResponseDTO? selectedBooking;
    /// <summary>
    /// The show student lending modal
    /// </summary>
    private bool showStudentLendingModal = false;
    /// <summary>
    /// The show student modal
    /// </summary>
    private bool showStudentModal = false;
    /// <summary>
    /// The show new lending modal
    /// </summary>
    private bool showNewLendingModal = false;
    /// <summary>
    /// The show laptop modal
    /// </summary>
    private bool showLaptopModal = false;
    /// <summary>
    /// The show edit modal
    /// </summary>
    private bool showEditModal = false;
    /// <summary>
    /// The show no laptops alert
    /// </summary>
    private bool showNoLaptopsAlert = false;
    /// <summary>
    /// The show return modal
    /// </summary>
    private bool showReturnModal = false;
    /// <summary>
    /// The search input reference
    /// </summary>
    private ElementReference searchInputRef;



    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// Override this method if you will perform an asynchronous operation and
    /// want the component to refresh when that operation is completed.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadAvailableLaptopsAsync();
        await LoadOpenBookingsAsync();
    }

    /// <summary>
    /// Loads the available laptops asynchronous.
    /// </summary>
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

    /// <summary>
    /// Loads the open bookings asynchronous.
    /// </summary>
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

    /// <summary>
    /// Searches the students asynchronous.
    /// </summary>
    /// <param name="e">The <see cref="ChangeEventArgs"/> instance containing the event data.</param>
    private async Task SearchStudentsAsync(ChangeEventArgs e)
    {
        searchQuery = e.Value.ToString();
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            autocompleteResults.Clear();
            highlightedIndex = -1;
            return;
        }

        try
        {
            autocompleteResults = (await StudentService.SearchStudentsAsync(searchQuery)).ToList();
            highlightedIndex = autocompleteResults.Any() ? 0 : -1; // Highlight the first item if results are present
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error searching students: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the key down.
    /// </summary>
    /// <param name="e">The <see cref="KeyboardEventArgs"/> instance containing the event data.</param>
    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "ArrowDown" && autocompleteResults.Any())
        {
            // Move highlight down
            highlightedIndex = (highlightedIndex + 1) % autocompleteResults.Count;
        }
        else if (e.Key == "ArrowUp" && autocompleteResults.Any())
        {
            // Move highlight up
            highlightedIndex = (highlightedIndex - 1 + autocompleteResults.Count) % autocompleteResults.Count;
        }
        else if (e.Key == "Enter")
        {
            if (highlightedIndex >= 0 && highlightedIndex < autocompleteResults.Count)
            {
                // Select the highlighted item
                await SelectStudent(autocompleteResults[highlightedIndex]);
            }
            else if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                // Process the entered username as a new student
                await OpenNewStudentModal();
            }
        }

        // Ensure the input field remains focused
        await searchInputRef.FocusAsync();
    }



    /// <summary>
    /// Selects the student.
    /// </summary>
    /// <param name="student">The student.</param>
    private async Task SelectStudent(StudentDTO student)
    {
        highlightedIndex = -1; // Reset highlight

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
            showStudentLendingModal = true;
        }
    }

    /// <summary>
    /// Opens the new student modal.
    /// </summary>
    private async Task OpenNewStudentModal()
    {
        try
        {
            // Trim the input to avoid issues with leading/trailing whitespace
            var username = searchQuery.Trim();

            // Check if the username exists in the autocomplete results
            var matchingStudent = autocompleteResults.FirstOrDefault(s =>
                s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (matchingStudent != null)
            {
                // Load existing student details if the username exists
                await SelectStudent(matchingStudent);
            }
            else
            {
                // Initialize modal for a new student with no bookings
                selectedStudent = new StudentDetailDTO
                {
                    Username = username,
                    Bookings = new List<BookingResponseDTO>()
                };
                showStudentLendingModal = true;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error handling new student modal: {ex.Message}");
        }
    }


    /// <summary>
    /// Closes the student modal.
    /// </summary>
    private void CloseStudentModal()
    {
        showStudentModal = false;
        selectedStudent = null;
    }

    /// <summary>
    /// Closes the student lending modal.
    /// </summary>
    private void CloseStudentLendingModal()
    {
        showStudentLendingModal = false;
        selectedStudent = null;
    }

    /// <summary>
    /// Opens the new lending modal.
    /// </summary>
    private async Task OpenNewLendingModal()
    {

        if (availableLaptopList == null || !availableLaptopList.Any())
        {
            CloseStudentLendingModal();
            searchQuery = "";
            autocompleteResults.Clear();
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
            showStudentLendingModal = false;
            showNewLendingModal = true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating new student or opening lending modal: {ex.Message}");
        }
    }

    /// <summary>
    /// Closes the new lending modal.
    /// </summary>
    private void CloseNewLendingModal()
    {
        showNewLendingModal = false;
        searchQuery = "";
        autocompleteResults.Clear();
        selectedStudent = null;
    }

    /// <summary>
    /// Saves the new booking.
    /// </summary>
    /// <param name="booking">The booking.</param>
    private async Task SaveNewBooking(BookingRequestDTO booking)
    {
        try
        {
            await BookingService.CreateBookingAsync(booking);
            Console.WriteLine("Booking successfully created.");

            await LoadAvailableLaptopsAsync();

            CloseNewLendingModal();

            await LoadOpenBookingsAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating booking: {ex.Message}");
        }
    }

    /// <summary>
    /// Dismisses the no laptops alert.
    /// </summary>
    private void DismissNoLaptopsAlert()
    {
        showNoLaptopsAlert = false;
    }

    /// <summary>
    /// Opens the laptop modal.
    /// </summary>
    /// <param name="laptop">The laptop.</param>
    private async Task OpenLaptopModal(LaptopDTO laptop)
    {
        if (laptop == null) return;

        try
        {
            selectedLaptop = laptop;
            // Fetch bookings associated with this laptop
            selectedLaptopBookings = (await BookingService.GetBookingsByLaptopAsync(laptop.Id)).ToList();
            showLaptopModal = true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching laptop bookings: {ex.Message}");
            selectedLaptopBookings = new List<BookingResponseDTO>();
        }
    }

    /// <summary>
    /// Closes the laptop modal.
    /// </summary>
    private void CloseLaptopModal()
    {
        showLaptopModal = false;
        selectedLaptop = null;
        selectedLaptopBookings = null;
    }

    /// <summary>
    /// Opens the student modal.
    /// </summary>
    /// <param name="username">The username.</param>
    private async Task OpenStudentModal(string username)
    {
        try
        {
            selectedStudent = await BookingService.GetStudentBookingsAsync(username);
            showStudentModal = true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching details for student {username}: {ex.Message}");
            selectedStudent = null;
        }
    }

    /// <summary>
    /// Edits the booking.
    /// </summary>
    /// <param name="booking">The booking.</param>
    private void EditBooking(BookingResponseDTO booking)
    {
        selectedBooking = new BookingResponseDTO
        {
            Id = booking.Id,
            StudentUsername = booking.StudentUsername,
            TeacherEmail = booking.TeacherEmail,
            Returned = booking.Returned,
            BookingDateTime = booking.BookingDateTime,
            PlannedReturn = booking.PlannedReturn,
            Comment = booking.Comment,
            Laptop = new LaptopDTO
            {
                Id = booking.Laptop?.Id ?? 0,
                IdentificationNumber = booking.Laptop?.IdentificationNumber,
                Model = booking.Laptop?.Model
            }
        };

        showEditModal = true;
    }

    /// <summary>
    /// Closes the edit modal.
    /// </summary>
    private void CloseEditModal()
    {
        showEditModal = false;
        selectedBooking = null;
    }

    /// <summary>
    /// Deletes the booking.
    /// </summary>
    /// <param name="id">The identifier.</param>
    private async Task DeleteBooking(int id)
    {
        try
        {
            await BookingService.DeleteBookingAsync(id);
            await LoadOpenBookingsAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting booking: {ex.Message}");
        }
        CloseEditModal();
    }

    /// <summary>
    /// Saves the booking changes.
    /// </summary>
    /// <param name="booking">The booking.</param>
    private async Task SaveBookingChanges(BookingResponseDTO booking)
    {
        try
        {
            var requestDto = new BookingRequestDTO
            {
                Id = booking.Id,
                StudentUsername = booking.StudentUsername,
                LaptopIdentificationNumber = booking.Laptop?.IdentificationNumber,
                TeacherEmail = booking.TeacherEmail,
                Returned = booking.Returned,
                BookingDateTime = booking.BookingDateTime,
                PlannedReturn = booking.PlannedReturn,
                Comment = booking.Comment
            };

            await BookingService.UpdateBookingAsync(booking.Id, requestDto);
            await LoadOpenBookingsAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating booking: {ex.Message}");
        }

        CloseEditModal();
    }

    /// <summary>
    /// Opens the return modal.
    /// </summary>
    /// <param name="booking">The booking.</param>
    private void OpenReturnModal(BookingResponseDTO booking)
    {
        selectedBooking = booking;
        showReturnModal = true;
    }

    /// <summary>
    /// Closes the return modal.
    /// </summary>
    private void CloseReturnModal()
    {
        showReturnModal = false;
        selectedBooking = null;
    }

    /// <summary>
    /// Handles the return booking.
    /// </summary>
    /// <param name="booking">The booking.</param>
    private async Task HandleReturnBooking(BookingResponseDTO booking)
    {
        try
        {
            // Update booking to mark as returned
            booking.Returned = true;

            // Update the booking comment
            var requestDto = new BookingRequestDTO
            {
                Id = booking.Id,
                StudentUsername = booking.StudentUsername,
                LaptopIdentificationNumber = booking.Laptop?.IdentificationNumber,
                TeacherEmail = booking.TeacherEmail,
                Returned = booking.Returned,
                BookingDateTime = booking.BookingDateTime,
                PlannedReturn = booking.PlannedReturn,
                Comment = booking.Comment
            };

            await BookingService.UpdateBookingAsync(booking.Id, requestDto);

            // Mark the associated laptop as available
            if (booking.Laptop != null)
            {
                booking.Laptop.IsAvailable = true;
                await LaptopService.UpdateLaptopAsync(booking.Laptop.Id, booking.Laptop);
            }

            // Reload data
            await LoadOpenBookingsAsync();
            await LoadAvailableLaptopsAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error returning booking: {ex.Message}");
        }

        CloseReturnModal();
    }
}


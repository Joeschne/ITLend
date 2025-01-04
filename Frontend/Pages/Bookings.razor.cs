using Microsoft.AspNetCore.Components;
using Frontend.Interfaces;
using Shared.DTOs;

namespace Frontend.Pages;

/// <summary>Component for managing and displaying bookings, with features for searching, filtering, pagination,
/// and editing bookings.</summary>
/// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
public partial class Bookings : ComponentBase
{
    /// <summary>
    /// Gets or sets the booking service.
    /// </summary>
    /// <value>
    /// The booking service.
    /// </value>
    [Inject]
    private IBookingService BookingService { get; set; } // Service to interact with bookings.

    /// <summary>
    /// Gets or sets the laptop service.
    /// </summary>
    /// <value>
    /// The laptop service.
    /// </value>
    [Inject]
    private ILaptopService LaptopService { get; set; } // Service to interact with laptops.

    /// <summary>
    /// The bookings
    /// </summary>
    private IEnumerable<BookingResponseDTO> bookings; // Complete list of bookings.
    /// <summary>
    /// The filtered bookings
    /// </summary>
    private List<BookingResponseDTO> filteredBookings = new(); // Filtered list based on search query.
    /// <summary>
    /// The selected booking
    /// </summary>
    private BookingResponseDTO? selectedBooking; // Currently selected booking for editing.
    /// <summary>
    /// The selected laptop
    /// </summary>
    private LaptopDTO selectedLaptop; // Currently selected laptop for details modal.
    /// <summary>
    /// The selected laptop bookings
    /// </summary>
    private List<BookingResponseDTO> selectedLaptopBookings; // Bookings for the selected laptop.
    /// <summary>
    /// The show edit modal
    /// </summary>
    private bool showEditModal; // State for showing/hiding the edit modal.
    /// <summary>
    /// The show student modal
    /// </summary>
    private bool showStudentModal = false; // State for showing/hiding the student modal.
    /// <summary>
    /// The show laptop modal
    /// </summary>
    private bool showLaptopModal = false; // State for showing/hiding the laptop modal.
    /// <summary>
    /// The selected student detail
    /// </summary>
    private StudentDetailDTO? selectedStudentDetail; // Details of the currently selected student.

    // Pagination Properties
    /// <summary>
    /// The current page
    /// </summary>
    private int currentPage = 1; // Current page number.
    /// <summary>
    /// The total pages
    /// </summary>
    private int totalPages = 1; // Total number of pages.
    /// <summary>
    /// The page size
    /// </summary>
    private const int pageSize = 20; // Number of items per page.
    /// <summary>
    /// Gets or sets the paginated bookings.
    /// </summary>
    /// <value>
    /// The paginated bookings.
    /// </value>
    private List<BookingResponseDTO> PaginatedBookings { get; set; } = new(); // Current page's bookings.

    // Search Properties
    /// <summary>
    /// The search query
    /// </summary>
    private string searchQuery = string.Empty; // Query for filtering bookings.

    /// <summary>
    /// Initializes the component by loading all bookings.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadBookingsAsync();
        FilterBookings(); // Initialize filtered and paginated bookings.
    }

    /// <summary>
    /// Loads all bookings from the service.
    /// </summary>
    private async Task LoadBookingsAsync()
    {
        try
        {
            bookings = await BookingService.GetBookingsAsync();
            filteredBookings = bookings.ToList(); // Initialize filtered list.
            UpdatePaginatedResults();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to load bookings: {ex.Message}");
        }
    }

    /// <summary>
    /// Filters bookings based on the search query.
    /// </summary>
    private void FilterBookings()
    {
        Console.WriteLine($"Filter triggered: searchQuery='{searchQuery}'");

        filteredBookings = bookings
            .Where(b => string.IsNullOrWhiteSpace(searchQuery) ||
                        b.StudentUsername.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
            .ToList();

        currentPage = 1; // Reset to first page.
        UpdatePaginatedResults();
        StateHasChanged(); // Ensure UI updates.
    }

    /// <summary>
    /// Updates the results displayed on the current page based on pagination.
    /// </summary>
    private void UpdatePaginatedResults()
    {
        totalPages = (int)Math.Ceiling((double)filteredBookings.Count / pageSize);
        PaginatedBookings = filteredBookings
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    /// <summary>
    /// Changes the current page and updates the results.
    /// </summary>
    /// <param name="page">The target page number.</param>
    private async Task ChangePage(int page)
    {
        Console.WriteLine($"Changing to page {page}");
        if (page > 0 && page <= totalPages)
        {
            currentPage = page;
            UpdatePaginatedResults(); // Refresh the current page's results.
        }
    }

    /// <summary>
    /// Nexts the page.
    /// </summary>
    private async Task NextPage() => await ChangePage(currentPage + 1);
    /// <summary>
    /// Previouses the page.
    /// </summary>
    private async Task PreviousPage() => await ChangePage(currentPage - 1);

    /// <summary>
    /// Opens the modal displaying details for a student and their bookings.
    /// </summary>
    /// <param name="username">The username of the student.</param>
    private async Task OpenStudentModal(string username)
    {
        try
        {
            selectedStudentDetail = await BookingService.GetStudentBookingsAsync(username);
            showStudentModal = true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching details for student {username}: {ex.Message}");
            selectedStudentDetail = null;
        }
    }

    /// <summary>
    /// Closes the student modal.
    /// </summary>
    private void CloseStudentModal()
    {
        showStudentModal = false;
        selectedStudentDetail = null;
    }

    /// <summary>
    /// Opens the modal displaying details for a laptop and its booking history.
    /// </summary>
    /// <param name="laptop">The laptop to display details for.</param>
    private async Task OpenLaptopModal(LaptopDTO laptop)
    {
        if (laptop == null) return;

        try
        {
            selectedLaptop = laptop;
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
    /// Opens the modal to edit a booking.
    /// </summary>
    /// <param name="booking">The booking to edit.</param>
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
    /// Saves changes made to a booking and updates the corresponding data.
    /// </summary>
    /// <param name="booking">The updated booking details.</param>
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

            if (booking.Laptop != null)
            {
                booking.Laptop.IsAvailable = booking.Returned; // Update availability based on return status.
                await LaptopService.UpdateLaptopAsync(booking.Laptop.Id, booking.Laptop);
            }

            await LoadBookingsAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating booking: {ex.Message}");
        }
        finally
        {
            CloseEditModal();
        }
    }

    /// <summary>
    /// Deletes a booking.
    /// </summary>
    /// <param name="id">The ID of the booking to delete.</param>
    private async Task DeleteBooking(int id)
    {
        try
        {
            await BookingService.DeleteBookingAsync(id);
            CloseEditModal();
            await LoadBookingsAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting booking: {ex.Message}");
        }
    }

    /// <summary>
    /// Closes the edit modal.
    /// </summary>
    private void CloseEditModal()
    {
        showEditModal = false;
        selectedBooking = null;
    }
}

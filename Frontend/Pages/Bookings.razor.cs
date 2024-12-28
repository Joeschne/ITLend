using Microsoft.AspNetCore.Components;
using Frontend.Interfaces;
using Shared.DTOs;

namespace Frontend.Pages;

public partial class Bookings : ComponentBase
{
    [Inject]
    private IBookingService BookingService { get; set; }

    private IEnumerable<BookingResponseDTO> bookings;
    private BookingResponseDTO selectedBooking;
    private bool showEditModal;

    protected override async Task OnInitializedAsync()
    {
        await LoadBookingsAsync();
    }

    private async Task LoadBookingsAsync()
    {
        try
        {
            bookings = await BookingService.GetBookingsAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to load bookings: {ex.Message}");
        }
    }

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

    private void CloseModal()
    {
        showEditModal = false;
        selectedBooking = null;
    }

    private async Task SaveChanges()
    {
        if (selectedBooking == null) return;

        try
        {
            var requestDto = new BookingRequestDTO
            {
                Id = selectedBooking.Id,
                StudentUsername = selectedBooking.StudentUsername,
                LaptopIdentificationNumber = selectedBooking.Laptop?.IdentificationNumber,
                TeacherEmail = selectedBooking.TeacherEmail,
                Returned = selectedBooking.Returned,
                BookingDateTime = selectedBooking.BookingDateTime,
                PlannedReturn = selectedBooking.PlannedReturn,
                Comment = selectedBooking.Comment
            };

            await BookingService.UpdateBookingAsync(selectedBooking.Id, requestDto);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating booking: {ex.Message}");
        }

        showEditModal = false;
        selectedBooking = null;
        await LoadBookingsAsync();
    }

    private void OnStudentClicked(string studentUsername)
    {
        Console.WriteLine($"Student '{studentUsername}' clicked. Navigating to Student page...");
    }
}

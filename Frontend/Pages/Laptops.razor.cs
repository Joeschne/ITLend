using Microsoft.AspNetCore.Components;
using Frontend.Interfaces;
using Shared.DTOs;

namespace Frontend.Pages;

public partial class Laptops : ComponentBase
{
    [Inject]
    private ILaptopService LaptopService { get; set; }

    [Inject]
    private IBookingService BookingService { get; set; }

    private List<LaptopDTO> laptops = new();
    private LaptopDTO selectedLaptop;
    private List<BookingResponseDTO>? selectedLaptopBookings;

    private bool showAddEditModal = false;
    private bool showBookingsModal = false;
    private string modalTitle = string.Empty;
    private bool isAddMode = false;
    private int availableLaptops;

    protected override async Task OnInitializedAsync()
    {
        await LoadLaptopsAsync();
    }

    private async Task LoadLaptopsAsync()
    {
        try
        {
            laptops = await LaptopService.GetLaptopsAsync();
            availableLaptops = laptops.Count(l => l.IsAvailable);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading laptops: {ex.Message}");
        }
    }

    private void OpenAddEditModal(LaptopDTO laptop, bool isAdd)
    {
        selectedLaptop = laptop;
        isAddMode = isAdd;
        modalTitle = isAdd ? "Add Laptop" : "Edit Laptop";
        showAddEditModal = true;
    }

    private void CloseAddEditModal()
    {
        showAddEditModal = false;
        selectedLaptop = null;
    }

    private async Task SaveLaptop()
    {
        try
        {
            if (isAddMode)
            {
                await LaptopService.CreateLaptopAsync(selectedLaptop);
            }
            else
            {
                await LaptopService.UpdateLaptopAsync(selectedLaptop.Id, selectedLaptop);
            }
            await LoadLaptopsAsync();
            CloseAddEditModal();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving laptop: {ex.Message}");
        }
    }

    private async Task DeleteLaptop()
    {
        if (isAddMode) return;

        try
        {
            await LaptopService.DeleteLaptopAsync(selectedLaptop.Id);
            await LoadLaptopsAsync();
            CloseAddEditModal();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting laptop: {ex.Message}");
        }
    }

    private async Task OpenBookingsModal(LaptopDTO laptop)
    {
        selectedLaptop = laptop;
        showBookingsModal = true;

        try
        {
            // Fetch booking history for the selected laptop
            selectedLaptopBookings = (await BookingService.GetBookingsByLaptopAsync(laptop.Id)).ToList();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching bookings for laptop {laptop.Id}: {ex.Message}");
            selectedLaptopBookings = new List<BookingResponseDTO>();
        }
    }

    private void CloseBookingsModal()
    {
        showBookingsModal = false;
        selectedLaptop = null;
        selectedLaptopBookings = null;
    }
}

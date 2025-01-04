using Microsoft.AspNetCore.Components;
using Frontend.Interfaces;
using Shared.DTOs;

namespace Frontend.Pages;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Components.ComponentBase" />
public partial class Laptops : ComponentBase
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
    /// Gets or sets the booking service.
    /// </summary>
    /// <value>
    /// The booking service.
    /// </value>
    [Inject]
    private IBookingService BookingService { get; set; }

    /// <summary>
    /// The laptops
    /// </summary>
    private List<LaptopDTO> laptops = new();
    /// <summary>
    /// The selected laptop
    /// </summary>
    private LaptopDTO selectedLaptop;
    /// <summary>
    /// The selected laptop bookings
    /// </summary>
    private List<BookingResponseDTO>? selectedLaptopBookings;

    /// <summary>
    /// The show add edit modal
    /// </summary>
    private bool showAddEditModal = false;
    /// <summary>
    /// The show bookings modal
    /// </summary>
    private bool showBookingsModal = false;
    /// <summary>
    /// The modal title
    /// </summary>
    private string modalTitle = string.Empty;
    /// <summary>
    /// The is add mode
    /// </summary>
    private bool isAddMode = false;
    /// <summary>
    /// The available laptops
    /// </summary>
    private int availableLaptops;

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// Override this method if you will perform an asynchronous operation and
    /// want the component to refresh when that operation is completed.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadLaptopsAsync();
    }

    /// <summary>
    /// Loads the laptops asynchronous.
    /// </summary>
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

    /// <summary>
    /// Opens the add edit modal.
    /// </summary>
    /// <param name="laptop">The laptop.</param>
    /// <param name="isAdd">if set to <c>true</c> [is add].</param>
    private void OpenAddEditModal(LaptopDTO laptop, bool isAdd)
    {
        selectedLaptop = laptop;
        isAddMode = isAdd;
        modalTitle = isAdd ? "Add Laptop" : "Edit Laptop";
        showAddEditModal = true;
    }

    /// <summary>
    /// Closes the add edit modal.
    /// </summary>
    private void CloseAddEditModal()
    {
        showAddEditModal = false;
        selectedLaptop = null;
    }

    /// <summary>
    /// Saves the laptop.
    /// </summary>
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

    /// <summary>
    /// Deletes the laptop.
    /// </summary>
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

    /// <summary>
    /// Opens the bookings modal.
    /// </summary>
    /// <param name="laptop">The laptop.</param>
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

    /// <summary>
    /// Closes the bookings modal.
    /// </summary>
    private void CloseBookingsModal()
    {
        showBookingsModal = false;
        selectedLaptop = null;
        selectedLaptopBookings = null;
    }
}

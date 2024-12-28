using Microsoft.AspNetCore.Components;
using Frontend.Interfaces;
using Shared.DTOs;

namespace Frontend.Pages;

public partial class Laptops : ComponentBase
{
    [Inject]
    private ILaptopService LaptopService { get; set; }

    private List<LaptopDTO> laptops = new();
    private LaptopDTO selectedLaptop;
    private bool showModal = false;
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

    private void AddLaptop()
    {
        selectedLaptop = new LaptopDTO
        {
            Id = 0,
            IsAvailable = true,
            Model = string.Empty,
            IdentificationNumber = string.Empty,
            DamageDescription = string.Empty
        };
        isAddMode = true;
        modalTitle = "Add Laptop";
        showModal = true;
    }

    private void EditLaptop(LaptopDTO laptop)
    {
        selectedLaptop = new LaptopDTO
        {
            Id = laptop.Id,
            IsAvailable = laptop.IsAvailable,
            Model = laptop.Model,
            IdentificationNumber = laptop.IdentificationNumber,
            DamageDescription = laptop.DamageDescription
        };
        isAddMode = false;
        modalTitle = "Edit Laptop";
        showModal = true;
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
            CloseModal();
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
            CloseModal();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting laptop: {ex.Message}");
        }
    }

    private void CloseModal()
    {
        showModal = false;
        selectedLaptop = null;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWeb.Models
{
public class Car
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Car brand is required")]
    [Display(Name = "Car Brand (Marca)")]
    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model is required")]
    [Display(Name = "Model")]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;

    [Display(Name = "Photo")]
    public string? PhotoPath { get; set; }

    [Required(ErrorMessage = "Mileage is required")]
    [Display(Name = "Mileage (KM)")]
    [Range(0, int.MaxValue, ErrorMessage = "Mileage must be a positive number")]
    public int Mileage { get; set; }

    [Required(ErrorMessage = "Year is required")]
    [Display(Name = "Year")]
    [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030")]
    public int Year { get; set; }

    [Required]
    public string OwnerId { get; set; } = string.Empty;

    [ForeignKey("OwnerId")]
    public virtual ApplicationUser Owner { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
}
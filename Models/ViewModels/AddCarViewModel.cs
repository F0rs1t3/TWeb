using System.ComponentModel.DataAnnotations;

namespace TWeb.Models.ViewModels
{
    public class AddCarViewModel
    {
        [Required(ErrorMessage = "Car brand is required")]
        [Display(Name = "Car Brand")]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required")]
        [Display(Name = "Model")]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Display(Name = "Photo")]
        public IFormFile? Photo { get; set; }

        [Required(ErrorMessage = "Mileage is required")]
        [Display(Name = "Mileage (KM)")]
        [Range(0, int.MaxValue, ErrorMessage = "Mileage must be a positive number")]
        public int Mileage { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Display(Name = "Year")]
        [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030")]
        public int Year { get; set; }
    }
}
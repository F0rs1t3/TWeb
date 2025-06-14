using Microsoft.AspNetCore.Mvc;
using TWeb.Services.Interfaces;

namespace TWeb.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsApiController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly ILogger<CarsApiController> _logger;

        public CarsApiController(ICarService carService, ILogger<CarsApiController> logger)
        {
            _carService = carService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCars([FromQuery] string? search, [FromQuery] string? brand, 
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            try
            {
                var cars = await _carService.GetCarsForSaleAsync(search, brand, minPrice, maxPrice);
                return Ok(cars);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while fetching cars");
            }
        }

        [HttpGet("rental")]
        public async Task<IActionResult> GetRentalCars([FromQuery] string? search, [FromQuery] string? brand, 
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            try
            {
                var cars = await _carService.GetCarsForRentalAsync(search, brand, minPrice, maxPrice);
                return Ok(cars);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while fetching rental cars");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar(int id)
        {
            try
            {
                var car = await _carService.GetCarByIdAsync(id);
                if (car == null)
                {
                    return NotFound();
                }
                return Ok(car);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while fetching car details");
            }
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands()
        {
            try
            {
                var brands = await _carService.GetAvailableBrandsAsync();
                return Ok(brands);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while fetching brands");
            }
        }

        [HttpGet("rental/brands")]
        public async Task<IActionResult> GetRentalBrands()
        {
            try
            {
                var brands = await _carService.GetRentalBrandsAsync();
                return Ok(brands);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while fetching rental brands");
            }
        }
    }
}
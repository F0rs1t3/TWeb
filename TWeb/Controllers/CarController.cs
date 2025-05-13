using AutoMapper;
using BusinessLogic;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TWeb.Controllers
{
    public class CarController : Controller
    {
        private readonly IMapper _mapper;
        private readonly CarService _carService;

        public CarController(IMapper mapper)
        {
            _mapper = mapper;
            _carService = new CarService();
        }

        public IActionResult Index()
        {
            var brands = _carService.GetAllCarBrands();
            var cars = _carService.GetAllCars(); 
            var carDTOs = _mapper.Map<List<CarDTO>>(cars); 

            return View(brands);
        }
    }
}

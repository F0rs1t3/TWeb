using BusinessLogic.Models;

namespace BusinessLogic
{
    public class CarService
    {
        public List<string> GetAllCarBrands()
        {
            return new List<string> { "Audi", "BMW", "Ferrari", "Ford", "Toyota", "Mercedes" };
        }

        private List<Car> _cars = new List<Car>
    {
        new Car { Id = 1, Brand = "Toyota", Model = "Corolla", Year = 2020, Price = 20000 },
        new Car { Id = 2, Brand = "Ford", Model = "Focus", Year = 2019, Price = 18000 },
        new Car { Id = 3, Brand = "BMW", Model = "X5", Year = 2021, Price = 50000 }
    };
        public List<Car> GetAllCars()
        {
            return _cars;
        }
    }
}

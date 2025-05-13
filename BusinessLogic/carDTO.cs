using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class CarDTO
    {
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CarRepository
    {
        public List<string> GetCarBrandsFromDatabase()
        {
            return new List<string> { "Audi", "BMW", "Mercedes" }; // simulare la db
        }
    }

}

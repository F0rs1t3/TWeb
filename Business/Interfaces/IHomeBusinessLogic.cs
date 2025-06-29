using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TWeb.Business.Interfaces
{
    public interface IHomeBusinessLogic
    {
        Task<JsonResult> CreateAdminAsync();
    }
}

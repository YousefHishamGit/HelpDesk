using HelpDesk.Application.DTOs.Categories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto?> GetByIdAsync(int id);
    }
}


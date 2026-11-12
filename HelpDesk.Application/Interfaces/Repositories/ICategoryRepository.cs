using HelpDesk.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id);
        Task<List<Category>> GetAllAsync();
    }
}

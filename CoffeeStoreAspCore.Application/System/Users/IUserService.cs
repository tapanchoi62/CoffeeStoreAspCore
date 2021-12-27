using CoffeeStoreAspCore.ViewModels.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeStoreAspCore.Application.System.Users
{
   public interface IUserService
    {
        Task<string> Authencate(LoginRequest request);

        Task<bool> Register(RegisterRequest request);
        Task<bool> Update(Guid id, UserUpdate request);

        Task<UserViewModel> GetByName(string name);
        Task<UserViewModel> GetById(Guid id);
    }
}

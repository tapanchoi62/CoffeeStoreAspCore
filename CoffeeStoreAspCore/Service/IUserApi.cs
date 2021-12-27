using CoffeeStoreAspCore.ViewModels.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeStoreAspCore.Service
{
    public interface IUserApi
    {
        Task<string> Authenticate(LoginRequest request);

    }
}

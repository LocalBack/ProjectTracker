﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IUserProjectSyncService
    {
        Task EnsureUserProjectsSyncedAsync(int userId);
    }
}

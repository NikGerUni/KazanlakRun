﻿using KazanlakRun.Web.Areas.User.Models;

namespace KazanlakRun.Web.Areas.User.Services
{
    public interface IVolunteerService
    {
        Task CreateAsync(string userId, VolunteerInputModel model);
        Task<bool> ExistsAsync(string userId);
        Task<VolunteerInputModel?> GetByUserIdAsync(string userId);
        Task UpdateAsync(string userId, VolunteerInputModel model);
        Task DeleteAsync(string userId);
    }
}


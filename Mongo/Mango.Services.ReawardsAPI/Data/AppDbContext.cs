﻿using Mango.Services.RewardsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.RewardsAPI
{
    public class AppDbContext: DbContext
    {
        //add sxolia
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {
                    
        }
        public DbSet<Rewards> Rewards { get; set; }

    }
}

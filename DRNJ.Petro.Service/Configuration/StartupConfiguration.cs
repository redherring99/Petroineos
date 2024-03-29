﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRNJ.Petro.Service.Configuration
{
    /// <summary>
    /// Get Configuration sections from appsettings in a structured manner
    /// </summary>
    public static class StartupConfiguration
    {

        public static IConfigurationSection GetLoggerConfiguration(this IConfiguration config)
        {
            return config.GetSection("Logging");
        }

        public static T Get<T>(this IConfiguration config, string name)
        {
            return config
                 .GetSection(name)
                 .Get<T>();
        }
    }
}

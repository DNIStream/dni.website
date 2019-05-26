﻿using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DNI.Testing {
    /// <summary>
    ///     Contains static helper methods for testing
    /// </summary>
    public static class TestHelpers {
        /// <summary>
        ///     Retrieves a configuration file for the current environment
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetConfigFromFile() {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                .Build();
            return config;
        }

        /// <summary>
        ///     Creates Options at the specified path, from the specified configuration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static IOptions<T> CreateOptions<T>(this IConfiguration config, string configPath)
            where T: class, new() {
            var options = new T();
            config.Bind(configPath, options);
            return Options.Create(options);
        }
    }
}
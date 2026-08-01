using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ODExplorer.Extensions
{
    // Host builder extensions are host/UI facing; for core library provide no-op implementations so the file compiles
    public static class HostBuilderExtentions
    {
        // Database registration is host-specific. UI/host should provide the real registrations.
        public static IHostBuilder AddDatabase(this IHostBuilder hostBuilder, string connectionString)
        {
            // No-op in core library.
            return hostBuilder;
        }

        public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder)
        {
            // ViewModels are UI-specific; host should register them. Keep method for API compatibility.
            return hostBuilder;
        }

        public static IHostBuilder AddWindows(this IHostBuilder hostBuilder)
        {
            // No-op in core library.
            return hostBuilder;
        }

        public static IHostBuilder AddStores(this IHostBuilder hostBuilder)
        {
            // Core stores should be registered by host; leave as no-op.
            return hostBuilder;
        }

        public static IHostBuilder AddServices(this IHostBuilder hostBuilder)
        {
            return hostBuilder;
        }

        public static IHostBuilder AddNavigation(this IHostBuilder hostBuilder)
        {
            return hostBuilder;
        }

        public static IHostBuilder AddHttpClients(this IHostBuilder hostBuilder)
        {
            return hostBuilder;
        }
    }
}

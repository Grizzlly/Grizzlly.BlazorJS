using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace Grizzlly.BlazorJS
{
    public static class WebAssemblyHostExtensions
    {
        internal const string ProjectId = "Grizzlly.BlazorJS";
        public static async Task UseJSComponents(this WebAssemblyHost host)
        {
            if (OperatingSystem.IsBrowser())
            {
                await JSHost.ImportAsync(ProjectId, $"../_content/Grizzlly.BlazorJS.MSBuild/{ProjectId}.js");
            }
        }

        public static async Task UseJSComponents(this WebAssemblyHost host, string customPath)
        {
            if (OperatingSystem.IsBrowser())
            {
                await JSHost.ImportAsync(ProjectId, customPath);
            }
        }
    }
}

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
        public static async Task UseJSComponents(this WebAssemblyHost host)
        {
            if (OperatingSystem.IsBrowser())
            {
                await JSHost.ImportAsync("vuez", "../_content/Grizzlly.BlazorJS.MSBuild/vuez.js");
            }
        }
    }
}

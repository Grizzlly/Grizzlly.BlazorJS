using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace Grizzlly.BlazorJS
{
    [SupportedOSPlatform("browser")]
    public sealed partial class VueComponent : ComponentBase, IDisposable
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; } = default!;

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? InputAttributes { get; set; }

        [Parameter]
        public Emit[] Emits { get; set; } = default!;

        [Parameter]
        public string As { get; set; } = "template";

        [JSImport("attachVueComponent", WebAssemblyHostExtensions.ProjectId)]
        [return: JSMarshalAs<JSType.Promise<JSType.Object>>]
        internal static partial Task<JSObject> AttachVueComponent(string elementid, string modulename,
                string[] attributeKeys, [JSMarshalAs<JSType.Array<JSType.Any>>] object[] attributeValues,
                [JSMarshalAs<JSType.Function<JSType.String, JSType.Any>>] Action<string, object> provide);

        [Parameter, EditorRequired]
        public string ComponentName { get; set; } = default!;

        ElementReference elRef;
#nullable disable
        JSObject vueInstance = null;
        JSObject componentInstance = null;

        private ElementReference contentRef;
#nullable enable
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                JSObject? ret = await AttachVueComponent(elRef.Id, ComponentName, InputAttributes?.Keys.ToArray() ?? Enumerable.Empty<string>().ToArray(), InputAttributes?.Values.ToArray() ?? Enumerable.Empty<object>().ToArray(), Provide);
                if (ret is null)
                {
                    Console.WriteLine("wrong component");
                }
                else
                {
                    vueInstance = ret;
                    componentInstance = ret.GetPropertyAsJSObject("$")?.GetPropertyAsJSObject("proxy");
                }
            }

            if (!firstRender && (vueInstance is not null))
            {

            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenElement(0, As);
            builder.AddElementReferenceCapture(1, val => elRef = val);
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }

        private void Provide(string name, object? data)
        {
            var emit = Emits.FirstOrDefault(e => e.Name == name);
            emit?.Func(data);
        }

        public void Dispose()
        {
            vueInstance?.Dispose();
            componentInstance?.Dispose();
        }
    }
}

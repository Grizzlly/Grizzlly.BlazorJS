using Grizzlly.BlazorJS;

namespace BlazorWASM.Pages
{
    public partial class Index
    {
        Emit[] emits;

        // It is ok to use constructor in this case
        public Index()
        {
            emits = new Emit[] {
                new("click", SayHi)
            };
        }
        private void SayHi(object? _)
        {
            Console.WriteLine("Hello from Vue + Blazor!");
        }
    }
}

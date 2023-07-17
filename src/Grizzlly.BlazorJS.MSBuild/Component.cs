using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Grizzlly.BlazorJS.MSBuild
{
    public class Component : Microsoft.Build.Utilities.Task
    {
        public string Components { get; set; }

        [Required]
        public string ComponentsOut { get; set; }

        [Required]
        public ITaskItem[] VueComponents { get; set; }

        [Output]
        public string ComponentsFileName { get; set; }

        [Output]
        public string PackagesList { get; set; } = string.Empty;

        public override bool Execute()
        {
            var (success, components) = GetJSComponents();
            if (!success)
            {
                return !Log.HasLoggedErrors;
            }

            success = CreateCompononentsTS(components);

            return !Log.HasLoggedErrors;
        }

        private bool CreateCompononentsTS(Dictionary<string, string[]> components)
        {
            ComponentsFileName = $"{ComponentsOut}.ts";
            try
            {
                using (StreamWriter writer = new StreamWriter(ComponentsFileName, false))
                {
                    foreach (var component in components)
                    {
                        string packageName = component.Key;
                        bool isLocalComponent = packageName.EndsWith(".vue");

                        if (isLocalComponent)
                        {
                            packageName = $"./{packageName.Replace(Path.DirectorySeparatorChar, '/')}";
                        }
                        else
                        {
                            PackagesList += $"{packageName} ";
                        }

                        string[] componentsArr = component.Value;

                        writer.Write("import ");

                        if (!isLocalComponent)
                        {
                            writer.Write("{ ");
                            foreach (string c in componentsArr)
                            {
                                writer.Write($"{c}, ");
                            }
                            writer.Write("} ");
                        }
                        else
                        {
                            writer.Write($"{componentsArr[0]} ");
                        }

                        writer.WriteLine($"from \"{packageName}\";");

                        writer.Write("export { ");

                        foreach (string c in componentsArr)
                        {
                            writer.Write($"{c}, ");
                        }

                        writer.WriteLine("};");
                    }
                }

                Log.LogMessage($"{components.Count} components to {ComponentsFileName}");
            }
            catch (Exception ex)
            {
                // This logging helper method is designed to capture and display information
                // from arbitrary exceptions in a standard way.
                Log.LogErrorFromException(ex, showStackTrace: true);
                return false;
            }

            return true;
        }

        private (bool, Dictionary<string, string[]>) GetJSComponents()
        {
            if (Components == string.Empty)
            {
                return (true, new());
            }

            var values = new Dictionary<string, string[]>();
            var imports = Components.Split(';');
            foreach (var import in imports)
            {
                string[] trimmed = import.Trim('{', '}', ' ').Split('~');
                if (trimmed.Length < 2)
                {
                    Log.LogError(subcategory: null,
                             errorCode: "BJS0001",
                             helpKeyword: null,
                             file: null,
                             lineNumber: 0,
                             columnNumber: 0,
                             endLineNumber: 0,
                             endColumnNumber: 0,
                             message: "Specify correct component name and package " + Components);
                    return (false, null);
                }

                string[] componentsArr = trimmed[0].Trim('[', ']', ' ').Split(',');
                string package = trimmed[1].Trim();

                if (string.IsNullOrWhiteSpace(package) || componentsArr.Length == 0)
                {
                    Log.LogError(subcategory: null,
                             errorCode: "BJS0001",
                             helpKeyword: null,
                             file: null,
                             lineNumber: 0,
                             columnNumber: 0,
                             endLineNumber: 0,
                             endColumnNumber: 0,
                             message: "Specify correct component name and package " + Components);
                    return (false, null);
                }

                values[package] = componentsArr;
            }

            foreach (ITaskItem localComp in VueComponents)
            {
                values[localComp.ItemSpec] = new string[] { Path.GetFileNameWithoutExtension(localComp.ItemSpec) };
            }

            return (true, values);
        }
    }
}

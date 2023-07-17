using Microsoft.Build.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Grizzlly.BlazorJS.MSBuild.Test
{
    public sealed class ComponentTest
    {
#nullable disable
        private Mock<IBuildEngine> buildEngine;
        private List<BuildErrorEventArgs> errors;
#nullable enable

        [TestInitialize]
        public void Startup()
        {
            buildEngine = new();
            errors = new();
            buildEngine.Setup(x => x.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()))
                .Callback<BuildErrorEventArgs>(errors.Add);
        }

        [TestMethod]
        public void EmptyComponentsList_EmptyFileGenerated()
        {
            // Arrange
            var component = new Component
            {
                Components = string.Empty,
                ComponentsOut = $"components",
                VueComponents = Array.Empty<ITaskItem>(),

                BuildEngine = buildEngine.Object
            };


            // Act
            var success = component.Execute();


            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(0, errors.Count);
            Assert.AreEqual("components.ts", component.ComponentsFileName);
            Assert.AreEqual(string.Empty, component.PackagesList);
            Assert.AreEqual(true, File.Exists(component.ComponentsFileName));
            Assert.AreEqual(true, string.IsNullOrWhiteSpace(File.ReadAllText(component.ComponentsFileName)));

            // Cleanup
            File.Delete(component.ComponentsFileName);
        }

        [TestMethod]
        public void ComponentsList_FileGenerated()
        {
            // Arrange
            var component = new Component
            {
                Components = "{[f1]~p1};{[f2,f3]~p2};{[f4,   f5, f6] ~ p3 }",
                ComponentsOut = $"components",
                VueComponents = Array.Empty<ITaskItem>(),

                BuildEngine = buildEngine.Object
            };


            // Act
            var success = component.Execute();


            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(0, errors.Count);
            Assert.AreEqual("components.ts", component.ComponentsFileName);
            Assert.AreEqual("p1 p2 p3", component.PackagesList.Trim());
            Assert.AreEqual(true, File.Exists(component.ComponentsFileName));
            Assert.AreEqual(false, string.IsNullOrWhiteSpace(File.ReadAllText(component.ComponentsFileName)));

            Console.WriteLine(File.ReadAllText(component.ComponentsFileName));

            // Cleanup
            File.Delete(component.ComponentsFileName);
        }
    }
}
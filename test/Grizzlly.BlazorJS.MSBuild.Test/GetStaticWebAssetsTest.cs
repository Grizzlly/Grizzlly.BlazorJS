using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grizzlly.BlazorJS.MSBuild.Test
{
    [TestClass]
    public sealed class GetStaticWebAssetsTest
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
        public void EmptyCandidates_NoStaticWebAssetsGenerated()
        {
            // Arange
            var getStaticAssets = new GetStaticWebAssets
            {
                Candidates = Array.Empty<ITaskItem>(),
                Pattern = "**/*",
                SourceId = "Grizzlly.BlazorJS.MSBuild",
                ContentRoot = string.Empty,
                BasePath = "_content/Grizzlly.BlazorJS.MSBuild",

                BuildEngine = buildEngine.Object,
            };

            // Act
            var success = getStaticAssets.Execute();

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(0, errors.Count);
            Assert.AreEqual(0, getStaticAssets.DiscoveredStaticWebAssets.Length);

            // Cleanup
        }

        [TestMethod]
        public void CandidatesEmptyContentRoot_StaticWebAssetsGenerated()
        {
            // Arange
            var item = new Mock<ITaskItem>();
            item.Setup(x => x.GetMetadata("RelativePath")).Returns("file1.txt");
            item.Setup(x => x.GetMetadata("FullPath")).Returns("file1.txt");

            var getStaticAssets = new GetStaticWebAssets
            {
                Candidates = new[] { item.Object },
                Pattern = "**/*",
                SourceId = "Grizzlly.BlazorJS.MSBuild",
                ContentRoot = string.Empty,
                BasePath = "_content/Grizzlly.BlazorJS.MSBuild",

                BuildEngine = buildEngine.Object,
            };

            // Act
            var success = getStaticAssets.Execute();

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(0, errors.Count);
            Assert.AreEqual(1, getStaticAssets.DiscoveredStaticWebAssets.Length);
            Assert.AreEqual("file1.txt", getStaticAssets.DiscoveredStaticWebAssets[0].ItemSpec);

            // Cleanup
        }
    }
}

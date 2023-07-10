using Microsoft.Build.Framework;
using Microsoft.Extensions.FileSystemGlobbing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Grizzlly.BlazorJS.MSBuild
{
    public class GetStaticWebAssets : Microsoft.Build.Utilities.Task
    {
        [Required]
        public ITaskItem[] Candidates { get; set; }

        [Required]
        public string Pattern { get; set; }

        [Required]
        public string SourceId { get; set; }

        [Required]
        public string ContentRoot { get; set; }

        [Required]
        public string BasePath { get; set; }

        public string AssetMergeSource { get; set; }

        [Output]
        public ITaskItem[] DiscoveredStaticWebAssets { get; set; }

        public override bool Execute()
        {
            try
            {
                var matcher = new Matcher().AddInclude(Pattern);
                var assets = new List<ITaskItem>();
                var assetsByRelativePath = new Dictionary<string, List<ITaskItem>>();

                for (var i = 0; i < Candidates.Length; i++)
                {
                    var candidate = Candidates[i];
                    var candidateMatchPath = GetCandidateMatchPath(candidate);
                    var candidateRelativePath = candidateMatchPath;
                    if (string.IsNullOrEmpty(candidate.GetMetadata("RelativePath")))
                    {
                        var match = matcher.Match(ContentRoot, candidateRelativePath);
                        if (!match.HasMatches)
                        {
                            Log.LogMessage(MessageImportance.Low, "Rejected asset '{0}' for pattern '{1}'", candidateMatchPath, Pattern);
                            continue;
                        }

                        Log.LogMessage(MessageImportance.Low, "Accepted asset '{0}' for pattern '{1}' with relative path '{2}'", candidateMatchPath, Pattern, match.Files.Single().Stem);

                        candidateRelativePath = Asset.Normalize(match.Files.Single().Stem);
                    }

                    var asset = new Asset
                    {
                        Identity = candidate.GetMetadata("FullPath"),
                        SourceId = SourceId,
                        SourceType = Asset.SourceTypes.Package,
                        ContentRoot = ContentRoot,
                        BasePath = BasePath,
                        RelativePath = candidateRelativePath,
                        AssetMode = Asset.AssetModes.All,
                        AssetMergeSource = AssetMergeSource,
                        AssetMergeBehavior = Asset.MergeBehaviors.PreferTarget,
                        CopyToOutputDirectory = candidate.GetMetadata(nameof(Asset.CopyToOutputDirectory)),
                        CopyToPublishDirectory = candidate.GetMetadata(nameof(Asset.CopyToPublishDirectory))
                    };

                    asset.ApplyDefaults();
                    asset.Normalize();

                    var assetItem = asset.ToTaskItem();

                    assetItem.SetMetadata("OriginalItemSpec", candidate.ItemSpec);
                    assets.Add(assetItem);

                    if (Log.HasLoggedErrors)
                    {
                        return false;
                    }
                }

                DiscoveredStaticWebAssets = assets.ToArray();
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, showStackTrace: true, showDetail: true, file: null);
            }

            return !Log.HasLoggedErrors;
        }

        private string GetCandidateMatchPath(ITaskItem candidate)
        {
            var computedPath = Asset.ComputeAssetRelativePath(candidate, out var property);
            if (property != null)
            {
                Log.LogMessage(
                    MessageImportance.Low,
                    "{0} '{1}' found for candidate '{2}' and will be used for matching.",
                    property,
                    computedPath,
                    candidate.ItemSpec);
            }

            return computedPath;
        }

    }
}

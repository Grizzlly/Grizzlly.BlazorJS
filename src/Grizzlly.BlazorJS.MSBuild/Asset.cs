using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Grizzlly.BlazorJS.MSBuild
{
    public class Asset
    {
        public string Identity { get; set; }

        public string SourceId { get; set; }

        public string SourceType { get; set; }

        public string ContentRoot { get; set; }

        public string BasePath { get; set; }

        public string RelativePath { get; set; }

        public string AssetKind { get; set; }

        public string AssetMode { get; set; }

        public string AssetRole { get; set; }

        public string AssetMergeBehavior { get; set; }

        public string AssetMergeSource { get; set; }

        public string RelatedAsset { get; set; }

        public string AssetTraitName { get; set; }

        public string AssetTraitValue { get; set; }

        public string CopyToOutputDirectory { get; set; }

        public string CopyToPublishDirectory { get; set; }

        public string OriginalItemSpec { get; set; }

        //public static Asset FromTaskItem(ITaskItem item)
        //{
        //    var result = FromTaskItemCore(item);

        //    result.Normalize();
        //    result.Validate();

        //    return result;
        //}

        public static string Normalize(string path, bool allowEmpyPath = false)
        {
            var normalizedPath = path.Replace('\\', '/').Trim('/');
            return !allowEmpyPath && normalizedPath.Equals("") ? "/" : normalizedPath;
        }

        public void Normalize()
        {
            ContentRoot = !string.IsNullOrEmpty(ContentRoot) ? NormalizeContentRootPath(ContentRoot) : ContentRoot;
            BasePath = Normalize(BasePath);
            RelativePath = Normalize(RelativePath, allowEmpyPath: true);
            RelatedAsset = !string.IsNullOrEmpty(RelatedAsset) ? Path.GetFullPath(RelatedAsset) : RelatedAsset;
        }

        public static string NormalizeContentRootPath(string path)
            => Path.GetFullPath(path) +
            // We need to do .ToString because there is no EndsWith overload for chars in .net472
            (path.EndsWith(Path.DirectorySeparatorChar.ToString()), path.EndsWith(Path.AltDirectorySeparatorChar.ToString())) switch
            {
                (true, _) => "",
                (false, true) => "", // Path.GetFullPath will have normalized it to Path.DirectorySeparatorChar.
                (false, false) => Path.DirectorySeparatorChar
            };

        public void ApplyDefaults()
        {
            CopyToOutputDirectory = string.IsNullOrEmpty(CopyToOutputDirectory) ? AssetCopyOptions.Never : CopyToOutputDirectory;
            CopyToPublishDirectory = string.IsNullOrEmpty(CopyToPublishDirectory) ? AssetCopyOptions.PreserveNewest : CopyToPublishDirectory;
            AssetKind = !string.IsNullOrEmpty(AssetKind) ? AssetKind : !ShouldCopyToPublishDirectory() ? AssetKinds.Build : AssetKinds.All;
            AssetMode = string.IsNullOrEmpty(AssetMode) ? AssetModes.All : AssetMode;
            AssetRole = string.IsNullOrEmpty(AssetRole) ? AssetRoles.Primary : AssetRole;
        }

        public static class AssetModes
        {
            public const string CurrentProject = nameof(CurrentProject);
            public const string Reference = nameof(Reference);
            public const string All = nameof(All);
        }

        public static class AssetKinds
        {
            public const string Build = nameof(Build);
            public const string Publish = nameof(Publish);
            public const string All = nameof(All);

            public static bool IsPublish(string assetKind) => string.Equals(Publish, assetKind, StringComparison.Ordinal);
            public static bool IsBuild(string assetKind) => string.Equals(Build, assetKind, StringComparison.Ordinal);
            internal static bool IsKind(string candidate, string assetKind) => string.Equals(candidate, assetKind, StringComparison.Ordinal);
            internal static bool IsAll(string assetKind) => string.Equals(All, assetKind, StringComparison.Ordinal);
        }

        public static class AssetRoles
        {
            public const string Primary = nameof(Primary);
            public const string Related = nameof(Related);
            public const string Alternative = nameof(Alternative);

            internal static bool IsPrimary(string assetRole)
                => string.Equals(assetRole, Primary, StringComparison.Ordinal);
        }

        public static class MergeBehaviors
        {
            public const string Exclude = nameof(Exclude);
            public const string PreferTarget = nameof(PreferTarget);
            public const string PreferSource = nameof(PreferSource);
            public const string None = nameof(None);
        }

        public static class SourceTypes
        {
            public const string Discovered = nameof(Discovered);
            public const string Computed = nameof(Computed);
            public const string Project = nameof(Project);
            public const string Package = nameof(Package);

            public static bool IsPackage(string sourceType) => string.Equals(Package, sourceType, StringComparison.Ordinal);
        }

        public static class AssetCopyOptions
        {
            public const string Never = nameof(Never);
            public const string PreserveNewest = nameof(PreserveNewest);
            public const string Always = nameof(Always);
        }

        public bool ShouldCopyToPublishDirectory()
            => !string.Equals(CopyToPublishDirectory, AssetCopyOptions.Never, StringComparison.Ordinal);

        public static string ComputeAssetRelativePath(ITaskItem asset, out string metadataProperty)
        {
            var relativePath = asset.GetMetadata("RelativePath");
            if (!string.IsNullOrEmpty(relativePath))
            {
                metadataProperty = "RelativePath";
                return relativePath;
            }

            var targetPath = asset.GetMetadata("TargetPath");
            if (!string.IsNullOrEmpty(targetPath))
            {
                metadataProperty = "TargetPath";
                return targetPath;
            }

            var linkPath = asset.GetMetadata("Link");
            if (!string.IsNullOrEmpty(linkPath))
            {
                metadataProperty = "Link";
                return linkPath;
            }

            metadataProperty = null;
            return asset.ItemSpec;
        }

        public ITaskItem ToTaskItem()
        {
            var result = new TaskItem(Identity);
            result.SetMetadata(nameof(SourceType), SourceType);
            result.SetMetadata(nameof(SourceId), SourceId);
            result.SetMetadata(nameof(ContentRoot), ContentRoot);
            result.SetMetadata(nameof(BasePath), BasePath);
            result.SetMetadata(nameof(RelativePath), RelativePath);
            result.SetMetadata(nameof(AssetKind), AssetKind);
            result.SetMetadata(nameof(AssetMode), AssetMode);
            result.SetMetadata(nameof(AssetRole), AssetRole);
            result.SetMetadata(nameof(AssetMergeSource), AssetMergeSource);
            result.SetMetadata(nameof(AssetMergeBehavior), AssetMergeBehavior);
            result.SetMetadata(nameof(RelatedAsset), RelatedAsset);
            result.SetMetadata(nameof(AssetTraitName), AssetTraitName);
            result.SetMetadata(nameof(AssetTraitValue), AssetTraitValue);
            result.SetMetadata(nameof(CopyToOutputDirectory), CopyToOutputDirectory);
            result.SetMetadata(nameof(CopyToPublishDirectory), CopyToPublishDirectory);
            result.SetMetadata(nameof(OriginalItemSpec), OriginalItemSpec);
            return result;
        }

    }
}

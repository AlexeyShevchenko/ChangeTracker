namespace ChangeTracker.Pipelines.CreatePackage
{
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Diagnostics;
    using Sitecore.Install;
    using Sitecore.Install.Framework;
    using Sitecore.Install.Items;
    using Sitecore.Install.Zip;
    using System.IO;
    using System.Linq;

    public class SaveZipInFileSystem
    {
        private CreatePackageArgs Args { get; set; }

        public void Process(CreatePackageArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.LastFinishedTaskItem, "args.LastFinishedTaskItem");
            Assert.ArgumentNotNull(args.ItemsForPackage, "args.ItemsForPackage");

            if (!args.ItemsForPackage.Any()) { args.AbortPipeline(); }

            Args = args;

            GeneratePackage();
        }

        private void GeneratePackage()
        {
            var packageProject = new PackageProject
            {
                Metadata =
                {
                    PackageName = Args.LastFinishedTaskItem.Name,
                    Author = Context.User.Name,
                    Version = string.Empty,
                    Publisher = string.Empty
                },
                SaveProject = true
            };

            var packageItemSource = new ExplicitItemSource();
            foreach (var item in Args.ItemsForPackage)
            {
                var itemUri = new ItemUri(item);
                packageItemSource.Entries.Add(new ItemReference(itemUri, false).ToString());
            }
            var sourceCollection = new SourceCollection<PackageEntry>();
            sourceCollection.Add(packageItemSource);

            if (sourceCollection.Sources.Count > 0 || packageItemSource.Entries.Count > 0) { packageProject.Sources.Add(sourceCollection); }

            var packagesFolder = Path.GetTempPath();
            var pathToPackage = string.Format("{0}\\{1}.zip", packagesFolder, Args.LastFinishedTaskItem.Name);

            Args.FilePath = pathToPackage;

            using (var writer = new PackageWriter(MainUtil.MapPath(pathToPackage)))
            {
                Context.SetActiveSite("shell");
                writer.Initialize(Installer.CreateInstallationContext());
                PackageGenerator.GeneratePackage(packageProject, writer);
                Context.SetActiveSite("website");
            }
        }
    }
}
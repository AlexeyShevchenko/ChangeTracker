namespace ChangeTracker.Commands
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Install;
    using Sitecore.Install.Framework;
    using Sitecore.Install.Items;
    using Sitecore.Install.Zip;
    using Sitecore.Shell.Framework.Commands;

    public class GenerateZip : Command
    {
        public override void Execute(CommandContext context)
        {
            var lastFinishedTask = TrackerUtil.LastFinishedTaskItem;
            var taskStartTime = lastFinishedTask[FieldIDs.Created];
            var taskEndTime = lastFinishedTask[ChangeTracker.Constants.Templates.Task.Fields.TaskEndDate];

            var masterDatabase = Factory.GetDatabase("master");
            var query = string.Format("fast:/sitecore//*[@__Updated > '{0}' and @__Updated < '{1}' and @@templateid != '{2}']", taskStartTime, taskEndTime, ChangeTracker.Constants.Templates.Task.ID);
            var items = masterDatabase.SelectItems(query);

            this.GeneratePackage(items, lastFinishedTask.Name);
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (TrackerUtil.LastFinishedTaskItem != null && !TrackerUtil.IsCurrentTaskInProcess)
            {
                return CommandState.Enabled;
            }
            return CommandState.Hidden;
        }

        private void GeneratePackage(Item[] items, string packageName)
        {
            var packageProject = new PackageProject
            {
                Metadata =
                {
                    PackageName = "Specify the package name",
                    Author = "Specify the author name",
                    Version = "Specify the version",
                    Publisher = "Specify the Publisher"
                },
                SaveProject = true
            };
            var sourceCollection = new SourceCollection<PackageEntry>();
            var packageItemSource = new ExplicitItemSource()
            {
                Name = "Item Source Name"
            };

            foreach (var item in items)
            {
                var itemUri = new ItemUri(item);
                packageItemSource.Entries.Add(new ItemReference(itemUri, false).ToString());
            }
            sourceCollection.Add(packageItemSource);

            if (packageItemSource.Entries.Count > 0 || sourceCollection.Sources.Count > 0) { packageProject.Sources.Add(sourceCollection); }

            var packagesFolder = Settings.GetSetting("ChangeTracker.PackagesFolder");
            var pathToPackage = string.Format("{0}\\{1}.zip", packagesFolder, packageName);
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
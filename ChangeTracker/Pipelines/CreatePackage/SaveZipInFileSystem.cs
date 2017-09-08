namespace ChangeTracker.Pipelines.CreatePackage
{
    using Commands;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Install;
    using Sitecore.Install.Framework;
    using Sitecore.Install.Items;
    using Sitecore.Install.Zip;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SaveZipInFileSystem
    {
        private GenerateZipArgs Args { get; set; }

        public void Process(GenerateZipArgs args)
        {
            this.Args = args;
            var lastFinishedTask = TrackerUtil.LastFinishedTaskItem;
            var items = this.GetItemForPackage(lastFinishedTask);
            this.GeneratePackage(items, lastFinishedTask.Name);
        }

        private IEnumerable<Item> GetItemForPackage(Item lastFinishedTask)
        {
            var taskStartTime = lastFinishedTask[FieldIDs.Created];
            var taskEndTime = lastFinishedTask[ChangeTracker.Constants.Templates.Task.Fields.TaskEndDate];
            var taskImplementer = lastFinishedTask[FieldIDs.CreatedBy];

            var masterDatabase = Factory.GetDatabase("master");
            var query = string.Format("fast:/sitecore//*[@__Updated > '{0}' and @__Updated < '{1}' and @__Updated by = '{2}' and @@templateid != '{3}']",
                taskStartTime,
                taskEndTime,
                taskImplementer,
                ChangeTracker.Constants.Templates.Task.ID);
            IEnumerable<Item> items = masterDatabase.SelectItems(query);

            MultilistField excludedItemsField = new MultilistField(lastFinishedTask.Fields[ChangeTracker.Constants.Templates.Task.Fields.ExcludedItems]);
            if (excludedItemsField.TargetIDs.Any())
            {
                var withoutExcludedItems = new List<Item>();
                foreach (var item in items)
                {
                    if (excludedItemsField.TargetIDs.Contains(item.ID)) { continue; }
                    withoutExcludedItems.Add(item);
                }
                items = withoutExcludedItems;
            }

            return items;
        }

        private void GeneratePackage(IEnumerable<Item> items, string packageName)
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

            var packagesFolder = Path.GetTempPath();            
            var pathToPackage = string.Format("{0}\\{1}.zip", packagesFolder, packageName);

            this.Args.FilePath = pathToPackage;
            this.Args.MediaItemName = packageName;

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
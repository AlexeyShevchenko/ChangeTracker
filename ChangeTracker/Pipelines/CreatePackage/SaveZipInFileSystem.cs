namespace ChangeTracker.Pipelines.CreatePackage
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
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
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.LastFinishedTaskItem, "args.LastFinishedTaskItem");

            Args = args;
            var items = GetItemForPackage();
            GeneratePackage(items);
        }

        private IEnumerable<Item> GetItemForPackage()
        {
            var taskStartTime = Args.LastFinishedTaskItem[FieldIDs.Created];
            var taskEndTime = Args.LastFinishedTaskItem[ChangeTracker.Constants.Templates.Task.Fields.TaskEndDate];
            var taskImplementer = Args.LastFinishedTaskItem[FieldIDs.CreatedBy];

            var masterDatabase = Factory.GetDatabase("master");
            var query = string.Format("fast:/sitecore//*[@__Updated > '{0}' and @__Updated < '{1}' and @__Updated by = '{2}' and @@templateid != '{3}']",
                taskStartTime,
                taskEndTime,
                taskImplementer,
                ChangeTracker.Constants.Templates.Task.ID);
            IEnumerable<Item> masterItems = masterDatabase.SelectItems(query);
                        
            var coreDatabase = Factory.GetDatabase("core");
            var coreQuery = string.Format("fast:/sitecore//*[@__Updated > '{0}' and @__Updated < '{1}' and @__Updated by = '{2}']",
                taskStartTime,
                taskEndTime,
                taskImplementer);
            IEnumerable<Item> coreItems = coreDatabase.SelectItems(coreQuery);

            var res = masterItems.Concat(coreItems);

            MultilistField excludedItemsField = new MultilistField(Args.LastFinishedTaskItem.Fields[ChangeTracker.Constants.Templates.Task.Fields.ExcludedItems]);
            if (excludedItemsField.TargetIDs.Any())
            {
                var withoutExcludedItems = new List<Item>();
                foreach (var item in res)
                {
                    if (excludedItemsField.TargetIDs.Contains(item.ID)) { continue; }
                    withoutExcludedItems.Add(item);
                }
                res = withoutExcludedItems;
            }

            return res;
        }

        private void GeneratePackage(IEnumerable<Item> items)
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
            foreach (var item in items)
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
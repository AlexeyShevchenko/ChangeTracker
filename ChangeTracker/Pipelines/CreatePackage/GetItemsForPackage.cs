namespace ChangeTracker.Pipelines.CreatePackage
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    public class GetItemsForPackage
    {
        public void Process(CreatePackageArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.LastFinishedTaskItem, "args.LastFinishedTaskItem");

            var taskStartTime = args.LastFinishedTaskItem[FieldIDs.Created];
            var taskEndTime = args.LastFinishedTaskItem[ChangeTracker.Constants.Templates.Task.Fields.TaskEndDate];
            var taskImplementer = args.LastFinishedTaskItem[FieldIDs.CreatedBy];

            var masterDatabase = Factory.GetDatabase("master");
            var masterQuery = string.Format("fast:/sitecore//*[@__Updated > '{0}' and @__Updated < '{1}' and @__Updated by = '{2}' and @@parentid != '{3}' and @@templateid != '{4}']",
                taskStartTime,
                taskEndTime,
                taskImplementer,
                ChangeTracker.Constants.ChangeTrackerMediaFolder,
                ChangeTracker.Constants.Templates.Task.ID);
            IEnumerable<Item> masterItems = masterDatabase.SelectItems(masterQuery);
            var coreDatabase = Factory.GetDatabase("core");
            var coreQuery = string.Format("fast:/sitecore//*[@__Updated > '{0}' and @__Updated < '{1}' and @__Updated by = '{2}']",
                taskStartTime,
                taskEndTime,
                taskImplementer);
            IEnumerable<Item> coreItems = coreDatabase.SelectItems(coreQuery);
            var itemsForPackage = masterItems.Concat(coreItems);

            MultilistField excludedItemsField = new MultilistField(args.LastFinishedTaskItem.Fields[ChangeTracker.Constants.Templates.Task.Fields.ExcludedItems]);
            if (excludedItemsField.TargetIDs.Any())
            {
                var withoutExcludedItems = new List<Item>();
                foreach (var item in itemsForPackage)
                {
                    if (excludedItemsField.TargetIDs.Contains(item.ID)) { continue; }
                    withoutExcludedItems.Add(item);
                }
                itemsForPackage = withoutExcludedItems;
            }

            args.ItemsForPackage = itemsForPackage;
        }
    }
}
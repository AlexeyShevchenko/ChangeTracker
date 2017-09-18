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

            Assert.ArgumentNotNullOrEmpty(taskStartTime, "taskStartTime");
            Assert.ArgumentNotNullOrEmpty(taskEndTime, "taskEndTime");
            Assert.ArgumentNotNullOrEmpty(taskImplementer, "taskImplementer");

            var masterDatabase = Factory.GetDatabase("master");
            Assert.ArgumentNotNull(masterDatabase, "masterDatabase");

            var masterQuery = QueryBuilder.BuildQueryForMasterDatabase(taskStartTime, taskEndTime, taskImplementer);
            var masterItems = TrackerUtil.FetchAffectedItems(masterDatabase, masterQuery);

            var coreDatabase = Factory.GetDatabase("core");
            Assert.ArgumentNotNull(coreDatabase, "coreDatabase");

            var coreQuery = QueryBuilder.BuildQueryForCoreDatabase(taskStartTime, taskEndTime, taskImplementer);
            var coreItems = TrackerUtil.FetchAffectedItems(coreDatabase, coreQuery);

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
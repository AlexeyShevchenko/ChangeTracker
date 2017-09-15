namespace ChangeTracker.Commands
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Publishing;
    using Sitecore.Shell.Framework.Commands;
    using System.Collections.Generic;
    using System.Linq;

    public class Publish : Command
    {
        public override void Execute(CommandContext context)
        {
            var lastFinishedTaskItem = TrackerUtil.LastFinishedTaskItem;
            Assert.ArgumentNotNull(lastFinishedTaskItem, "lastFinishedTaskItem");

            MultilistField publishTargetsField = lastFinishedTaskItem.Fields[Constants.Templates.Task.Fields.PublishTargets];
            Assert.IsTrue(publishTargetsField.TargetIDs.Any(), "no publis targets");

            var taskStartTime = lastFinishedTaskItem[Sitecore.FieldIDs.Created];
            var taskEndTime = lastFinishedTaskItem[Constants.Templates.Task.Fields.TaskEndDate];
            var taskImplementer = lastFinishedTaskItem[Sitecore.FieldIDs.CreatedBy];

            var masterDatabase = Factory.GetDatabase("master");
            Assert.ArgumentNotNull(masterDatabase, "masterDatabase");

            var masterQuery = string.Format("fast:/sitecore//*[@__Updated > '{0}' and @__Updated < '{1}' and @__Updated by = '{2}' and @@parentid != '{3}' and @@templateid != '{4}']",
                taskStartTime,
                taskEndTime,
                taskImplementer,
                Constants.ChangeTrackerMediaFolder,
                Constants.Templates.Task.ID);
            IEnumerable<Item> itemsToPublish = masterDatabase
                .SelectItems(masterQuery)
                .OrderBy(item => item[Sitecore.FieldIDs.Created]);

            MultilistField excludedItemsField = new MultilistField(lastFinishedTaskItem.Fields[Constants.Templates.Task.Fields.ExcludedItems]);
            if (excludedItemsField.TargetIDs.Any())
            {
                var withoutExcludedItems = new List<Item>();
                foreach (var item in itemsToPublish)
                {
                    if (excludedItemsField.TargetIDs.Contains(item.ID)) { continue; }
                    withoutExcludedItems.Add(item);
                }
                itemsToPublish = withoutExcludedItems;
            }

            IEnumerable<Database> targetDatabases = GetTargetDatabases(publishTargetsField.TargetIDs, masterDatabase);

            foreach (var itemToPublish in itemsToPublish)
            {
                PublishManager.PublishItem(itemToPublish, targetDatabases.ToArray(), itemToPublish.Languages, false, false);
            }
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (TrackerUtil.LastFinishedTaskItem != null && !TrackerUtil.IsCurrentTaskInProcess)
            {
                return CommandState.Enabled;
            }
            return CommandState.Hidden;
        }

        private IEnumerable<Database> GetTargetDatabases(ID[] targets, Database masterDatabase)
        {
            var targetDatabases = new List<Database>();
            foreach (var publishTargetId in targets)
            {
                var publishTargetItem = masterDatabase.GetItem(publishTargetId);
                if (publishTargetItem == null) { continue; }

                var targetDatabaseName = publishTargetItem[Sitecore.FieldIDs.PublishingTargetDatabase];
                if (string.IsNullOrEmpty(targetDatabaseName)) { continue; }

                var targetDatabase = Factory.GetDatabase(targetDatabaseName);
                if (targetDatabase == null) { continue; }

                targetDatabases.Add(targetDatabase);
            }
            return targetDatabases;
        }
    }
}
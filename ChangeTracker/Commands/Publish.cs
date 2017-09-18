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
            var lastFinishedTaskItem = Context.LastFinishedTaskItem;
            Assert.ArgumentNotNull(lastFinishedTaskItem, "lastFinishedTaskItem");

            MultilistField publishTargetsField = lastFinishedTaskItem.Fields[Constants.Templates.Task.Fields.PublishTargets];
            Assert.IsTrue(publishTargetsField.TargetIDs.Any(), "no publis targets");

            var taskStartTime = lastFinishedTaskItem[Sitecore.FieldIDs.Created];
            var taskEndTime = lastFinishedTaskItem[Constants.Templates.Task.Fields.TaskEndDate];
            var taskImplementer = lastFinishedTaskItem[Sitecore.FieldIDs.CreatedBy];

            Assert.ArgumentNotNullOrEmpty(taskStartTime, "taskStartTime");
            Assert.ArgumentNotNullOrEmpty(taskEndTime, "taskEndTime");
            Assert.ArgumentNotNullOrEmpty(taskImplementer, "taskImplementer");

            var masterDatabase = Factory.GetDatabase("master");
            Assert.ArgumentNotNull(masterDatabase, "masterDatabase");

            var masterQuery = QueryBuilder.BuildQueryForMasterDatabase(taskStartTime, taskEndTime, taskImplementer);
            var itemsToPublish = TrackerUtil.FetchAffectedItems(masterDatabase, masterQuery)
                .OrderBy(item => item[Sitecore.FieldIDs.Created])
                .AsEnumerable();

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
            if (Context.LastFinishedTaskItem != null && !Context.IsCurrentTaskInProcess)
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
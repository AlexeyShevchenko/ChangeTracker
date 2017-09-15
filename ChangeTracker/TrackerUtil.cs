namespace ChangeTracker
{
    using Sitecore.Configuration;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using System.Linq;

    public static class TrackerUtil
    {
        private static Item TasksFolder
        {
            get
            {
                var tasksFolder = Factory.GetDatabase("master").GetItem(Constants.TasksFolder);
                var currentTaskItem = tasksFolder.Children.FirstOrDefault(taskItem => string.IsNullOrEmpty(taskItem[Constants.Templates.Task.Fields.TaskEndDate]));

                return tasksFolder;
            }
        }

        public static Item CurrentTaskItem
        {
            get
            {
                var currentTaskItem = TasksFolder.Children
                    .FirstOrDefault(taskItem =>
                        string.IsNullOrEmpty(taskItem[Constants.Templates.Task.Fields.TaskEndDate])
                        && taskItem[Sitecore.FieldIDs.CreatedBy] == Sitecore.Context.User.Name);
                return currentTaskItem;
            }
        }

        public static bool IsCurrentTaskInProcess
        {
            get
            {
                return CurrentTaskItem != null;
            }
        }

        public static bool IsItemInExcludedList(Item item)
        {
            if (item == null) { return false; }

            MultilistField excludedItemsField = CurrentTaskItem.Fields[Constants.Templates.Task.Fields.ExcludedItems];
            return excludedItemsField.TargetIDs.Contains(item.ID);
        }

        public static Item LastFinishedTaskItem
        {
            get
            {
                var lastFinishedTaskItem = TasksFolder.Children
                    .Where(taskItem => taskItem[Sitecore.FieldIDs.CreatedBy] == Sitecore.Context.User.Name)
                    .OrderByDescending(taskItem => taskItem[Sitecore.FieldIDs.Created]).FirstOrDefault();
                return lastFinishedTaskItem;
            }
        }

        public static void ReloadRibbon(Item item, object sender)
        {
            var load = string.Concat(new object[] { "item:load(id=", item.ID, ",language=", item.Language, ",version=", item.Version, ")" });
            Sitecore.Context.ClientPage.SendMessage(sender, load);
        }
    }
}
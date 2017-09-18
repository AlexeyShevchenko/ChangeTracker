namespace ChangeTracker
{
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using System.Linq;

    public static class Context
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
    }
}
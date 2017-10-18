namespace ChangeTracker
{
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using System.Linq;

    public static class Context
    {
        private static Item TasksFolder => Factory.GetDatabase("master").GetItem(Constants.TasksFolder);

        public static Item CurrentTaskItem => TasksFolder.Children
            .FirstOrDefault(taskItem => string.IsNullOrEmpty(taskItem[Constants.Templates.Task.Fields.TaskEndDate]) && taskItem[Sitecore.FieldIDs.CreatedBy] == Sitecore.Context.User.Name);

        public static bool IsCurrentTaskInProcess => CurrentTaskItem != null;

        public static Item LastFinishedTaskItem => TasksFolder.Children
            .Where(taskItem => taskItem[Sitecore.FieldIDs.CreatedBy] == Sitecore.Context.User.Name)
            .OrderByDescending(taskItem => taskItem[Sitecore.FieldIDs.Created]).FirstOrDefault();
    }
}
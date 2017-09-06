namespace ChangeTracker.Commands
{
    using Sitecore.Data.Fields;
    using Sitecore.Shell.Framework.Commands;

    public class RemoveItemFromExcludeList : Command
    {
        public override void Execute(CommandContext context)
        {
            var currentTask = TrackerUtil.CurrentTaskItem;
            MultilistField excludedItemsField = currentTask.Fields[Constants.Templates.Task.Fields.ExcludedItems];

            currentTask.Editing.BeginEdit();
            excludedItemsField.Remove(context.Items[0].ID.ToString());
            currentTask.Editing.EndEdit();

            TrackerUtil.ReloadRibbon(context.Items[0], this);
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (!TrackerUtil.IsCurrentTaskInProcess)
            {
                return CommandState.Hidden;
            }
            if (TrackerUtil.IsItemInExcludedList(context.Items[0]))
            {
                return CommandState.Enabled;
            }
            return CommandState.Hidden;
        }
    }
}
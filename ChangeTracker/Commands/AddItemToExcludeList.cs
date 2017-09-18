namespace ChangeTracker.Commands
{
    using Sitecore.Data.Fields;
    using Sitecore.Shell.Framework.Commands;

    public class AddItemToExcludeList : Command
    {
        public override void Execute(CommandContext context)
        {
            var currentTask = Context.CurrentTaskItem;
            MultilistField excludedItemsField = currentTask.Fields[Constants.Templates.Task.Fields.ExcludedItems];

            currentTask.Editing.BeginEdit();
            excludedItemsField.Add(context.Items[0].ID.ToString());
            currentTask.Editing.EndEdit();

            UIHelper.ReloadRibbon(context.Items[0], this);
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (!Context.IsCurrentTaskInProcess)
            {
                return CommandState.Hidden;
            }
            if (TrackerUtil.IsItemInExcludedList(context.Items[0]))
            {
                return CommandState.Hidden;
            }
            return CommandState.Enabled;
        }
    }
}
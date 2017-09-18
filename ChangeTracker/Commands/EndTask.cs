namespace ChangeTracker.Commands
{
    using Sitecore;
    using Sitecore.Shell.Framework.Commands;

    public class EndTask : Command
    {
        public override void Execute(CommandContext context)
        {
            var currentTask = ChangeTracker.Context.CurrentTaskItem;
            currentTask.Editing.BeginEdit();
            currentTask[ChangeTracker.Constants.Templates.Task.Fields.TaskEndDate] = DateUtil.ToIsoDate(System.DateTime.Now);
            currentTask.Editing.EndEdit();

            UIHelper.ReloadRibbon(context.Items[0], this);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return ChangeTracker.Context.IsCurrentTaskInProcess ? CommandState.Enabled : CommandState.Hidden;
        }
    }
}
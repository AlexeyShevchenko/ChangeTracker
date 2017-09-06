namespace ChangeTracker.Commands
{
    using Sitecore;
    using Sitecore.Shell.Framework.Commands;

    public class EndTask : Command
    {
        public override void Execute(CommandContext context)
        {
            var currentTask = TrackerUtil.CurrentTaskItem;
            currentTask.Editing.BeginEdit();
            currentTask[ChangeTracker.Constants.Templates.Task.Fields.TaskEndDate] = DateUtil.ToIsoDate(System.DateTime.Now);
            currentTask.Editing.EndEdit();

            //var currentItem = context.Items[0];
            //var load = string.Concat(new object[] { "item:load(id=", currentItem.ID, ",language=", currentItem.Language, ",version=", currentItem.Version, ")" });
            //Context.ClientPage.SendMessage(this, load);

            TrackerUtil.ReloadRibbon(context.Items[0], this);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return TrackerUtil.IsCurrentTaskInProcess ? CommandState.Enabled : CommandState.Hidden;
        }
    }
}
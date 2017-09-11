namespace ChangeTracker.Commands
{
    using Pipelines.CreatePackage;
    using Sitecore.Pipelines;
    using Sitecore.Shell.Framework.Commands;

    public class GenerateZip : Command
    {
        public override void Execute(CommandContext context)
        {
            var lastFinishedTaskItem = TrackerUtil.LastFinishedTaskItem;
            var args = new GenerateZipArgs
            {
                LastFinishedTaskItem = lastFinishedTaskItem,
                MediaItemName = lastFinishedTaskItem.Name
            };
            CorePipeline.Run("createPackage", args);
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (TrackerUtil.LastFinishedTaskItem != null && !TrackerUtil.IsCurrentTaskInProcess)
            {
                return CommandState.Enabled;
            }
            return CommandState.Hidden;
        }
    }
}
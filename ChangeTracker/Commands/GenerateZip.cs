namespace ChangeTracker.Commands
{
    using Pipelines.CreatePackage;
    using Sitecore.Pipelines;
    using Sitecore.Shell.Framework.Commands;

    public class GenerateZip : Command
    {
        public override void Execute(CommandContext context)
        {
            var lastFinishedTaskItem = Context.LastFinishedTaskItem;
            var args = new CreatePackageArgs
            {
                LastFinishedTaskItem = lastFinishedTaskItem,
                MediaItemName = lastFinishedTaskItem.Name
            };
            CorePipeline.Run("createPackage", args);
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (Context.LastFinishedTaskItem != null && !Context.IsCurrentTaskInProcess)
            {
                return CommandState.Enabled;
            }
            return CommandState.Hidden;
        }
    }
}
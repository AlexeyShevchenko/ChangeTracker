namespace ChangeTracker.Commands
{
    using Pipelines.CreatePackage;
    using Sitecore.Pipelines;
    using Sitecore.Shell.Framework.Commands;

    public class GenerateZip : Command
    {
        public override void Execute(CommandContext context)
        {
            var args = new GenerateZipArgs();
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
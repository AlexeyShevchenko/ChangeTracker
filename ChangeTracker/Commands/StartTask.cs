namespace ChangeTracker.Commands
{
    using Sitecore.Shell.Framework.Commands;
    using System.Collections.Specialized;

    public class StartTask : Command
    {
        public override void Execute(CommandContext context)
        {
            var nameValueCollection = new NameValueCollection();
            Sitecore.Context.ClientPage.Start("createTaskItem", nameValueCollection);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return Context.IsCurrentTaskInProcess ? CommandState.Hidden : CommandState.Enabled;
        }
    }
}
namespace ChangeTracker.Commands
{
    using Sitecore.Shell.Framework.Commands;

    public class StartTask : Command
    {
        public override void Execute(CommandContext context)
        {
            var nameValueCollection = new System.Collections.Specialized.NameValueCollection();
            //nameValueCollection.Add("currentItemUri", context.Items[0].Uri.ToString());
            Sitecore.Context.ClientPage.Start("changeTracker", nameValueCollection);
        }

        public override CommandState QueryState(CommandContext context)
        {
            return TrackerUtil.IsCurrentTaskInProcess ? CommandState.Hidden : CommandState.Enabled;
        }
    }
}
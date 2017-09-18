namespace ChangeTracker.Pipelines.ShowChangeTrackerWarnings
{
    using Sitecore.Pipelines.GetContentEditorWarnings;

    public class ItemIsInExcludeList
    {
        public void Process(GetContentEditorWarningsArgs args)
        {
            if (args.Item == null)
            {
                return;
            }

            if (!Context.IsCurrentTaskInProcess)
            {
                return;
            }

            if (TrackerUtil.IsItemInExcludedList(args.Item))
            {
                foreach (var warning in args.Warnings) { warning.IsExclusive = false; }

                var contentEditorWarning = args.Add();
                contentEditorWarning.Text = "The current item is added to exclude list of current task.";
                contentEditorWarning.IsExclusive = false;
                contentEditorWarning.AddOption("Remove item from exclude list", "changetracker:removeitemfromexcludelist");
            }
        }
    }
}
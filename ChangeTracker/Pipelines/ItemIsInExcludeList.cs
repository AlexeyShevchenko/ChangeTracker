namespace ChangeTracker.Pipelines
{
    using Commands;
    using Sitecore.Pipelines.GetContentEditorWarnings;

    public class ItemIsInExcludeList
    {
        public void Process(GetContentEditorWarningsArgs args)
        {
            var item = args.Item;
            if (item == null)
            {
                return;
            }

            if (!TrackerUtil.IsCurrentTaskInProcess)
            {
                return;
            }

            if (TrackerUtil.IsItemInExcludedList(item))
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
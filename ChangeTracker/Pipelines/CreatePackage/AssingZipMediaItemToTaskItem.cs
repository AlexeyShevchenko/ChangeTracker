namespace ChangeTracker.Pipelines.CreatePackage
{
    using Commands;
    using Sitecore.Diagnostics;

    public class AssingZipMediaItemToTaskItem
    {
        public void Process(GenerateZipArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var taskItem = TrackerUtil.LastFinishedTaskItem;
            taskItem.Editing.BeginEdit();
            taskItem[Constants.Templates.Task.Fields.Package] = string.Format("<file mediaid=\"{0}\" src=\"-/media/{1}.ashx\" />", args.ZipMediaItem.ID, args.ZipMediaItem.ID.ToShortID());
            taskItem.Editing.EndEdit();
        }
    }
}

namespace ChangeTracker.Pipelines.CreatePackage
{
    using Sitecore.Diagnostics;

    public class AssingZipMediaItemToTaskItem
    {
        public void Process(GenerateZipArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.LastFinishedTaskItem, "args.LastFinishedTaskItem");
            Assert.ArgumentNotNull(args.ZipMediaItem, "args.ZipMediaItem");

            args.LastFinishedTaskItem.Editing.BeginEdit();
            args.LastFinishedTaskItem[Constants.Templates.Task.Fields.Package] = string.Format("<file mediaid=\"{0}\" src=\"-/media/{1}.ashx\" />", args.ZipMediaItem.ID, args.ZipMediaItem.ID.ToShortID());
            args.LastFinishedTaskItem.Editing.EndEdit();
        }
    }
}
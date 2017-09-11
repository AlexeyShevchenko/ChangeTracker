namespace ChangeTracker.Pipelines.CreatePackage
{
    using Sitecore.Data.Items;
    using Sitecore.Pipelines;

    public class GenerateZipArgs : PipelineArgs
    {
        public Item LastFinishedTaskItem { get; set; }

        public string FilePath { get; set; }

        public string MediaItemName { get; set; }

        public MediaItem ZipMediaItem { get; set; }
    }
}
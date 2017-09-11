namespace ChangeTracker.Pipelines.CreatePackage
{
    using Sitecore.Data.Items;
    using Sitecore.Pipelines;
    using System.Collections.Generic;

    public class CreatePackageArgs : PipelineArgs
    {
        public Item LastFinishedTaskItem { get; set; }

        public IEnumerable<Item> ItemsForPackage { get; set; }

        public string FilePath { get; set; }

        public string MediaItemName { get; set; }

        public MediaItem ZipMediaItem { get; set; }
    }
}
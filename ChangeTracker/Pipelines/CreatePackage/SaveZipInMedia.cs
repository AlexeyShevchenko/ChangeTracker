﻿namespace ChangeTracker.Pipelines.CreatePackage
{
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Resources.Media;

    public class SaveZipInMedia
    {
        public void Process(CreatePackageArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNullOrEmpty(args.MediaItemName, "args.MediaItemName");
            Assert.ArgumentNotNullOrEmpty(args.FilePath, "args.FilePath");

            args.ZipMediaItem = AddFile(args.MediaItemName, args.FilePath);
        }

        public MediaItem AddFile(string mediaItemName, string filePath)
        {
            var masterDatabase = Factory.GetDatabase("master");
            var mediaFolder = masterDatabase.GetItem(Constants.ChangeTrackerMediaFolder);
            var path = mediaFolder.Paths.FullPath;
            var options = new MediaCreatorOptions
            {
                FileBased = false,
                IncludeExtensionInItemName = false,
                OverwriteExisting = false,
                Versioned = false,
                Destination = string.Format("{0}/{1}", path, mediaItemName),
                Database = masterDatabase
            };

            var creator = new MediaCreator();
            var mediaItem = creator.CreateFromFile(filePath, options);

            return mediaItem;
        }
    }
}
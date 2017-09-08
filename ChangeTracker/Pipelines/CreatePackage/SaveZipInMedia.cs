namespace ChangeTracker.Pipelines.CreatePackage
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Resources.Media;

    public class SaveZipInMedia
    {
        public void Process(GenerateZipArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            args.ZipMediaItem = AddFile(args.FilePath, args.MediaItemName);
        }

        public MediaItem AddFile(string filePath, string mediaItemName)
        {
            var mediaFolder = Factory.GetDatabase("master").GetItem(new ID("{584099B0-D510-4137-966C-5376A558EF19}"));
            var path = mediaFolder.Paths.FullPath;
            var options = new MediaCreatorOptions
            {
                FileBased = false,
                IncludeExtensionInItemName = false,
                OverwriteExisting = false,
                Versioned = false,
                Destination = string.Format("{0}/{1}", path, mediaItemName),
                Database = Factory.GetDatabase("master")
            };

            var creator = new MediaCreator();
            var mediaItem = creator.CreateFromFile(filePath, options);

            return mediaItem;
        }
    }
}
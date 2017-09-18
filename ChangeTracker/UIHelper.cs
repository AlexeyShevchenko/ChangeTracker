namespace ChangeTracker
{
    using Sitecore.Data.Items;

    public static class UIHelper
    {
        public static void ReloadRibbon(Item item, object sender)
        {
            var load = string.Concat(new object[] { "item:load(id=", item.ID, ",language=", item.Language, ",version=", item.Version, ")" });
            Sitecore.Context.ClientPage.SendMessage(sender, load);
        }
    }
}

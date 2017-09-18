namespace ChangeTracker
{
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    public static class TrackerUtil
    {
        public static bool IsItemInExcludedList(Item item)
        {
            if (item == null) { return false; }

            MultilistField excludedItemsField = Context.CurrentTaskItem.Fields[Constants.Templates.Task.Fields.ExcludedItems];
            return excludedItemsField.TargetIDs.Contains(item.ID);
        }

        public static IEnumerable<Item> FetchAffectedItems(Database database, string query)
        {
            Assert.ArgumentNotNull(database, "database");
            Assert.ArgumentNotNull(database, "query");
            var items = database.SelectItems(query);

            return items;
        }
    }
}
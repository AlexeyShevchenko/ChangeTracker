namespace ChangeTracker
{
    public static class QueryBuilder
    {
        public static string BuildQueryForMasterDatabase(string taskStartTime, string taskEndTime, string taskImplementer)
        {
            var masterQuery = string.Format(
                "fast:/sitecore//*[@__Updated > '{0}' and @__Updated < '{1}' and @__Updated by = '{2}' and @@parentid != '{3}' and @@templateid != '{4}']",
                taskStartTime,
                taskEndTime,
                taskImplementer,
                Constants.ChangeTrackerMediaFolder,
                Constants.Templates.Task.ID);

            return masterQuery;
        }

        public static string BuildQueryForCoreDatabase(string taskStartTime, string taskEndTime, string taskImplementer)
        {
            var coreQuery = string.Format("fast:/sitecore//*[@__Updated > '{0}' and @__Updated < '{1}' and @__Updated by = '{2}']",
                taskStartTime,
                taskEndTime,
                taskImplementer);

            return coreQuery;
        }
    }
}
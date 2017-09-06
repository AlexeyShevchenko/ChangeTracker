namespace ChangeTracker.Pipelines
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Diagnostics;
    using Sitecore.Web.UI.Sheer;

    public class CreateTaskItem
    {
        public void Process(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.IsPostBack)
            {
                if (string.IsNullOrEmpty(args.Result) || args.Result == "null" || args.Result == "undefined")
                {
                    args.AbortPipeline();
                    return;
                }
                if (args.Result.Trim().Length == 0)
                {
                    Sitecore.Context.ClientPage.ClientResponse.Alert("The name of task cannot be blank.");
                    Sitecore.Context.ClientPage.ClientResponse.Input("Enter a name of task:", "Task", Settings.ItemNameValidation, "'$Input' is not a valid name.", Settings.MaxItemNameLength, "Task name");
                    args.WaitForPostBack();
                    return;
                }

                var masterDatabase = Factory.GetDatabase("master");
                var tasksFolder = masterDatabase.GetItem(Constants.TasksFolder);
                var taskName = args.Result;
                tasksFolder.Add(taskName, Constants.Templates.Task.ID);

                //var currentItemUri = new ItemUri(args.Parameters["currentItemUri"]);
                //var currentDataUri = new DataUri(currentItemUri);
                //var currentItem = masterDatabase.GetItem(currentDataUri);

                //var load = string.Concat(new object[] { "item:load(id=", currentItem.ID, ",language=", currentItem.Language, ",version=", currentItem.Version, ")" });
                //Sitecore.Context.ClientPage.SendMessage(this, load);                
            }
            else
            {
                SheerResponse.Input("Enter a name of task:", "Task", Settings.ItemNameValidation, "'$Input' is not a valid name.", Settings.MaxItemNameLength, "Task name");
                args.WaitForPostBack();
                return;
            }
        }
    }
}
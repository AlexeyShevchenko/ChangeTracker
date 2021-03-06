﻿namespace ChangeTracker
{
    using Sitecore.Data;

    public static class Constants
    {
        public static ID TasksFolder = new ID("{C382857C-A13A-46C9-9404-C2E3483892BE}");

        public static ID ChangeTrackerMediaFolder = new ID("{584099B0-D510-4137-966C-5376A558EF19}");

        public static class Templates
        {
            public static class Task
            {
                public static TemplateID ID = new TemplateID(new ID("{101FC73C-EA31-4694-86A6-BADB51ACB136}"));

                public static class Fields
                {
                    public static ID TaskEndDate = new ID("{EAE9D83F-0CAD-4594-8BF2-A152A1D6F897}");

                    public static ID ExcludedItems = new ID("{7225D34B-D69B-494D-B615-063FB271EC8C}");

                    public static ID Package = new ID("{073E276F-358C-4A7F-8531-C6C4C9FD342A}");

                    public static ID PublishTargets = new ID("{3A3CA28C-C727-420D-A52F-40B941290812}");
                }
            }
        }
    }
}
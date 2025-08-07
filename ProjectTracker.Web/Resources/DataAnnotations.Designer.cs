namespace ProjectTracker.Web.Resources
{
    public static class DataAnnotations
    {
        private static readonly System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("ProjectTracker.Web.Resources.DataAnnotations", typeof(DataAnnotations).Assembly);

        public static string Required => resourceManager.GetString("Required")!;
        public static string EmailInvalid => resourceManager.GetString("EmailInvalid")!;
        public static string StringLength => resourceManager.GetString("StringLength")!;
        public static string PasswordMismatch => resourceManager.GetString("PasswordMismatch")!;
        public static string HoursRange => resourceManager.GetString("HoursRange")!;
    }
}

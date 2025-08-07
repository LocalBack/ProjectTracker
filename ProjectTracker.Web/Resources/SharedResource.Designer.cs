namespace ProjectTracker.Web.Resources
{
    public static class SharedResource
    {
        private static readonly System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("ProjectTracker.Web.Resources.SharedResource", typeof(SharedResource).Assembly);

        public static string AppName => resourceManager.GetString("AppName")!;
        public static string Email => resourceManager.GetString("Email")!;
        public static string Password => resourceManager.GetString("Password")!;
        public static string RememberMe => resourceManager.GetString("RememberMe")!;
        public static string VerificationCode => resourceManager.GetString("VerificationCode")!;
        public static string RememberThisDevice => resourceManager.GetString("RememberThisDevice")!;
        public static string FirstName => resourceManager.GetString("FirstName")!;
        public static string LastName => resourceManager.GetString("LastName")!;
        public static string RegistrationDate => resourceManager.GetString("RegistrationDate")!;
        public static string LastLogin => resourceManager.GetString("LastLogin")!;
        public static string Title => resourceManager.GetString("Title")!;
        public static string Description => resourceManager.GetString("Description")!;
        public static string WorkDate => resourceManager.GetString("WorkDate")!;
        public static string HoursSpent => resourceManager.GetString("HoursSpent")!;
        public static string Project => resourceManager.GetString("Project")!;
        public static string Employee => resourceManager.GetString("Employee")!;
        public static string CurrentPassword => resourceManager.GetString("CurrentPassword")!;
        public static string NewPassword => resourceManager.GetString("NewPassword")!;
        public static string ConfirmNewPassword => resourceManager.GetString("ConfirmNewPassword")!;
    }
}

namespace ProjectTracker.Web.Resources.Models
{
    public static class User
    {
        private static readonly System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("ProjectTracker.Web.Resources.Models.User", typeof(User).Assembly);

        public static string FirstNameRequired => resourceManager.GetString("FirstNameRequired")!;
        public static string LastNameRequired => resourceManager.GetString("LastNameRequired")!;
        public static string EmailRequired => resourceManager.GetString("EmailRequired")!;
        public static string EmailInvalid => resourceManager.GetString("EmailInvalid")!;
        public static string PasswordRequired => resourceManager.GetString("PasswordRequired")!;
        public static string PasswordLength => resourceManager.GetString("PasswordLength")!;
        public static string PasswordMismatch => resourceManager.GetString("PasswordMismatch")!;
        public static string FirstName => resourceManager.GetString("FirstName")!;
        public static string LastName => resourceManager.GetString("LastName")!;
        public static string Email => resourceManager.GetString("Email")!;
        public static string Password => resourceManager.GetString("Password")!;
        public static string ConfirmPassword => resourceManager.GetString("ConfirmPassword")!;
        public static string KvkkAccepted => resourceManager.GetString("KvkkAccepted")!;
    }
}

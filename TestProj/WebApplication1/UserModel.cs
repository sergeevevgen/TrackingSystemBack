namespace WebApplication1
{
    public class UserModel
    {
        public UserModel(string dn, string uid, string ou, string userPassword)
        {
            this.dn = dn;
            this.uid = uid;
            this.ou = ou;
            this.userPassword = userPassword;
        }

        public string DN { get { return dn; } }
        public string UID { get { return uid; } }
        public string OU { get { return ou; } }
        public string UserPassword { get { return userPassword; } }

        private readonly string dn;
        private readonly string uid;
        private readonly string ou;
        private readonly string userPassword;
    }
}

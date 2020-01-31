namespace Graphene.ApiCommunication.Models
{
    public class LoginModelView
    {
        public LoginModelView()
        {
            
        }
        
        public LoginModelView(string name, string password)
        {
            UserName = name;
            Password = password;

            IsPersistent = true;
        }

        public string UserName { get; set; }
        public string Password { get; set; }

        public bool IsPersistent { get; set; }
        
        public override string ToString()
        {
            return $"name: {UserName}";
        }
    }
}
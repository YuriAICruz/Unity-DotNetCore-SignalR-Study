namespace Graphene.SharedModels.ModelView
{
    public class UserModelView
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        public UserModelView()
        {
            
        }
        
        public UserModelView(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }

        public override string ToString()
        {
            return $"{UserName} - {Email}";
        }
    }
}
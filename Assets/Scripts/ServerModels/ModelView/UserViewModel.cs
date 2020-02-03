namespace Graphene.SharedModels.ModelView
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        public UserViewModel()
        {
            
        }
        
        public UserViewModel(string name, string email)
        {
            UserName = name;
            Email = email;
        }
    }
}
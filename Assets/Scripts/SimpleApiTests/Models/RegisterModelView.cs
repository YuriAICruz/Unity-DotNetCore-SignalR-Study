namespace Graphene.ApiCommunication.Models
{
    public class RegisterModelView : LoginModelView
    {
        public RegisterModelView(string name, string email, string password)
        {
            Password = password;
            UserName = name;
            Email = email;
        }

        public string Email { get; set; }


        public override string ToString()
        {
            return $"name: {UserName}, email: {Email}";
        }
    }
}
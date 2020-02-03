using System.ComponentModel.DataAnnotations;

namespace Graphene.SharedModels.ModelView
{
    public class RegisterModelView : LoginModelView
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public RegisterModelView(string userName, string email, string password, bool isPersistent = false) : base(userName, password, isPersistent)
        {
            Email = email;
        }
    }
}
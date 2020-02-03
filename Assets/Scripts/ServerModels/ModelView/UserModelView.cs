namespace Graphene.SharedModels.ModelView
{
    public class UserModelView
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"{UserName} - {Email}";
        }
    }
}
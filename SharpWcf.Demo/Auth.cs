using System.IdentityModel.Selectors;

namespace SharpWcf.Demo
{
    public class Auth : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {            
        }
    }
}
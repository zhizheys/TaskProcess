
namespace TaskProcess.MS.Model
{
   public class User
    {
        public string UserName { get; set; }
        public int Age { get; set; }
        public Address Address { get; set; }
    }

    public class UserInfo_ViewModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string PhoneNum { get; set; }
    }
}

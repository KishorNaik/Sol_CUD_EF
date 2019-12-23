using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Cud_EF.Model
{
    public class UserModel
    {
        public decimal UserId { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public UserLoginModel UserLogin { get; set; }
    }
}

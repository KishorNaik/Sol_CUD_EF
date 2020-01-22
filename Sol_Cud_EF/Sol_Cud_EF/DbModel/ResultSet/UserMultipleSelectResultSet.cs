using Sol_Cud_EF.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Cud_EF.DbModel.ResultSet
{
    public class UserMultipleSelectResultSet
    {
        public List<UserModel> UserModelList { get; set; }

        public List<UserLoginModel> UserLoginList { get; set; }
    }
}

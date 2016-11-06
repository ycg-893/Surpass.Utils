using Surpass.Utils.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Test
{
    public class EmitTest
    {
        public object Abc(object user, object[] objects)
        {
            UserInfo u = (UserInfo)user;
            int userId = (int)objects[0];
            long state = (long)objects[1];
            return u.Calc(userId, state);
        }

        public void NewUser(object[] objects)
        {
            UserInfo user = new UserInfo((string)objects[0], (string)objects[1]);
        }
    }
}

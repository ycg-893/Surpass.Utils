using NUnit.Framework;
using Surpass.Utils.Net;
using Surpass.Utils.Net.Http;
using Surpass.Utils.Reflection;
using Surpass.Utils.Reflection.Meta;
using Surpass.Utils.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Test.Reflection
{
    /// <summary>
    /// 元属测试
    /// </summary>
    [TestFixture]
    public class MetaTest
    {

        [Test]
        public void PropertyTest()
        {
            var metaObject = MetaObject.GetOrAddCacheMetaObject(typeof(UserInfo));
            Assert.True(metaObject.Propertys.Count > 0);
            UserInfo user = new UserInfo();
            MetaProperty mp;

            if (metaObject.Propertys.TryGetValue("UserId", out mp))
            {
                string userId = "123456";
                mp.SetValue(user, userId);
                mp.SetValue(user, userId);
                Assert.True(user.UserId.Equals(userId));
                Assert.True(mp.GetValue<string>(user).Equals(userId));
            }

            if (metaObject.Propertys.TryGetValue("Id", out mp))
            {
                long userId = 58;
                mp.SetValue(user, userId);
                mp.SetValue(user, userId);
                Assert.True(user.Id.Equals(userId));
                Assert.True(mp.GetValue<long>(user).Equals(userId));
            }
        }

        [Test]
        public void FiledTest()
        {
            var metaObject = MetaObject.GetOrAddCacheMetaObject(typeof(UserInfo));
            Assert.True(metaObject.Fields.Count > 0);
            UserInfo user = new UserInfo();
            MetaField mf;
            if (metaObject.Fields.TryGetValue("UserVersion", out mf))
            {
                string userId = "123456";
                mf.SetValue(user, userId);
                Assert.True(user.UserVersion.Equals(userId));
                Assert.True(mf.GetValue<string>(user).Equals(userId));
            }
        }

        [Test]
        public void MethodTest()
        {
            var metaObject = MetaObject.GetOrAddCacheMetaObject(typeof(UserInfo));
            

            List<UserInfo> users = new List<UserInfo>();



            UserInfo user = new UserInfo();
            Type type = typeof(UserInfo);

            var method = type.GetMethod("Calc");
            var metaMethod = new MetaMethod(method);
            var value = metaMethod.Invoke(user, 1, 2L);


            ConstructorInfo con = type.GetConstructor(Type.EmptyTypes);

            var fun = MethodFactory.CreateConstructorMethod<object>(con);

            var ut = fun(null);

            if (ut != null)
            {

            }

            con = type.GetConstructor(new Type[] { typeof(string) });

            var fun1 = MethodFactory.CreateConstructorMethod<object>(con);

            var ut1 = fun1(new object[] { "aaaaa" });

            if (ut1 != null)
            {

            }


        }

    }
}

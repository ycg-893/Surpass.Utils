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
            UserInfo user = new UserInfo();
            Type type = typeof(UserInfo);
            MethodInfo method;
            MetaMethod metaMethod;
            object value;


            method = type.GetMethod("set_Add");
            metaMethod = new MetaMethod(method);
            value = metaMethod.Invoke(user);

            Assert.True(value == null);

            method = type.GetMethod("Calc");
            metaMethod = new MetaMethod(method);
            value = metaMethod.Invoke(user, 1, 2L);
       

            method = type.GetMethod("Calc1");
            metaMethod = new MetaMethod(method);
            value = metaMethod.Invoke(user, 1, 1);

            //Assert.True(value!=null);

            //method = type.GetMethod("Abc");
            //metaMethod = new MetaMethod(method);
            //value = metaMethod.Invoke(user);
            //Assert.True(value != null);

            method = type.GetMethod("Up");
            metaMethod = new MetaMethod(method);
            value = metaMethod.Invoke(user, 1);
            Assert.True(value != null);
        }

        [Test]
        public void ConstructorTest()
        {
            var metaObject = MetaObject.GetOrAddCacheMetaObject(typeof(UserInfo));

            object obj = metaObject.CreateInstance<UserInfo>();

            if (obj != null)
            {

            }

            var con = metaObject.GetMetaConstructor(typeof(string), typeof(string));

            if (con != null)
            {
                obj = con.CreateInstance("中华人民共和国", "好的");
                if (obj != null)
                {

                }
            }

        }
    }
}

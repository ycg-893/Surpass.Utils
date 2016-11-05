using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Test.Entities
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {


        /// <summary>
        /// 用户版本
        /// </summary>
        public string UserVersion = "1.0";

        /// <summary>
        /// 用户Tag
        /// </summary>
        public readonly string UserTag = "USER";

        public UserInfo()
        {

        }

        public UserInfo(string userId)
        {
            this.UserId = userId;
        }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserName { get; set; }

        public long Id { get; set; }

        public void set_Add()
        {

        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        public void SetState(string userId, int state)
        {

        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state"></param>
        public long Calc(int userId, long state)
        {
            return userId + state;
        }

        /// <summary>
        /// 添加
        /// </summary>      
        public long Abc()
        {
            return 58;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Up<T>(T value)
        {
            return value;
        }

    }
}

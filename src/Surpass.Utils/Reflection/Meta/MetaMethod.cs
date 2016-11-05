using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Reflection.Meta
{
    /// <summary>
    /// 元方法
    /// </summary>
    public class MetaMethod : MetaMember
    {
        /// <summary>
        /// 实例化 MetaMethod 类新实例
        /// </summary>
        /// <param name="methodInfo">方法信息</param>
        public MetaMethod(MethodInfo methodInfo)
            : base(methodInfo)
        {
            this.MemberInfo = methodInfo;
        }
           

        /// <summary>
        /// 获取方法
        /// </summary>       
        public new MethodInfo MemberInfo { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Func<object, object[], object> invoke;

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="args">参数集合</param>
        /// <returns></returns>
        public object Invoke(object instance, params object[] args)
        {
            var call = this.GetRefField<Func<object, object[], object>>(ref this.invoke, () =>
            {
                return MethodFactory.CreateInvokeMethod(this.MemberInfo);
            });
            if (args == null)
            {
                args = new object[0];
            }
            return call(instance, args);
        }

        /// <summary>
        /// 获取元类型
        /// </summary>
        public sealed override MemberType MemberType
        {
            get
            {
                return MemberType.Method;
            }
        }
    }
}

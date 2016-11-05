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
    /// 元构造函数
    /// </summary>
    public class MetaConstructor : MetaMember
    {
        /// <summary>
        /// 实例化 MetaConstructor 类新实例
        /// </summary>
        /// <param name="constructorInfo">构造</param>
        public MetaConstructor(ConstructorInfo constructorInfo)
            : base(constructorInfo)
        {
            this.MemberInfo = constructorInfo;
            var paramInfos = constructorInfo.GetParameters();            
            if (paramInfos.Length == 0)
            {
                this.ConstructorTypeKey = new TypeKey(Type.EmptyTypes);
            }
            else
            {
                List<Type> types = new List<Type>(paramInfos.Length);
                foreach(ParameterInfo info in paramInfos)
                {
                    types.Add(info.ParameterType);
                }
                this.ConstructorTypeKey = new TypeKey(types.ToArray());
            }         
        }

        /// <summary>
        /// 获取构造类型键
        /// </summary>
        public TypeKey ConstructorTypeKey { get; private set; }

        /// <summary>
        /// 获取方法
        /// </summary>       
        public new ConstructorInfo MemberInfo { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Func<object[], object> instance_Build;

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="args">参数集合</param>
        /// <returns></returns>
        public object CreateInstance(params object[] args)
        {
            var call = this.GetRefField<Func<object[], object>>(ref this.instance_Build, () =>
            {
                return MethodFactory.CreateConstructorMethod<object>(this.MemberInfo);
            });
            if (args == null)
            {
                args = new object[0];
            }
            return instance_Build(args);
        }

        /// <summary>
        /// 获取成员类型
        /// </summary>
        public sealed override MemberType MemberType
        {
            get
            {
                return MemberType.Constructor;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Reflection.Meta
{
    /// <summary>
    /// 类型键
    /// </summary>
    public class TypeKey : IEquatable<TypeKey>
    {
        private readonly Type[] types;

        /// <summary>
        /// 实例化 TypeKey 类新实例
        /// </summary>
        /// <param name="types">类型集合</param>
        public TypeKey(Type[] types)
        {
            this.types = ExceptionUtils.CheckNotNull(types, nameof(types));
        }

        /// <summary>
        /// 获取类型集合
        /// </summary>
        public Type[] Types
        {
            get
            {
                return types;
            }
        }

        /// <summary>
        /// 比较两个类型
        /// </summary>
        /// <param name="other">对方</param>
        /// <returns></returns>
        public bool Equals(TypeKey other)
        {
            if (other == null)
            {
                return false;
            }
            if (other.Types.Length != Types.Length)
            {
                return false;
            }
            for (int i = 0; i < other.Types.Length; i++)
            {
                Type t1 = Types[i];
                Type t2 = other.Types[i];
                if (t1 != null && t2 != null)
                {
                    if (!t1.Equals(t2))
                    {
                        return false;
                    }
                }
                else
                {
                    if (t1 == null && t2 == null)
                    {
                        continue;
                    }
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as TypeKey);
        }

        /// <summary>
        /// 哈希
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            const int index = 1;
            const int initialHasCode = 32;
            if (!this.Types.Any())
            {
                return initialHasCode;
            }
            var hashCode = initialHasCode;
            var changeMultiplier = false;
            foreach (var type in this.Types)
            {
                if (type == null)
                {
                    hashCode = hashCode ^ (index * 12);
                    continue;
                }
                hashCode = hashCode * (changeMultiplier ? 58 : 115) + type.GetHashCode();
                changeMultiplier = !changeMultiplier;
            }
            return hashCode;
        }
    }
}

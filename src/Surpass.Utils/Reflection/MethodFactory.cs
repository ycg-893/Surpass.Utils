using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Reflection
{
    /// <summary>
    /// 方法工厂
    /// </summary>
    public static class MethodFactory
    {

        /// <summary>
        /// 创建动态成员
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="returnType">返回类型</param>
        /// <param name="parameterTypes">参数集合</param>
        /// <param name="owner">成员类型</param>
        /// <returns></returns>
        public static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
        {
            DynamicMethod dynamicMethod = !owner.IsInterface
              ? new DynamicMethod(name, returnType, parameterTypes, owner, true)
              : new DynamicMethod(name, returnType, parameterTypes, owner.Module, true);
            return dynamicMethod;
        }

        /// <summary>
        /// 创建属性Set方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyInfo">属性</param>
        /// <returns></returns>
        public static Action<T, object> CreatePropertySet<T>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }
            DynamicMethod dynamicMethod = CreateDynamicMethod("Set" + propertyInfo.Name, typeof(void), new[] { typeof(T), typeof(object) }, propertyInfo.DeclaringType);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);
            if (!setMethod.IsStatic)
            {
                generator.PushInstance(propertyInfo.DeclaringType);
            }
            generator.Emit(OpCodes.Ldarg_1);
            generator.UnboxIfNeeded(propertyInfo.PropertyType);
            generator.CallMethod(setMethod);
            generator.Emit(OpCodes.Ret);
            return (Action<T, object>)dynamicMethod.CreateDelegate(typeof(Action<T, object>));
        }

        /// <summary>
        /// 创建属性Get方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="propertyInfo">属性</param>
        /// <returns></returns>
        public static Func<T, object> CreatePropertyGet<T>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }
            DynamicMethod dynamicMethod = CreateDynamicMethod("Get" + propertyInfo.Name, typeof(T), new[] { typeof(object) }, propertyInfo.DeclaringType);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            MethodInfo getMethod = propertyInfo.GetGetMethod(true);
            if (!getMethod.IsStatic)
            {
                generator.PushInstance(propertyInfo.DeclaringType);
            }
            generator.CallMethod(getMethod);
            generator.BoxIfNeeded(propertyInfo.PropertyType);
            generator.Emit(OpCodes.Ret);
            return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
        }

        /// <summary>
        /// 创建字段Set方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="fieldInfo">字段</param>
        /// <returns></returns>
        public static Action<T, object> CreateFieldSet<T>(FieldInfo fieldInfo)
        {
            DynamicMethod dynamicMethod = CreateDynamicMethod("Set" + fieldInfo.Name, null, new Type[] { typeof(T), typeof(object) }, fieldInfo.DeclaringType);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            if (!fieldInfo.IsStatic)
            {
                generator.PushInstance(fieldInfo.DeclaringType);
            }
            generator.Emit(OpCodes.Ldarg_1);
            generator.UnboxIfNeeded(fieldInfo.FieldType);
            generator.Emit(OpCodes.Stfld, fieldInfo);
            generator.Emit(OpCodes.Ret);
            return (Action<T, object>)dynamicMethod.CreateDelegate(typeof(Action<T, object>));
        }

        /// <summary>
        /// 创建字段Get方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="fieldInfo">字段</param>
        /// <returns></returns>
        public static Func<T, object> CreateFieldGet<T>(FieldInfo fieldInfo)
        {
            DynamicMethod dynamicMethod = CreateDynamicMethod("Get" + fieldInfo.Name, typeof(T), new[] { typeof(object) }, fieldInfo.DeclaringType);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            if (!fieldInfo.IsStatic)
            {
                generator.PushInstance(fieldInfo.DeclaringType);
            }
            generator.Emit(OpCodes.Ldfld, fieldInfo);
            generator.BoxIfNeeded(fieldInfo.FieldType);
            generator.Emit(OpCodes.Ret);
            return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
        }

        /// <summary>
        /// 调用成员
        /// </summary>
        /// <param name="generator">指令</param>
        /// <param name="methodInfo">方法元素</param>
        public static void CallMethod(this ILGenerator generator, MethodInfo methodInfo)
        {
            if (methodInfo.IsFinal || !methodInfo.IsVirtual)
            {
                generator.Emit(OpCodes.Call, methodInfo);
            }
            else
            {
                generator.Emit(OpCodes.Callvirt, methodInfo);
            }
        }
    }
}

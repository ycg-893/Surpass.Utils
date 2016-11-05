﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// <param name="owner">动态方法与其逻辑关联</param>
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
            ExceptionUtils.CheckNotNull(propertyInfo, nameof(propertyInfo));
            DynamicMethod dynamicMethod = CreateDynamicMethod("Set" + propertyInfo.Name, typeof(void), new[] { typeof(T), typeof(object) }, propertyInfo.ReflectedType);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);
            if (!setMethod.IsStatic)
            {
                generator.PushInstance(propertyInfo.ReflectedType);
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
            ExceptionUtils.CheckNotNull(propertyInfo, nameof(propertyInfo));
            DynamicMethod dynamicMethod = CreateDynamicMethod("Get" + propertyInfo.Name, typeof(T), new[] { typeof(object) }, propertyInfo.ReflectedType);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            MethodInfo getMethod = propertyInfo.GetGetMethod(true);
            if (!getMethod.IsStatic)
            {
                generator.PushInstance(propertyInfo.ReflectedType);
            }
            generator.CallMethod(getMethod);
            generator.BoxIfNeeded(propertyInfo.PropertyType);
            generator.Emit(OpCodes.Ret);
            return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
        }

        private static ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "instance");
        private static ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

        /// <summary>
        /// 创建构造方法
        /// </summary>       
        /// <param name="constructorInfo">构造</param>
        /// <returns></returns>
        public static Func<object[], T> CreateConstructorMethod<T>(ConstructorInfo constructorInfo)
        {
            ExceptionUtils.CheckNotNull(constructorInfo, nameof(constructorInfo));
            ParameterInfo[] paramInfos = constructorInfo.GetParameters();
            Expression instanceExpression;
            if (paramInfos.Length == 0)
            {
                instanceExpression = Expression.New(constructorInfo);
                if (constructorInfo.ReflectedType.IsValueType)
                {
                    instanceExpression = Expression.Convert(instanceExpression, typeof(object));
                }
                Func<T> result = Expression.Lambda<Func<T>>(instanceExpression).Compile();
                return (o) =>
                {
                    return result();
                };
            }
            List<Expression> parameterExpressions = new List<Expression>();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);
                parameterExpressions.Add(valueCast);
            }
            instanceExpression = Expression.New(constructorInfo, parameterExpressions);
            if (constructorInfo.ReflectedType.IsValueType)
            {
                instanceExpression = Expression.Convert(instanceExpression, typeof(object));
            }
            var lambda = Expression.Lambda<Func<object[], T>>(instanceExpression, parametersParameter);
            return lambda.Compile();
        }


        private static Type[] GetMethodParameterTypes(MethodInfo methodInfo)
        {
            var paramArray = methodInfo.GetParameters();
            var types = new Type[paramArray.Length];
            for (int i = 0; i < paramArray.Length; i++)
            {
                types[i] = paramArray[i].ParameterType;
            }
            return types;
        }

        /// <summary>
        /// 创建调用方法
        /// </summary>       
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static Func<object, object[], object> CreateInvokeMethod(MethodInfo methodInfo)
        {
            ExceptionUtils.CheckNotNull(methodInfo, nameof(methodInfo));
            //DynamicMethod dynamicMethod = CreateDynamicMethod(methodInfo.Name, typeof(object), GetMethodParameterTypes(methodInfo),
            //    methodInfo.ReflectedType);
            //ILGenerator generator = dynamicMethod.GetILGenerator();

            //if (!methodInfo.IsStatic)
            //{
            //    generator.PushInstance(methodInfo.ReflectedType);
            //}
            //ParameterInfo[] paramInfos = methodInfo.GetParameters();
            //foreach (var info in paramInfos)
            //{
            //    generator.DeclareLocal(info.ParameterType);
            //}
            //for (int i = 0; i < paramInfos.Length; ++i)
            //{
            //    var parameterType = paramInfos[i].ParameterType;
            //    generator.Ldarg(0);
            //    generator.EmitIndex(i);
            //    generator.Emit(parameterType.GetLdelem());
            //    generator.BoxIfNeeded(parameterType);
            //    generator.StoreStloc(i);
            //}
            //for (int i = 0; i < paramInfos.Length; ++i)
            //{
            //    generator.LoadStloc(i);
            //}

            //if (methodInfo.ReturnType == typeof(void))
            //{
            //    generator.Emit(OpCodes.Ldnull);
            //}

            //generator.CallMethod(methodInfo);
            //generator.BoxIfNeeded(methodInfo.ReturnType);
            //generator.Emit(OpCodes.Ret);
            //return (Func<object, object[], object>)dynamicMethod.CreateDelegate(typeof(Func<object, object[], object>));


            List<Expression> parameterExpressions = new List<Expression>();
            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);
                parameterExpressions.Add(valueCast);
            }
            Expression instanceCast = methodInfo.IsStatic ? null : Expression.Convert(instanceParameter, methodInfo.ReflectedType);
            MethodCallExpression methodCall = Expression.Call(instanceCast, methodInfo, parameterExpressions);
            if (methodCall.Type == typeof(void))
            {
                Expression<Action<object, object[]>> lambda = Expression.Lambda<Action<object, object[]>>(methodCall, instanceParameter, parametersParameter);
                Action<object, object[]> execute = lambda.Compile();
                return (instance, parameters) =>
                    {
                        execute(instance, parameters);
                        return null;
                    };
            }
            else
            {
                UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));
                Expression<Func<object, object[], object>> lambda = Expression.Lambda<Func<object, object[], object>>(castMethodCall, instanceParameter, parametersParameter);
                return lambda.Compile();
            }
        }

        /// <summary>
        /// 创建字段Set方法
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="fieldInfo">字段</param>
        /// <returns></returns>
        public static Action<T, object> CreateFieldSet<T>(FieldInfo fieldInfo)
        {
            ExceptionUtils.CheckNotNull(fieldInfo, nameof(fieldInfo));
            DynamicMethod dynamicMethod = CreateDynamicMethod("Set" + fieldInfo.Name, null, new Type[] { typeof(T), typeof(object) }, fieldInfo.ReflectedType);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            if (!fieldInfo.IsStatic)
            {
                generator.PushInstance(fieldInfo.ReflectedType);
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
            ExceptionUtils.CheckNotNull(fieldInfo, nameof(fieldInfo));
            DynamicMethod dynamicMethod = CreateDynamicMethod("Get" + fieldInfo.Name, typeof(T), new[] { typeof(object) }, fieldInfo.ReflectedType);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            if (!fieldInfo.IsStatic)
            {
                generator.PushInstance(fieldInfo.ReflectedType);
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

using Surpass.Utils.Properties;
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
    /// Emit 工厂
    /// </summary>
    public static class EmitFactory
    {
        /// <summary>
        /// 整数压入堆栈方法
        /// </summary>
        /// <param name="il">指令</param>
        /// <param name="value">值</param>
        public static void EmitInt32(this ILGenerator il, int value)
        {
            switch (value)
            {
                case -1: il.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (value >= -128 && value <= 127)
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }

        /// <summary>
        /// 压入整数堆栈方法
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public static OpCode GetLdc_I4(int index)
        {
            if (index == 0)
                return OpCodes.Ldc_I4_0;
            if (index == 1)
                return OpCodes.Ldc_I4_1;
            if (index == 2)
                return OpCodes.Ldc_I4_2;
            if (index == 3)
                return OpCodes.Ldc_I4_3;
            if (index == 4)
                return OpCodes.Ldc_I4_4;
            if (index == 5)
                return OpCodes.Ldc_I4_5;
            if (index == 6)
                return OpCodes.Ldc_I4_6;
            if (index == 7)
                return OpCodes.Ldc_I4_7;
            if (index == 8)
                return OpCodes.Ldc_I4_8;
            if (index > 8)
                return OpCodes.Ldc_I4_S;

            throw new ArgumentException(string.Format(Resources.UnknownLdcId, index));
        }

        /// <summary>
        /// 根据传入的数据类型，获取数组元素的op
        /// </summary>
        /// <param name="reflectType">类型</param>
        /// <returns></returns>
        public static OpCode GetLdelem(this Type reflectType)
        {
            if (!reflectType.IsValueType)
                return OpCodes.Ldelem_Ref;

            if (typeof(byte).Equals(reflectType))
                return OpCodes.Ldelem_I1;

            if (typeof(short).Equals(reflectType))
                return OpCodes.Ldelem_I2;

            if (typeof(Int32).Equals(reflectType))
                return OpCodes.Ldelem_I4;

            if (typeof(Int64).Equals(reflectType))
                return OpCodes.Ldelem_I8;

            if (typeof(float).Equals(reflectType))
                return OpCodes.Ldelem_R4;

            if (typeof(double).Equals(reflectType))
                return OpCodes.Ldelem_R8;

            return OpCodes.Ldelem_Ref;
        }

        /// <summary>
        /// 判断是否需要装箱
        /// </summary>
        /// <param name="generator">指令</param>
        /// <param name="type">类型</param>
        public static void BoxIfNeeded(this ILGenerator generator, Type type)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Box, type);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>
        /// 判断是否需要拆箱
        /// </summary>
        /// <param name="generator">指令</param>
        /// <param name="type">类型</param>
        public static void UnboxIfNeeded(this ILGenerator generator, Type type)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>
        /// 压入实例
        /// </summary>
        /// <param name="generator">指令</param>
        /// <param name="type">类型</param>
        public static void PushInstance(this ILGenerator generator, Type type)
        {
            generator.Emit(OpCodes.Ldarg_0);
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Unbox, type);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, type);
            }
        }

        

        /// <summary>
        /// 将指定索引变量推到当前堆栈上
        /// </summary>
        /// <param name="generator">指令</param>
        /// <param name="slot">索引(0表示 this)</param>
        public static void Ldarg(this ILGenerator generator, int slot)
        {
            switch (slot)
            {
                case 0:
                    generator.Emit(OpCodes.Ldarg_0);
                    return;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    return;
                case 2:
                    generator.Emit(OpCodes.Ldarg_2);
                    return;
                case 3:
                    generator.Emit(OpCodes.Ldarg_3);
                    return;
            }
            if (slot <= 0xff)
            {
                generator.Emit(OpCodes.Ldarg_S, slot);
            }
            else
            {
                generator.Emit(OpCodes.Ldarg, slot);
            }
        }

        /// <summary>
        /// 从当前方法中返回
        /// </summary>
        /// <param name="generator">指令</param>
        public static void Ret(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// 用新值替换在对象引用或指针的字段中存储的值。
        /// </summary>
        /// <param name="generator">指令</param>
        /// <param name="field">字段</param>
        public static void Stfld(this ILGenerator generator, FieldBuilder field)
        {
            generator.Emit(OpCodes.Stfld, field);
        }

        /// <summary>
        /// 查找对象中其引用当前位于计算堆栈的字段的值。
        /// </summary>
        /// <param name="generator">指令</param>
        /// <param name="field">字段</param>
        public static void Ldfld(this ILGenerator generator, FieldBuilder field)
        {
            generator.Emit(OpCodes.Ldfld, field);
        }
    }
}

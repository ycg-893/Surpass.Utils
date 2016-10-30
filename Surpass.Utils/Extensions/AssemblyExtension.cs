using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils
{
    /// <summary>
    /// 程序集扩展
    /// </summary>
    public static class AssemblyExtension
    {
        private static string GetAssemblyPath(Assembly assembly, bool full)
        {
            ExceptionUtils.CheckNotNull(assembly, nameof(assembly));
            string codeBase = assembly.CodeBase;
            codeBase = codeBase.Substring(8, codeBase.Length - 8);
            string[] arrSection = codeBase.Split(new char[] { '/' });
            StringBuilder path = new StringBuilder();
            int length = full ? arrSection.Length : arrSection.Length - 1;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    path.Append("\\");
                }
                path.Append(arrSection[i]);
            }
            return path.ToString();
        }

        /// <summary>
        /// 获取程序集目录路径
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns></returns>
        public static string AssemblyFolderPath(this Assembly assembly)
        {
            return AssemblyExtension.GetAssemblyPath(assembly, false);
        }

        /// <summary>
        /// 获取程序集文件路径
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns></returns>
        public static string AssemblyFilePath(this Assembly assembly)
        {
            return AssemblyExtension.GetAssemblyPath(assembly, true);
        }



    }
}

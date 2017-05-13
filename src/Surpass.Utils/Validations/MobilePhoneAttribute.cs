using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Validations
{
    /// <summary>
    /// 移动电话验证
    /// </summary>
    public class MobilePhoneAttribute : ValidationAttribute
    {
        /// <summary>
        /// 手机号码开头
        /// </summary>
        public const string StartNumberString = "130;131;132;152;155;156;185;186;"
            + "134;135;136;137;138;139;150;151;157;158;159;187;188;"
            + "133;153;180;189;"
            + "170;171";

        /// <summary>
        /// 实例化 MobilePhoneAttribute 类新实例，默认11位长度
        /// </summary>
        public MobilePhoneAttribute()
            : this(11, StartNumberString.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {

        }

        /// <summary>
        /// 实例化 MobilePhoneAttribute 类新实例
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="startNumbers">开头数字</param>
        public MobilePhoneAttribute(int length, params string[] startNumbers)
        {
            Length = length;
            if (startNumbers != null)
            {
                StartNumbers = new HashSet<string>(startNumbers);
            }
            else
            {
                StartNumbers = new HashSet<string>();
            }
        }

        /// <summary>
        /// 获取开头
        /// </summary>
        public ISet<string> StartNumbers { get; private set; }

        /// <summary>
        /// 获长度
        /// </summary>
        public int Length { get; private set; }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            var str = value.ToString();
            if (str.Length == 0)
            {
                return true;
            }
            var mob = "手机号码[" + str + "]";
            if (str.Length < 3)
            {
                throw new ValidationException(mob + "格式不正确");
            }
            if (str.Length != Length)
            {
                throw new ValidationException(mob + "长度必须为 " + Length + " 位");
            }
            if (!StartNumbers.Contains(str.Left(3)))
            {
                throw new ValidationException(mob + "开头数字不符合规则限定");
            }
            if (!str.IsALLNumber())
            {
                throw new ValidationException(mob + "含用非数字规字符");
            }
            return true;
        }

    }
}

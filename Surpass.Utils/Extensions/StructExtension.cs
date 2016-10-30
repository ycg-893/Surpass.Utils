using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils
{
    /// <summary>
    /// 结构类型扩展
    /// </summary>
    public static class StructExtension
    {

        #region DateTime


        /// <summary>
        /// 转换为日期
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this double value)
        {
            return DateTime.FromOADate(value);
        }

        /// <summary>
        /// 获取本年初日期
        /// </summary>
        /// <param name="value">日期</param>
        /// <returns></returns>
        public static DateTime YearBeginDate(this DateTime value)
        {
            return new DateTime(value.Year, 1, 1);
        }

        /// <summary>
        /// 获取本年底日期
        /// </summary>
        /// <param name="value">日期</param>
        /// <returns></returns>
        public static DateTime YearEndDate(this DateTime value)
        {
            return new DateTime(value.Year, 12, 31);
        }

        /// <summary>
        /// 获取上年初日期
        /// </summary>
        /// <param name="value">日期</param>
        /// <returns></returns>
        public static DateTime PreviousYearBeginDate(this DateTime value)
        {
            return value.AddYears(-1).YearBeginDate();
        }

        /// <summary>
        /// 获取上年底日期
        /// </summary>
        /// <param name="value">日期</param>
        /// <returns></returns>
        public static DateTime PreviousYearEndDate(this DateTime value)
        {
            return value.AddYears(-1).YearEndDate();
        }

        /// <summary>
        /// 昨天
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime PreviousDay(this DateTime value)
        {
            return value.AddDays(-1);
        }

        /// <summary>
        /// 明天
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime NextDay(this DateTime value)
        {
            return value.AddDays(1);
        }

        /// <summary>
        /// 获取本月底日期
        /// </summary>
        /// <param name="value">日期</param>
        /// <returns></returns>
        public static DateTime MonthEndDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month));
        }

        /// <summary>
        /// 获取指定日期的月天数
        /// </summary>
        /// <param name="value">日期</param>
        /// <returns></returns>
        public static int MonthDays(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }


        /// <summary>
        /// 获取本月初日期
        /// </summary>
        /// <param name="value">日期</param>
        /// <returns></returns>
        public static DateTime MonthBeginDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        /// <summary>
        /// 上月初
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime PreviousMonthBeginDate(this DateTime value)
        {
            return value.AddMonths(-1).MonthBeginDate();
        }

        /// <summary>
        /// 上月末
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime PreviousMonthEndDate(this DateTime value)
        {
            return value.AddMonths(-1).MonthEndDate();
        }

        /// <summary>
        /// 下月初
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime NextMonthBeginDate(this DateTime value)
        {
            return value.AddMonths(1).MonthBeginDate();
        }

        /// <summary>
        /// 下月末
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime NextMonthEndDate(this DateTime value)
        {
            return value.AddMonths(1).MonthEndDate();
        }

        /// <summary>
        /// 获取本周开始日期
        /// </summary>
        /// <param name="value">日期</param>
        /// <returns></returns>
        public static DateTime WeekBeginDate(this DateTime value)
        {
            return value.AddDays(-Convert.ToInt32(value.DayOfWeek));
        }

        /// <summary>
        /// 获取本周结束日期
        /// </summary>
        /// <param name="value">日期</param>
        /// <returns></returns>
        public static DateTime WeekEndDate(this DateTime value)
        {
            return value.WeekBeginDate().AddDays(6);
        }

        /// <summary>
        /// 上周开始日期
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime PreviousWeekBeginDate(this DateTime value)
        {
            return value.WeekBeginDate().AddDays(-1).WeekBeginDate();
        }

        /// <summary>
        /// 上周结束日期
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime PreviousWeekEndDate(this DateTime value)
        {
            return value.WeekBeginDate().AddDays(-1).WeekEndDate();
        }

        /// <summary>
        /// 下周开始日期
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime NextWeekBeginDate(this DateTime value)
        {
            return value.WeekEndDate().AddDays(1).WeekBeginDate();
        }

        /// <summary>
        /// 下周结束日期
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime NextWeekEndDate(this DateTime value)
        {
            return value.WeekEndDate().AddDays(1).WeekEndDate();
        }

        /// <summary>
        /// 获取季度的开始日期
        /// </summary>
        /// <param name="YearID">年度Id</param>
        /// <param name="QuarterOrderID">季度顺序Id</param>
        /// <returns></returns>
        public static DateTime QuarterBeginDate(int YearID, int QuarterOrderID)
        {
            switch (QuarterOrderID)
            {
                case 1:
                    return new DateTime(YearID, 1, 1);
                case 2:
                    return new DateTime(YearID, 4, 1);
                case 3:
                    return new DateTime(YearID, 7, 1);
                case 4:
                    return new DateTime(YearID, 10, 1);
                default:
                    throw new ArgumentException("季度必须介于1-4之间。");
            }
        }

        /// <summary>
        /// 获取季度的开始日期
        /// </summary>
        /// <param name="YearID">年度Id</param>
        /// <param name="QuarterOrderID">季度顺序Id</param>
        /// <returns></returns>
        public static DateTime QuarterEndDate(int YearID, int QuarterOrderID)
        {
            switch (QuarterOrderID)
            {
                case 1:
                    return new DateTime(YearID, 3, 31);
                case 2:
                    return new DateTime(YearID, 6, 30);
                case 3:
                    return new DateTime(YearID, 9, 30);
                case 4:
                    return new DateTime(YearID, 12, 31);
                default:
                    throw new ArgumentException("季度必须介于1-4之间。");
            }
        }

        /// <summary>
        /// 根据月份获取季度顺序id
        /// </summary>
        /// <param name="MonthID">月份</param>
        /// <returns></returns>
        public static int QuarterOrderID(int MonthID)
        {
            if (MonthID >= 1 && MonthID <= 12)
            {
                if (MonthID >= 1 && MonthID <= 3)
                {
                    return 1;
                }
                else if (MonthID >= 4 && MonthID <= 6)
                {
                    return 2;
                }
                else if (MonthID >= 7 && MonthID <= 10)
                {
                    return 3;
                }
                else
                {
                    return 4;
                }
            }
            else
            {
                throw new ArgumentException("月份必须介于1-12之间。");
            }
        }

        /// <summary>
        /// 根据月份获取上一季度季度顺序id
        /// </summary>
        /// <param name="YearID">年度</param>
        /// <param name="MonthID">月份</param>
        /// <param name="OutYearID">输出新年度</param>
        /// <returns></returns>
        public static int PreviousQuarterOrderID(int YearID, int MonthID, out int OutYearID)
        {
            int orderID = QuarterOrderID(MonthID);
            if (orderID == 1)
            {
                OutYearID = YearID - 1;
                return 4;
            }
            else
            {
                OutYearID = YearID;
                return orderID - 1;
            }
        }

        /// <summary>
        /// 根据月份获取下一季度季度顺序id
        /// </summary>
        /// <param name="YearID">年度</param>
        /// <param name="MonthID">月份</param>
        /// <param name="OutYearID">输出新年度</param>
        /// <returns></returns>
        public static int NextQuarterOrderID(int YearID, int MonthID, out int OutYearID)
        {
            int orderID = QuarterOrderID(MonthID);
            if (orderID == 4)
            {
                OutYearID = YearID + 1;
                return 4;
            }
            else
            {
                OutYearID = YearID;
                return orderID + 1;
            }
        }

        /// <summary>
        /// 对两个日期之间的月边界进行计数
        /// </summary>
        /// <param name="startDate">时间段的起始日期</param>
        /// <param name="endDate">时间段的结束日期</param>
        /// <returns></returns>
        public static int DateDiffMonths(this DateTime startDate, DateTime endDate)
        {
            if (endDate >= startDate)
            {
                return Months(startDate, endDate);
            }
            else
            {
                return Months(endDate, startDate) * -1;
            }
        }

        static int Months(DateTime startDate, DateTime endDate)
        {
            return ((endDate.Year - startDate.Year) * 12) + (endDate.Month - startDate.Month);
        }

        /// <summary>
        /// 对两个日期之间的日边界进行计数
        /// </summary>
        /// <param name="startDate">时间段的起始日期</param>
        /// <param name="endDate">时间段的结束日期</param>
        /// <returns></returns>
        public static int DateDiffDays(this DateTime startDate, DateTime endDate)
        {
            return endDate.Subtract(startDate).Days;
        }

        /// <summary>
        /// 获取日期月总数
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static int DateMonths(this DateTime value)
        {
            return DateMonths(value.Year, value.Month);
        }

        /// <summary>
        /// 获取日期月份总数
        /// </summary>
        /// <param name="year">年度</param>
        /// <param name="month">月度</param>
        /// <returns></returns>
        public static int DateMonths(int year, int month)
        {
            if (year < 1 || year > 9999)
            {
                throw new ArgumentOutOfRangeException("year 小于 1 或大于 9999。");
            }
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException("month 小于 1 或大于 12。");
            }
            return (year - 1) * 12 + month;
        }

        #endregion

        #region Abs

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static sbyte Abs(this sbyte value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static short Abs(this short value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static int Abs(this int value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static long Abs(this long value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static float Abs(this float value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static double Abs(this double value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static decimal Abs(this decimal value)
        {
            return Math.Abs(value);
        }

        #endregion

        #region Round

        /// <summary>
        /// 将小数值舍入到最接近的整数值。
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static decimal Round(this decimal value)
        {
            return Math.Round(value);
        }

        /// <summary>
        /// 将小数值按指定的小数位数舍入。
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="decimals">返回值中的小数位数</param>
        /// <returns></returns>
        public static decimal Round(this decimal value, int decimals)
        {
            return Math.Round(value, decimals);
        }

        /// <summary>
        /// 将双精浮点数值舍入到最接近的整数值。
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static double Round(this double value)
        {
            return Math.Round(value);
        }

        /// <summary>
        /// 将双精浮点数值按指定的小数位数舍入。
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="decimals">返回值中的小数位数</param>
        /// <returns></returns>
        public static double Round(this double value, int decimals)
        {
            return Math.Round(value, decimals);
        }

        /// <summary>
        /// 将单精浮点数值舍入到最接近的整数值。
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static float Round(this float value)
        {
            return Convert.ToSingle(Math.Round(value));
        }

        /// <summary>
        /// 将单精浮点数值按指定的小数位数舍入。
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="decimals">返回值中的小数位数</param>
        /// <returns></returns>
        public static float Round(this float value, int decimals)
        {
            return Convert.ToSingle(Math.Round(value, decimals));
        }

        #endregion

        #region Ceiling

        /// <summary>
        /// 返回大于或等于指定的十进制数的最小整数值。
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static decimal Ceiling(this decimal value)
        {
            return Math.Ceiling(value);
        }

        /// <summary>
        /// 返回大于或等于指定的单精浮点数的最小整数值。
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static float Ceiling(this float value)
        {
            return Convert.ToSingle(Math.Ceiling(value));
        }

        /// <summary>
        /// 返回大于或等于指定的双精浮点数的最小整数值。
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static double Ceiling(this double value)
        {
            return Math.Ceiling(value);
        }

        #endregion

        #region Floor

        /// <summary>
        /// 返回小于或等于指定的十进制数的最小整数值。
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static decimal Floor(this decimal value)
        {
            return Math.Floor(value);
        }

        /// <summary>
        /// 返回小于或等于指定的单精浮点数的最小整数值。
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static float Floor(this float value)
        {
            return Convert.ToSingle(Math.Floor(value));
        }

        /// <summary>
        /// 返回小于或等于指定的双精浮点数的最小整数值。
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static double Floor(this double value)
        {
            return Math.Floor(value);
        }

        #endregion

    }
}

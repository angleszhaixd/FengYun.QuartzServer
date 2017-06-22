using System.Text;
namespace System
{
    public static class ObjectExtensions
    {
        /*
        /// <summary>
        /// 如果对象为null则返回指定的nullString
        /// </summary>
        public static string ToDefaultString(this object obj, string nullString = "")
        {
            return obj == null ? nullString : obj.ToString().Trim().Default(nullString);
        }
        /// <summary>
        /// 如果对象为null则返回指定的nullString
        /// </summary>
        public static string ToDateString(this DateTime? obj, string format = "yyyy-MM-dd")
        {
            return obj == null ? string.Empty : DateTime.Parse(obj.ToString()).ToString(format);
        }

        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool IsNullOrEmpty(this object obj)
        {
            return obj == null || obj.ToString().IsNullOrEmpty();
        }

        public static bool IsNotNullOrEmpty(this object obj)
        {
            return obj != null && obj.ToDefaultString() != "";
        }

        public static T Default<T>(this T obj, T defaultValue)
            where T : class
        {
            return obj ?? defaultValue;
        }

        /// <summary>
        /// 类型转换（转换失败返回Null）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>转换失败返回Null</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return obj as T;
        }

        /// <summary>
        /// 强制类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <exception cref="InvalidCastException">
        /// 转换失败抛出异常
        /// </exception>
        /// <returns></returns>
        public static T Cast<T>(this object obj)
        {
            return (T)obj;
        }
        */
    }
}
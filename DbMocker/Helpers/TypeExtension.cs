using System;

namespace Apps72.Dev.Data.DbMocker.Helpers
{
    /// <summary />
    internal static class TypeExtension
    {
        /// <summary>
        /// Returns True if this object is a simple type.
        /// See https://msdn.microsoft.com/en-us/library/system.type.isprimitive.aspx
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitive(this Type type)
        {
            return type == typeof(DateTime) || type == typeof(Nullable<DateTime>) ||
                   type == typeof(Decimal) || type == typeof(Nullable<Decimal>) ||
                   type == typeof(String) ||
                   type == typeof(Boolean) || type == typeof(Nullable<Boolean>) ||
                   type == typeof(Byte) || type == typeof(Nullable<Byte>) ||
                   type == typeof(SByte) || type == typeof(Nullable<SByte>) ||
                   type == typeof(Int16) || type == typeof(Nullable<Int16>) ||
                   type == typeof(UInt16) || type == typeof(Nullable<UInt16>) ||
                   type == typeof(Int32) || type == typeof(Nullable<Int32>) ||
                   type == typeof(UInt32) || type == typeof(Nullable<UInt32>) ||
                   type == typeof(Int64) || type == typeof(Nullable<Int64>) ||
                   type == typeof(UInt64) || type == typeof(Nullable<UInt64>) ||
                   type == typeof(IntPtr) || type == typeof(Nullable<IntPtr>) ||
                   type == typeof(UIntPtr) || type == typeof(Nullable<UIntPtr>) ||
                   type == typeof(Char) || type == typeof(Nullable<Char>) ||
                   type == typeof(Double) || type == typeof(Nullable<Double>) ||
                   type == typeof(Single) || type == typeof(Nullable<Single>);
        }

        /// <summary>
        /// Returns the best data type infered from the value text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Type BestType(this string text)
        {
            if (DateTime.TryParse(text, out DateTime _)) return typeof(DateTime);
            if (Boolean.TryParse(text, out Boolean _)) return typeof(DateTime);
            if (Int32.TryParse(text, out Int32 _)) return typeof(Int32);
            if (Int64.TryParse(text, out Int64 _)) return typeof(Int64);
            if (Double.TryParse(text, out Double _)) return typeof(Double);
            if (Decimal.TryParse(text, out Decimal _)) return typeof(Decimal);
            if (Char.TryParse(text, out Char _)) return typeof(Char);
            return typeof(string);
        }
    }
}

namespace Fluent.Extensions
{
    using System.Windows;

    internal static class StructExtensions
    {
        public static bool IsZero(this Thickness thickness)
        {
            return thickness.Left == 0 && thickness.Top == 0.0 && thickness.Right == 0 && thickness.Bottom == 0;
        }

        public static Thickness Add(this Thickness thickness, double value)
        {
            thickness.Left += value;
            thickness.Top += value;
            thickness.Right += value;
            thickness.Bottom += value;
            return thickness;
        }

        public static Thickness Scale(this Thickness thickness, double scale)
        {
            thickness.Left *= scale;
            thickness.Top *= scale;
            thickness.Right *= scale;
            thickness.Bottom *= scale;
            return thickness;
        }
    }
}
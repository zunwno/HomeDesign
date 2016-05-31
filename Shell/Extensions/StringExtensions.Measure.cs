using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Shell.Extensions
{
    public static partial class StringExtensions
    {
        /// <summary>
        /// Get the required height and width of the specified text. Uses FortammedText
        /// </summary>
        public static Size MeasureTextSize(this string text, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize)
        {
            FormattedText ft = new FormattedText(text,
                                                 CultureInfo.CurrentCulture,
                                                 FlowDirection.LeftToRight,
                                                 new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                                                 fontSize,
                                                 Brushes.Black);
            return new Size(ft.Width, ft.Height);
        }

        /// <summary>
        /// Get the required height and width of the specified text. Uses Glyph's
        /// </summary>
        public static Size MeasureText(this string text, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize)
        {
            Typeface typeface = new Typeface(fontFamily, fontStyle, fontWeight, fontStretch);
            GlyphTypeface glyphTypeface;

            if (!typeface.TryGetGlyphTypeface(out glyphTypeface))
            {
                return MeasureTextSize(text, fontFamily, fontStyle, fontWeight, fontStretch, fontSize);
            }

            double totalWidth = 0;
            double height = 0;

            for (int n = 0; n < text.Length; n++)
            {
                ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];

                double width = glyphTypeface.AdvanceWidths[glyphIndex] * fontSize;

                double glyphHeight = glyphTypeface.AdvanceHeights[glyphIndex] * fontSize;

                if (glyphHeight > height)
                {
                    height = glyphHeight;
                }

                totalWidth += width;
            }

            return new Size(totalWidth, height);
        }
    }
}

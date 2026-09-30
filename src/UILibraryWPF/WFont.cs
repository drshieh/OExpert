using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OExpert.UILibraryWPF
{
    public class WFont
    {
        public static List<string> FontFamilyList;
        public static List<FontFamily> ffList = Fonts.SystemFontFamilies.ToList<FontFamily>();
        public static List<string> FontSizeList = new List<string>() { "6", "6.5", "7", "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" };

        //--------------------------------------------------------
        //--Get the Font Families 
        //-- This is the name of font
        public static List<string> GetFontFamilies()
        {
            if (FontFamilyList != null) return FontFamilyList;
            FontFamilyList = new List<string>();
            foreach (FontFamily a in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                FontFamilyList.Add(a.Source);
                foreach (FamilyTypeface f in a.FamilyTypefaces)
                {
                    string sStyle = f.Style + "-" + f.Weight + "-" + f.Stretch;
                }
            }
            return FontFamilyList;
        }

        //--------------------------------------------------------
        //--Get the Font Families 
        //-- This is the name of font
        public static FontFamily GetFontFamily(string familyName)
        {
            string familyNameU = familyName.ToUpper();
            foreach (FontFamily a in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                if (a.Source.ToUpper() == familyNameU) return a;
            }
            return null;
        }

        //--------------------------------------------------------
        //--Get the Font proper name from Font Families 
        //-- This is the name of font
        public static string GetFontName(string familyName)
        {
            string familyNameU = familyName.ToUpper();
            foreach (FontFamily a in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                if (a.Source.ToUpper() == familyNameU) return a.Source;
            }
            return "Arial";
        }

        //--------------------------------------------------------
        //--Get the Font Families 
        //-- This is the name of font
        public static List<string> GetFontFamilyTypefaces(string familyName)
        {
            List<string> aTypefaceList = new List<string>();
            string familyNameU = familyName.ToUpper();
            foreach (FontFamily a in Fonts.SystemFontFamilies)
            {
                // FontFamily.Source contains the font family name.
                if (a.Source.ToUpper() == familyNameU)
                {
                    foreach (FamilyTypeface f in a.FamilyTypefaces)
                    {
                        aTypefaceList.Add(f.Style + "-" + f.Weight + "-" + f.Stretch);
                    }
                    return aTypefaceList;
                }
            }
            return null;
        }

        #region -- Font
        //--------------------------------------------------------
        //--Get the Font Families 
        //-- This is the name of font
        public static int GetFontIndex(string familyName)
        {
            string familyNameU = familyName.ToUpper();
            if (FontFamilyList == null) GetFontFamilies();
            for (int i = 0; i < FontFamilyList.Count; i++)
            {
                // FontFamily.Source contains the font family name.
                if (FontFamilyList[i].ToUpper() == familyNameU) return i;
            }
            return 0;
        }

        //--------------------------------------------------------
        //--Get the Font Families name 
        //-- This is the name of font
        public static string GetFontName(int iFont)
        {
            if (iFont < 0 || iFont >= FontFamilyList.Count) return null;
            else return FontFamilyList[iFont];
        }

        //--------------------------------------------------------
        public static void GetFitFont(double width, double height, string sFontName, ref double iFontSize, string sTypefaceStyle, string str)
        {
            FontFamily aFontFamily = GetFontFamily("Arial");
            if (!String.IsNullOrEmpty(sFontName) && sFontName != "Arial") aFontFamily = GetFontFamily(sFontName);

            int iFontTypeFace = DFontEnum.GetIndexDTypefaceStyle(sTypefaceStyle);
            List<Typeface> tfList = aFontFamily.GetTypefaces().ToList<Typeface>();

            double iFSize = 2;
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            iFSize = (double)((int)Math.Min(width, height)+1);
            while (iFSize >= 2)
            {
                FormattedText x = new FormattedText(str, culture, FlowDirection.LeftToRight, tfList[iFontTypeFace], iFSize, brush);
                if (x.Width <= width && x.Height <= height)
                {
                    break;
                }
                iFSize -= 0.25;
            }
            iFontSize = iFSize;
            if (iFontSize < 2) iFontSize = 2;
        }

        //--------------------------------------------------------
        public static double GetFitFont(double width, double height, XFont aXFont, string str)
        {
            FontFamily aFontFamily = GetFontFamily("Arial");
            if (!String.IsNullOrEmpty(aXFont.FontName) && aXFont.FontName != "Arial") aFontFamily = GetFontFamily(aXFont.FontName);

            int iFontTypeFace = DFontEnum.GetIndexDTypefaceStyle(aXFont.FontTypeFace);
            List<Typeface> tfList = aFontFamily.GetTypefaces().ToList<Typeface>();

            double iFSize = 2;
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            iFSize = (double)((int)Math.Min(width, height) + 1);

            FontWeight aFontWeight = DFontEnum.GetValueDFontWeight(aXFont.FontWeight);
            FontStretch aFontStretch = DFontEnum.GetValueDFontStretch(aXFont.FontStretch);
            TextDecorationCollection aTextDecorations = DFontEnum.GetValueDTextDecoration(aXFont.FontTextDecoration);
            FontStyle aTypefaceStyle = DFontEnum.GetValueDTypefaceStyle(aXFont.FontTypeFace);

            while (iFSize >= 2)
            {
                FormattedText x = new FormattedText(str, culture, FlowDirection.LeftToRight, tfList[iFontTypeFace], iFSize, brush);

                x.SetFontWeight(aFontWeight);
                if (aTextDecorations != null) x.SetTextDecorations(aTextDecorations);
                x.SetFontStretch(aFontStretch);
                x.SetFontStyle(aTypefaceStyle);

                if (x.Width <= width && x.Height <= height)
                {
                    break;
                }
                iFSize -= 0.25;
            }

            if (iFSize < 2) iFSize = 2;
            return iFSize;
        }

        //--------------------------------------------------------
        public static void GetFontCharSize(string sFontName, double iFontSize, string sTypefaceStyle, string str, ref double cx, ref double cy)
        {
            FontFamily aFontFamily = GetFontFamily("Arial");
            if (!String.IsNullOrEmpty(sFontName))
            {
                FontFamily aFontFamilyX = GetFontFamily(sFontName);
                if (aFontFamilyX != null) aFontFamily = aFontFamilyX;
            }

            int iFontTypeFace = DFontEnum.GetIndexDTypefaceStyle(sTypefaceStyle);
            if (iFontTypeFace < 0) iFontTypeFace = 0;
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            List<Typeface> tfList = aFontFamily.GetTypefaces().ToList<Typeface>();
            FormattedText x = new FormattedText(str, culture, FlowDirection.LeftToRight, tfList[iFontTypeFace], iFontSize, brush);
            cx = x.Width;
            cy = x.Height;
        }

        //------------------------------------------------------------
        public static void PrintFonts()
        {
            int i = 0;

            foreach (FontFamily ff in ffList)
            {
                Console.WriteLine("{0} FontFamily SOURCE: [{1}]", i, ff.Source);
                int k = 0;
                foreach (var a in ff.FamilyNames)
                {
                    Console.WriteLine("  {0} FamilyName: {1} : {2}", k, a.Key, a.Value);
                    k++;
                }
                i++;
            }

        }
        #endregion

        #region -- FontStyle = iFaceStyle + iWeight * 100 + iStretch * 10000 + iTextDecoration * 1000000
        //--------------------------------------------------------
        public static int ParseFontStyle(string sstr, char cSep, XFont aXFont)
        {
            string[] subx = sstr.Split(cSep);
            int iTFStyle = DFontEnum.GetIndexDTypefaceStyle(subx[0]);
            if (iTFStyle < 0) iTFStyle = 0;
            aXFont.FontTypeFace = DFontEnum.GetNameDTypefaceStyle(iTFStyle);
            aXFont.FontWeight = "Normal";
            if (subx.Length > 1) aXFont.FontWeight = DFontEnum.GetNameDFontWeight(DFontEnum.GetIndexDFontWeight(subx[1]));
            aXFont.FontStretch = "Normal";
            if (subx.Length > 2) aXFont.FontStretch = DFontEnum.GetNameDFontStretch(DFontEnum.GetIndexDFontStretch(subx[2]));
            aXFont.FontTextDecoration = "None";
            if (subx.Length > 3) aXFont.FontTextDecoration = DFontEnum.GetNameDFontStretch(DFontEnum.GetIndexDTextDecoration(subx[3]));
            return 1;
        }

        //--------------------------------------------------------
        public static int SetFontStyle(int iStyle, string sOption, string sValue)
        {
            int iTypefaceStyle = 0;
            int iWeight = 0;
            int iStretch = 0;
            int iTextDecoration = 0;
            GetFontStyleIndexX(iStyle, ref iTypefaceStyle, ref iWeight, ref iStretch, ref iTextDecoration);
            switch (sOption)
            {
                case "FontWeight":
                    iWeight = DFontEnum.GetIndexDFontWeight(sValue);
                    break;
                case "FontStretch":
                    iStretch = DFontEnum.GetIndexDFontStretch(sValue);
                    break;
                case "TextDecoration":
                    iTextDecoration = DFontEnum.GetIndexDTextDecoration(sValue);
                    break;
                case "TypefaceStyle":
                    iTypefaceStyle = DFontEnum.GetIndexDTypefaceStyle(sValue);
                    break;
                default:
                    break;
            }
            return iTypefaceStyle + iWeight * 100 + iStretch * 10000 + iTextDecoration * 1000000;
        }

        //--------------------------------------------------------
        // iStyle = iFaceStyle + iWeight * 100 + iStretch * 10000 + iTextDecoration * 1000000
        public static void GetFontStyleIndexX(int iStyle, ref int iFStyle, ref int iWeight, ref int iStretch, ref int iTextDecoration)
        {
            if (iStyle <= 0) return;
            iTextDecoration = iStyle / 1000000;
            iStretch = (iStyle - iTextDecoration * 1000000) / 10000;
            iWeight = (iStyle - iTextDecoration * 1000000 - iStretch * 10000) / 100;
            iFStyle = iStyle - iTextDecoration * 1000000 - iStretch * 10000 - iWeight * 100;
        }

        //--------------------------------------------------------
        // iStyle = iFaceStyle + iWeight * 100 + iStretch * 10000 + iTextDecoration * 1000000
        public static void GetFontStyle(int iStyle, ref FontStyle aTypefaceStyle, ref FontWeight aWeight, ref FontStretch aStretch, ref TextDecorationCollection aTextDecoration)
        {
            int iTypefaceStyle = 0;
            int iTextDecoration = 0;
            int iStretch = 0;
            int iWeight = 0;
            GetFontStyleIndexX(iStyle, ref iTypefaceStyle, ref iWeight, ref iStretch, ref iTextDecoration);
            aTypefaceStyle = DFontEnum.GetValueDTypefaceStyle(iTypefaceStyle);
            aWeight = DFontEnum.GetValueDFontWeight(iWeight);
            aStretch = DFontEnum.GetValueDFontStretch(iStretch);
            aTextDecoration = DFontEnum.GetValueDTextDecoration(iTextDecoration);
        }

        //--------------------------------------------------------
        public static void GetFontStyle(XFont aXFont, ref FontStyle aTypefaceStyle, ref FontWeight aWeight, ref FontStretch aStretch, ref TextDecorationCollection aTextDecoration)
        {
            aTypefaceStyle = DFontEnum.GetValueDTypefaceStyle(aXFont.FontTypeFace);
            aWeight = DFontEnum.GetValueDFontWeight(aXFont.FontWeight);
            aStretch = DFontEnum.GetValueDFontStretch(aXFont.FontStretch);
            aTextDecoration = DFontEnum.GetValueDTextDecoration(aXFont.FontTextDecoration);
        }

        //--------------------------------------------------------
        public static string GetFontStyleStr(XFont aXFont)
        {
            return aXFont.FontTypeFace + "|" + aXFont.FontWeight + "|" + aXFont.FontStretch + "|" + aXFont.FontTextDecoration;
        }

        //--------------------------------------------------------
        public static bool ParseFontStyleStr(string sStyle, XFont aXFont)
        {
            string[] subx = sStyle.Split('|');

            aXFont.FontTypeFace = subx[0].Trim();
            aXFont.FontWeight = subx[1].Trim();
            aXFont.FontStretch = subx[2].Trim();
            aXFont.FontTextDecoration = subx[3].Trim();
            return true;
        }

        #endregion

    }

}

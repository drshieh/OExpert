using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
//using System.Windows;

namespace OExpert.UILibraryWPF
{
    public class DFontEnum
    {
        #region --TextDecoration
        public static List<DTextDecoration> DTextDecorationList = new List<DTextDecoration>() {
            new DTextDecoration("None", null),
            new DTextDecoration("Baseline", TextDecorations.Baseline),
            new DTextDecoration("OverLine",  TextDecorations.OverLine),
            new DTextDecoration("Strikethrough", TextDecorations.Strikethrough),
            new DTextDecoration("Underline",TextDecorations.Underline)
        };

        //-----------------------------------------------------
        public static List<string> ListNameDTextDecoration()
        {
            List<string> aList = new List<string>();
            foreach (DTextDecoration a in DTextDecorationList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        //-----------------------------------------------------
        public static int GetIndexDTextDecoration(string sName)
        {
            int i = 0;
            foreach (DTextDecoration a in DTextDecorationList)
            {
                if (a.Name == sName) return i;
                i++;
            }
            return -1;
        }

        //-----------------------------------------------------
        public static string GetNameDTextDecoration(TextDecorationCollection v)
        {
            foreach (DTextDecoration a in DTextDecorationList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        //-----------------------------------------------------
        public static string GetNameDTextDecoration(int index)
        {
            if (index < 0 || index >= DTextDecorationList.Count) return null;
            else return DTextDecorationList[index].Name;
        }

        //-----------------------------------------------------
        public static TextDecorationCollection GetValueDTextDecoration(string name)
        {
            foreach (DTextDecoration a in DTextDecorationList)
            {
                if (a.Name == name) return a.Value;
            }
            return null;
        }

        //-----------------------------------------------------
        public static TextDecorationCollection GetValueDTextDecoration(int index)
        {
            if (index < 0 || index >= DTextDecorationList.Count) return null;
            else return DTextDecorationList[index].Value;
        }
        #endregion

        #region --FontWeight

        public static List<DFontWeight> DFontWeightList = new List<DFontWeight>(){
            new DFontWeight("Normal", FontWeights.Normal), //--Specifies a "Normal" font weight.
            new DFontWeight("Black", FontWeights.Black),  //--Specifies a "Black" font weight.
            new DFontWeight("Bold", FontWeights.Bold), //--Specifies a "Bold" font weight.
            new DFontWeight("DemiBold", FontWeights.DemiBold), //--Specifies a "Demi-bold" font weight.
            new DFontWeight("ExtraBlack", FontWeights.ExtraBlack), //--Specifies an "Extra-black" font weight.
            new DFontWeight("ExtraBold", FontWeights.ExtraBold), //--Specifies an "Extra-bold" font weight.
            new DFontWeight("ExtraLight", FontWeights.ExtraLight), //--Specifies an "Extra-light" font weight.
            new DFontWeight("Heavy", FontWeights.Heavy), //--Specifies a "Heavy" font weight.
            new DFontWeight("Light", FontWeights.Light), //--Specifies a "Light" font weight.
            new DFontWeight("Medium", FontWeights.Medium), //--Specifies a "Medium" font weight.
            new DFontWeight("Regular", FontWeights.Regular), //--Specifies a "Regular" font weight.
            new DFontWeight("SemiBold", FontWeights.SemiBold), //--Specifies a "Semi-bold" font weight.
            new DFontWeight("Thin", FontWeights.Thin), //--Specifies a "Thin" font weight.
            new DFontWeight("UltraBlack", FontWeights.UltraBlack), //--Specifies an "Ultra-black" font weight.
            new DFontWeight("UltraBold", FontWeights.UltraBold), //--Specifies an "Ultra-bold" font weight.
            new DFontWeight("UltraLight", FontWeights.UltraLight)  //--Specifies an "Ultra-light" font weight.
        };

        //-----------------------------------------------------
        public static List<string> ListNameDFontWeight()
        {
            List<string> aList = new List<string>();
            foreach (DFontWeight a in DFontWeightList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        //-----------------------------------------------------
        public static int GetIndexDFontWeight(string sName)
        {
            int i = 0;
            foreach (DFontWeight a in DFontWeightList)
            {
                if (a.Name == sName) return i;
                i++;
            }
            return -1;
        }

        //-----------------------------------------------------
        public static string GetNameDFontWeight(FontWeight v)
        {
            foreach (DFontWeight a in DFontWeightList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        //-----------------------------------------------------
        public static string GetNameDFontWeight(int index)
        {
            if (index < 0 || index >= DFontWeightList.Count) return null;
            else return DFontWeightList[index].Name;
        }

        //-----------------------------------------------------
        public static FontWeight GetValueDFontWeight(string name)
        {
            foreach (DFontWeight a in DFontWeightList)
            {
                if (a.Name == name) return a.Value;
            }
            return FontWeights.Normal;
        }

        //-----------------------------------------------------
        public static FontWeight GetValueDFontWeight(int index)
        {
            if (index < 0 || index >= DFontWeightList.Count) return FontWeights.Normal;
            else return DFontWeightList[index].Value;
        }
        #endregion

        #region --FontStretch
        //--------------------------------------------------------
        public static List<DFontStretch> DFontStretchList = new List<DFontStretch>() {
            new DFontStretch("Normal", FontStretches.Normal),
            new DFontStretch("Condensed", FontStretches.Condensed),
            new DFontStretch("Expanded", FontStretches.Expanded),
            new DFontStretch("ExtraCondensed", FontStretches.ExtraCondensed),
            new DFontStretch("ExtraExpanded", FontStretches.ExtraExpanded),
            new DFontStretch("Medium", FontStretches.Medium),
            new DFontStretch("SemiCondensed", FontStretches.SemiCondensed),
            new DFontStretch("SemiExpanded", FontStretches.SemiExpanded),
            new DFontStretch("UltraCondensed", FontStretches.UltraCondensed),
            new DFontStretch("UltraExpanded", FontStretches.UltraExpanded)
        };

        //-----------------------------------------------------
        public static List<string> ListNameDFontStretch()
        {
            List<string> aList = new List<string>();
            foreach (DFontStretch a in DFontStretchList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        //-----------------------------------------------------
        public static int GetIndexDFontStretch(string sName)
        {
            int i = 0;
            foreach (DFontStretch a in DFontStretchList)
            {
                if (a.Name == sName) return i;
                i++;
            }
            return -1;
        }

        //-----------------------------------------------------
        public static string GetNameDFontStretch(FontStretch v)
        {
            foreach (DFontStretch a in DFontStretchList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        //-----------------------------------------------------
        public static string GetNameDFontStretch(int index)
        {
            if (index < 0 || index >= DFontStretchList.Count) return null;
            else return DFontStretchList[index].Name;
        }

        //-----------------------------------------------------
        public static FontStretch GetValueDFontStretch(string name)
        {
            foreach (DFontStretch a in DFontStretchList)
            {
                if (a.Name == name) return a.Value;
            }
            return FontStretches.Normal;
        }

        //-----------------------------------------------------
        public static FontStretch GetValueDFontStretch(int index)
        {
            if (index < 0 || index >= DFontStretchList.Count) return FontStretches.Normal;
            else return DFontStretchList[index].Value;
        }
        #endregion

        #region --Typeface Style

        public static List<DTypefaceStyle> DTypefaceStyleList = new List<DTypefaceStyle>() {
            new DTypefaceStyle("Normal", FontStyles.Normal),
            new DTypefaceStyle("Italic", FontStyles.Italic),
            new DTypefaceStyle("Oblique", FontStyles.Oblique)
        };

        //-----------------------------------------------------
        public static List<string> ListNameDTypefaceStyle()
        {
            List<string> aList = new List<string>();
            foreach (DTypefaceStyle a in DTypefaceStyleList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        //-----------------------------------------------------
        public static int GetIndexDTypefaceStyle(string sName)
        {
            int i = 0;
            foreach (DTypefaceStyle a in DTypefaceStyleList)
            {
                if (a.Name == sName) return i;
                i++;
            }
            return -1;
        }

        //-----------------------------------------------------
        public static string GetNameDTypefaceStyle(FontStyle v)
        {
            foreach (DTypefaceStyle a in DTypefaceStyleList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        //-----------------------------------------------------
        public static string GetNameDTypefaceStyle(int index)
        {
            if (index < 0 || index >= DTypefaceStyleList.Count) return null;
            else return DTypefaceStyleList[index].Name;
        }

        //-----------------------------------------------------
        public static FontStyle GetValueDTypefaceStyle(string name)
        {
            foreach (DTypefaceStyle a in DTypefaceStyleList)
            {
                if (a.Name == name) return a.Value;
            }
            return FontStyles.Normal;
        }

        //-----------------------------------------------------
        public static FontStyle GetValueDTypefaceStyle(int index)
        {
            if (index < 0 || index >= DTypefaceStyleList.Count) return FontStyles.Normal;
            else return DTypefaceStyleList[index].Value;
        }
        #endregion

        #region --TextAlignment
        public static List<DTextAlignment> DTextAlignmentList = new List<DTextAlignment>() {
            new DTextAlignment("Left", TextAlignment.Left),
            new DTextAlignment("Right", TextAlignment.Right),
            new DTextAlignment("Center",  TextAlignment.Center),
            new DTextAlignment("Justify", TextAlignment.Justify)
        };

        //-----------------------------------------------------
        public static List<string> ListNameDTextAlignment()
        {
            List<string> aList = new List<string>();
            foreach (DTextAlignment a in DTextAlignmentList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        //-----------------------------------------------------
        public static int GetIndexDTextAlignment(string sName)
        {
            int i = 0;
            foreach (DTextAlignment a in DTextAlignmentList)
            {
                if (a.Name == sName) return i;
                i++;
            }
            return -1;
        }

        //-----------------------------------------------------
        public static string GetNameDTextAlignment(TextAlignment v)
        {
            foreach (DTextAlignment a in DTextAlignmentList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        //-----------------------------------------------------
        public static string GetNamDTextAlignment(int index)
        {
            if (index < 0 || index >= DTextAlignmentList.Count) return null;
            else return DTextAlignmentList[index].Name;
        }

        //-----------------------------------------------------
        public static TextAlignment GetValueDTextAlignment(string name)
        {
            foreach (DTextAlignment a in DTextAlignmentList)
            {
                if (a.Name == name) return a.Value;
            }
            return TextAlignment.Left;
        }

        //-----------------------------------------------------
        public static TextAlignment GetValueDTextAlignment(int index)
        {
            if (index < 0 || index >= DTextAlignmentList.Count) return TextAlignment.Left;
            else return DTextAlignmentList[index].Value;
        }
        #endregion

    }

    //=========================================================
    public class DTextDecoration
    {
        public string Name;
        public TextDecorationCollection Value;

        public DTextDecoration()
        {
        }
        public DTextDecoration(string _name, TextDecorationCollection _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    //=========================================================
    public class DFontWeight
    {
        public string Name;
        public FontWeight Value;

        public DFontWeight()
        {
        }
        public DFontWeight(string _name, FontWeight _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    //=========================================================
    public class DFontStretch
    {
        public string Name;
        public FontStretch Value;

        public DFontStretch()
        {
        }
        public DFontStretch(string _name, FontStretch _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    //=========================================================
    public class DTypefaceStyle
    {
        public string Name;
        public FontStyle Value;

        public DTypefaceStyle()
        {
        }
        public DTypefaceStyle(string _name, FontStyle _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    //=========================================================
    public class DTextAlignment
    {
        public string Name;
        public TextAlignment Value;

        public DTextAlignment()
        {
        }
        public DTextAlignment(string _name, TextAlignment _value)
        {
            Name = _name;
            Value = _value;
        }
    }

}

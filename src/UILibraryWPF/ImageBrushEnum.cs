using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace OExpert.UILibraryWPF
{
    public class ImageBrushEnum
    {
        public static List<DAlignmentX> DAlignmentXList = new List<DAlignmentX> { new DAlignmentX("Center", AlignmentX.Center),
                                                                                  new DAlignmentX("Left",  AlignmentX.Left),
                                                                                  new DAlignmentX("Right", AlignmentX.Right) };
        public static List<DAlignmentY> DAlignmentYList = new List<DAlignmentY> { new DAlignmentY("Bottom", AlignmentY.Bottom),
                                                                                  new DAlignmentY("Center", AlignmentY.Center),
                                                                                  new DAlignmentY("Top", AlignmentY.Top)};
        public static List<DStretch> DStretchList = new List<DStretch> { new DStretch("Fill", Stretch.Fill),
                                                                         new DStretch("None", Stretch.None),
                                                                         new DStretch("Uniform", Stretch.Uniform),
                                                                         new DStretch("UniformToFill", Stretch.UniformToFill) };
        public static List<DTileMode> DTileModeList = new List<DTileMode> { new DTileMode("FlipX", TileMode.FlipX),
                                                                            new DTileMode("FlipXY", TileMode.FlipXY),
                                                                            new DTileMode("FlipY", TileMode.FlipY),
                                                                            new DTileMode("None", TileMode.None),
                                                                            new DTileMode("Tile", TileMode.Tile) };
        public static List<DBrushMappingMode> DBrushMappingModeList = new List<DBrushMappingMode> { new DBrushMappingMode("Absolute", BrushMappingMode.Absolute),
                                                                                                    new DBrushMappingMode("RelativeToboundingBox", BrushMappingMode.RelativeToBoundingBox) };
        public static List<DGradientSpreadMethod> DGradientSpreadMethodList = new List<DGradientSpreadMethod> { new DGradientSpreadMethod("Pad", GradientSpreadMethod.Pad),
                                                                                                                new DGradientSpreadMethod("Reflect", GradientSpreadMethod.Reflect),
                                                                                                                new DGradientSpreadMethod("Repeat", GradientSpreadMethod.Repeat) };
        public static List<DColorInterpolationMode> DColorInterpolationModeList = new List<DColorInterpolationMode> { new DColorInterpolationMode("ScRgbLinearInterpolation", ColorInterpolationMode.ScRgbLinearInterpolation ),   //-Colors are interpolated in the scRGB color space 
                                                                                                                      new DColorInterpolationMode("SRgbLinearInterpolation", ColorInterpolationMode.SRgbLinearInterpolation)};    //-Colors are interpolated in the sRGB color space 

        #region --AlignmentX
        public static List<string> ListNameDAlignmentX()
        {
            List<string> aList = new List<string>();
            foreach (DAlignmentX a in DAlignmentXList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        public static string GetNameDAlignmentX(AlignmentX v)
        {
            foreach (DAlignmentX a in DAlignmentXList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        public static AlignmentX GetValueDAlignmentX(string name)
        {
            foreach (DAlignmentX a in DAlignmentXList)
            {
                if (a.Name == name) return a.Value;
            }
            return AlignmentX.Left;
        }
        #endregion

        #region --AlignmentY
        public static List<string> ListNameDAlignmentY()
        {
            List<string> aList = new List<string>();
            foreach (DAlignmentY a in DAlignmentYList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        public static string GetNameDAlignmentY(AlignmentY v)
        {
            foreach (DAlignmentY a in DAlignmentYList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        public static AlignmentY GetValueDAlignmentY(string name)
        {
            foreach (DAlignmentY a in DAlignmentYList)
            {
                if (a.Name == name) return a.Value;
            }
            return AlignmentY.Bottom;
        }
        #endregion

        #region --Stretch
        public static List<string> ListNameDStretch()
        {
            List<string> aList = new List<string>();
            foreach (DStretch a in DStretchList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        public static string GetNameDStretch(Stretch v)
        {
            foreach (DStretch a in DStretchList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        public static Stretch GetValueDStretch(string name)
        {
            foreach (DStretch a in DStretchList)
            {
                if (a.Name == name) return a.Value;
            }
            return Stretch.None;
        }
        #endregion

        #region --TileMode
        public static List<string> ListNameDTileMode()
        {
            List<string> aList = new List<string>();
            foreach (DTileMode a in DTileModeList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        public static string GetNameDTileMode(TileMode v)
        {
            foreach (DTileMode a in DTileModeList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        public static TileMode GetValueDTileMode(string name)
        {
            foreach (DTileMode a in DTileModeList)
            {
                if (a.Name == name) return a.Value;
            }
            return TileMode.None;
        }
        #endregion

        #region --BrushMappingMode
        public static List<string> ListNameDBrushMappingMode()
        {
            List<string> aList = new List<string>();
            foreach (DBrushMappingMode a in DBrushMappingModeList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        public static string GetNameDBrushMappingMode(BrushMappingMode v)
        {
            foreach (DBrushMappingMode a in DBrushMappingModeList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        public static BrushMappingMode GetValueDBrushMappingMode(string name)
        {
            foreach (DBrushMappingMode a in DBrushMappingModeList)
            {
                if (a.Name == name) return a.Value;
            }
            return BrushMappingMode.Absolute;
        }
        #endregion

        #region --GradientSpreadMethod
        public static List<string> ListNameDGradientSpreadMethod()
        {
            List<string> aList = new List<string>();
            foreach (DGradientSpreadMethod a in DGradientSpreadMethodList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        public static string GetNameDGradientSpreadMethod(GradientSpreadMethod v)
        {
            foreach (DGradientSpreadMethod a in DGradientSpreadMethodList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        public static GradientSpreadMethod GetValueDGradientSpreadMethod(string name)
        {
            foreach (DGradientSpreadMethod a in DGradientSpreadMethodList)
            {
                if (a.Name == name) return a.Value;
            }
            return GradientSpreadMethod.Pad;
        }
        #endregion

        #region --ColorInterpolationMode
        public static List<string> ListNameDColorInterpolationMode()
        {
            List<string> aList = new List<string>();
            foreach (DColorInterpolationMode a in DColorInterpolationModeList)
            {
                aList.Add(a.Name);
            }
            return aList;
        }

        public static string GetNameDColorInterpolationMode(ColorInterpolationMode v)
        {
            foreach (DColorInterpolationMode a in DColorInterpolationModeList)
            {
                if (a.Value == v) return a.Name;
            }
            return null;
        }

        public static ColorInterpolationMode GetValueDColorInterpolationMode(string name)
        {
            foreach (DColorInterpolationMode a in DColorInterpolationModeList)
            {
                if (a.Name == name) return a.Value;
            }
            return ColorInterpolationMode.ScRgbLinearInterpolation;
        }
        #endregion
    }


    //-------------------------------------------------------
    public class DAlignmentX
    {
        public string Name;
        public AlignmentX Value;

        public DAlignmentX()
        {
        }
        public DAlignmentX(string _name, AlignmentX _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    //-------------------------------------------------------
    public class DAlignmentY
    {
        public string Name;
        public AlignmentY Value;

        public DAlignmentY()
        {
        }
        public DAlignmentY(string _name, AlignmentY _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    //-------------------------------------------------------
    public class DStretch
    {
        public string Name;
        public Stretch Value;

        public DStretch()
        {
        }
        public DStretch(string _name, Stretch _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    //-------------------------------------------------------
    public class DTileMode
    {
        public string Name;
        public TileMode Value;

        public DTileMode()
        {
        }
        public DTileMode(string _name, TileMode _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    //-------------------------------------------------------
    public class DBrushMappingMode
    {
        public string Name;
        public BrushMappingMode Value;

        public DBrushMappingMode()
        {
        }
        public DBrushMappingMode(string _name, BrushMappingMode _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    //-------------------------------------------------------
    public class DGradientSpreadMethod
    {
        public string Name;
        public GradientSpreadMethod Value;

        public DGradientSpreadMethod()
        {
        }
        public DGradientSpreadMethod(string _name, GradientSpreadMethod _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    //-------------------------------------------------------
    public class DColorInterpolationMode
    {
        public string Name;
        public ColorInterpolationMode Value;

        public DColorInterpolationMode()
        {
        }
        public DColorInterpolationMode(string _name, ColorInterpolationMode _value)
        {
            Name = _name;
            Value = _value;
        }
    }

}

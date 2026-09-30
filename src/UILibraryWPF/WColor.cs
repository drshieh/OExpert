using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Reflection;
using System.IO;

namespace OExpert.UILibraryWPF
{
    public class WColor
    {
        //============================================COLOR=============================
        public static List<SColor> sysColorList;
        public static List<string> sysColorNameList;

        //------------------------------------------------------------
        public static List<string> GetColorNameList()
        {
            sysColorNameList = new List<string>();
            if (sysColorList == null) GetColorList();
            foreach (SColor c in sysColorList)
            {
                sysColorNameList.Add(c.aName);
            }
            return sysColorNameList;
        }

        //------------------------------------------------------------
        //--Get the System Colors
        public static void GetColorList()
        {
            Type colorsType = typeof(System.Windows.Media.Colors);
            PropertyInfo[] colorsTypePropertyInfos = colorsType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            sysColorNameList = new List<string>();
            sysColorList = new List<SColor>();
            foreach (PropertyInfo colorsTypePropertyInfo in colorsTypePropertyInfos)
            {
                var x = colorsTypePropertyInfo.GetValue("Color", null);
                sysColorList.Add(new SColor(colorsTypePropertyInfo.Name, (Color)colorsTypePropertyInfo.GetValue("Color", null)));
                sysColorNameList.Add(colorsTypePropertyInfo.Name);
            }
            Console.WriteLine("WColor.GetColorList: {0}", sysColorNameList.Count);
            for(int i=0;i<sysColorNameList.Count;i++)
            {
                Console.WriteLine(" {0} : {1} ", i, sysColorNameList[i]);
            }
        }

        //-----------------------------------------------------
        //--Find the System Color (SColor) by name
        public static SColor FindSColor(string cName)
        {
            if (String.IsNullOrEmpty(cName)) return null;
            if (sysColorList == null) GetColorList();
            foreach (SColor c in sysColorList)
            {
                if (c.aName.ToUpper() == cName.ToUpper()) return c;
            }
            return null;
        }

        //-----------------------------------------------------
        //--Find the System Color (SColor) by name
        public static string GetColorName(int icolor)
        {
            if (sysColorList == null) GetColorList();
            if (icolor >= 0 && icolor < sysColorList.Count) return sysColorList[icolor].aName;
            else return null;
        }

        //-----------------------------------------------------
        //--Find the System Color (SColor) by name, the input color name may not be the right name
        public static string GetColorName(string scolor, string sDefaultColor)
        {
            if (sysColorList == null) GetColorList();
            if (String.IsNullOrEmpty(scolor)) return sDefaultColor;

            string scolorU = scolor.ToUpper();
            foreach (SColor c in sysColorList)
            {
                if (c.aName.ToUpper() == scolorU) return c.aName;
            }
            //--Try fuzze match 1
            foreach (SColor c in sysColorList)
            {
                if (c.aName.ToUpper().IndexOf(scolorU) >= 0) return c.aName;
            }
            //--Try fuzze match 2
            foreach (SColor c in sysColorList)
            {
                if (scolorU.IndexOf(c.aName.ToUpper()) >= 0) return c.aName;
            }
            Console.WriteLine("WARNING: GetColorName: {0} not found, use default: {1}", scolor, sDefaultColor);
            return sDefaultColor;
        }

        //-----------------------------------------------------
        //--Find the System Color (SColor) by name, the input color name may not be the right name
        public static string GetColorName(string scolor)
        {
            if (sysColorList == null) GetColorList();
            if (String.IsNullOrEmpty(scolor)) return "";

            string scolorU = scolor.ToUpper();
            foreach (SColor c in sysColorList)
            {
                if (c.aName.ToUpper() == scolorU) return c.aName;
            }
            //--Try fuzze match 1
            foreach (SColor c in sysColorList)
            {
                if (c.aName.ToUpper().IndexOf(scolorU) >= 0) return c.aName;
            }
            //--Try fuzze match 2
            foreach (SColor c in sysColorList)
            {
                if (scolorU.IndexOf(c.aName.ToUpper()) >= 0) return c.aName;
            }
            return scolor;
        }

        //-----------------------------------------------------
        //--Find the System Color (SColor) by name
        public static string GetColorName(Color color)
        {
            if (sysColorList == null) GetColorList();
            foreach (SColor c in sysColorList)
            {
                if (c.aColor == color) return c.aName;
            }
            return null;
        }

        //-----------------------------------------------------
        //--Find the System Color (SColor) by name
        public static string GetColorName(Brush b)
        {
            if (sysColorList == null) GetColorList();
            SolidColorBrush scb = b as SolidColorBrush;
            return GetColorName(scb.Color);
        }

        //-----------------------------------------------------
        //--Get Color (Color) by name
        public static Color GetColor(string colorName)
        {
            if (colorName.StartsWith("#"))
            {
                object o = ColorConverter.ConvertFromString(colorName);
                if (o != null) return (Color)o;
            }
            if (sysColorList == null) GetColorList();
            SColor c = FindSColor(colorName);
            if (c == null) return Colors.Transparent;
            else return c.aColor;
        }

        //-----------------------------------------------------
        //--Get Color (Color) by name
        public static int GetColorIndex(string colorName, string defaultColor)
        {
            if (sysColorList == null) GetColorList();
            if (colorName == "-" || colorName.ToUpper() == "NONE") return -1;
            SColor c = FindSColor(colorName);
            if (c == null)
            {
                c = FindSColor(defaultColor);
                return sysColorList.IndexOf(c);
            }
            else return sysColorList.IndexOf(c);
        }

        //-----------------------------------------------------
        //--Get Color (Color) by idx
        public static Color GetColor(int idx)
        {
            if (sysColorList == null) GetColorList();
            if (idx < 0 || idx >= sysColorList.Count) return Colors.Transparent;
            return sysColorList[idx].aColor;
        }

        //-----------------------------------------------------
        //--Get Color (Color) by a, r, g, b
        public static Color GetColor(int a, int r, int g, int b)
        {
            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }

        //----------------------------------------------------
        // Set a Color to the sysColorList using the ARGB values
        // ARGB values is 0 - 255
        //  If the color Name exists in the SysColorList, the color value will be replace
        //  otherwise, a new SColor is added to the list
        public static SColor SetColorByARGB(string colorName, int a, int r, int g, int b)
        {
            Color c = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
            SColor sc = null;
            if ((sc = FindSColor(colorName)) != null)
            {
                sc.aColor = c;
            }
            else
            {
                sc = new SColor();
                sc.aColor = c;
                sc.aName = colorName;
                sysColorList.Add(sc);
                sysColorNameList.Add(colorName);
            }
            return sc;
        }

        //----------------------------------------------------
        // Read from a file and add to the SysColorList
        // ColorName = Existing ColorName | (a, r, g, b)
        // ARGB values is 0 - 255
        //  If the color Name exists in the SysColorList, the color value will be replace
        //  otherwise, a new SColor is added to the list
        public static void ReadFile(string sFileName)
        {
            if (!File.Exists(sFileName))
            {
                Console.WriteLine("ReadColorFile: {0} does not exist!", sFileName);
                return;
            }
            StreamReader sr = new StreamReader(sFileName);
            string str = "";
            string colorName = "";
            string sValue = "";
            while (!sr.EndOfStream)
            {
                str = sr.ReadLine().Trim();
                if (String.IsNullOrEmpty(str) || str.StartsWith("//") || str.StartsWith("/*")) continue;
                string[] subx = str.Split('=');
                if (subx.Length < 2) continue;
                colorName = subx[0].Trim();
                sValue = subx[1].Trim();
                if (sValue.StartsWith("("))
                {
                    string strx = sValue.Replace("(", "").Replace(")", "").Trim();
                    string[] subxx = strx.Split(',');
                    int a = 0;
                    int r = 0;
                    int g = 0;
                    int b = 0;
                    a = Int32.Parse(subxx[0]);
                    if (subxx.Length > 1) r = Int32.Parse(subxx[1]);
                    if (subxx.Length > 2) g = Int32.Parse(subxx[2]);
                    if (subxx.Length > 3) b = Int32.Parse(subxx[3]);
                    SetColorByARGB(colorName, a, r, g, b);
                    Console.WriteLine("Added Color: {0}", str);
                }
                else
                {
                    SColor sc = null;
                    SColor tc = FindSColor(colorName);
                    if ((sc = FindSColor(sValue)) != null)  //--Value Color good
                    {
                        if (tc == null)
                        {
                            tc = new SColor(sValue, sc.aColor);
                            sysColorList.Add(tc);
                            sysColorNameList.Add(colorName);
                            Console.WriteLine("Added Color: {0}", str);
                        }
                        else tc.aColor = sc.aColor;
                    }
                }
            }
            sr.Close();
            Console.WriteLine("WColor.ReadFile: {0} => {1}", sFileName, sysColorNameList.Count);
            for (int i = 0; i < sysColorNameList.Count; i++)
            {
                Console.WriteLine(" {0}> COLOR: {1}", i, sysColorNameList[i]);
            }
        }

        //----------------------------------------------------
        // Get the ARGB values of the color by name
        // the return value is the full int value of the ARGB
        public static int GetARGB(string colorName, ref int a, ref int r, ref int g, ref int b)
        {
            SColor sc = FindSColor(colorName);
            if (sc == null) return 0;
            a = sc.aColor.A;
            r = sc.aColor.R;
            g = sc.aColor.G;
            b = sc.aColor.B;
            return (a | r | g | b);
        }
    }

    //=====================================================
    //-SColor to hold the color name and Color value
    public class SColor
    {
        public string aName;
        public Color aColor;

        public SColor()
        {
        }

        public SColor(string n, Color c)
        {
            aName = n;
            aColor = c;
        }
    }

}

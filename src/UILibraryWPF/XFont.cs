using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OExpert.UILibraryWPF
{
    public class XFont
    {
        public string FontName;    //--font Name
        public string FontTypeFace;    //--Font Type Face Style
        public double FontSize;    //--Font size
        public string FontWeight;    //--Font Weight
        public string FontStretch;    //--Font Stretch
        public string FontTextDecoration;    //--Text Decoration

        //-----------------Constructors---------------
        public XFont()
        {
            Init();
        }

        //-----------------Constructors---------------
        public XFont(string fontName, double fontSize)
        {
            Init();
            FontName = fontName;
            FontSize = fontSize;
        }

        //-----------------Constructors---------------
        public XFont(string fontName, double fontSize, string fontWeight, string fontTypefaceStyle, string fontStretch, string textDecoration)
        {
            Init();
            FontName = fontName;
            FontSize = fontSize;
            FontWeight = fontWeight;
            FontTypeFace = fontTypefaceStyle;
            FontStretch = fontStretch;
            FontTextDecoration = textDecoration;
        }

        //-------------------------------------------
        public void Init()
        {
            FontName = "Arial";
            FontSize = 12;
            FontWeight = "Normal";
            FontTypeFace = "Normal";  //--Typeface style
            FontStretch = "Normal";
            FontTextDecoration = "None";
        }

    }
}

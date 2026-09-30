using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace OExpert.UILibraryWPF
{
    public enum EnumOLObject
    {
        OBJ_NONE,
        OBJ_RULE,
        OBJ_FACT,
        OBJ_CONCLUSION
    }

    public enum EnumOper
    {
        WIN_NONE,
        WIN_RUN,
        WIN_EDIT
    }

    //-------------------------------------------------
    public partial class GConstants
    {
        // public const double C_PI = 3.14159;

        public const int AXIS_NONE = 0;
        public const int AXIS_2XY = 1;
        public const int AXIS_4XY = 2;

        /*################### Graphic Component Alignment ###########*/

        public const int ALGN_LEFT = 0x1;
        public const int ALGN_RIGHT = 0x2;
        public const int ALGN_TOP = 0x4;
        public const int ALGN_BOTTOM = 0x8;
        public const int ALGN_SAMEWIDTH = 0x10;
        public const int ALGN_SAMEHEIGHT = 0x20;
        public const int ALGN_HCENTER = 0x40;
        public const int ALGN_VCENTER = 0x80;
        public const int DIST_HORI = 0x100;
        public const int DIST_VERT = 0x200;

        public const int MARK = 4;

        public const int DW_NONE = 0x00;
        public const int DW_3DTITLE = 0x01;
        public const int DW_XNUM = 0x02;
        public const int DW_YNUM = 0x04;
        public const int DW_3DITEM = 0x08;
        public const int DW_3DTDOWN = 0x10;
        public const int DW_3DTUP = 0x20;
        public const int DW_3DIDOWN = 0x40;
        public const int DW_3DIUP = 0x80;


        public const char P_NONE = 'N';  /* IN/OUT port */
        public const char P_IN = 'I';  /* IN port */
        public const char P_OUT = 'O';  /* OUT port */

        /*=================================================================
        Window Operation Status
        =================================================================*/

        public const int WIN_NORMAL = 200;  //'N';
        public const int WIN_VIEW = 201;  //'V';
        public const int WIN_ACTION = 202;  //'X';
        public const int WIN_RUN = 203;  //'R';
        public const int WIN_CONNECT = 204;  //'C';
        public const int WIN_STOP = 205;  //'S';
        public const int WIN_QUIT = 206;  //'Q';

        public const int _FOCUSWIN = -1;
        public const int _BASEWIN = -2;
        public const int _ACTWIN = -3;
        public const int _USERACTWIN = -4;

        /*===================================================================
         Model Component Editable flags
        ===================================================================*/
        public const int G_NONE = 0;
        public const int G_SELECT = 1;
        public const int G_MOVE = 2;
        public const int G_RESIZE = 4;
        public const int G_DELETE = 8;
        public const int G_CONNECT = 16;
        public const int G_ALL = (G_SELECT | G_MOVE | G_RESIZE | G_DELETE | G_CONNECT);

        public const int CONN_NONE = 0;
        public const int CONN_FROM = 1;
        public const int CONN_TO = 2;
        public const int CONN_BOTH = 4;

        public const int CONN_NORMAL = 0;
        public const int CONN_DIAG1X = 1;
        public const int CONN_DIAG2X = 2;
        public const int CONN_DIAG1Y = 4;
        public const int CONN_DIAG2Y = 8;
        public const int CONN_CURVEPOLY1 = 16;  //--Bezier
        public const int CONN_CURVEPOLY2 = 32;  //--QuadraticBezier

        public const int CONN_FIRST = 0;
        public const int CONN_NEXT = 1;
        public const int CONN_PREVIOUS = 2;
        public const int CONN_LAST = 3;
        public const int CONN_ALL = 4;

        /*------Arrow Position-------*/
        public const int ARROW_NONE = 0;     /* default */
        public const int ARROW_HEAD = 1;
        public const int ARROW_END = 2;
        public const int ARROW_BOTH = 3;

        /*------Arrow Style-----------*/
        public const int ARROW_NORMAL = 0;    /* default */
        public const int ARROW_SHARPEND = 1;
        public const int ARROW_OTHER = 2;

        /*------Connection corner type------*/
        public const int CORNER_SHARP = 0x0;    /* default */
        public const int CORNER_ROUND = 0x0100;
        public const int CORNER_HEDGE = 0x0200;

        /*------Curve fit type------*/
        public const int CURVE_NONE = 0x0;    /* default */
        public const int CURVE_POLY_1 = 0x1000;
        public const int CURVE_POLY_2 = 0x2000;
        public const int CURVE_POLY_3 = 0x4000;
        public const int CURVE_POLY_4 = 0x8000;

        /* ====================================================================
                                GRAPHIC ELEMENT TYPE INDEX
         ==================================================================== */
        public const int GPH_NONE = -1;
        public const int GPH_PORT = 1;
        public const int GPH_LINE = 2;
        public const int GPH_RECTANGLE = 3;
        public const int GPH_ELLIPSE = 4;
        public const int GPH_ROUNDRECT = 5;
        public const int GPH_ARC = 6;
        public const int GPH_CHORD = 7;
        public const int GPH_PIE = 8;
        public const int GPH_POLYLINE = 9;
        public const int GPH_POLYGON = 10;
        public const int GPH_TEXT = 11;
        public const int GPH_BITMAP = 12;
        public const int GPH_ICON = 13;
        public const int GPH_GROUP = 14;
        public const int GPH_RUBLINES = 17;
        public const int GPH_GISLNE = 18;
        public const int GPH_TXTFILE = 19;
        public const int GPH_PLOT = 20;
        public const int GPH_XYPLOT = 120;
        public const int GPH_MODEL = 21;
        public const int GPH_PRINTERPAGE = 22;
        public const int GPH_OBJCLASS = 23;
        public const int GPH_OLE = 24;
        public const int GPH_GRIDFILE = 25;
        public const int GPH_BEZIERCURVE = 26;
        public const int GPH_QUADBEZIERCURVE = 27;

        public const int GPH_OBJECT = 28;
        public const int GPH_MDLOBJ = 28;  //--For ModelEditor
        public const int GPH_CONN = 29;
        public const int GPH_MDLCONN = 29;  //--For ModelEditor
        public const int GPH_MDLCLASS = 30;  //--For ModelEditor

        public const int ICN_DRAW = 30000;
        public const int ICN_EDIT = 30001;
        public const int ICN_GROUP = 30002;
        public const int ICN_UNGROUP = 30003;

        public static int IcnMode;

        /*------MDL/GPHELMT Update type Flag ----*/
        public const char T_MOVE = 'M';
        public const char T_SIZE = 'S';
        public const char T_ROTATE = 'R';
        public const char T_COLOR = 'C';
        /* -------------------Selection Drawing Shape */
        public const int SEL_1POINT = 0;
        public const int SEL_LINE = 1;
        public const int SEL_ELLIPSE = 2;
        public const int SEL_ROUNDRECT = 3;
        public const int SEL_BOX = 4;
        public const int SEL_4POINTS = 5;
        public const int SEL_MULTIPOINTS = 6;
        public const int SEL_DASHLINE = 7;
        public const int SEL_DASHBOX = 8;
        public const int SEL_RUBLINES = 9;

        public static int selShape;


        /* -------------------Icon/Model Operation flags----------*/
        public const int OPR_NONE = 0;
        public const int OPR_CUT = 1;
        public const int OPR_COPY = 2;
        public const int OPR_PASTE = 3;
        public const int OPR_GROUP = 4;
        public const int OPR_UNGROUP = 5;
        public const int OPR_PUSH = 6;
        public const int OPR_POP = 7;
        public const int OPR_MOVE = 8;
        public const int OPR_RESIZE = 9;
        public const int OPR_EDIT = 10;
        public const int OPR_FLIPV = 11;
        public const int OPR_FLIPH = 12;
        public const int OPR_ALIGN = 13;
        public const int OPR_EDITPOLY = 14;
        public const int OPR_EDITTYPE = 15;

        public const int MDL_NONE = 0;
        public const int MDL_MOVE = 1;
        public const int MDL_SIZE = 2;

        //----------------------------------RULE---------------------
        public const int RULE_HEIGHT = 16;
        public const int RULE_WIDTH = 80;

        /*--------Inference Strategy type (Forward)-------*/
        public const int INFER_DEPTH = 0;
        public const int INFER_BREADTH = 1;

        /*--------Rule Value------------------------------*/
        public const int RULE_TRUE = 1;
        public const int RULE_FALSE = 0;
        public const int RULE_UNKNOWN = -1;
        /*--------Rule Operation Index--------------------*/
        public const int ROPR_NONE = 0;
        public const int ROPR_IS = 1;
        public const int ROPR_ISNOT = 2;
        public const int ROPR_EQ = 3;
        public const int ROPR_NOTEQ = 4;
        public const int ROPR_GTEQ = 5;
        public const int ROPR_LTEQ = 6;
        public const int ROPR_GT = 7;
        public const int ROPR_LT = 8;
        public const int ROPR_HAVECHILD = 9;
        public const int ROPR_HAVEPARENT = 10;
        public const int ROPR_LET = 11;
        public const int ROPR_DO = 12;
        public const int ROPR_CHECK = 13;
        public const int ROPR_MESSAGE = 14;
        public const int ROPR_CFUN = 15;
        public const int ROPR_PROC = 16;

        /*--------------------------System Color Defines---*/
        public static Color BLACK = Color.FromRgb(0, 0, 0);
        public static Color DARKBLUE = Color.FromRgb(0, 0, 128);
        public static Color DARKRED = Color.FromRgb(128, 0, 0);
        public static Color DARKCYAN = Color.FromRgb(0, 128, 128);
        public static Color DARKMAGENTA = Color.FromRgb(128, 0, 128);
        public static Color DARKYELLOW = Color.FromRgb(128, 128, 0);
        public static Color DARKWHITE = Color.FromRgb(128, 128, 128);
        public static Color BLUE = Color.FromRgb(0, 0, 255);
        public static Color GREEN = Color.FromRgb(0, 255, 0);
        public static Color RED = Color.FromRgb(255, 0, 0);
        public static Color CYAN = Color.FromRgb(0, 255, 255);
        public static Color MAGENTA = Color.FromRgb(255, 0, 255);
        public static Color YELLOW = Color.FromRgb(255, 255, 0);
        public static Color WHITE = Color.FromRgb(255, 255, 255);
        /*----Extra Color definition---*/
        public static Color NAVYBLUE = Color.FromRgb(000, 000, 128);
        public static Color MEDBLUE = Color.FromRgb(000, 000, 205);
        public static Color DARKGREEN = Color.FromRgb(000, 100, 000);
        public static Color DARKTURQUOISE = Color.FromRgb(000, 206, 209);
        public static Color MEDSPRINGGREEN = Color.FromRgb(000, 250, 154);
        public static Color SPRINGGREEN = Color.FromRgb(000, 255, 127);
        public static Color MIDNIGHTBLUE = Color.FromRgb(025, 025, 112);
        public static Color FORESTGREEN = Color.FromRgb(034, 139, 034);
        public static Color SEAGREEN = Color.FromRgb(046, 139, 087);
        public static Color DARKSLATEGRAY = Color.FromRgb(047, 079, 079);
        public static Color LIMEGREEN = Color.FromRgb(050, 205, 050);
        public static Color MEDSEAGREEN = Color.FromRgb(060, 179, 113);
        public static Color TURQUOISE = Color.FromRgb(064, 224, 208);
        public static Color STEELBLUE = Color.FromRgb(070, 130, 180);
        public static Color DARKSLATEBLUE = Color.FromRgb(072, 061, 139);
        public static Color MEDTURQUOISE = Color.FromRgb(072, 209, 204);
        public static Color DARKOliveGREEN = Color.FromRgb(085, 107, 047);
        public static Color CADETBLUE = Color.FromRgb(095, 158, 160);
        public static Color CORNFLOWERBLUE = Color.FromRgb(100, 149, 237);
        public static Color MEDAQUAMARINE = Color.FromRgb(102, 205, 170);
        public static Color DIMGRAY = Color.FromRgb(105, 105, 105);
        public static Color SLATEBLUE = Color.FromRgb(106, 090, 205);
        public static Color MEDFORESTGREEN = Color.FromRgb(107, 142, 035);
        public static Color MEDSLATEBLUE = Color.FromRgb(123, 104, 238);
        public static Color AQUAMARINE = Color.FromRgb(127, 255, 212);
        public static Color skyBLUE = Color.FromRgb(135, 206, 235);
        public static Color BLUEVIOLET = Color.FromRgb(138, 043, 226);
        public static Color PALEGREEN = Color.FromRgb(152, 251, 152);
        public static Color DARKORCHID = Color.FromRgb(153, 050, 204);
        public static Color YELLOWGREEN = Color.FromRgb(154, 205, 050);
        public static Color SIENNA = Color.FromRgb(160, 082, 045);
        public static Color BROWN = Color.FromRgb(165, 042, 042);
        public static Color LIGHTBLUE = Color.FromRgb(173, 216, 230);
        public static Color GREENYELLOW = Color.FromRgb(173, 255, 047);
        public static Color MAROON = Color.FromRgb(176, 048, 096);
        public static Color LIGHTSTEELBLUE = Color.FromRgb(176, 196, 222);
        public static Color FIREBRICK = Color.FromRgb(178, 034, 034);
        public static Color MEDORCHID = Color.FromRgb(186, 085, 211);
        public static Color GRAY = Color.FromRgb(192, 192, 192);
        public static Color MEDVIOLETRED = Color.FromRgb(199, 021, 133);
        public static Color INDIANRED = Color.FromRgb(205, 092, 092);
        public static Color VIOLETRED = Color.FromRgb(208, 032, 144);
        public static Color TAN = Color.FromRgb(210, 180, 140);
        public static Color LIGHTGRAY = Color.FromRgb(211, 211, 211);
        public static Color THISTLE = Color.FromRgb(216, 191, 216);
        public static Color ORCHID = Color.FromRgb(218, 112, 214);
        public static Color GOLDENROD = Color.FromRgb(218, 165, 032);
        public static Color PLUM = Color.FromRgb(221, 160, 221);
        public static Color MEDGOLDENROD = Color.FromRgb(234, 234, 173);
        public static Color VIOLET = Color.FromRgb(238, 130, 238);
        public static Color KHAKI = Color.FromRgb(240, 230, 140);
        public static Color WHEAT = Color.FromRgb(245, 222, 179);
        public static Color SALMON = Color.FromRgb(250, 128, 114);
        public static Color CORAL = Color.FromRgb(255, 127, 080);
        public static Color ORANGERED = Color.FromRgb(255, 069, 000);
        public static Color ORANGE = Color.FromRgb(255, 165, 000);
        public static Color PINK = Color.FromRgb(255, 192, 203);
        public static Color GOLD = Color.FromRgb(255, 215, 000);


        /*################### Element Drawing Gradient Type ################*/
        /*----Gradient Type--*/
        /*--DIRECTION--0,1,2 bits--*/
        public const int GRAD_NONE = 0;
        public const int GRAD_LEFT = 1;
        public const int GRAD_RIGHT = 2;
        public const int GRAD_TOP = 3;
        public const int GRAD_BOTTOM = 4;
        public const int GRAD_CENTER = 5;
        public const int GRAD_OUTER = 6;
        /*--COLOR--3,4 bits----*/
        public const int GRAD_RED = 0;
        public const int GRAD_GREEN = 8;
        public const int GRAD_BLUE = 16;
        public const int GRAD_COMBINE = 24;
        /*--INC--5 bit----*/
        public const int GRAD_INC = 0;
        public const int GRAD_DEC = 32;


        //-------------------------------------------------------------
        //--Symbol in the Plot
        public const int SYM_ARROW = 1;
        public const int SYM_CIRCLE = 2;
        public const int SYM_TRIUP = 3;
        public const int SYM_TRIDOWN = 4;
        public const int SYM_SQUARE = 5;
        public const int SYM_DIAMOND = 6;
        public const int SYM_PLUS = 7;
        public const int SYM_MINUS = 8;
        public const int SYM_CROSS = 9;
        public const int SYM_STAR = 10;

        //---------------------------------------------------------------------
        public const int HALIGN_LEFT = (int)HorizontalAlignment.Left;
        public const int HALIGN_RIGHT = (int)HorizontalAlignment.Right;
        public const int HALIGN_CENTER = (int)HorizontalAlignment.Center;
        public const int HALIGN_STRETCH = (int)HorizontalAlignment.Stretch;

        //---------------------------------------------------------------------
        public const int VALIGN_TOP = (int)VerticalAlignment.Top;
        public const int VALIGN_BOTTOM = (int)VerticalAlignment.Bottom;
        public const int VALIGN_CENTER = (int)VerticalAlignment.Center;
        public const int VALIGN_STRETCH = (int)VerticalAlignment.Stretch;


        /****** Text support ********************************************************/

        /* DrawText() Format Flags */
        public const int DT_TOP = 0x0001;
        public const int DT_LEFT = 0x0002;
        public const int DT_CENTER = 0x0004;
        public const int DT_RIGHT = 0x0008;
        public const int DT_VCENTER = 0x0010;
        public const int DT_BOTTOM = 0x0020;
        public const int DT_JUSTIFY = 0x0040;


        public const int DT_HORIALL = DT_LEFT | DT_CENTER | DT_RIGHT;
        public const int DT_VERTALL = DT_TOP | DT_VCENTER | DT_BOTTOM;

        public static List<RuleOper> aRuleOperList = new List<RuleOper>()
                 { new RuleOper("Ask","Get the Selected data, set ivalue=selected index"),
                   new RuleOper("Is","check if Condition = wantValue"),
                   new RuleOper("IsNot","check if Condition != wantValue"),
                   new RuleOper("In","check if Condition is part of wantValue"),
                   new RuleOper("NotIn","check if Condition is not part of wantValue"),
                   new RuleOper("==","check if Condition = wantValue"),
                   new RuleOper("!=","check if Condition != wantValue"),
                   new RuleOper(">","check if Condition > wantValue"),
                   new RuleOper(">=","check if Condition >= wantValue"),
                   new RuleOper("<","check if Condition < wantValue"),
                   new RuleOper("<=","check if Condition <= wantValue")
        };
    }

    public class RuleOper
    {
        public string Oper { get; set; }
        public string Description { get; set; }

        public RuleOper()
        {

        }

        public RuleOper(string _oper, string _desp)
        {
            Oper = _oper;
            Description = _desp;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OExpert.UILibraryWPF
{
    public class GlobalVarX
    {
        public static double MinProbability = 0.00001;  //--Min Probability for false rules

        public static int gARROW_HEAD = 1;
        public static int gARROW_END = 2;
        public static int gARROW_BOTH = 3;

        public static double gARW_Width = 4;
        public static double gARW_Length = 6;
        public static double gARW_Dent = 2;

        public static Pen hDefPen = new Pen(Brushes.Black, 1);  //--current font index to list

        public static Brush hDefBrush = Brushes.White;  //--current font index to list

        public static string SysDir = Directory.GetCurrentDirectory();
        public static string sDirDefault = Directory.GetCurrentDirectory();  //--Default directory for data, Application

        public static bool fDebug = true;  //--Turn on/off the internal debug

        public static double xPageRes = 96;
        public static double yPageRes = 96;

        public static string PrintObjType = "";
        public static string PrintObjSpec = "";
        public static double PrintPageLeft = 0.25;  //--In Inch, 96 pixels/in
        public static double PrintPageTop = 0.25;
        public static double PrintPageWidth = 7;
        public static double PrintPageHeight = 10;

        public static double PaperSizeLeft = 0.25;  //--In pixels, 96 pixels/in
        public static double PaperSizeTop = 0.25;
        public static double PaperSizeWidth = 8.5;
        public static double PaperSizeHeight = 11;

        public static int PrintOrientation = 0;  //--0:Portrait, 1:Landscape
        public static bool PrintKeepScale = true;
        public static bool PrintAddFrame = true;
        public static char PrintUnit = 'I';  //--I:Inch, C:cm
        public static int PrintNItemPerPage = 1;  //--# of items on a page

        public static bool fPrintAbort = false;

        public static string sVersion = "1.0";

        public static int IXApplication = -1;  //--Need to assigned

        public static int iGphAlign = 0;   /* Global GphElmt Align Flag */

        public static int iBlinkDelay = 5;  //--5 msec, blinking

        public static bool fHierarchy = false;

        public static bool fPrinter = false;

        public static string sMdlRunHiRectColor = "Red";   /* color for drawing the selected component rect */
        public static double iMdlRunHiRectLine = 2;   /* line width for drawing the selected component rect */

        public static char fUpTbl = 'N';

        public static int fgArwType = 0;
        public static int fgDrawType = GConstants.CONN_NORMAL;

        public static bool fgFit = false;
        public static bool fgKScale = true;
        public static int fgCurveFit = 0;
        public static int fgPolygonCurve = 0;

        public static int fgShowTag = 0;

        public static string DefPenColor = "Black";
        public static string DefBrushColor = "White";

        public static double DefIconHight = 12;
        public static double DefIconWidth = 12;

        public static double BrwItemSpace = 20;

        /* -------------------Default Graphic Element Value */
        /* Font and FontSize is define by curFont, curFontSize */
        public static int Def_FontStyle = 0x0;  /* 0:normal, 1:bold, 8:italic,
                           4:underline, 2:strikeout */
        public static int Def_Align = GConstants.DT_LEFT;     /* text alignment */
        public static int Def_ArwType = 0;
        public static int Def_CornType = 0;
        public static int Def_FitType = 0;
        public static bool fAutoFontFit = true;

        public static int Def_GradDir = 0;
        public static int Def_GradColor = 0;
        public static int Def_GradInc = 0;

        public static string DefTextColor = "Black";

        public static object aObxStatus = null;  //--OBXSTATUS window

        /* ===================================================================
                        OPERATION MODE
        =================================================================== */
        public static bool bTrack = true;
        public static bool bMove = true;
        public static bool bResize = true;

        public static int defIncX = 5;
        public static int defIncY = 5;

        public static string sgPrefix = "";

        public static int sys3Depth = 2;
        public static string light3DShade = "White";
        public static string dark3DShade = "Gray";
        public static string face3DShade = "LightGray";

        public static bool fgAddQuote = true;
        public static bool bChanges = false;
        public static char iWarnFlag = 'N';

        public static string StrBuf = "";


        //--Arrow shape 
        public static int arwLength = 6;
        public static int arwWidth = 2;
        public static int arwDent = 1;

        public static char fGblTxtFit = 'N';
        public static int fgPortDraw = 0;

        public static int igAddCmp = 0;

        public static string sCInsObj = "";
        public static string sCInsCmpId = "";

        public static string gcTopWin = "";
        public static string pgcTopWin = "";

        public static int cRtn = 0;
        public static int cRtnI = 0;  //--return int
        public static long cRtnL = 0;  //--return long
        public static double cRtnF = 0;  //--return float
        public static DateTime cRtnD = DateTime.Now; //--return datetime
        public static char cRtnC = '\0';  //--return char
        public static string cRtnS = "";  //--return string
        public static long lMsgRtn = 0;

        public static bool isLogin = false;
        public static int cUserPrvg = 10;
        public static string cUserID = "";
        public static string cUserName = "";
        public static string cPassWord = "";
        public static string cUserDesc = "";
        public static string cUserProj = "";
        public static string cExpireDate = "";

        public static int iInferType = 0;
        public static char iResetData = 'Y';  //--Y | N
        public static char iResetRule = 'Y';  //--Y | N

        public static XFont aRuleDefXFont = new XFont("Arial", 12, "Normal", "Normal", "Normal", "None");
        public static string sRulePenColor = "Black";
        public static string sRuleBrushColor = "White";

        public static double RuleWidth = 80;
        public static double RuleHeight = 30;

        public static double CharX = 10;  //--Char x size
        public static double CharY = 12;  //--Char y size;

        public static int DlgReturn = 0;  //-Dialog return value

        public static char gxdType = '0';
        public static string gxObjName = "";
        public static string gxPropName = "";

        //--Run Variables
        public static int fPrcWaitON = 1;
        public static string sHiTextColor = "";
        public static string sHiTextBColor = "";
        public static string VColor = "";
        public static int iMainBx = 0;
        public static int iMainBy = 0;
        public static int iMainWidth = 640;
        public static int iMainHeight = 480;
        public static char fPrimaryON = 'Y';

        public static object hWndPrimary = null;

        public static int iListenBx = 0;
        public static int iListenBy = 0;
        public static int iListenWidth = 640;
        public static int iListenHeight = 480;
        public static char fListenON = 'Y';

        public static object hWndBListen = null;

        public static int oldScnWidth = 16;
        public static int oldScnHeight = 8;

        public static string sBaseFont = "Arial";

        public static string sShowFont = "Arial";
        public static int iShowFontSize = 12;

        public static int fgODBC = 1;

        //-- DDESVR
        public static string szDdeSvr = "";
        public static string szTopic = "";

        public static string sCallFromDB = "";

        public static string sPalette = "";

        //--INI
        public static int appObjType = 0;
        public static string appObj = "";
        public static string appID = "";
        public static string appShow = "";
        public static char fAppFit = 'Y';
        public static char fAppCenter = 'Y';
        public static char fCoordType = 'A';  // I:increment A:absolute
        public static double appBx = 0;
        public static double appBy = 0;
        public static double appEx = 640;
        public static double appEy = 480;
    }

}


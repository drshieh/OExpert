using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Numerics;
using System.Windows.Media;

namespace OExpert.UILibraryWPF
{
    public enum XSort
    {
        None,
        AscendingByName,
        DescendingByName,
        AscendingByUpdateTime,
        DescendingByUpdateTime
    }

    public class WHelper
    {
        public delegate void CBHandler(object o);  //--Caller Provided Callback function
        public delegate void CBHandler2(object o1, object o2);  //--Caller Provided Callback function

        //-------------------------------------------------------------------
        public static void SetZoomFactor(string sSel, double gridActualWidth, double gridActualHeight,
                                         double frameWidth, double frameHeight,
                                         ref double zoomFactorX, ref double zoomFactorY)
        {
            if (String.IsNullOrEmpty(sSel)) zoomFactorX = zoomFactorY = 1.0;
            else
            {
                string[] subx = sSel.ToUpper().Split(' ');
                string sValue = subx[0].Trim().ToUpper();
                if (sValue == "FIT")
                {
                    if (frameWidth <= 0 || frameHeight <= 0) zoomFactorX = zoomFactorY = 1.0;
                    else
                    {
                        zoomFactorX = gridActualWidth / frameWidth;
                        zoomFactorY = gridActualHeight / frameHeight;
                        if (zoomFactorX > zoomFactorY) zoomFactorX = zoomFactorY;
                        else zoomFactorY = zoomFactorX;
                    }
                }
                else
                {
                    zoomFactorX = zoomFactorY = Double.Parse(sValue);
                }
            }
        }

        //-------------------------------------------------------------------
        public static string ValidateName(string name)
        {
            if (String.IsNullOrEmpty(name)) return "empty";
            return name.Replace(" ", "_").Replace("\t", "_");
        }

        //-------------------------------------------------------------------
        public static bool EditVarString(ref string str, string svar, string svalue)
        {
            if (String.IsNullOrEmpty(str)) return false;
            string[] subx = str.Split('=');
            if (subx[0].Trim() != svar) return false;
            if (subx.Length < 2) return false;
            str = subx[0].TrimEnd() + " = \"" + svalue + "\"";
            return true;
        }

        //-------------------------------------------------------------------
        public static int CBSetSelect(ComboBox cb, string sValue)
        {
            if (cb == null || cb.Items == null || String.IsNullOrEmpty(sValue)) return -1;
            int i = 0;
            foreach (var a in cb.Items)
            {
                if (a.GetType() == typeof(ComboBoxItem))
                {
                    ComboBoxItem x = (ComboBoxItem)a;
                    if (x.Content.ToString().IndexOf(sValue) >= 0)
                    {
                        cb.SelectedIndex = i;
                        break;
                    }
                }
                else
                {
                    if (a.ToString().IndexOf(sValue) >= 0)
                    {
                        cb.SelectedIndex = i;
                        break;
                    }
                }
                i++;
            }
            return i;
        }

        //-------------------------------------------------------------------
        public static string CBGetSelect(ComboBox cb)
        {
            if (cb == null || cb.Items == null) return null;
            if (cb.SelectedItem == null) return null;
            return cb.SelectedItem.ToString();
        }

        //-------------------------------------------------------------------
        public static int GetIImageInName(string sImageFile)
        {
            string sFileName = Path.GetFileNameWithoutExtension(sImageFile).Replace("X", "").Replace("A", "");
            int ib = sFileName.IndexOf("-");
            if (ib < 0) return -1;
            return WHelper.GetInt32(sFileName.Substring(ib + 1));
        }

        //------------------------------------------------------------------------------
        public static string GetNextFileName(string sFileName)
        {
            string sDir = Path.GetDirectoryName(sFileName);
            sDir = WHelper.ValidateDir(sDir, false);
            string sName = Path.GetFileNameWithoutExtension(sFileName);
            string sExt = Path.GetExtension(sFileName);
            for (int i = 1; i < 10000; i++)
            {
                string sNewName = sDir + sName + i.ToString() + sExt;
                if (!File.Exists(sNewName)) return sNewName;
            }
            return "";
        }


        //-------------------------------------------------------------------
        public static bool IsImageFile(string sFile)
        {
            if (String.IsNullOrEmpty(sFile)) return false;
            string sExt = Path.GetExtension(sFile).ToLower();
            if (sExt == ".png" || sExt == ".jpg" || sExt == ".jpeg" || sExt == ".jfif" || sExt == ".bmp") return true;
            else return false;
        }

        //-------------------------------------------------------------------
        public static int Mode(int iValue, int iMode)
        {
            var v = iValue % iMode;
            return v;
        }


        //--------------------------------------------------------------------------
        public static string SetFrameFileName(string sFrameDir, string sName, int iImage, string sPosfix)
        {
            //           iImage++;
            var imgNumber = iImage.ToString().PadLeft(8, '0');
            return $@"{sFrameDir}{sName}-{imgNumber}{sPosfix}.png";
        }

        //-------------Get only the file name: replace x.png, a.png=>.png------------------------------------------------
        public static string GetFrameFileNameX(string sFile, string sImageExt, bool fNameOnly)
        {
            if (String.IsNullOrEmpty(sFile) || String.IsNullOrEmpty(sImageExt)) return sFile;
            string sName = sFile.Replace("A.png", sImageExt + ".png").Replace("X.png", sImageExt + ".png");
            if (fNameOnly) return System.IO.Path.GetFileName(sName);
            else return sName;
        }

        //--------------------------------------------------------------------------
        public static string ReplaceExt(string sFile, string sImageExt)
        {
            string sExt = sImageExt + ".png";
            return sFile.Replace("A.png", sExt).Replace("X.png", sExt);
        }

        //-------------------------------------------------------------------------------------------------------------
        public static void DeleteDirFiles(string sDir, string sFileFilter)
        {
            try
            {
                string[] files = Directory.GetFiles(sDir, sFileFilter);
                foreach (string file in files) File.Delete(file);

            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteDirFiles: " + sDir + ex.ToString());
            }
        }

        //--------------------------------------------------------------------------
        public static System.Windows.Window GetParentWindow(FrameworkElement e)
        {
            while (e != null)
            {
                System.Windows.Window w = e as System.Windows.Window;
                if (w != null) return w;
                e = e.Parent as FrameworkElement;
            }
            return null;
        }

        //--------------------------------------------------------------------------
        public static string List2Str(List<string> aList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string a in aList) sb.AppendLine(a);
            return sb.ToString();
        }

        //--------------------------------------------------------------------------
        public static string GetIndent(string sstr)
        {
            if (String.IsNullOrEmpty(sstr)) return "";
            char[] array = sstr.ToCharArray();
            string sInd = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != ' ') break;
                sInd += " ";
            }
            return sInd;
        }

        //--------------------------------------------------------------------------
        // iType: 0:Picture, 1:Video, 2:Text/Json
        public static string SelectFile(int iType, TextBox tb)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (iType == 0) dlg.Filter = "Image files(*.bmp; *.png; *.jpg; *.jpeg; *.jfif)|*.bmp;*.png;*.jpg;*.jpeg;*.jfif|All files(*.*)|*.*";
            else if (iType == 1) dlg.Filter = "Video files(*.wmv; *.mp4 )|*.wmv;*.mp4|All files(*.*)|*.*";
            else dlg.Filter = "Text files(*.txt; *.json )|*.txt;*.json|All files(*.*)|*.*";

            string sFileName = "";
            if (dlg.ShowDialog() == true)
            {
                sFileName = dlg.FileName;
                if (tb != null) tb.Text = sFileName;
            }
            return sFileName;
        }

        //--------------------------------------------------------------------------
        //--fShort = true, remove the system directory part
        public static ObservableCollection<string> ListFiles(string sDir, string sFilter0, bool fShort, bool fShowNameOnly, XSort eSort)
        {
            if (!Directory.Exists(sDir)) return null;
            List<string> filterList = new List<string>();

            List<string> files = GetDirectDirFiles(sDir, sFilter0);
            List<FileInfo> aFileList = new List<FileInfo>();
            if (files != null && files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    FileInfo fileinfo = new FileInfo(files[i]);
                    aFileList.Add(fileinfo);
                }
            }
            GetSubDirFiles(sDir, sFilter0, ref aFileList);
            if (aFileList == null || aFileList.Count <= 0) return null;

            List<FileInfo> xList = null;
            if (aFileList != null && aFileList.Count > 0)
            {
                if (eSort == XSort.DescendingByUpdateTime) xList = (from a in aFileList orderby a.LastWriteTime descending select a).ToList();
                else if (eSort == XSort.AscendingByUpdateTime) xList = (from a in aFileList orderby a.LastWriteTime ascending select a).ToList();
                else if (eSort == XSort.DescendingByName) xList = (from a in aFileList orderby a.Name descending select a).ToList();
                else if (eSort == XSort.AscendingByName) xList = (from a in aFileList orderby a.Name ascending select a).ToList();
                else xList = aFileList;
            }

            ObservableCollection<string> aRList = new ObservableCollection<string>();

            if (xList != null && xList.Count > 0)
            {
                foreach (FileInfo a in xList)
                {
                    string sFileName = a.FullName;
                    if (fShort) sFileName = ShortFileName(sFileName, Directory.GetCurrentDirectory());
                    if (fShowNameOnly) aRList.Add(sFileName);
                    else aRList.Add(sFileName + " => Length:" + a.Length + ", Created:" + a.CreationTime + ", Last Write:" + a.LastWriteTime);
                }
            }
            return aRList;
        }
        //--------------------------------------------------------------------------
        public static List<string> GetDirectDirFiles(string sDir, string sFilter0)
        {
            List<string> files = null;
            if (String.IsNullOrEmpty(sFilter0))
            {
                files = (Directory.GetFiles(sDir, "*.*")).ToList<string>();
            }
            else
            {
                string[] subx = sFilter0.Split('|');
                files = new List<string>();
                for (int i = 0; i < subx.Length; i++)
                {
                    string[] xfiles = Directory.GetFiles(sDir, subx[i].Trim());
                    if (xfiles != null && xfiles.Length > 0)
                    {
                        files.AddRange(xfiles);
                    }
                }
            }
            return files;
        }

        //--------------------------------------------------------------------------
        public static void GetSubDirFiles(string sDir, string sFilter0, ref List<FileInfo> aFileList)
        {
            string[] subDirs = Directory.GetDirectories(sDir);
            if (subDirs != null)
            {
                for (int i = 0; i < subDirs.Length; i++)
                {
                    List<string> files1 = GetDirectDirFiles(subDirs[i], sFilter0);
                    if (files1 != null && files1.Count > 1)
                    {
                        for (int k = 0; k < files1.Count; k++)
                        {
                            FileInfo fileinfo = new FileInfo(files1[k]);
                            aFileList.Add(fileinfo);
                        }
                    }
                    GetSubDirFiles(subDirs[i], sFilter0, ref aFileList);
                }
            }
        }

        //--------------------------------------------------------------------------
        //--fShort = true, remove the system directory part
        public static string LatestFile(string sDir, string sFilter, bool fShort, bool fShowNameOnly)
        {
            if (!Directory.Exists(sDir)) return null;
            List<FileInfo> aFileList = new List<FileInfo>();
            ListDirFile(sDir, sFilter, ref aFileList);
            if (aFileList.Count <= 0) return null;

            List<string> aRList = new List<string>();
            var x = from a in aFileList orderby a.LastWriteTime descending select a.FullName;
            if (x != null) aRList = x.ToList();
            if (aRList.Count > 0) return aRList[0];
            else return "";
        }

        //--------------------------------------------------------------------------
        //--fShort = true, remove the system directory part
        public static int ListDirFile(string sDir, string sFilter, ref List<FileInfo> fileList)
        {
            if (!Directory.Exists(sDir)) return 0;
            string[] files = Directory.GetFiles(sDir, sFilter);
            string[] subDirs = Directory.GetDirectories(sDir);
            if (files == null && files == null) return 0;
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    fileList.Add(new FileInfo(files[i]));
                }
            }
            if (subDirs != null)
            {
                for (int i = 0; i < subDirs.Length; i++)
                {
                    ListDirFile(subDirs[i], sFilter, ref fileList);
                }
            }
            return fileList.Count;
        }

        //--------------------------------------------------------------------------
        public static string ShortFileName(string sFileName, string sSysDir)
        {
            if (String.IsNullOrEmpty(sSysDir)) return sFileName;
            return sFileName.Replace(sSysDir, "");
        }

        //--------------------------------------------------------------------------
        public static string LongFileName(string sFileName)
        {
            return System.IO.Path.GetFullPath(sFileName);
        }

        //--------------------------------------------------------------------------
        public static string LongFileName(string sFileName, string sSysDir)
        {
            string sFile = sSysDir + sFileName;
            if (sFileName.IndexOf("//") >= 0 || sFileName.IndexOf(":") >= 0) sFile = sFileName;
            return sFile;
        }

        //--------------------------------------------------------------------------
        public static ObservableCollection<FileInfo> XListFiles(string sDir, string sFilter)
        {
            if (!Directory.Exists(sDir)) return null;
            if (String.IsNullOrEmpty(sFilter)) sFilter = "*.*";
            string[] files = Directory.GetFiles(sDir, sFilter);
            if (files == null) return null;
            ObservableCollection<FileInfo> aFileList = new ObservableCollection<FileInfo>();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo fileinfo = new FileInfo(files[i]);
                aFileList.Add(fileinfo);
            }
            return aFileList;
        }


        public static string ServiceError = "";

        //------------------------------------------------------------------------
        public static int GetInt32(string str)
        {
            int ivalue = Int32.MinValue;
            if (Int32.TryParse(str, out ivalue)) return ivalue;
            return ivalue;
        }

        //------------------------------------------------------------------------
        public static double GetDouble(string str)
        {
            double ivalue = Double.MinValue;
            if (Double.TryParse(str, out ivalue)) return ivalue;
            return ivalue;
        }

        //------------------------------------------------------------------------
        public static bool GetBoolean(string str)
        {
            string strU = str.ToUpper();
            if (strU == "YES" || strU == "TRUE" || strU == "T" || strU == "Y") return true;
            return false;
        }

        //------------------------------------------------------------------------
        public static DateTime? GetDateTime(string str)
        {
            DateTime rtn;
            str = str.Trim();
            if (!String.IsNullOrEmpty(str))
            {
                if (DateTime.TryParse(str, out rtn)) return rtn;
                else return null;
            }
            return null;
        }

        //------------------------------------------------------------------------
        public static System.Windows.Window FindParentWindow(object sender)
        {
            if (sender == null) return null;
            FrameworkElement e = sender as FrameworkElement;
            while (e != null)
            {
                System.Windows.Window w = e as System.Windows.Window;
                if (w != null)
                {
                    return w;
                }
                e = e.Parent as FrameworkElement;
            }
            return null;
        }

        //------------------------------------------------------------------------
        public static void TVExpandAll(TreeView tv, bool fExpandSubTree)
        {
            foreach (var item in tv.Items)
            {
                var tvi = item as TreeViewItem;
                if (tvi != null)
                {
                    if (fExpandSubTree) tvi.ExpandSubtree();
                    else tvi.IsExpanded = false;
                }
            }
        }

        //--------------------------------------------------------------------------
        public static int GetTreeNodeLevel(TreeViewItem t)
        {
            if (t == null) return 0;
            if (t.Parent == null) return 0;
            Console.WriteLine("*" + t.Header);
            int iLevel = 0;
            if (t.Parent != null) GetTreeNodeParentLevel(t.Parent as TreeViewItem, ref iLevel);
            return iLevel;
        }

        //--------------------------------------------------------------------------
        public static void GetTreeNodeParentLevel(TreeViewItem t, ref int iLevel)
        {
            if (t == null) return;
            Console.WriteLine(t.Header);
            iLevel++;
            if (t.Parent != null) GetTreeNodeParentLevel(t.Parent as TreeViewItem, ref iLevel);
            return;
        }

        //--------------------------------------------------------------------
        //--Loop 1: start from the current selected
        //--If can not find in Loop 1, start from the beginning
        public static bool fStartFind = false;
        public static TreeViewItem FindInTree(TreeView tv, string sSearch)
        {
            if (String.IsNullOrEmpty(sSearch)) return null;
            TreeViewItem selItem = tv.SelectedItem as TreeViewItem;
            if (selItem == null) fStartFind = true;
            else fStartFind = false;
            foreach (TreeViewItem a in tv.Items)
            {
                if (a.Header.ToString().IndexOf(sSearch) >= 0)
                {
                    if (fStartFind) return a;
                    else if (selItem == a) fStartFind = true;
                }
                if (a.Items != null)
                {
                    TreeViewItem x = FindInTreeChild((ICollection)a.Items, selItem, sSearch);
                    if (x != null) return x;
                }
            }
            if (selItem != null) selItem.IsSelected = false;
            //--Loop 1: Did not find, start from the top, only for no current select
            if (selItem != null)
            {
                selItem = null;
                fStartFind = true;
                foreach (TreeViewItem a in tv.Items)
                {
                    if (a.Header.ToString().IndexOf(sSearch) >= 0)
                    {
                        return a;
                    }
                    if (a.Items != null)
                    {
                        TreeViewItem x = FindInTreeChild((ICollection)a.Items, selItem, sSearch);
                        if (x != null) return x;
                    }
                }
            }
            return null;
        }

        //--------------------------------------------------------------------
        public static TreeViewItem FindInTreeChild(ICollection aList, TreeViewItem selItem, string sSearch)
        {
            if (aList == null) return null;
            foreach (TreeViewItem a in aList)
            {
                if (a.Header.ToString().IndexOf(sSearch) >= 0)
                {
                    if (fStartFind) return a;
                    else if (selItem == a) fStartFind = true;
                }
                if (a.Items != null)
                {
                    TreeViewItem x = FindInTreeChild(a.Items, selItem, sSearch);
                    if (x != null) return x;
                }
            }

            return null;
        }

        //-------------------------------------------------------------------
        public static void ShowTextFile2TB(TextBox tb, string sFile)
        {
            if (String.IsNullOrEmpty(sFile) || !File.Exists(sFile)) return;
            string stext = File.ReadAllText(sFile);
            tb.Text = stext;
        }

        //-------------------------------------------------------------------
        public static void ShowLabelFile(FrameworkElement tb, string sFile)
        {
            if (String.IsNullOrEmpty(sFile) || !File.Exists(sFile)) return;
            string stext = "";
            string[] lines = File.ReadAllLines(sFile);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] subx = lines[i].Split(' ');
                stext += "Class: " + subx[0].Trim() + "\r\n";
                stext += "   CX: " + String.Format("{0:0.00}", Double.Parse(subx[1].Trim())) + "\r\n";
                stext += "   CY: " + String.Format("{0:0.00}", Double.Parse(subx[2].Trim())) + "\r\n";
                stext += "    W: " + String.Format("{0:0.00}", Double.Parse(subx[3].Trim())) + "\r\n";
                stext += "    H: " + String.Format("{0:0.00}", Double.Parse(subx[4].Trim())) + "\r\n\r\n";
            }
            if (tb.GetType() == typeof(TextBox)) (tb as TextBox).Text = stext;
            else if (tb.GetType() == typeof(TextBlock)) (tb as TextBlock).Text = stext;
        }

        //-------------------------------------------------------------------
        public static void ShowImage(System.Windows.Controls.Image imgCtrl, string imageFile, double factor)
        {
            string sPath = "";
            if (!String.IsNullOrEmpty(imageFile))
            {
                sPath = System.IO.Path.GetFullPath(imageFile);
                if (File.Exists(sPath))
                {
                    try
                    {
                        Uri sUri = new Uri(sPath);
                        BitmapImage myBitmapImage = new BitmapImage();
                        // BitmapImage.UriSource must be in a BeginInit/EndInit block
                        myBitmapImage.BeginInit();
                        myBitmapImage.UriSource = sUri;
                        myBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        myBitmapImage.EndInit();
                        //set image source
                        if (factor > 0 && factor != 1)
                        {
                            TransformedBitmap x = new TransformedBitmap(myBitmapImage, new ScaleTransform(factor, factor));
                            imgCtrl.Source = x;
                        }
                        else imgCtrl.Source = myBitmapImage;
                        if (imgCtrl.ActualWidth > myBitmapImage.Width) ;
                        if (imgCtrl.ActualHeight > myBitmapImage.Height) ;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ERROR: " + ex.ToString());
                        imgCtrl.Source = null;
                    }
                }
                else imgCtrl.Source = null;
            }
            else imgCtrl.Source = null;
        }

        //-------------------------------------------------------------------
        public static ImageSource GetImage(string imageFile, double angle)
        {
            string sPath = "";
            if (String.IsNullOrEmpty(imageFile)) return null;
            sPath = System.IO.Path.GetFullPath(imageFile);
            if (!File.Exists(sPath)) return null;
            try
            {
                Uri sUri = new Uri(sPath);
                BitmapImage myBitmapImage = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = sUri;
                myBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                myBitmapImage.EndInit();
                double factor = 24 / myBitmapImage.Width;
                ScaleTransform myScaleTransform = new ScaleTransform();
                myScaleTransform.ScaleX = factor;
                myScaleTransform.ScaleY = factor;
                RotateTransform myRotateTransform = new RotateTransform();
                myRotateTransform.Angle = angle;
                TransformGroup myTransformGroup = new TransformGroup();
                myTransformGroup.Children.Add(myScaleTransform);
                myTransformGroup.Children.Add(myRotateTransform);

                TransformedBitmap xg = new TransformedBitmap(myBitmapImage, myTransformGroup);
                TransformedBitmap x = new TransformedBitmap(myBitmapImage, new ScaleTransform(factor, factor));
                if (angle != 0)
                {
                    TransformedBitmap x2 = new TransformedBitmap();

                    // BitmapSource objects like TransformedBitmap can only have their properties
                    // changed within a BeginInit/EndInit block.
                    x2.BeginInit();

                    // Use the BitmapSource object defined above as the source for this BitmapSource.
                    // This creates a "chain" of BitmapSource objects which essentially inherit from each other.
                    x2.Source = x;

                    // Flip the source 90 degrees.
                    x2.Transform = new RotateTransform(angle);
                    x2.EndInit();

                    return x2 as ImageSource;
                }
                else return x as ImageSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.ToString());
                return null;
            }
        }

        //-------------------------------------------------------------------
        public static bool GetImageSize(string imageFile, ref double width, ref double height)
        {
            width = height = 0;
            if (!String.IsNullOrEmpty(imageFile))
            {
                string sPath = System.IO.Path.GetFullPath(imageFile);
                if (File.Exists(sPath))
                {
                    Uri sUri = new Uri(sPath);
                    BitmapImage myBitmapImage = new BitmapImage();
                    // BitmapImage.UriSource must be in a BeginInit/EndInit block
                    myBitmapImage.BeginInit();
                    myBitmapImage.UriSource = sUri;
                    myBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    myBitmapImage.EndInit();
                    width = myBitmapImage.Width;
                    height = myBitmapImage.Height;
                    return true;
                }
            }
            return false;
        }

        //---------------------------------------------------
        public static Uri GetUri(string sstr)
        {
            if (String.IsNullOrEmpty(sstr)) return null;
            try
            {
                return new Uri(sstr, UriKind.RelativeOrAbsolute);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: GetUri: " + ex.ToString());
            }
            return null;
        }

        //------------------------------------------------------
        // A = a * b
        public static double CalcRectArea(double bx, double by, double ex, double ey)
        {
            return Math.Abs((ex - bx) * (ey - by));
        }

        //------------------------------------------------------
        // A = pi * a * b
        public static double CalcEllipseArea(double bx, double by, double ex, double ey)
        {
            double a = (ex - bx) / 2;
            double b = (ey - by) / 2;
            return Math.PI * a * b;
        }

        //------------------------------------------------------
        // Return the polygon's area in "square units."
        // The value will be negative if the polygon is
        // oriented clockwise.
        public static double CalcPolygonArea(double bx, double by, double ex, double ey, List<System.Windows.Point> Points)
        {
            List<System.Windows.Point> aNewPoints = new List<System.Windows.Point>();

            foreach (System.Windows.Point p in Points)
            {
                aNewPoints.Add(new System.Windows.Point(bx + p.X, by + p.Y));
            }
            // Add the first point to the end.
            aNewPoints.Add(new System.Windows.Point(bx + Points[0].X, by + Points[0].Y));

            // Get the areas.
            double area = 0;
            for (int i = 0; i < aNewPoints.Count - 1; i++)
            {
                area +=
                    (aNewPoints[i + 1].X - aNewPoints[i].X) *
                    (aNewPoints[i + 1].Y + aNewPoints[i].Y) / 2;
            }

            // Return the result.
            return Math.Abs(area);
        }

        //-------------------------------------------------------------------------
        //--If Does not exists: create and return true;
        public static string ValidateDir(string sDir, bool fCreate)
        {
            if (String.IsNullOrEmpty(sDir)) return sDir;
            if (!sDir.EndsWith("\\")) sDir += "\\";
            if (!Directory.Exists(sDir))
            {
                if (fCreate) Directory.CreateDirectory(sDir);
            }
            return sDir;
        }

        //-------------------------------------------------------------------------
        //--If Does not exists: create and return true;
        public static bool ValidateDirX(ref string sDir, bool fCreate)
        {
            if (String.IsNullOrEmpty(sDir)) return false;
            if (!sDir.EndsWith("\\")) sDir += "\\";
            if (!Directory.Exists(sDir))
            {
                if (fCreate)
                {
                    Directory.CreateDirectory(sDir);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        //-------------------------------------------------------------------------
        public static int FindInList(List<string> aList, string str)
        {
            if (aList == null) return -1;
            for (int i = 0; i < aList.Count; i++)
            {
                if (aList[i] == str) return i;
            }
            return -1;
        }

        //------------------------------------------------------
        public static bool isBinaryFile(string path)
        {
            long length = new FileInfo(path).Length;
            if (length == 0) return false;

            using (StreamReader stream = new StreamReader(path))
            {
                int ch;
                while ((ch = stream.Read()) != -1)
                {
                    if (isControlChar(ch))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //------------------------------------------------------
        public static bool isControlChar(int ch)
        {
            return (ch > Chars.NUL && ch < Chars.BS)
                || (ch > Chars.CR && ch < Chars.SUB);
        }

        //------------------------------------------------------
        public static class Chars
        {
            public static char NUL = (char)0; // Null char
            public static char BS = (char)8; // Back Space
            public static char CR = (char)13; // Carriage Return
            public static char SUB = (char)26; // Substitute
        }

        //--------------------------------------------------------
        public static void ReadFile2LB(string sFile, ListBox lb, bool fClear)
        {
            if (String.IsNullOrEmpty(sFile) || !File.Exists(sFile)) return;
            if (fClear) lb.Items.Clear();
            string[] lines = File.ReadAllLines(sFile);
            for (int i = 0; i < lines.Length; i++)
            {
                lb.Items.Add(lines[i]);
            }
        }
        //---------------------------------------------------------------
        public static DashStyle GetDashes(double thickness, string sdoubles)
        {
            if (String.IsNullOrEmpty(sdoubles)) return null;
            DashStyle dStyle = new DashStyle();
            DoubleCollection dList = new DoubleCollection();
            char[] cSep = { ' ' };
            string[] subx = sdoubles.Split(cSep, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < subx.Length; i++)
            {
                if (String.IsNullOrEmpty(subx[i].Trim())) continue;
                dList.Add(thickness * Double.Parse(subx[i].Trim()));
            }
            dStyle.Dashes = dList;
            return dStyle;
        }

        //---------------------------------------------------------------
        public static PenLineCap GetLineCap(string sname)
        {
            if (sname == "Flat") return PenLineCap.Flat;
            else if (sname == "Round") return PenLineCap.Round;
            else if (sname == "Square") return PenLineCap.Square;
            else if (sname == "Triangle") return PenLineCap.Triangle;
            else return PenLineCap.Flat;
        }

        //---------------------------------------------------------------
        public static PenLineJoin GetLineJoin(string sname)
        {
            if (sname == "Bevel") return PenLineJoin.Bevel;
            else if (sname == "Miter") return PenLineJoin.Miter;
            else if (sname == "Round") return PenLineJoin.Round;
            else return PenLineJoin.Bevel;
        }

        //--------------------------------------------------------------------
        //-Get the ImageWrapMode for TextureBrush
        public static TileMode GetImageTileMode(string sMode)
        {
            switch (sMode)
            {
                case "Clamp":
                    return TileMode.Tile;
                case "Tile":
                    return TileMode.Tile;
                case "TileFlipX":
                    return TileMode.FlipX;
                case "TileFlipXY":
                    return TileMode.FlipXY;
                case "TileFlipY":
                    return TileMode.FlipY;
                default:
                    return TileMode.None;
            }
        }


        //-------------------------------------------------------------------
        public static void CB_SetSelect(ComboBox cb, string svalue)
        {
            if (cb == null) return;
            if (cb.IsEditable)
            {
                cb.Text = svalue;
                return;
            }
            int i = 0;
            foreach (var cbi in cb.Items)
            {
                ComboBoxItem cbix = cbi as ComboBoxItem;
                if (cbix != null)
                {
                    if (svalue == null && cbix.Content == null)
                    {
                        cb.SelectedIndex = i;
                        return;
                    }
                    if (cbix.Content != null && svalue == cbix.Content.ToString())
                    {
                        cb.SelectedIndex = i;
                        return;
                    }
                }
                else
                {
                    if (svalue == cbi.ToString())
                    {
                        cb.SelectedIndex = i;
                        return;
                    }
                }
                i++;
            }
            cb.SelectedIndex = -1;
        }

        //---------------------------------------------------------
        public static string CB_GetSelect(ComboBox cb)
        {
            if (cb == null) return null;
            if (cb.IsEditable) return cb.Text.Trim();
            if (cb.SelectedItem == null) return null;
            ComboBoxItem cbi = cb.SelectedItem as ComboBoxItem;
            if (cbi != null) return cbi.Content.ToString();
            else
            {
                return cb.SelectedItem.ToString();
            }
        }

        public static string SplitStrByLen(string str, int nc)
        {
            if (str.Length < nc) return str;
            string rstr = "";
            for(int i = 0; i < str.Length; i++)
            {
                if (i % nc == 0 && !String.IsNullOrEmpty(rstr))
                {
                    rstr += "\r\n";
                }
                rstr += str[i];
            }
            return rstr;
        }
    }
}


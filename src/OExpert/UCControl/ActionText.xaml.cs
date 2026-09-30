using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace OExpert
{

    //-----------------------------------------------------------
    public enum PMode
    {
        ActionMode,
        EditMode
    }

    //-----------------------------------------------------------
    public static class ActionTextPMode
    {
        public static PMode pmode;
    }

    //-----------------------------------------------------------
    public class ObjectLink
    {
        public string sText;
        public string sTag;
    }


    //--------------------------------------------------------------
    public partial class ActionText : UserControl
    {
        public delegate void ActionLeftMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e);
        public delegate void ActionLeftMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e);
        public delegate void ActionMouseEnter(object sender, System.Windows.Input.MouseEventArgs e);

        private ActionLeftMouseUp _actionLeftMouseUp = null;
        private ActionLeftMouseDown _actionLeftMouseDown = null;
        private ActionMouseEnter _actionMouseEnter = null;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ActionText), new PropertyMetadata(new PropertyChangedCallback(OnTextChanged)));
        //        private WrapPanel.WrapPanel layoutRoot;
        //        private StackPanel layoutRoot;

        public PMode pMode = PMode.ActionMode;

        private System.Windows.Media.Brush gForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Magenta);
        private System.Windows.Media.Brush gForegroundNormal = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);


        public ActionText()
        {
            InitializeComponent();
        }

        //-----------------------------------------------------------
        public void SetPMode(PMode pm)
        {
            pMode = pm;
        }

        //-----------------------------------------------------------
        public void SetAction(ActionLeftMouseUp pAction)
        {
            pActionLeftMouseUp = new ActionLeftMouseUp(pAction);
        }

        //-----------------------------------------------------------
        public ActionLeftMouseUp pActionLeftMouseUp
        {
            get
            {
                return _actionLeftMouseUp;
            }
            set
            {
                _actionLeftMouseUp = new ActionLeftMouseUp(value);
            }
        }

        //-----------------------------------------------------------
        public ActionLeftMouseDown pActionLeftMouseDown
        {
            get
            {
                return _actionLeftMouseDown;
            }
            set
            {
                _actionLeftMouseDown = new ActionLeftMouseDown(value);
            }
        }

        //-----------------------------------------------------------
        public ActionMouseEnter pActionMouseEnter
        {
            get
            {
                return _actionMouseEnter;
            }
            set
            {
                _actionMouseEnter = new ActionMouseEnter(value);
            }
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Gets or sets the text of the ActionText control
        /// </summary>
        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }

            set
            {
                base.SetValue(TextProperty, value);
            }
        }

        //-----------------------------------------------------------
        public override void OnApplyTemplate()
        {
            this.ProcessText();

            base.OnApplyTemplate();
        }

        //-----------------------------------------------------------
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ActionText actText = d as ActionText;

            actText.ProcessText();
        }

        //-----------------------------------------------------------
        private void ProcessText()
        {
            ProcessText(this.Text);
        }

        //-----------------------------------------------------------
        public void ProcessText(string sstr)
        {
            if (this.LayoutRootX == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(sstr))
            {
                this.LayoutRootX.Children.Clear();
                return;
            }

            // clear current text
            this.LayoutRootX.Children.Clear();
            if (ActionTextPMode.pmode == PMode.ActionMode)
            {
                string[] sSep = { "\r\n" };
                string[] subx = sstr.Split(sSep, StringSplitOptions.None);
                for (int i = 0; i < subx.Length; i++)
                {
                    List<ObjectLink> allLinks = this.GetObjectMatches(subx[i]);
                    this.MakeLayout(allLinks, subx[i]);
                }
            }
            else
            {
                this.MakeSimpleTextBox(sstr);
            }
        }

        //-----------------------------------------------------------
        private void MakeSimpleTextBox(string sstr)
        {
            TextBox tbText = new TextBox();
            tbText.TextWrapping = TextWrapping.Wrap;
            tbText.Text = sstr;
            tbText.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            tbText.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            tbText.Margin = new Thickness(0, 0, 0, 0);
            LayoutRootX.Children.Add(tbText);
        }

        //------------------------------------------------Not Used: cannot activate
        public void aWPanel_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.C)
            {
                if ((System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
                {
                    //specific Ctrl+C action here
                    MessageBox.Show("KEYUP:CTRL+C");
                    System.Windows.Clipboard.SetText(this.Text);
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        //-----Global action to copy to clipboard
        //----Need permission of users
        public void aWPanel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //            System.Windows.Clipboard.SetText(XStr.RemoveDBTag(this.Text));
            System.Windows.Clipboard.SetText(this.Text);
        }

        //-----------------------------------------------------------
        //-Composing the layout string with all the elements
        private void MakeLayout(List<ObjectLink> links, string sstr)
        {
            if (pActionLeftMouseUp == null) pActionLeftMouseUp = MainWindow.tp.ActionTextAction;

            string actTextText0 = sstr;

            string preUri = string.Empty;
            string postUri = string.Empty;
            int iStartIndexOfUri = 0;
            //            char[] delimiter = { ' ', ',', '。' };
            char[] delimiter = { ' ' };
            WrapPanel aWPanel = new WrapPanel();
            aWPanel.Margin = new Thickness(0, 0, 0, 0);
            aWPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            aWPanel.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            LayoutRootX.Children.Add(aWPanel);
            // no uris found
            if (links == null || links.Count == 0)
            {
                // put all the words before the current Uri
                aWPanel.Children.Add(new TextBlock()
                {
                    Text = sstr,
                    TextWrapping = TextWrapping.Wrap,
                });
                return;
            }

            string actTextText = sstr;

            foreach (ObjectLink link in links)
            {
                string sUrl = "<<" + link.sText;
                if (link.sTag != null && link.sTag != "") sUrl += "@" + link.sTag;
                sUrl += ">>";
                iStartIndexOfUri = actTextText.IndexOf(sUrl, StringComparison.OrdinalIgnoreCase);
                preUri = actTextText.Substring(0, iStartIndexOfUri);
                postUri = actTextText.Substring(preUri.Length + sUrl.Length);
                actTextText = postUri;

                // put all the words before the current Uri, Needed due to the textblock is used as an element
                ProcessString2Words(ref aWPanel, preUri);

                // insert the object link
                TextBlock hyperlinkT = null;
                bool fBold = false;
                if (link.sText.StartsWith("&fb"))
                {
                    string stext = link.sText.Replace("&fb", "");
                    hyperlinkT = new TextBlock()
                    {
                        Text = stext + " ",
                        Foreground = gForegroundNormal,
                        FontWeight = FontWeights.Black,
                        Tag = "",
                        Cursor = System.Windows.Input.Cursors.Arrow,
                        TextWrapping = TextWrapping.Wrap,
                    };
                    fBold = true;
                }
                else
                {
                    hyperlinkT = new TextBlock()
                    {
                        Text = link.sText + " ",
                        Foreground = gForeground,
                        Tag = link.sTag,
                        Cursor = System.Windows.Input.Cursors.Hand,
                        TextWrapping = TextWrapping.Wrap,
                    };
                }
                if (!fBold)
                {
                    //?? hyperlinkT.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.ClickActionText);
                    if (pActionLeftMouseUp != null) hyperlinkT.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(pActionLeftMouseUp);
                    if (pActionLeftMouseDown != null) hyperlinkT.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(pActionLeftMouseDown);
                    if (pActionMouseEnter != null) hyperlinkT.MouseEnter += new System.Windows.Input.MouseEventHandler(pActionMouseEnter);
                    hyperlinkT.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(aWPanel_MouseLeftButtonUp);
                }

                aWPanel.Children.Add(hyperlinkT);
            }

            // append the text after the last uri found
            if (!string.IsNullOrEmpty(actTextText))
            {
                ProcessString2Words(ref aWPanel, postUri);
            }
        }

        //-----------------------------------------------------------
        private void ProcessString2Words(ref WrapPanel aWPanel, string sstr)
        {
            char[] delimiter = { ' ' };

            string[] aWords = null;
            string[] sSep = { "\r\n" };
            string[] substr = sstr.Split(sSep, StringSplitOptions.RemoveEmptyEntries);
            // put all the words before the current Uri, Needed due to the textblock is used as an element
            for (int ix = 0; ix < substr.Length; ix++)
            {
                if (ix > 0)
                {
                    if (aWPanel == null)
                    {
                        aWPanel = new WrapPanel();
                        LayoutRootX.Children.Add(aWPanel);
                    }
                }
                aWords = substr[ix].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                foreach (string a in aWords)
                {
                    TextBlock t = new TextBlock()
                    {
                        Text = a + " ",
                        TextWrapping = TextWrapping.Wrap
                    };
                    aWPanel.Children.Add(t);
                    t.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(aWPanel_MouseLeftButtonUp);
                }
            }
        }

        //-----------------------------------------------------------
        private List<ObjectLink> GetObjectMatches(string sstr)
        {
            List<ObjectLink> rList = new List<ObjectLink>();
            string pstr = sstr;
            int ib = 0;
            int ie = 0;
            while (pstr.Length > 0)
            {
                ib = pstr.IndexOf("<<");
                if (ib >= 0)
                {
                    ie = pstr.IndexOf(">>", ib);
                    if (ie >= 0)
                    {
                        ie += 2;
                        string sobject = pstr.Substring(ib, ie - ib);
                        //Make ObjectLink
                        ObjectLink aobj = GetObjectLink(sobject);
                        //Add the the ObjectLink list
                        if (aobj != null) rList.Add(aobj);
                        pstr = pstr.Substring(ie);
                    }
                    else break;
                }
                else break;
            }
            return rList;
        }

        //-----------------------------------------------------------
        private ObjectLink GetObjectLink(string sstr0)
        {
            if (sstr0 == null || sstr0 == "") return null;
            string sstr = sstr0.Replace('<', ' ').Replace('>', ' ').Trim();
            ObjectLink aobj = new ObjectLink();
            string[] substr = sstr.Split('@');
            aobj.sText = substr[0];
            aobj.sTag = "";
            if (substr.Length > 1)
            {
                aobj.sTag = substr[1].Trim();
            }
            return aobj;
        }

        /*
        //-----------------------------------------------------------
        private void ClickActionText(object sender, RoutedEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            //            if (pActionLeftMouseUp != null) ActionLeftMouseUp(sender, (System.Windows.Input.MouseButtonEventArgs)e);
            MessageBox.Show(tb.Tag.ToString());
        }
        */
    }

}


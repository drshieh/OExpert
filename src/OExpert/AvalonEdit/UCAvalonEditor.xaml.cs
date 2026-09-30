using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;

namespace OExpert
{
    /// <summary>
    /// Interaction logic for AvalonEdit.xaml
    /// </summary>
    public partial class UCAvalonEditor : UserControl
    {
        public string aTText;
        //       public string aTType = ".cs";
        public string aTType = ".cs";

        public string currentFileName;

        //    public FindJsonData wFindForm;
        //    public ReplaceText wReplaceForm;


        //--------------------------------------------------------
        public UCAvalonEditor()
        {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("OExpert.AvalonEdit.CustomHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, customHighlighting);


            InitializeComponent();

            this.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);


            //textEditor.TextArea.SelectionBorder = null;

            //textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            //textEditor.SyntaxHighlighting = customHighlighting;
            // initial highlighting now set by XAML

            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            SearchPanel.Install(textEditor);

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            foldingUpdateTimer.Start();

            textEditor.Text = aTText;
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(aTType);
        }

        //--------------------------------------------------------
        public void ShowData(string _ttype, string _ttext)
        {
            aTType = _ttype;
            aTText = _ttext;
            textEditor.Text = aTText;
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(aTType);
        }

        //--------------------------------------------------------
        public void ClearData()
        {
            aTText = "";
            textEditor.Text = aTText;
        }

        void openFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                currentFileName = dlg.FileName;
                textEditor.Text = File.ReadAllText(currentFileName);
                //				textEditor.Load(currentFileName);
                textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
            }
        }

        void saveFileClick(object sender, EventArgs e)
        {
            if (currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".txt";
                if (dlg.ShowDialog() ?? false)
                {
                    currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            textEditor.Save(currentFileName);
        }

        CompletionWindow completionWindow;

        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(textEditor.TextArea);
                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("Item1"));
                data.Add(new MyCompletionData("Item2"));
                data.Add(new MyCompletionData("Item3"));
                data.Add(new MyCompletionData("Another item"));
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        #region Folding
        FoldingManager foldingManager;
        object foldingStrategy;

        void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (textEditor.SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {
                switch (textEditor.SyntaxHighlighting.Name)
                {
                    case "XML":
                        foldingStrategy = new XmlFoldingStrategy();
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                        foldingStrategy = new BraceFoldingStrategy();
                        break;
                    case "Json":
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                        foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        foldingStrategy = null;
                        break;
                }
            }
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(textEditor.TextArea);
                UpdateFoldings();
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
        }

        void UpdateFoldings()
        {
            if (foldingStrategy is BraceFoldingStrategy)
            {
                ((BraceFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            }
            if (foldingStrategy is XmlFoldingStrategy)
            {
                ((XmlFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            }
        }
        #endregion

        //-----------------------------------------------------------
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                if (tb == tbFind)
                {
                    FindText(tbFind.Text, false);
                }
                else if (tb == tbGoToLine)
                {
                    int iLine = -1;
                    if (Int32.TryParse(tbGoToLine.Text, out iLine))
                    {
                        iLine--;  //Because the display start with 1, but here start with 0
                        GoToLine(iLine);
                    }
                }
            }
        }

        //-----------------------------------------------------------
        public void FindText(string sFind, bool fCase)
        {
            if (String.IsNullOrEmpty(sFind) ||
                String.IsNullOrEmpty(this.textEditor.Text)) return;

            int ib = this.textEditor.SelectionStart + this.textEditor.SelectionLength;
            if (ib < 0) ib = 0;
            string sFindU = "";
            int ib1 = -1;
            if (fCase)
            {
                sFindU = sFind.ToUpper();
                ib1 = this.textEditor.Text.ToUpper().IndexOf(sFindU, ib);
            }
            else
            {
                sFindU = sFind;
                ib1 = this.textEditor.Text.IndexOf(sFindU, ib);
            }
            if (ib1 >= 0)
            {
                this.textEditor.SelectionStart = ib1;
                this.textEditor.SelectionLength = sFindU.Length;
            }
            else
            {
                this.textEditor.SelectionStart = 0;
                this.textEditor.SelectionLength = 0;
            }
            textEditor.ScrollToLine(this.textEditor.TextArea.Caret.Line - 1);
        }

        //-----------------------------------------------------------
        public void ReplaceText(string sOldString, string sNewString, bool fCase)
        {
            if (String.IsNullOrEmpty(sOldString) ||
                String.IsNullOrEmpty(this.textEditor.Text)) return;

            //            int ib = this.textEditor.SelectionStart + this.textEditor.SelectionLength;
            int ib = this.textEditor.SelectionStart;
            if (ib < 0) ib = 0;
            string sFindU = "";
            int ib1 = -1;
            if (fCase)
            {
                sFindU = sOldString.ToUpper();
                ib1 = this.textEditor.Text.ToUpper().IndexOf(sFindU, ib);
            }
            else
            {
                sFindU = sOldString;
                ib1 = this.textEditor.Text.IndexOf(sFindU, ib);
            }
            if (ib1 >= 0)
            {
                this.textEditor.Text = this.textEditor.Text.Substring(0, ib1) +
                                       sNewString +
                                       this.textEditor.Text.Substring(ib1 + sFindU.Length);
                this.textEditor.SelectionStart = ib1;
                this.textEditor.SelectionLength = sNewString.Length;
            }
            else
            {
                this.textEditor.SelectionStart = 0;
                this.textEditor.SelectionLength = 0;
            }
            textEditor.ScrollToLine(this.textEditor.TextArea.Caret.Line - 1);
        }

        //----------------------------------------------------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == btnFind)  //-Use Find control value
            {
                FindText(tbFind.Text, false);
            }
            else if (btn == btnGo)
            {
                int iLine = -1;
                if (Int32.TryParse(tbGoToLine.Text, out iLine))
                {
                    iLine--;  //Because the display start with 1, but here start with 0
                    GoToLine(iLine);
                }
            }
        }

        //-------------------------------------------------------
        public void GoToLine(int iLine)
        {
            string[] lines = textEditor.Text.Split('\n');
            if (iLine < 0)
            {
                iLine = 0;
                tbGoToLine.Text = "0";
            }
            if (iLine >= lines.Length)
            {
                iLine = lines.Length - 1;
                tbGoToLine.Text = (lines.Length - 1).ToString();
            }
            textEditor.ScrollTo(iLine, 0);
            int istart = 0;
            for (int i = 0; i < iLine; i++)
            {
                istart += lines[i].Length + 1;
            }
            textEditor.SelectionStart = istart;
            textEditor.SelectionLength = lines[iLine].Length;
        }
    }
}

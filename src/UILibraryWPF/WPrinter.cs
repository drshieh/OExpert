using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace OExpert.UILibraryWPF
{
    public class WPrinter
    {
        public static double gxPageRes = 96;
        public static double gyPageRes = 96;
        public static double gPaperSizeLeft = 0.25;  //--In pixels, 96 pixels/in
        public static double gPaperSizeTop = 0.25;
        public static double gPaperSizeWidth = 8.5;
        public static double gPaperSizeHeight = 11;

        /// <summary>  
        /// This method creates a dynamic FlowDocument. You can add anything to this  
        /// FlowDocument that you would like to send to the printer  
        /// </summary>  
        /// <returns></returns>  
        private FlowDocument CreateFlowDocument()
        {
            // Create a FlowDocument  
            FlowDocument doc = new FlowDocument();
            // Create a Section  
            Section sec = new Section();
            // Create first Paragraph  
            Paragraph p1 = new Paragraph();
            // Create and add a new Bold, Italic and Underline  
            Bold bld = new Bold();
            bld.Inlines.Add(new Run("First Paragraph"));
            Italic italicBld = new Italic();
            italicBld.Inlines.Add(bld);
            Underline underlineItalicBld = new Underline();
            underlineItalicBld.Inlines.Add(italicBld);
            // Add Bold, Italic, Underline to Paragraph  
            p1.Inlines.Add(underlineItalicBld);
            // Add Paragraph to Section  
            sec.Blocks.Add(p1);
            // Add Section to FlowDocument  
            doc.Blocks.Add(sec);
            return doc;
        }

        //-----------------------------------------------------------------
        private void PrintSimpleTextButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a PrintDialog  
            System.Windows.Controls.PrintDialog printDlg = new PrintDialog();
            // Create a FlowDocument dynamically.  
            FlowDocument doc = CreateFlowDocument();
            doc.Name = "FlowDoc";
            // Create IDocumentPaginatorSource from FlowDocument  
            IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer  
            printDlg.PrintDocument(idpSource.DocumentPaginator, "Hello WPF Printing.");
        }

        /*
        Print a Control, User Control or a Window in WPF

In WPF, a Visual is an object that is parent class of all user interfaces including UIElement, Containers, Controls, UserControls, 
and even Viewport3DVisual.If you notice all control or user controls classes, they are inherited from a UIElement class.
The PrintVisual print a Visual object. That means, by using the PrintVisual method, we can print any control, container, Window or user control.
*/
        //-The following code snippet in creates a PrintDialog object and calls its PrintVisual method by passing a UserControl to print 
        //the UserControl.Using this method, we can print any controls in WPF including a Window, page, or a ListBox.

        private void PrintUserControl()
        {
            PrintDialog printDlg = new PrintDialog();
            UserControl uc = new UserControl();
            printDlg.PrintVisual(uc, "User Control Printing.");

        }

        //-What if you want to print a Grid control or any other control?
        //-As said above, printing any control in WPF is same process.Just pass the Grid or other control in the PrintVisual method of PrintDialog.
        private void PrintGrid(Grid grid1)
        {
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintVisual(grid1, "Grid Printing.");
        }

        //-How about printing the entire Window?
        //-For entire window, you can either pass the window object or this keyword.This time, you pass "this" that is the object of current Window where you are writing this code.
        private void PrintWindow(Window aWnd)
        {
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintVisual(aWnd, "Window Printing.");
        }

        //=============================================================================================
        public Font verdana10Font = new Font("Verdana", 10);
        public StreamReader gReader = null;

        public void PrintTextFile(string fileName)
        {
            //Create a StreamReader object  
            gReader = new StreamReader(fileName);
            //Create a Verdana font with size 10  
            verdana10Font = new Font("Verdana", 10);
            //Create a PrintDocument object  
            PrintDocument pd = new PrintDocument();
            //Add PrintPage event handler  
            pd.PrintPage += new PrintPageEventHandler(this.PrintTextFileHandler);
            //Call Print Method  
            pd.Print();
            //Close the reader  
            if (gReader != null)
                gReader.Close();
        }

        private void PrintTextFileHandler(object sender, PrintPageEventArgs ppeArgs)
        {
            //Get the Graphics object  
            Graphics g = ppeArgs.Graphics;
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = ppeArgs.MarginBounds.Left;
            float topMargin = ppeArgs.MarginBounds.Top;
            string line = null;
            //Calculate the lines per page on the basis of the height of the page and the height of the font  
            linesPerPage = ppeArgs.MarginBounds.Height / verdana10Font.GetHeight(g);
            //Now read lines one by one, using StreamReader  
            while (count < linesPerPage && ((line = gReader.ReadLine()) != null))
            {
                //Calculate the starting position  
                yPos = topMargin + (count * verdana10Font.GetHeight(g));
                //Draw text  
                g.DrawString(line, verdana10Font, Brushes.Black, leftMargin, yPos, new StringFormat());
                //Move to next line  
                count++;
            }
            //If PrintPageEventArgs has more pages to print  
            if (line != null)
            {
                ppeArgs.HasMorePages = true;
            }
            else
            {
                ppeArgs.HasMorePages = false;
            }
        }

        //=============================================================================================
        protected void PrintText_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPosition = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;
            SolidBrush myBrush = new SolidBrush(Color.Black);
            // Work out the number of lines per page, using the MarginBounds.  
            linesPerPage = ev.MarginBounds.Height / verdana10Font.GetHeight(ev.Graphics);
            // Iterate over the string using the StringReader, printing each line.  
            while (count < linesPerPage && ((line = gReader.ReadLine()) != null))
            {
                // calculate the next line position based on the height of the font according to the printing device  
                yPosition = topMargin + (count * verdana10Font.GetHeight(ev.Graphics));
                // draw the next line in the rich edit control  
                ev.Graphics.DrawString(line, verdana10Font, myBrush, leftMargin, yPosition, new StringFormat());
                count++;
            }
            // If there are more lines, print another page.  
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
            myBrush.Dispose();
        }

        protected void PreviewFile(string fName)
        {
            try
            {
                //in a methode on the same class:
                PageSettings setting = new PageSettings();
                setting.Landscape = true;

                PrintDocument _document = new PrintDocument();
                _document.DocumentName = fName;
                //                _document.PrintPage += _document_PrintPage;
                _document.DefaultPageSettings = setting;

                System.Windows.Forms.PrintPreviewDialog printDlg = new System.Windows.Forms.PrintPreviewDialog();
                printDlg.Document = _document;
                printDlg.Height = 500;
                printDlg.Width = 200;
                if (printDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _document.Print();
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message.ToString());
            }
        }

        //=========================================================================================
        /*
        What you want to do, is to create an xpsDocument out from the content you want to print(a flowDocument) 
            and use that XpsDocument to preview the content, for example let say you have the following Xaml, 
            with a flowDocument that you want to print its content :
 <Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height = "*" />
        < RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
    <FlowDocumentScrollViewer>
        <FlowDocument x:Name="FD">
            <Paragraph>
                <Image Source = "http://www.wpf-tutorial.com/images/logo.png" Width="90" Height="90" Margin="0,0,30,0" />
                <Run FontSize = "120" > WPF </ Run >
            </ Paragraph >

            < Paragraph >
                WPF, which stands for
                <Bold>Windows Presentation Foundation</Bold> ,
                is Microsoft's latest approach to a GUI framework, used with the .NET framework.
                Some advantages include:
            </Paragraph>

            <List>
                <ListItem>
                    <Paragraph>
                        It's newer and thereby more in tune with current standards
                    </Paragraph>
                </ListItem>
                <ListItem>
                    <Paragraph>
                        Microsoft is using it for a lot of new applications, e.g.Visual Studio
                    </Paragraph>
                </ListItem>
                <ListItem>
                    <Paragraph>
                        It's more flexible, so you can do more things without having to write or buy new controls
                    </Paragraph>
                </ListItem>
            </List>

        </FlowDocument>
    </FlowDocumentScrollViewer>        
    <Button Content = "Print" Grid.Row= "1" Click= "Button_Click" ></ Button >
</ Grid >

the flowDocument Sample is from Wpf tutorials site

the print button Click event handler should looks like this :
 private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("printPreview.xps"))
            {
                File.Delete("printPreview.xps");
            }
            var xpsDocument = new XpsDocument("printPreview.xps", FileAccess.ReadWrite);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            writer.Write(((IDocumentPaginatorSource)FD).DocumentPaginator);
            Document = xpsDocument.GetFixedDocumentSequence();
            xpsDocument.Close();
            var windows = new PrintWindow(Document);
            windows.ShowDialog();
        }

        public FixedDocumentSequence Document { get; set; }

        so here you are mainly :
•Creating an Xps document and storing it in printPreview.xps file,
•Writing the FlowDocument content into that file,
•passing the XpsDocument to the PrintWindow in which you will handle the preview and the print actions,

here how the PrintWindow looks like :
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width = "*" />
        < ColumnDefinition Width="1.5*"/>
    </Grid.ColumnDefinitions>
    <StackPanel>
        <Button Content = "Print" Click="Button_Click"></Button>
        <!--Other print operations-->
    </StackPanel>
    <DocumentViewer Grid.Column="1" x:Name= "PreviewD" >
    </ DocumentViewer >
</ Grid >

and the code behind :
public partial class PrintWindow : Window
        {
            private FixedDocumentSequence _document;
            public PrintWindow(FixedDocumentSequence document)
            {
                _document = document;
                InitializeComponent();
                PreviewD.Document = document;
            }

            private void Button_Click(object sender, RoutedEventArgs e)
            {
                //print directly from the Xps file 
            }
        }
        */


        //###############################################################
        public static void PrintXPS(string filename)
        {
            FixedDocument doc = CreateFixedDocument();
            XpsDocumentWriter xw = GetPrintXpsDocumentWriter();
            xw.Write(doc);
        }

        //-------------------------------------------------------------
        public static void WriteXPS(string filename)
        {
            FixedDocument doc = CreateFixedDocument();
            XpsDocument xpsd = new XpsDocument(filename, FileAccess.ReadWrite);
            XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
            xw.Write(doc);
            xpsd.Close();
        }

        public static FixedDocument CreateFixedDocument()
        {
            FixedDocument doc = new FixedDocument();
            doc.DocumentPaginator.PageSize = new System.Windows.Size(96 * 8.5, 96 * 11);

            //            foreach (page that I want)
            for (int i = 0; i < 5; i++)
            {
                PageContent page = new PageContent();
                FixedPage fixedPage = new FixedPage();
                fixedPage.Background = System.Windows.Media.Brushes.White;
                fixedPage.Width = 96 * 8.5;
                fixedPage.Height = 96 * 11;

                //--Title at the top
                TextBlock tbTitle = new TextBlock();
                tbTitle.Text = "My Page Title";
                tbTitle.FontSize = 24;
                tbTitle.FontFamily = new System.Windows.Media.FontFamily("Arial");
                FixedPage.SetLeft(tbTitle, 96 * 0.75); // left margin
                FixedPage.SetTop(tbTitle, 96 * 0.75); // top margin
                fixedPage.Children.Add((UIElement)tbTitle);

                //Now we add our Viewport3D to the page.  We want it to be 2 inches from the top of the sheet and 2 inches from the left side.We'll assume we have already prepared a Viewport3D which is the proper size to fit.  I think I'll draw a thin black border around it as well.

                Border b = new Border();
                b.BorderThickness = new Thickness(1);
                b.BorderBrush = System.Windows.Media.Brushes.Black;
                //--An UI element               b.Child = myViewport3D;
                FixedPage.SetLeft(b, 96 * 2);
                FixedPage.SetTop(b, 96 * 2);
                fixedPage.Children.Add((UIElement)b);


                System.Windows.Size sz = new System.Windows.Size(96 * 8.5, 96 * 11);
                page.Measure(sz);
                page.Arrange(new Rect(new System.Windows.Point(), sz));


                ((IAddChild)page).AddChild(fixedPage);


                page.UpdateLayout();
                doc.Pages.Add(page);
            }
            return doc;
        }

        // ---------------------- GetPrintTicketFromPrinter -----------------------
        /// <summary>
        ///   Returns a PrintTicket based on the current default printer.</summary>
        /// <returns>
        ///   A PrintTicket for the current local default printer.</returns>
        public static PrintTicket GetPrintTicketFromPrinter()
        {
            PrintQueue printQueue = null;

            LocalPrintServer localPrintServer = new LocalPrintServer();

            // Retrieving collection of local printer on user machine
            PrintQueueCollection localPrinterCollection = localPrintServer.GetPrintQueues();

            System.Collections.IEnumerator localPrinterEnumerator = localPrinterCollection.GetEnumerator();

            if (localPrinterEnumerator.MoveNext())
            {
                // Get PrintQueue from first available printer
                printQueue = (PrintQueue)localPrinterEnumerator.Current;
            }
            else
            {
                // No printer exist, return null PrintTicket
                return null;
            }

            // Get default PrintTicket from printer
            PrintTicket printTicket = printQueue.DefaultPrintTicket;

            PrintCapabilities printCapabilites = printQueue.GetPrintCapabilities();

            // Modify PrintTicket
            if (printCapabilites.CollationCapability.Contains(Collation.Collated))
            {
                printTicket.Collation = Collation.Collated;
            }

            if (printCapabilites.DuplexingCapability.Contains(Duplexing.TwoSidedLongEdge))
            {
                printTicket.Duplexing = Duplexing.TwoSidedLongEdge;
            }

            if (printCapabilites.StaplingCapability.Contains(Stapling.StapleDualLeft))
            {
                printTicket.Stapling = Stapling.StapleDualLeft;
            }

            return printTicket;
        }// end:GetPrintTicketFromPrinter()

        // -------------------- GetPrintXpsDocumentWriter() -------------------
        /// <summary>
        ///   Returns an XpsDocumentWriter for the default print queue.</summary>
        /// <returns>
        ///   An XpsDocumentWriter for the default print queue.</returns>
        public static XpsDocumentWriter GetPrintXpsDocumentWriter()
        {
            // Create a local print server
            LocalPrintServer ps = new LocalPrintServer();

            // Get the default print queue
            PrintQueue pq = ps.DefaultPrintQueue;

            // Get an XpsDocumentWriter for the default print queue
            XpsDocumentWriter xpsdw = PrintQueue.CreateXpsDocumentWriter(pq);
            return xpsdw;
        }// end:GetPrintXpsDocumentWriter()

        // -------------------- GetPrintXpsDocumentWriter() -------------------
        /// <summary>
        ///   Returns an XpsDocumentWriter for the default print queue.</summary>
        /// <returns>
        ///   An XpsDocumentWriter for the default print queue.</returns>
        public static XpsDocumentWriter GetPrintXpsDocumentWriter2()
        {
            // Create a local print server
            LocalPrintServer ps = new LocalPrintServer();

            // Get the default print queue
            PrintQueue pq = ps.DefaultPrintQueue;

            // Get an XpsDocumentWriter for the default print queue
            XpsDocumentWriter xpsdw = PrintQueue.CreateXpsDocumentWriter(pq);
            return xpsdw;
        }// end:GetPrintXpsDocumentWriter()

        //--------------------------------------------------------------------
        public static bool GetPrintPageSize()
        {
            PrintDialog Objprint = new PrintDialog();
            //get selected printer capabilities
            PrintCapabilities capabilities = Objprint.PrintQueue.GetPrintCapabilities(Objprint.PrintTicket);
            //get the size of the printer page
            gPaperSizeLeft = capabilities.PageImageableArea.OriginWidth / gxPageRes;
            gPaperSizeTop = capabilities.PageImageableArea.OriginHeight / gyPageRes;
            gPaperSizeWidth = capabilities.PageImageableArea.ExtentWidth / gxPageRes;
            gPaperSizeHeight = capabilities.PageImageableArea.ExtentHeight / gyPageRes;
            return true;
        }
    }
}

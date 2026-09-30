using HelixToolkit.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

//using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace OExpert
{
    //========================================================================
    public class XRule_UI
    {
        public static Rectangle selectionBox = null;

        public static void SetItemTip(object o, TextBlock tbHead, TextBlock tbTip, TextBlock tbRulesHead1, TextBlock tbRules1, TextBlock tbRulesHead2, TextBlock tbRules2)
        {
            tbHead.Text = "";
            tbTip.Text = "";
            tbRulesHead1.Text = "";
            tbRules1.Text = "";
            tbRulesHead2.Text = "";
            tbRules2.Text = "";
            if (o == null) return;
            string sRtn = "\r\n";
            if (o.GetType() == typeof(XRule))
            {
                XRule x = o as XRule;
                tbHead.Text = String.Format("Rule: {0} ({1})", x.Name, x.ID);
                tbTip.Text = "Conditions : ";
                if (x.Conditions != null)
                {
                    foreach (XCondition c in x.Conditions)
                    {
                        if (!String.IsNullOrEmpty(c.Or)) tbTip.Text += sRtn + String.Format("  {0} {1} {2} {3} => {4} ({5})", c.Or, c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue);
                        else tbTip.Text += sRtn + String.Format("  {0} {1} {2} => {3} ({4})", c.Operator, c.Condition, c.WantValue, c.InputValue, c.EvaluatedValue);
                    }
                }
                tbTip.Text += sRtn + "Conclusion: " + x.Conclusion;
                tbTip.Text += sRtn + "Probability: " + x.Probability;
                tbTip.Text += sRtn + "CalcProbability: " + x.CalcProbability;
                tbTip.Text += sRtn + "Value: " + x.Value;
                tbTip.Text += sRtn + "Message : " + x.Message;

                if (x.aChildRules != null && x.aChildRules.Count > 0)
                {
                    tbRulesHead1.Visibility = Visibility.Visible;
                    tbRules1.Visibility = Visibility.Visible;
                    tbRulesHead1.Text = "Child Rules:";
                    tbRules1.Text = "";
                    int i = 0;
                    foreach (XRule r in x.aChildRules)
                    {
                        if(i > 0) tbRules1.Text += sRtn + " Rule " + r.toStr2();
                        else tbRules1.Text += " Rule " + r.toStr2();
                        i++;
                    }
                }
                else {
                    tbRulesHead1.Visibility = Visibility.Collapsed;
                    tbRules1.Visibility = Visibility.Collapsed;
                }
                if (x.aFromRules != null && x.aFromRules.Count > 0)
                {
                    tbRulesHead2.Visibility = Visibility.Visible;
                    tbRules2.Visibility = Visibility.Visible;
                    tbRulesHead2.Text = "From Rules:";
                    tbRules2.Text = "";
                    int i = 0;
                    foreach (XRule r in x.aFromRules)
                    {
                        if (i > 0) tbRules2.Text += sRtn + " Rule " + r.toStr2();
                        else tbRules2.Text += " Rule " + r.toStr2();
                        i++;
                    }
                }
                else
                {
                    tbRulesHead2.Visibility = Visibility.Collapsed;
                    tbRules2.Visibility = Visibility.Collapsed;
                }

            }
            else if (o.GetType() == typeof(XFact))
            {
                tbRulesHead2.Visibility = Visibility.Collapsed;
                tbRules2.Visibility = Visibility.Collapsed;
                XFact x = o as XFact;
                tbHead.Text = "Fact: " + x.fact;
                tbTip.Text = "  in # Rules: " + (x.ruleList == null ? 0 : x.ruleList.Count);
                tbRulesHead1.Text = "";
                tbRules1.Text = "";
                if (x.ruleList != null)
                {
                    tbRulesHead1.Visibility = Visibility.Visible;
                    tbRules1.Visibility = Visibility.Visible;

                    tbRulesHead1.Text = "Rules:";
                    int i = 0;
                    foreach (XRule r in x.ruleList)
                    {
                        if(i > 0) tbRules1.Text += sRtn + "Rule " + r.toStr2();
                        else tbRules1.Text += "Rule " + r.toStr2();
                        i++;
                    }
                }
                else
                {
                    tbRulesHead1.Visibility = Visibility.Collapsed;
                    tbRules1.Visibility = Visibility.Collapsed;
                }
            }
            else if (o.GetType() == typeof(XConclusion))
            {
                tbRulesHead2.Visibility = Visibility.Collapsed;
                tbRules2.Visibility = Visibility.Collapsed;
                XConclusion x = o as XConclusion;
                tbHead.Text = "Conclusion: " + x.conclusion;
                tbTip.Text = "  in # Rules: " + (x.ruleList == null ? 0 : x.ruleList.Count);
                tbRulesHead1.Text = "";
                tbRules1.Text = "";
                if (x.ruleList != null)
                {
                    tbRulesHead1.Visibility = Visibility.Visible;
                    tbRules1.Visibility = Visibility.Visible;
                    tbRulesHead1.Text = "Rules:";
                    int i = 0;
                    foreach (XRule r in x.ruleList)
                    {
                        if (i > 0) tbRules1.Text += sRtn + " Rule " + r.toStr2();
                        else tbRules1.Text += " Rule " + r.toStr2();
                        i++;
                    }
                }
                else
                {
                    tbRulesHead1.Visibility = Visibility.Collapsed;
                    tbRules1.Visibility = Visibility.Collapsed;
                }
            }
        }

        //---------------------------------------------------
        public static string GetItemStr(object o)
        {
            if (o == null) return null;
            if (o.GetType() == typeof(XRule))
            {
                XRule x = o as XRule;
                return "Rule: " + x.toStr();
            }
            else if (o.GetType() == typeof(XFact))
            {
                XFact x = o as XFact;
                return "Fact: " + x.toStr();
            }
            else if (o.GetType() == typeof(XConclusion))
            {
                XConclusion x = o as XConclusion;
                return "Conclusion: " + x.toStr();
            }
            return null;
        }

        //------------------------------------------------------------------
        public static void ResetItems(List<object> aList)
        {
            if (aList.Count <= 0) return;
            foreach (object o in aList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = o as XRule;
                    a.FColor = Colors.Yellow;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = o as XFact;
                    a.FColor = Colors.Green;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = o as XConclusion;
                    a.FColor = Colors.LightBlue;
                }

            }
        }

        //------------------------------------------------------------------
        public static void SetItems(List<object> aList)
        {
            if (aList.Count <= 0) return;
            foreach (object o in aList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = o as XRule;
                    a.FColor = Colors.PaleVioletRed;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = o as XFact;
                    a.FColor = Colors.PaleVioletRed;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = o as XConclusion;
                    a.FColor = Colors.PaleVioletRed;
                }
            }
        }

        //-------------------------------------------------------------------------------------------------
        public static void MoveItems(List<object> aList, double dx, double dy)
        {
            if (aList == null || aList.Count <= 0) return;
            foreach (object o in aList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = o as XRule;
                    a.X += dx;
                    a.Y += dy;
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = o as XFact;
                    a.FColor = Colors.PaleVioletRed;
                    a.X += dx;
                    a.Y += dy;
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = o as XConclusion;
                    a.FColor = Colors.PaleVioletRed;
                    a.X += dx;
                    a.Y += dy;
                }
            }
        }

        //-------------------------------------------------------------
        public static void DrawRuleNetwork(Canvas canvas2D, XRuleNet aRuleNet)
        {
            if (aRuleNet == null) return;
            XRule_UI.ClearUI(canvas2D);
            if (aRuleNet.aRuleList == null) return;

            XRuleNet.SetCanvasSize(canvas2D, aRuleNet, 1, 1);

            //---Drawing
            foreach (XFact f in aRuleNet.aFactList)
            {
                XRule_UI.DrawFact2D(canvas2D, f, f.X, f.Y, f.Width, f.Height, f.FColor);
            }
            //--Draw Rules
            foreach (XRule r in aRuleNet.aRuleList)
            {
                XRule_UI.DrawRule2D(canvas2D, r, r.X, r.Y, r.Width, r.Height, r.FColor);
            }
            //--Draw Conclusion
            foreach (XConclusion a in aRuleNet.aConclusionList)
            {
                XRule_UI.DrawConclusion2D(canvas2D, a, a.X, a.Y, a.Width, a.Height, a.FColor);
            }

            //--Draw Connection lines------------------------------------------
            //--Draw Rule Links
            foreach (XRule a in aRuleNet.aRuleList)
            {
                if (a.aFromRules == null || a.aFromRules.Count < 0) continue;
                double cx = a.X + a.Width / 2;
                double cy = a.Y + a.Height / 2;
                foreach (XRule r in a.aFromRules)
                {
                    double ex = r.X + r.Width / 2;
                    double ey = r.Y + r.Height / 2;
                    XRule_UI.DrawLine2D(canvas2D, cx, cy, ex, ey, Colors.DarkBlue);
                }
            }

            foreach (XConclusion a in aRuleNet.aConclusionList)
            {
                double cx = a.X + a.Width / 2;
                double cy = a.Y + a.Height / 2;
                foreach (XRule r in a.ruleList)
                {
                    double ex = r.X + r.Width / 2;
                    double ey = r.Y + r.Height / 2;
                    XRule_UI.DrawLine2D(canvas2D, cx, cy, ex, ey, Colors.DarkGreen);
                }
            }
            //--Draw Connection lines
            foreach (XFact a in aRuleNet.aFactList)
            {
                double cx = a.X + a.Width / 2;
                double cy = a.Y + a.Height / 2;
                foreach (XRule r in a.ruleList)
                {
                    double ex = r.X + r.Width / 2;
                    double ey = r.Y + r.Height / 2;
                    XRule_UI.DrawLine2D(canvas2D, cx, cy, ex, ey, Colors.Red);
                }
            }

        }

        //-------------------------------------------------------------
        public static void ClearUI(Canvas canvas2D)
        {
            canvas2D.Children.Clear();
        }

        //--------------------------------------------------------------------------
        public static List<object> SelectItemInRect(XRuleNet aRuleNet, double bx, double by, double ex, double ey)
        {
            List<object> aList = new List<object>();
            double minX = Math.Min(bx, ex);
            double maxX = Math.Max(bx, ex);
            double minY = Math.Min(by, ey);
            double maxY = Math.Max(by, ey);
            foreach (XRule a in aRuleNet.aRuleList)
            {
                if(a.X >= minX && a.X <= maxX && a.Y >= minY && a.Y <= maxY)
                {
                    aList.Add(a);
                }
            }
            foreach (XFact f in aRuleNet.aFactList)
            {
                if (f.X >= minX && f.X <= maxX && f.Y >= minY && f.Y <= maxY)
                {
                    aList.Add(f);
                }
            }
            foreach (XConclusion c in aRuleNet.aConclusionList)
            {
                if (c.X >= minX && c.X <= maxX && c.Y >= minY && c.Y <= maxY)
                {
                    aList.Add(c);
                }
            }
            return aList;
        }

        //-------------------------------------------------------
        public static void RemoveSelectionBox(Canvas canvas)
        {
            if(selectionBox != null) canvas.Children.Remove(selectionBox);
        }

        //-------------------------------------------------------
        public static void DrawSelectionBox(Canvas canvas, double bx, double by, double ex, double ey)
        {
            selectionBox = new Rectangle();
            selectionBox.HorizontalAlignment = HorizontalAlignment.Left;
            selectionBox.VerticalAlignment = VerticalAlignment.Top;
            double minX = Math.Min(bx, ex);
            double maxX = Math.Max(bx, ex);
            double minY = Math.Min(by, ey);
            double maxY = Math.Max(by, ey);
            selectionBox.Width = Math.Abs(ex - bx);
            selectionBox.Height = Math.Abs(ey - by);
            selectionBox.Fill = null;
            selectionBox.Stroke = new SolidColorBrush(Colors.Black);
            selectionBox.Margin = new Thickness(minX, minY, 0, 0);
            canvas.Children.Add(selectionBox);
        }

        //-------------------------------------------------------
        public static void DrawFact2D(Canvas canvas, XFact aFact, double bx, double by, double width, double height, Color fillColor)
        {
            Rectangle e = new Rectangle();
            e.HorizontalAlignment = HorizontalAlignment.Left;
            e.VerticalAlignment = VerticalAlignment.Top;
            e.Width = width;
            e.Height = height;
            e.Fill = new SolidColorBrush(fillColor);
            e.Stroke = new SolidColorBrush(Colors.Black);
            e.Margin = new Thickness(bx, by, 0, 0);
            e.Tag = aFact;
            e.ToolTip = "Fact: " + aFact.fact;

            TextBlock t = new TextBlock();
            t.HorizontalAlignment = HorizontalAlignment.Left;
            t.VerticalAlignment = VerticalAlignment.Top;
            t.Width = width;
            t.Height = height;
            t.Background = new SolidColorBrush(Colors.Transparent);
            t.Foreground = new SolidColorBrush(Colors.Black);
            t.Margin = new Thickness(bx, by, 0, 0);
            t.TextAlignment = TextAlignment.Center;
            t.Tag = aFact;
            t.ToolTip = "Fact: " + aFact.fact;
            t.Text = "Fact:\r\n" + aFact.fact;

            canvas.Children.Add(e);
            canvas.Children.Add(t);
        }

        //-------------------------------------------------------
        public static void DrawLine2D(Canvas canvas, double bx, double by, double ex, double ey, Color strokeColor)
        {
            Line e = new Line();
            //            e.HorizontalAlignment = HorizontalAlignment.Left;
            //            e.VerticalAlignment = VerticalAlignment.Top;
            e.X1 = bx;
            e.Y1 = by;
            e.X2 = ex;
            e.Y2 = ey;
            e.Stroke = new SolidColorBrush(strokeColor);
            //            e.Margin = new Thickness(Math.Min(bx,ex), Math.Min(by,ey), Math.Max(bx, ex), Math.Max(by, ey));
            e.Visibility = System.Windows.Visibility.Visible;
            e.StrokeThickness = 1;
            canvas.Children.Add(e);
        }

        //-------------------------------------------------------
        public static void DrawRule2D(Canvas canvas, XRule aRule, double bx, double by, double width, double height, Color fillColor)
        {
            Ellipse e = new Ellipse();
            e.HorizontalAlignment = HorizontalAlignment.Left;
            e.VerticalAlignment = VerticalAlignment.Top;
            e.Width = width;
            e.Height = height;
            e.Fill = new SolidColorBrush(fillColor);
            e.Stroke = new SolidColorBrush(Colors.Black);
            e.Margin = new Thickness(bx, by, 0, 0);
            e.Tag = aRule;
            e.ToolTip = "Rule: " + XRule.ToRule(aRule).toStr();

            TextBlock t = new TextBlock();
            t.HorizontalAlignment = HorizontalAlignment.Left;
            t.VerticalAlignment = VerticalAlignment.Top;
            t.Width = width;
            t.Height = height;
            t.Background = new SolidColorBrush(Colors.Transparent);
            t.Foreground = new SolidColorBrush(Colors.Black);
            t.Margin = new Thickness(bx, by, 0, 0);
            t.TextAlignment = TextAlignment.Center;
            t.Tag = aRule;
            t.ToolTip = "Rule: " + aRule.ID;
            t.Text = "Rule: " + aRule.ID;

            canvas.Children.Add(e);
            canvas.Children.Add(t);
        }

        //-------------------------------------------------------
        public static void DrawConclusion2D(Canvas canvas, XConclusion aCon, double bx, double by, double width, double height, Color fillColor)
        {
            Rectangle e = new Rectangle();
            e.HorizontalAlignment = HorizontalAlignment.Left;
            e.VerticalAlignment = VerticalAlignment.Top;
            e.Width = width;
            e.Height = height;

                        e.Fill = new SolidColorBrush(fillColor);
                        e.Stroke = new SolidColorBrush(Colors.Black);
            e.Margin = new Thickness(bx, by, 0, 0);
            e.Tag = aCon;
            e.ToolTip = "Conclusion: " + aCon.conclusion;

            TextBlock t = new TextBlock();
            t.HorizontalAlignment = HorizontalAlignment.Left;
            t.VerticalAlignment = VerticalAlignment.Top;
            t.Width = width;
            t.Height = height;
            t.Background = new SolidColorBrush(Colors.Transparent);
            t.Foreground = new SolidColorBrush(Colors.Black);
            t.Margin = new Thickness(bx, by, 0, 0);
            t.TextAlignment = TextAlignment.Center;
            t.Tag = aCon;
            t.ToolTip = "Conclusion: " + aCon.conclusion;
            t.Text = "Conclusion:\r\n" + aCon.conclusion;

            canvas.Children.Add(e);
            canvas.Children.Add(t);
        }

        //------------------------------------------------------------------
        public static void ShowArrow(List<Rule> aRuleList, int i0, int i1, Brush brush)
        {
        }

        //------------------------------------------------------------------
        public static void RemoveRule(Canvas canvas2D,
                                       List<Rule> aRuleList, int nBase2D)
        {
            int ix3 = nBase2D;
            if (ix3 >= 0 && ix3 < canvas2D.Children.Count)
            {
                canvas2D.Children.RemoveAt(ix3);
            }

        }

    }
}

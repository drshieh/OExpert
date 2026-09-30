using OExpert.UILibraryWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Schema;

namespace OExpert
{
    public class XRuleNet
    {
        public string Name { get; set; }
        public double xMax { get; set; }
        public double yMax { get; set; }
        public ObservableCollection<XRule> aRuleList { get; set; }
        public ObservableCollection<XFact> aFactList { get; set; }
        public ObservableCollection<XConclusion> aConclusionList { get; set; }

        public List<object> aSelectedObjectList = new List<object>();
        public List<object> aPreSelectedObjectList = null;
        public object aSelectedObject4Info = null;  //--Right click => ItemInfoDlg


        //-----------------------------------------------------------
        public XRuleNet()
        {
            Name = "";
            xMax = 0;
            yMax = 0;
            aRuleList = new ObservableCollection<XRule>();
            aFactList = new ObservableCollection<XFact>();
            aConclusionList = new ObservableCollection<XConclusion>();

            aSelectedObjectList = new List<object>();
        }

        //--------------------------------------------------------
        public static XRuleNet Copy(XRuleNet a)
        {
            if (a == null) return null;
            XRuleNet x = new XRuleNet();
            x.Name = a.Name;
            x.xMax = a.xMax;
            x.yMax = a.yMax;
            x.aRuleList = XRule.CopyList(a.aRuleList);
            x.aFactList = XFact.CopyList(a.aFactList);
            x.aConclusionList = XConclusion.CopyList(a.aConclusionList);
            XRuleNet.ResolveRuleLink(x);
            SetSelectFromOLD(x, a);
            SetPreSelectFromOLD(x, a);
            x.aSelectedObject4Info = null;  //--Right click => ItemInfoDlg

            return x;
        }

        //-----------------------------------------------------------
        public static void SetSelectFromOLD(XRuleNet aNew, XRuleNet aOld)
        {
            if (aOld.aSelectedObjectList == null) return;
            aNew.aSelectedObjectList = new List<object>();
            foreach(object o in aOld.aSelectedObjectList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = (XRule)o;
                    int idx = XRule.Find(aOld.aRuleList, a.ID);
                    if (idx >= 0) aNew.aSelectedObjectList.Add(aNew.aRuleList[idx]);
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = (XFact)o;
                    int idx = XFact.Find(aOld.aFactList, a.fact);
                    if (idx >= 0) aNew.aSelectedObjectList.Add(aNew.aFactList[idx]);
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = (XConclusion)o;
                    int idx = XConclusion.Find(aOld.aConclusionList, a.conclusion);
                    if (idx >= 0) aNew.aSelectedObjectList.Add(aNew.aConclusionList[idx]);
                }
            }
        }

        //-----------------------------------------------------------
        public static void SetPreSelectFromOLD(XRuleNet aNew, XRuleNet aOld)
        {
            if (aOld.aPreSelectedObjectList == null) return;
            aNew.aPreSelectedObjectList = new List<object>();
            foreach (object o in aOld.aPreSelectedObjectList)
            {
                if (o.GetType() == typeof(XRule))
                {
                    XRule a = (XRule)o;
                    int idx = XRule.Find(aOld.aRuleList, a.ID);
                    if (idx >= 0) aNew.aPreSelectedObjectList.Add(aNew.aRuleList[idx]);
                }
                else if (o.GetType() == typeof(XFact))
                {
                    XFact a = (XFact)o;
                    int idx = XFact.Find(aOld.aFactList, a.fact);
                    if (idx >= 0) aNew.aPreSelectedObjectList.Add(aNew.aFactList[idx]);
                }
                else if (o.GetType() == typeof(XConclusion))
                {
                    XConclusion a = (XConclusion)o;
                    int idx = XConclusion.Find(aOld.aConclusionList, a.conclusion);
                    if (idx >= 0) aNew.aPreSelectedObjectList.Add(aNew.aConclusionList[idx]);
                }
            }
        }

        //-----------------------------------------------------------
        public static RuleNet ToRuleNet(XRuleNet aXRuleNet)
        {
            if (aXRuleNet == null) return null;
            RuleNet ruleNet = new RuleNet();
            ruleNet.Name = aXRuleNet.Name;
            ruleNet.aRuleList = XRule.ToRuleList(aXRuleNet.aRuleList);
            ruleNet.aFactList = XFact.ToRFactList(aXRuleNet.aFactList);
            ruleNet.aConclusionList = XConclusion.ToRConclusionList(aXRuleNet.aConclusionList);
            return ruleNet;
        }

        //-----------------------------------------------------------
        public static XRuleNet FromRuleNet(RuleNet aRuleNet)
        {
            if (aRuleNet == null) return null;
            XRuleNet ruleNet = new XRuleNet();
            ruleNet.Name = aRuleNet.Name;
            ruleNet.aRuleList = XRule.FromRuleList(aRuleNet.aRuleList);
            ruleNet.aFactList = XFact.FromRFactList(aRuleNet.aFactList);
            ruleNet.aConclusionList = XConclusion.FromRConclusionList(aRuleNet.aConclusionList);
            XRuleNet.ResolveRuleLink(ruleNet);

            AssignRuleNetCoord(ruleNet, true);
            return ruleNet;
        }

        //-----------------------------------------------------------
        public static void Resolve(XRuleNet aRuleNet)
        {
            if (aRuleNet == null) return;
            aRuleNet.aConclusionList = XRule.GetXConclusions(aRuleNet.aRuleList);
            aRuleNet.aFactList = XRule.GetXFacts(aRuleNet.aRuleList, aRuleNet.aConclusionList);
            XRuleNet.ResolveRuleLink(aRuleNet);
            AssignRuleNetCoord(aRuleNet, false);
        }

        //----------------------------------------------------------------
        public static void AssignRuleNetCoord(XRuleNet aNet, bool fSetRuleCoord)
        {
            if (aNet == null || aNet.aRuleList == null) return;

            if (aNet.aConclusionList == null) aNet.aConclusionList = XRule.GetXConclusions(aNet.aRuleList);
            if (aNet.aFactList == null) aNet.aFactList = XRule.GetXFacts(aNet.aRuleList, aNet.aConclusionList);  //--Facts do not show in the Conclusion

            SetRuleXYs(aNet, fSetRuleCoord);
            GetRuleNetSize(aNet);
            return;

            Pen pen = new Pen(new SolidColorBrush(WColor.GetColor(GlobalVarX.sRulePenColor)), 1);

            int nMaxPerRow = 20;
            double bx = 10;
            double by = 50;
            double width = 80;
            double height = 40;

            int i = 0;
            foreach (XFact a in aNet.aFactList)
            {
                if (a.X <= 0) a.X = bx;
                if (a.Y <= 0) a.Y = by;
                if (a.Width <= 0) a.Width = width;
                if (a.Height <= 0) a.Height = height;
                SetXFactSize(a, pen);
                bx += width + 10;
                i++;
                if (i != 0 && i % nMaxPerRow == 0)
                {
                    bx = 10;
                    by += height + 10;
                }
            }

            bx = 10;
            by += height + 100;
            i = 0;
            foreach (XRule a in aNet.aRuleList)
            {
                if (a.X <= 0) a.X = bx;
                if (a.Y <= 0) a.Y = by;
                if (a.Width <= 0) a.Width = width;
                if (a.Height <= 0) a.Height = height;
                SetXRuleSize(a, pen);
                bx += width + 10;
                i++;
                if (i != 0 && i % nMaxPerRow == 0)
                {
                    bx = 10;
                    by += height + 10;
                }
            }

            bx = 10;
            by += height + 100;
            i = 0;
            foreach (XConclusion a in aNet.aConclusionList)
            {
                if (a.X <= 0) a.X = bx;
                if (a.Y <= 0) a.Y = by;
                if (a.Width <= 0) a.Width = width;
                if (a.Height <= 0) a.Height = height;
                SetXConclusionSize(a, pen);
                bx += width + 10;
                i++;
                if (i != 0 && i % nMaxPerRow == 0)
                {
                    bx = 10;
                    by += height + 10;
                }
            }
        }

        //----------------------------------------------------------------
        public static void SetRuleXYs(XRuleNet aNet, bool fSetRuleCoord)
        {
            if (aNet == null || aNet.aRuleList == null) return;
            //--Step one : get the top list (without referencing to other Rules)
            List<XRule> aTopList = new List<XRule>();
            foreach (XRule a in aNet.aRuleList)
            {
                if (a.aFromRules == null || a.aFromRules.Count <= 0) aTopList.Add(a);
            }
            foreach (XRule a in aTopList)
            {
                a.iLevel = 0;
                SetXRuleChildLevel(a.aChildRules, a.iLevel + 1);
            }

            Pen pen = new Pen(new SolidColorBrush(WColor.GetColor(GlobalVarX.sRulePenColor)), 1);

            double xMax=0, yMax=0;

            int nMaxPerRow = 20;
            double maxWidth = 2000;  //--Max width of the x direction
            double bx = 10;
            double by = 50;
            double width = 80;
            double height = 40;

            int i = 0;
            foreach (XFact a in aNet.aFactList)
            {
                if (a.X <= 0) a.X = bx;
                if (a.Y <= 0) a.Y = by;
                if (a.Width <= 0) a.Width = width;
                if (a.Height <= 0) a.Height = height;
                SetXFactSize(a, pen);
                if (a.Height > height) height = a.Height;
                if (a.Width > width) bx += a.Width + 10;
                else bx += width + 10;
                i++;
                if ((i != 0 && i % nMaxPerRow == 0) || bx > maxWidth)
                {
                    bx = 10;
                    by += height + 10;
                }
            }
            XRuleNet.GetFactsSize(aNet, out xMax, out yMax);

            bx = 10;
            by = yMax + 100;

            if (fSetRuleCoord)
            {
                int nMaxTopItemPerRow = 10;
                width = 80;
                height = 40;
                double gapX = 20;
                double gapY = 20;
                //--Set for the Top with no Child
                int iCol = 0;
                int iRow = 0;
                foreach (XRule a in aTopList)
                {
                    if (a.aChildRules != null && a.aChildRules.Count > 0) continue;
                    if (iCol >= nMaxTopItemPerRow)
                    {
                        bx = 10;
                        by += height + gapX;
                        iRow++;
                    }
                    if (a.X <= 0 || a.Y <= 0)
                    {
                        a.X = bx;
                        a.Y = by;
                        a.Width = width;
                        a.Height = height;
                    }
                    bx += width + gapX;
                    iCol++;
                }
                if (iRow > 0) by += height + gapX;

                //--Set for the Top with Child

                bx = 10;
                foreach (XRule a in aTopList)
                {
                    if (a.aChildRules == null || a.aChildRules.Count <= 0) continue;
                    if (a.X <= 0 || a.Y <= 0)
                    {
                        a.X = bx;
                        a.Y = by;
                        a.Width = width;
                        a.Height = height;
                    }
                    SetChildXY(aNet, a.aChildRules, width, height, gapX, gapY, bx + width + gapX, ref by);
                    bx += width + gapX;
                }
            }
            XRuleNet.GetRulesSize(aNet, out xMax, out yMax);

            bx = 10;
            by = yMax + 100;
            i = 0;
            foreach (XConclusion a in aNet.aConclusionList)
            {
                if (a.X <= 0) a.X = bx;
                if (a.Y <= 0) a.Y = by;
                if (a.Width <= 0) a.Width = width;
                if (a.Height <= 0) a.Height = height;
                SetXConclusionSize(a, pen);
                if (a.Height > height) height = a.Height;
                if (a.Width > width) bx += a.Width + 10;
                else bx += width + 10;
                i++;
                if ((i != 0 && i % nMaxPerRow == 0) || bx > maxWidth)
                {
                    bx = 10;
                    by += height + 10;
                }
            }
        }

        //----------------------------------------------------------------
        public static void SetChildXY(XRuleNet aNet, List<XRule> aList, double width, double height, double gapX, double gapY, double bx, ref double by)
        {
            if (aList == null || aList.Count <= 0) return;
            foreach (XRule a in aList)
            {
                bool fEdit = false;
                if (a.X <= 0 || a.Y <= 0)
                {
                    if (IsCoordUsed(aNet, bx, by)) by += height + gapY;
                    a.X = bx;
                    a.Y = by;
                    a.Width = width;
                    a.Height = height;
                    fEdit = true;
                }
                SetChildXY(aNet, a.aChildRules, width, height, gapX, gapY, bx + width + gapX, ref by);
                if (!fEdit) by += height + gapY;
            }
        }

        //----------------------------------------------------------------
        public static bool IsCoordUsed(XRuleNet aNet, double bx, double by)
        {
            foreach (XRule a in aNet.aRuleList)
            {
                if (a.X == bx && a.Y == by) return true;
            }
            return false;
        }

        //----------------------------------------------------------------
        public static void SetXRuleChildLevel(List<XRule> aList, int iLevel)
        {
            if (aList == null || aList.Count <= 0) return;
            foreach (XRule a in aList)
            {
                a.iLevel = iLevel;
                SetXRuleChildLevel(a.aChildRules, a.iLevel + 1);
            }
        }

        //----------------------------------------------------------------
        public static void AssignRuleNetCoord0(XRuleNet aNet)
        {
            if (aNet == null || aNet.aRuleList == null) return;

            if (aNet.aConclusionList == null) aNet.aConclusionList = XRule.GetXConclusions(aNet.aRuleList);
            if (aNet.aFactList == null) aNet.aFactList = XRule.GetXFacts(aNet.aRuleList, aNet.aConclusionList);  //--Facts do not show in the Conclusion

            Pen pen = new Pen(new SolidColorBrush(WColor.GetColor(GlobalVarX.sRulePenColor)), 1);

            int nMaxPerRow = 20;
            double bx = 10;
            double by = 50;
            double width = 80;
            double height = 40;

            int i = 0;
            foreach (XFact a in aNet.aFactList)
            {
                if (a.X <= 0) a.X = bx;
                if (a.Y <= 0) a.Y = by;
                if (a.Width <= 0) a.Width = width;
                if (a.Height <= 0) a.Height = height;
                SetXFactSize(a, pen);
                bx += width + 10;
                i++;
                if (i != 0 && i % nMaxPerRow == 0)
                {
                    bx = 10;
                    by += height + 10;
                }
            }

            bx = 10;
            by += height + 100;
            i = 0;
            foreach (XRule a in aNet.aRuleList)
            {
                if (a.X <= 0) a.X = bx;
                if (a.Y <= 0) a.Y = by;
                if (a.Width <= 0) a.Width = width;
                if (a.Height <= 0) a.Height = height;
                SetXRuleSize(a, pen);
                bx += width + 10;
                i++;
                if (i != 0 && i % nMaxPerRow == 0)
                {
                    bx = 10;
                    by += height + 10;
                }
            }

            bx = 10;
            by += height + 100;
            i = 0;
            foreach (XConclusion a in aNet.aConclusionList)
            {
                if (a.X <= 0) a.X = bx;
                if (a.Y <= 0) a.Y = by;
                if (a.Width <= 0) a.Width = width;
                if (a.Height <= 0) a.Height = height;
                SetXConclusionSize(a, pen);
                bx += width + 10;
                i++;
                if (i != 0 && i % nMaxPerRow == 0)
                {
                    bx = 10;
                    by += height + 10;
                }
            }
        }

        //----------------------------------------------------------------
        public static void ResolveRuleLink(XRuleNet ruleNet)
        {
            if (ruleNet.aRuleList == null) return;
            foreach (XRule a in ruleNet.aRuleList)
            {
                if (a.ID <= 0) a.ID = ruleNet.GetNextRuleID();
                a.aFromRules = new List<XRule>();
                foreach (XCondition c in a.Conditions)
                {
                    List<XRule> fromRules = XRule.FindByConclusion(ruleNet.aRuleList, c.Condition);
                    if (fromRules != null && fromRules.Count > 0)
                    {
                        c.isConclusionType = true;
                        a.aFromRules.AddRange(fromRules);
                    }
                }
                if (a.aFromRules.Count > 0)
                {
                    foreach (XRule x in a.aFromRules)
                    {
                        if (x.aChildRules == null) x.aChildRules = new List<XRule>();
                        if (!x.aChildRules.Contains(a)) x.aChildRules.Add(a);
                    }
                }
            }
            if (ruleNet.aFactList != null)
            {
                foreach (XFact a in ruleNet.aFactList)
                {
                    a.ruleList = new ObservableCollection<XRule>();
                    foreach (XRule r in ruleNet.aRuleList)
                    {
                        foreach (XCondition c in r.Conditions)
                        {
                            if (c.Condition != a.fact) continue;
                            if (XConclusion.Find(ruleNet.aConclusionList, c.Condition) < 0)  //--exclude the Conclusions
                            {
                                if (!a.ruleList.Contains(r)) a.ruleList.Add(r);
                            }
                        }
                    }
                }
            }

            if (ruleNet.aConclusionList != null)
            {
                foreach (XConclusion a in ruleNet.aConclusionList)
                {
                    a.ruleList = new ObservableCollection<XRule>();
                    foreach (XRule r in ruleNet.aRuleList)
                    {
                        if (r.Conclusion == a.conclusion)
                        {
                            if (!a.ruleList.Contains(r)) a.ruleList.Add(r);
                        }
                    }
                }
            }
        }

        //-----------------------------------------------------------
        public void SetupNetwork(bool fSetCoord)
        {
            if (aRuleList == null) return;
            ObservableCollection<XConclusion> aPreviousConclusions = XConclusion.CopyList(aConclusionList);
            ObservableCollection<XFact> aPreviousFacts = XFact.CopyList(aFactList);
            ObservableCollection<XRule> aPreviousRules = XRule.CopyList(aRuleList);

            XRule.ResolveRuleLink(aRuleList);
            aConclusionList = XRule.GetXConclusions(aRuleList);
            aFactList = XRule.GetXFacts(aRuleList, aConclusionList);  //--Facts do not show in the Conclusion

            Pen pen = new Pen(new SolidColorBrush(WColor.GetColor(GlobalVarX.sRulePenColor)), 1);

            if (!fSetCoord)  //--keep the existing coord from previous
            {
                double bx = 10;
                double by = 50;
                double width = 80;
                double height = 40;

                foreach (XFact a in aFactList)
                {
                    int ip = XFact.Find(aPreviousFacts, a.fact);
                    if (ip >= 0)
                    {
                        a.X = aPreviousFacts[ip].X;
                        a.Y = aPreviousFacts[ip].Y;
                        a.Width = aPreviousFacts[ip].Width;
                        a.Height = aPreviousFacts[ip].Height;
                        if (a.X <= 0) a.X = bx;
                        if (a.Y <= 0) a.Y = by;
                    }
                    else
                    {
                        a.X = bx;
                        a.Y = by;
                        a.Width = width;
                        a.Height = height;
                        SetXFactSize(a, pen);
                    }
                    bx += width + 10;
                }

                bx = 10;
                by = 250;
                foreach (XRule a in aRuleList)
                {
                    int ip = XRule.Find(aPreviousRules, a.ID);
                    if (ip >= 0)
                    {
                        a.X = aPreviousRules[ip].X;
                        a.Y = aPreviousRules[ip].Y;
                        a.Width = aPreviousRules[ip].Width;
                        a.Height = aPreviousRules[ip].Height;
                        if (a.X <= 0) a.X = bx;
                        if (a.Y <= 0) a.Y = by;
                    }
                    else
                    {
                        a.X = bx;
                        a.Y = by;
                        a.Width = width;
                        a.Height = height;
                        SetXRuleSize(a, pen);
                    }
                    bx += width + 10;
                }

                bx = 10;
                by = 450;
                foreach (XConclusion a in aConclusionList)
                {
                    int ip = XConclusion.Find(aPreviousConclusions, a.conclusion);
                    if (ip >= 0)
                    {
                        a.X = aPreviousConclusions[ip].X;
                        a.Y = aPreviousConclusions[ip].Y;
                        a.Width = aPreviousConclusions[ip].Width;
                        a.Height = aPreviousConclusions[ip].Height;
                        if (a.X <= 0) a.X = bx;
                        if (a.Y <= 0) a.Y = by;
                    }
                    else
                    {
                        a.X = bx;
                        a.Y = by;
                        a.Width = width;
                        a.Height = height;
                        SetXConclusionSize(a, pen);
                    }
                    bx += width + 10;
                }
            }
            else
            {
                AssignRuleNetCoord(this, true);
            }
            GetRuleNetSize(this);
        }

        //------------------------------------------------------------------
        public int GetNextRuleID()
        {
            if (aRuleList == null || aRuleList.Count <= 0) return 1;
            int imax = 0;
            foreach (XRule aRule in aRuleList)
            {
                if (aRule.ID > imax) imax = aRule.ID;
            }
            return imax + 1;
        }

        //---------------------------------------------
        public static bool SetXFactSize(XFact aFact, Pen pen)
        {
            double bx = aFact.X;
            double ex = aFact.X + aFact.Width;
            double by = aFact.Y;
            double ey = aFact.Y + aFact.Height;
            string sfact = aFact.fact;
            bool fSizeChanged = false;
            WDrawingVisual.GetProperFormattedText(ref sfact, ref bx, ref by, ref ex, ref ey,
                 GlobalVarX.aRuleDefXFont, pen, ref fSizeChanged);
            if (fSizeChanged)
            {
                aFact.Width = ex - bx;
                aFact.Height = ey - by;
            }
            return fSizeChanged;
        }

        //---------------------------------------------
        public static bool SetXRuleSize(XRule aRule, Pen pen)
        {
            double bx = aRule.X;
            double ex = aRule.X + aRule.Width;
            double by = aRule.Y;
            double ey = aRule.Y + aRule.Height;
            string srule = "Rule: " + aRule.ID;
            bool fSizeChanged = false;
            WDrawingVisual.GetProperFormattedText(ref srule, ref bx, ref by, ref ex, ref ey,
                 GlobalVarX.aRuleDefXFont, pen, ref fSizeChanged);
            if (fSizeChanged)
            {
                aRule.Width = ex - bx;
                aRule.Height = ey - by;
            }
            return fSizeChanged;
        }

        //---------------------------------------------
        public static bool SetXConclusionSize(XConclusion aCon, Pen pen)
        {
            double bx = aCon.X;
            double ex = aCon.X + aCon.Width;
            double by = aCon.Y;
            double ey = aCon.Y + aCon.Height;
            string scon = aCon.conclusion;
            bool fSizeChanged = false;
            WDrawingVisual.GetProperFormattedText(ref scon, ref bx, ref by, ref ex, ref ey,
                 GlobalVarX.aRuleDefXFont, pen, ref fSizeChanged);
            if (fSizeChanged)
            {
                aCon.Width = ex - bx;
                aCon.Height = ey - by;
            }
            return fSizeChanged;
        }

        //---------------------------------------------
        public static void GetRuleNetSize(XRuleNet aRuleNet)
        {
            double xMax = Double.MinValue;
            double yMax = Double.MinValue;

            if (aRuleNet == null || aRuleNet.aRuleList == null) return;
            //---Fact
            foreach (XFact f in aRuleNet.aFactList)
            {
                if (f.X + f.Width > xMax) xMax = f.X + f.Width;
                if (f.Y + f.Height > yMax) yMax = f.Y + f.Height;
            }
            //--Rules
            foreach (XRule r in aRuleNet.aRuleList)
            {
                if (r.X + r.Width > xMax) xMax = r.X + r.Width;
                if (r.Y + r.Height > yMax) yMax = r.Y + r.Height;
            }
            //--Conclusion
            foreach (XConclusion a in aRuleNet.aConclusionList)
            {
                if (a.X + a.Width > xMax) xMax = a.X + a.Width;
                if (a.Y + a.Height > yMax) yMax = a.Y + a.Height;
            }
            aRuleNet.xMax = xMax;
            aRuleNet.yMax = yMax;
        }

        //---------------------------------------------
        public static void GetFactsSize(XRuleNet aRuleNet, out double xMax, out double yMax)
        {
            xMax = Double.MinValue;
            yMax = Double.MinValue;

            if (aRuleNet == null || aRuleNet.aFactList == null) return;
            //--Facts
            foreach (XFact r in aRuleNet.aFactList)
            {
                if (r.X + r.Width > xMax) xMax = r.X + r.Width;
                if (r.Y + r.Height > yMax) yMax = r.Y + r.Height;
            }
        }

        //---------------------------------------------
        public static void GetConclusionsSize(XRuleNet aRuleNet, out double xMax, out double yMax)
        {
            xMax = Double.MinValue;
            yMax = Double.MinValue;

            if (aRuleNet == null || aRuleNet.aConclusionList == null) return;
            //--Facts
            foreach (XConclusion r in aRuleNet.aConclusionList)
            {
                if (r.X + r.Width > xMax) xMax = r.X + r.Width;
                if (r.Y + r.Height > yMax) yMax = r.Y + r.Height;
            }
        }

        //---------------------------------------------
        public static void GetRulesSize(XRuleNet aRuleNet, out double xMax, out double yMax)
        {
            xMax = Double.MinValue;
            yMax = Double.MinValue;

            if (aRuleNet == null || aRuleNet.aRuleList == null) return;
            //--Rules
            foreach (XRule r in aRuleNet.aRuleList)
            {
                if (r.X + r.Width > xMax) xMax = r.X + r.Width;
                if (r.Y + r.Height > yMax) yMax = r.Y + r.Height;
            }
            aRuleNet.xMax = xMax;
            aRuleNet.yMax = yMax;
        }

        //---------------------------------------------
        public static ScrollViewer GetParentScrollViewer(FrameworkElement e)
        {
            if (e == null) return null;
            int i = 0;
            while (e != null && i < 10)
            {
                ScrollViewer sv = e.Parent as ScrollViewer;
                if (sv != null) return sv;
                e = e.Parent as FrameworkElement;
                i++;
            }
            return null;
        }

        //---------------------------------------------
        public static void SetCanvasSize(FrameworkElement canvas, XRuleNet aRuleNet, double xrat, double yrat)
        {
            double xMax = MainWindow.aXRuleNet.xMax * xrat;
            double yMax = MainWindow.aXRuleNet.yMax * yrat;

            ScrollViewer sv = GetParentScrollViewer(canvas);

            if (canvas.ActualWidth < xMax + 60)
            {
                canvas.Width = xMax + 60;
            }

            if (canvas.ActualHeight < yMax + 60)
            {
                canvas.Height = yMax + 60;
            }

            /*
            if (sv != null && sv.ActualWidth > 0)  //--so ScrollViewer is activated
            {
                if (xrat < 1)
                {
                    if (canvas.ActualWidth < sv.ActualWidth) canvas.Width = sv.ActualWidth / xrat;
                    else
                    {
                        if (canvas.ActualWidth > xMax + 60) canvas.Width = sv.ActualWidth / xrat;
                    }
                    if (canvas.ActualHeight < sv.ActualHeight) canvas.Height = sv.ActualHeight / yrat;
                    else
                    {
                        if (canvas.ActualHeight > yMax + 60) canvas.Height = sv.ActualHeight / yrat;
                    }
                    if (canvas.Width * xrat <= sv.ActualWidth) sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    else sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    if (canvas.Height * yrat <= sv.ActualHeight) sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    else sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
                else
                {
                    if (canvas.ActualWidth < sv.ActualWidth) canvas.Width = sv.ActualWidth;
                    else
                    {
                        if (canvas.ActualWidth > xMax + 60) canvas.Width = sv.ActualWidth;
                    }
                    if (canvas.ActualHeight < sv.ActualHeight) canvas.Height = sv.ActualHeight;
                    else
                    {
                        if (canvas.ActualHeight > yMax + 60) canvas.Height = sv.ActualHeight;
                    }
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
            }
            */
        }

        //---------------------------------------------------
        public void ResetValues()
        {
            if (aRuleList == null) return;
            foreach (XRule a in aRuleList)
            {
                a.Value = -1;
            }
            foreach (XFact a in aFactList)
            {
                a.Value = "";
            }
            foreach (XConclusion a in aConclusionList)
            {
                a.Value = -1;
            }
        }

        //-------------------------------------------------
        public string GetRuleItemStr(object o)
        {
            if (o == null) return "";
            if (o.GetType() == typeof(XRule))
            {
                XRule x = o as XRule;
                return x.toStr();
            }
            else if (o.GetType() == typeof(XFact))
            {
                XFact x = o as XFact;
                return x.toStr();
            }
            else if (o.GetType() == typeof(XConclusion))
            {
                XConclusion x = o as XConclusion;
                return x.toStr();
            }
            else return "";
        }

        //-------------------------------------------------
        public void RemoveRuleItemList(List<object> aList)
        {
            if (aList == null) return;
            foreach (object a in aList)
            {
                RemoveRuleItem(a);
            }
        }

        //-----------------------------------------------------
        public void RemoveRuleItem(object o)
        {
            if (o.GetType() == typeof(XRule))
            {
                XRule x = o as XRule;
                if (aRuleList != null && aRuleList.Count > 0) aRuleList.Remove(x);
            }
            else if (o.GetType() == typeof(XFact))
            {
                XFact x = o as XFact;
                if (aFactList != null && aFactList.Count > 0) aFactList.Remove(x);
            }
            else if (o.GetType() == typeof(XConclusion))
            {
                XConclusion x = o as XConclusion;
                if (aConclusionList != null && aConclusionList.Count > 0) aConclusionList.Remove(x);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace OExpert.UILibraryWPF
{
    public class WPOINT
    {
        public double X;
        public double Y;

        public WPOINT()
        {
        }

        public WPOINT(double x, double y)
        {
            X = x;
            Y = y;
        }

        public WPOINT(WPOINT a)
        {
            X = a.X;
            Y = a.Y;
        }

        public WPOINT(Point a)
        {
            X = a.X;
            Y = a.Y;
        }

        //-------------------------------------------------------------
        //-----str: x,y|x,y|..
        public static List<WPOINT> Parse(string str)
        {
            if (String.IsNullOrEmpty(str)) return null;
            List<WPOINT> rList = new List<WPOINT>();
            string[] substr = str.Split('|');
            double x = 0;
            double y = 0;
            foreach (string s in substr)
            {
                string[] subx = s.Split(',');
                if (Double.TryParse(subx[0].Trim(), out x) &&
                    Double.TryParse(subx[1].Trim(), out y))
                {
                    rList.Add(new WPOINT(x, y));
                }
            }
            return rList;
        }

        //-------------------------------------------------------------
        public static List<Point> toPoint(List<WPOINT> aList)
        {
            if (aList == null) return null;
            List<Point> ptList = new List<Point>();
            foreach (WPOINT a in aList)
            {
                ptList.Add(new Point(a.X, a.Y));
            }
            return ptList;
        }

        //-------------------------------------------------------------
        public static List<Point> toPoint(List<WPOINT> aList, double adjx, double adjy, double xrat, double yrat)
        {
            if (aList == null) return null;
            List<Point> ptList = new List<Point>();
            foreach (WPOINT a in aList)
            {
                ptList.Add(new Point(adjx+a.X*xrat, adjy+a.Y*yrat));
            }
            return ptList;
        }

        //-------------------------------------------------------------
        public static List<WPOINT> GetRelativeCoords(List<WPOINT> ptList, double bx, double by)
        {
            List<WPOINT> rList = new List<WPOINT>();
            foreach(WPOINT pt in ptList)
            {
                rList.Add(new WPOINT(pt.X-bx, pt.Y-by));
            }
            return rList;
        }

        //-------------------------------------------------------------
        //-----str: x,y|x,y|..
        public static string MakeString(List<WPOINT> pts)
        {
            if (pts == null || pts.Count <= 0) return null;
            string rstr = "";
            foreach (WPOINT p in pts)
            {
                rstr += "|" + p.X + "," + p.Y;
            }
            if (rstr.StartsWith("|")) return rstr.Substring(1);
            else return rstr;
        }

        public static void Adjust(List<WPOINT> aPtList, double adjx, double adjy, double xrat, double yrat)
        {
            foreach(WPOINT a in aPtList)
            {
                a.X = adjx + a.X * xrat;
                a.X = adjy + a.Y * yrat;
            }
        }

    }
}

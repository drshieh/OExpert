using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace OExpert.UILibraryWPF
{
    public class WDrawingCanvas : Panel
    {
        private List<Visual> visuals = new List<Visual>();
        private List<DrawingVisual> hits = new List<DrawingVisual>();

        public static WDrawingCanvas Create(string Id, object o, double bx, double by, double ex, double ey, Color backColor)
        {
            WDrawingCanvas ctrl = new WDrawingCanvas();
            ctrl.Name = "CTRL_" + Id;
            ctrl.HorizontalAlignment = HorizontalAlignment.Stretch;
            ctrl.VerticalAlignment = VerticalAlignment.Stretch;
            ctrl.Margin = new Thickness(bx, by, ex, ey);
            ctrl.Background = new SolidColorBrush(backColor);
            ctrl.Tag = o;
            return ctrl;
        }

        //--------------------------------------------------------
        public void Clear()
        {
            foreach(DrawingVisual v in visuals)
            {
                base.RemoveVisualChild(v);
                base.RemoveLogicalChild(v);
            }
            visuals = new List<Visual>();
            hits = new List<DrawingVisual>();
        }

        //--------------------------------------------------------
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        //--------------------------------------------------------
        protected override int VisualChildrenCount
        {
            get
            {
                return visuals.Count;
            }
        }

        //--------------------------------------------------------
        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);

            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        //--------------------------------------------------------
        public void DeleteVisual(Visual visual)
        {
            if (visual == null) return;
            visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }

        //--------------------------------------------------------
        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, point);
            return hitResult.VisualHit as DrawingVisual;
        }

        //--------------------------------------------------------
        public List<DrawingVisual> GetVisuals(Geometry region)
        {
            hits.Clear();
            GeometryHitTestParameters parameters = new GeometryHitTestParameters(region);
            HitTestResultCallback callback = new HitTestResultCallback(this.HitTestCallback);
            VisualTreeHelper.HitTest(this, null, callback, parameters);
            return hits;
        }

        //--------------------------------------------------------
        private HitTestResultBehavior HitTestCallback(HitTestResult result)
        {
            GeometryHitTestResult geometryResult = (GeometryHitTestResult)result;
            DrawingVisual visual = result.VisualHit as DrawingVisual;
            if (visual != null &&
                geometryResult.IntersectionDetail == IntersectionDetail.FullyInside)
            {
                hits.Add(visual);
            }
            return HitTestResultBehavior.Continue;
        }
    }
}

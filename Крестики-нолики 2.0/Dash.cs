using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Крестики_нолики_2._0 {
    public enum DashType { Vertical, Horizontal };

    public class Dash {
        public DashPoint StartPoint { get; set; }
        public DashPoint EndPoint { get; set; }
        public Player Player { get; set; }
        public DashType Type {
            get
            {
                if (Math.Abs(StartPoint.IIndex - EndPoint.IIndex) == 1)
                    return DashType.Horizontal;
                else
                    return DashType.Vertical;
            }
        }

        public Dash() {
        }
        
        public Dash(ref DashPoint start, ref DashPoint end) {
            StartPoint = start;
            EndPoint = end;
            
        }

        public bool HasPoint(DashPoint point) {
            return point.IsIndexesEquals(StartPoint) || point.IsIndexesEquals(EndPoint);
        }

        public bool HasPoints(DashPoint point1, DashPoint point2) {
            return (point1.IsIndexesEquals(StartPoint) && point2.IsIndexesEquals(EndPoint)) ||
                   (point1.IsIndexesEquals(EndPoint) && point2.IsIndexesEquals(StartPoint));
        }

        public bool IsEqualsLocation(Dash dash) {
            return (dash.StartPoint.IsIndexesEquals(StartPoint) && dash.EndPoint.IsIndexesEquals(EndPoint)) ||
                   (dash.StartPoint.IsIndexesEquals(EndPoint) && dash.EndPoint.IsIndexesEquals(StartPoint));
        }

        public bool HasCommonPoint(Dash dash) {
            return StartPoint.IsIndexesEquals(dash.StartPoint) || EndPoint.IsIndexesEquals(dash.EndPoint) ||
                   StartPoint.IsIndexesEquals(dash.EndPoint) || EndPoint.IsIndexesEquals(dash.StartPoint);
        }
    }
}
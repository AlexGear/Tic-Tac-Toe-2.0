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
    public enum Sides { None = 0, Right = 1, Top = 2, Left = 4, Bottom = 8 };

    public class DashPoint {
        public double Size { get; set; }
        public Point Center { get; set; }
        public int IIndex { get; set; }
        public int JIndex { get; set; }
        
        public DashPoint() {
        }

        public DashPoint(int iIndex, int jIndex) {
            IIndex = iIndex;
            JIndex = jIndex;
        }

        public bool IsIndexesEquals(DashPoint point) {
            return (IIndex == point.IIndex) && (JIndex == point.JIndex);
        }

        public bool IsInside(double x, double y) {
            double halfSize = Size / 2;
            double _x = Center.X;
            double _y = Center.Y;
            return x > (_x - halfSize) && x < (_x + halfSize) &&
                   y > (_y - halfSize) && y < (_y + halfSize);
        }

        public bool IsNeighbour(DashPoint point) {
            if(Math.Abs(point.IIndex - this.IIndex) == 1 ||
               Math.Abs(point.JIndex - this.JIndex) == 1) {
                return true;
            }
            return false;
        }

        public static DashPoint MinPoint(ref DashPoint point1, ref DashPoint point2) {
            if(point1.IIndex < point2.IIndex || point1.JIndex < point2.JIndex)
                return point1;
            else 
                return point2;
        }

        private List<DashPoint> ExcludeUnavailableMoves(int fieldSize, List<DashPoint> neighbours) {
            Sides avSides = SidesCheck(fieldSize);
            var result = new List<DashPoint>(neighbours);
            foreach(var i in neighbours) {
                if(GameCore.IsThereDash(this, i)) {
                    if(i.IIndex == IIndex && i.JIndex == JIndex - 1)
                        result.Remove(i);
                    else if(i.IIndex == IIndex && i.JIndex == JIndex + 1)
                        result.Remove(i);
                    else if(i.IIndex == IIndex - 1 && i.JIndex == JIndex)
                        result.Remove(i);
                    else if(i.IIndex == IIndex + 1 && i.JIndex == JIndex)
                        result.Remove(i);
                }
            }
            return result;
        }

        private Sides SidesCheck(int fieldSize) {
            Sides result = Sides.Right | Sides.Top | Sides.Left | Sides.Bottom;
            if(IIndex == 0)
                result &= ~(Sides.Left | Sides.Top | Sides.Bottom);
            if(IIndex == fieldSize)
                result &= ~(Sides.Right | Sides.Top | Sides.Bottom);
            if(JIndex == 0)
                result &= ~(Sides.Top | Sides.Right | Sides.Left);
            if(JIndex == fieldSize)
                result &= ~(Sides.Bottom | Sides.Right | Sides.Left);
            return result;
        }

        public List<DashPoint> GetNeighbours(int fieldSize, DashPoint[,] points, bool excludeUnavailableMoves) {
            Sides availableMoves = SidesCheck(fieldSize);            
            List<DashPoint> result = new List<DashPoint>();
            if(availableMoves == Sides.None)
                return result;
            if((availableMoves & Sides.Top) > 0)
                result.Add(points[IIndex, JIndex - 1]);
            if((availableMoves & Sides.Left) > 0)
                result.Add(points[IIndex - 1, JIndex]);
            if((availableMoves & Sides.Right) > 0)
                result.Add(points[IIndex + 1, JIndex]);
            if((availableMoves & Sides.Bottom) > 0)
                result.Add(points[IIndex, JIndex + 1]);
            if(excludeUnavailableMoves)
                result = ExcludeUnavailableMoves(fieldSize, result);
            return result;
        }

        public Sides GetAvailableMoves(int fieldSize, DashPoint[,] points) {
            Sides result = SidesCheck(fieldSize);
            List<DashPoint> neighbours = GetNeighbours(fieldSize, points, false);
            foreach(var i in neighbours) {
                if(GameCore.IsThereDash(this, i)) {
                    if(i.IIndex == IIndex && i.JIndex == JIndex - 1)
                        result &= ~Sides.Top;
                    else if(i.IIndex == IIndex && i.JIndex == JIndex + 1)
                        result &= ~Sides.Bottom;
                    else if(i.IIndex == IIndex - 1 && i.JIndex == JIndex)
                        result &= ~Sides.Left;
                    else if(i.IIndex == IIndex + 1 && i.JIndex == JIndex)
                        result &= ~Sides.Right;
                }
            }
            return result;
        }
    }
}
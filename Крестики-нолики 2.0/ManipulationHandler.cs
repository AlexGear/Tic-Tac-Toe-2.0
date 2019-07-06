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
    public class ManipulationHandler {
        private int _fieldSize;
        private double linesSpace;
        private double circleSize;
        private double halfCircleSize;
        private double _linesWidth;
        private Grid _fieldGrid;
        private Line previewLine;
        private Ellipse lineStartCircle;
        private Ellipse lineEndCircle;        
        private DashPoint startPoint;
        private List<DashPoint> startPointNeighbours;
        private DashPoint endPoint;
        private Point endPosition;
        private SolidColorBrush brush;
        private AvailableMovesVisualizer movesVisualizer;

        public ManipulationHandler(int fieldSize, double linesWidth, ref Grid fieldGrid) {
            _fieldSize = fieldSize;
            _fieldGrid = fieldGrid;
            linesSpace = fieldGrid.Width / fieldSize;
            _linesWidth = linesWidth;
            startPoint = null;
            endPoint = null;
            lineEndCircle = new Ellipse();
            lineEndCircle.UseLayoutRounding = true;
            circleSize = linesSpace * Consts.LineCirclesSizeRatio;
            halfCircleSize = circleSize / 2;
            brush = new SolidColorBrush(GameCore.GetCurrentPlayerColor());
        }

        public void ManipulationStarted(Point origin) {
            foreach(var point in GameCore.dashPoints) {
                if(point.IsInside(origin.X, origin.Y) && point.GetAvailableMoves(_fieldSize, GameCore.dashPoints) != Sides.None) {
                    startPoint = point;
                    startPointNeighbours = point.GetNeighbours(_fieldSize, GameCore.dashPoints, true);
                    previewLine = new Line() {
                        X1 = point.Center.X, Y1 = point.Center.Y,
                        X2 = point.Center.X, Y2 = point.Center.Y,
                        Stroke = brush,
                        StrokeThickness = _linesWidth * Consts.DashWidthRatio / 2,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round
                    };
                    lineStartCircle = new Ellipse() {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(point.Center.X - halfCircleSize,
                                               point.Center.Y - halfCircleSize, 0, 0),
                        Width = circleSize, Height = circleSize, Fill = brush
                    };
                    _fieldGrid.Children.Add(previewLine);
                    _fieldGrid.Children.Add(lineStartCircle);
                    endPosition = point.Center;
                    movesVisualizer = new AvailableMovesVisualizer(point.Center, _fieldSize, _linesWidth, ref _fieldGrid);
                    movesVisualizer.StartAnimation(point.GetAvailableMoves(_fieldSize, GameCore.dashPoints));
                    break;
                }
            }
        }

        private Point LimitLine(Point start, Point end, double limit) {
            double a = end.Y - start.Y;
            double b = end.X - start.X;
            double length = Math.Sqrt((a * a) + (b * b));
            if(length <= limit)
                return end;
            double x = 0, y;
            if(b == 0) {
                y = Math.Min(end.Y, limit);
            }
            else {
                double delta = a / b;
                x = Math.Sqrt((limit * limit) / (delta * delta + 1)) * Math.Sign(b);
                y = delta * x;
            }
            return new Point(start.X + x, start.Y + y);
        }

        public void ManipulationDelta(Point deltaPos) {
            if(previewLine == null)
                return;
            endPosition.X += deltaPos.X;
            endPosition.Y += deltaPos.Y;
            Point startPosition = new Point(startPoint.Center.X, startPoint.Center.Y);
            Point limited = LimitLine(startPosition, endPosition, linesSpace);
            previewLine.X2 = limited.X;
            previewLine.Y2 = limited.Y;
            if(startPointNeighbours != null) {
                bool found = false;
                foreach(var point in startPointNeighbours) {
                    if(point.IsInside(limited.X, limited.Y)) {
                        found = true;
                        endPoint = point;
                        previewLine.X2 = point.Center.X;                // Snap to dash point
                        previewLine.Y2 = point.Center.Y;
                        lineEndCircle.HorizontalAlignment = HorizontalAlignment.Left;
                        lineEndCircle.VerticalAlignment = VerticalAlignment.Top;
                        lineEndCircle.Margin = new Thickness(point.Center.X - halfCircleSize, 
                                                             point.Center.Y - halfCircleSize, 0, 0);
                        lineEndCircle.Width = circleSize;
                        lineEndCircle.Height = circleSize;
                        lineEndCircle.Fill = brush;
                        if(!_fieldGrid.Children.Contains(lineEndCircle))
                            _fieldGrid.Children.Add(lineEndCircle);                        
                        break;
                    }
                }
                if(!found) {
                    endPoint = null;
                    _fieldGrid.Children.Remove(lineEndCircle);
                }
            }
        }

        public void ManipulationCompleted(Point pos) {
            if(previewLine == null)
                return;
            movesVisualizer.ReverseAnimation();
            if(startPoint != null && endPoint != null) {
                GameCore.CreateDash(ref startPoint, ref endPoint);
            }
            _fieldGrid.Children.Remove(previewLine);
            _fieldGrid.Children.Remove(lineStartCircle);
            if(_fieldGrid.Children.Contains(lineEndCircle))
                _fieldGrid.Children.Remove(lineEndCircle);
        }

        public void Clear() {
            if(previewLine != null)
                _fieldGrid.Children.Remove(previewLine);
            if(lineStartCircle != null)
                _fieldGrid.Children.Remove(lineStartCircle);
            if(lineEndCircle != null)
                _fieldGrid.Children.Remove(lineEndCircle);
            if(movesVisualizer != null)
                movesVisualizer.Clear();
        }
    }
}
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
    public class Cell {
        public Point Center { get; set; }
        public Player Owner { get; set; }

        public Cell() {
        }

        public Cell(Point center, Player owner) {
            Center = center;
            Owner = owner;
        }

        public Grid GetSignImage(double linesSpace, double linesWidth) {
            Grid result = new Grid();
            double size = linesSpace - (linesWidth * Consts.DashWidthRatio / 2);
            double halfSize = size / 2;
            double crossOffset = size * Consts.CrossOffsetRatio;
            double zeroOffset = size * Consts.ZeroOffsetRatio;
            result.HorizontalAlignment = HorizontalAlignment.Left;
            result.VerticalAlignment = VerticalAlignment.Top;
            result.Width = size;
            result.Height = size;
            result.Margin = new Thickness(Center.X - halfSize, Center.Y - halfSize, 0, 0);
            Color signColor = GameCore.GetCurrentPlayerColor();
            if(Owner == Player.Red) {                
                Line crossLine = new Line() {
                    X1 = crossOffset, Y1 = crossOffset,
                    X2 = size - crossOffset, Y2 = size - crossOffset,
                    Stroke = new SolidColorBrush(signColor),
                    StrokeThickness = linesWidth / 2
                };
                result.Children.Add(crossLine);
                crossLine = new Line() {
                    X1 = crossOffset, Y1 = size - crossOffset,
                    X2 = size - crossOffset, Y2 = crossOffset,
                    Stroke = new SolidColorBrush(signColor),
                    StrokeThickness = linesWidth / 2
                };
                result.Children.Add(crossLine);
            }
            else {
                Ellipse zero = new Ellipse() {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(zeroOffset, zeroOffset, 0, 0),
                    Width = size - zeroOffset * 2, Height = size - zeroOffset * 2,
                    Stroke = new SolidColorBrush(signColor),
                    StrokeThickness = linesWidth / 2
                };
                result.Children.Add(zero);
            }
            return result;
        }

        public bool IsEqualsLocation(Cell cell) {
            return Center == cell.Center;
        }
    }
}
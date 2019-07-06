using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Крестики_нолики_2._0 {
    public class AvailableMovesVisualizer {
        private Grid _fieldGrid;
        private int _fieldSize;
        private double _linesSpace;
        private double _linesWidth;
        private double offset;
        private double _x;
        private double _y;
        private const int movesCount = 4;
        private double ellipseSize;
        private double halfEllipseSize;
        private Ellipse[] ellipses;
        private Point[] offsets;
        private Sides[] movesSequence;
        private Storyboard storyboard;
        private const double fullOpacity = 100.0d;

        public delegate void AnimationCompletedHandler(object sender, EventArgs e);
        public event AnimationCompletedHandler AnimationCompleted;

        public void RaiseAnimationCompleted(object sender, EventArgs e) {
            if(AnimationCompleted != null)
                AnimationCompleted(this, new EventArgs());
        }
      
        public AvailableMovesVisualizer(Point center, int fieldSize, double linesWidth, ref Grid fieldGrid) {
            _x = center.X;
            _y = center.Y;
            _fieldSize = fieldSize;
            _linesWidth = linesWidth;
            _fieldGrid = fieldGrid;
            offset = _linesSpace = fieldGrid.Width / _fieldSize;
            ellipseSize = _linesSpace * Consts.PointHitboxSizeRatio;
            halfEllipseSize = ellipseSize / 2;
            ellipses = new Ellipse[movesCount];
            offsets = new Point[movesCount] {
                new Point(-offset, 0), new Point(0, -offset), new Point(offset, 0), new Point(0, offset)
            };
            movesSequence = new Sides[movesCount] {
                Sides.Left, Sides.Top, Sides.Right, Sides.Bottom
            };
            storyboard = new Storyboard();
        }

        public void StartAnimation(Sides availableMoves) {
            if(availableMoves == Sides.None)
                return;            
            for(int i = 0; i < movesCount; i++) {
                if(availableMoves.HasFlag(movesSequence[i])) {
                    ellipses[i] = new Ellipse() {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = ellipseSize, Height = ellipseSize,
                        Stroke = new SolidColorBrush(ConstantColors.AvailableMovesEllipses),
                        StrokeThickness = _linesWidth / 2
                    };
                    _fieldGrid.Children.Add(ellipses[i]);
                    ellipses[i].RenderTransform = new TranslateTransform();
                    DoubleAnimation anim = new DoubleAnimation() {
                        From = _x - halfEllipseSize,
                        To = _x - halfEllipseSize + offsets[i].X,
                        Duration = TimeSpan.FromMilliseconds(Consts.EllipsesAnimationDuration)
                    };
                    Storyboard.SetTarget(anim, ellipses[i]);
                    Storyboard.SetTargetProperty(anim, new PropertyPath("(UIElement.RenderTransform).X"));
                    storyboard.Children.Add(anim);
                    anim = new DoubleAnimation() {
                        From = _y - halfEllipseSize,
                        To = _y - halfEllipseSize + offsets[i].Y,
                        Duration = TimeSpan.FromMilliseconds(Consts.EllipsesAnimationDuration)
                    };
                    Storyboard.SetTarget(anim, ellipses[i]);
                    Storyboard.SetTargetProperty(anim, new PropertyPath("(UIElement.RenderTransform).Y"));
                    storyboard.Children.Add(anim);
                }
            }
            storyboard.Begin();
            storyboard.Completed += RaiseAnimationCompleted;
        }

        public void ReverseAnimation() {
            storyboard.Stop();
            storyboard.Children.Clear();
            for(int i = 0; i < movesCount; i++) {
                if(ellipses[i] == null)
                    continue;
                DoubleAnimation anim = new DoubleAnimation() {
                    From = _x - halfEllipseSize + offsets[i].X, To = _x - halfEllipseSize,
                    Duration = TimeSpan.FromMilliseconds(Consts.EllipsesAnimationDuration)
                };
                Storyboard.SetTarget(anim, ellipses[i]);
                Storyboard.SetTargetProperty(anim, new PropertyPath("(UIElement.RenderTransform).X"));
                storyboard.Children.Add(anim);
                anim = new DoubleAnimation() {
                    From = _y - halfEllipseSize + offsets[i].Y, To = _y - halfEllipseSize,
                    Duration = TimeSpan.FromMilliseconds(Consts.EllipsesAnimationDuration)
                };
                Storyboard.SetTarget(anim, ellipses[i]);
                Storyboard.SetTargetProperty(anim, new PropertyPath("(UIElement.RenderTransform).Y"));
                storyboard.Children.Add(anim);
                anim = new DoubleAnimation() {
                    From = fullOpacity, To = 0,
                    Duration = TimeSpan.FromMilliseconds(Consts.EllipsesAnimationDuration)
                };
                Storyboard.SetTarget(anim, ellipses[i]);
                Storyboard.SetTargetProperty(anim, new PropertyPath(Ellipse.OpacityProperty));
                storyboard.Children.Add(anim);
            }
            storyboard.Begin();
            storyboard.Completed -= RaiseAnimationCompleted;
            storyboard.Completed += (sender, e) => {
                foreach(var i in ellipses) {
                    _fieldGrid.Children.Remove(i);
                }
            };
        }

        public void Clear() {
            foreach(var i in ellipses) {
                _fieldGrid.Children.Remove(i);
            }
        }
    }
}
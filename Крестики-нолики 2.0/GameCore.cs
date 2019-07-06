using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Крестики_нолики_2._0 {
    public enum Player { Red, Blue };
    public enum FirstMovePlayer { Random, Red, Blue };

    public static class GameCore {
        public static DashPoint[,] dashPoints { get; set; }
        private static Dash[,] vertDashes;
        private static Dash[,] horDashes;
        private static List<Cell> cells;
        private static Grid fieldGrid;
        private static double linesSpace;
        private static int fieldSize;
        private static double linesWidth;
        private static int cellsCount;
        private static Player currentPlayer;

        public delegate void SwapPlayersHandler(Player currentPlayer);
        public delegate void UnargumentedEventhandler();
        public delegate void ScoreEventHandler(Player player);
        public static event SwapPlayersHandler SwapPlayersEvent;
        public static event UnargumentedEventhandler Initialized;
        public static event UnargumentedEventhandler FieldFilled;
        public static event ScoreEventHandler ScoreChanged;

        public static void GenerateField() {
            const double maxLinesWidth = Consts.MaxFieldLinesWidth;
            const double minLinesWidth = Consts.MinFieldLinesWidth;
            linesWidth = maxLinesWidth - (maxLinesWidth - minLinesWidth) * ((double)fieldSize / (double)Consts.MaxFieldSize);
            for(int i = 1; i < fieldSize; i++) {
                Line line = new Line() {
                    X1 = i * linesSpace, X2 = i * linesSpace,
                    Y1 = 0, Y2 = fieldGrid.Height,
                    Stroke = new SolidColorBrush(ConstantColors.FieldGridLinesColor),
                    StrokeThickness = linesWidth / 2
                };
                fieldGrid.Children.Add(line);
                line = new Line() {
                    X1 = 0, X2 = fieldGrid.Height,
                    Y1 = i * linesSpace, Y2 = i * linesSpace,
                    Stroke = new SolidColorBrush(ConstantColors.FieldGridLinesColor),
                    StrokeThickness = linesWidth / 2
                };
                fieldGrid.Children.Add(line);
            }
            Rectangle border = new Rectangle() {
                Width = fieldGrid.Width, Height = fieldGrid.Height,
                Stroke = new SolidColorBrush(ConstantColors.FieldGridBorderColor),
                StrokeThickness = linesWidth / 2
            };
            fieldGrid.Children.Add(border);
        }

        public static double GetLinesWidth() {
            return linesWidth;
        }

        public static Player GetCurrentPlayer() {
            return currentPlayer;
        }

        public static Color GetCurrentPlayerColor() {
            return currentPlayer == Player.Red ? ConstantColors.RedPlayer : ConstantColors.BluePlayer;
        }

        private static void InitDashPoints() {
            dashPoints = new DashPoint[fieldSize + 1, fieldSize + 1];
            for(int i = 0; i < fieldSize + 1; i++) {
                for(int j = 0; j < fieldSize + 1; j++) {
                    dashPoints[i, j] = new DashPoint() {
                        Center = new Point(i * linesSpace, j * linesSpace),
                        Size = linesSpace * Consts.PointHitboxSizeRatio,
                        IIndex = i, JIndex = j
                    };
                }
            }
        }

        private static void CreateBorderDashes() {
            for (int i = 0; i < fieldSize; i++) {
                horDashes[i, 0] = new Dash(ref dashPoints[i, 0], ref dashPoints[i + 1, 0]);
                vertDashes[0, i] = new Dash(ref dashPoints[0, i], ref dashPoints[0, i + 1]);
                horDashes[i, fieldSize] = new Dash(ref dashPoints[i, fieldSize], ref dashPoints[i + 1, fieldSize]);
                vertDashes[fieldSize, i] = new Dash(ref dashPoints[fieldSize, i], ref dashPoints[fieldSize, i + 1]);
            }
        }

        public static void Init(ref Grid fieldGrid, int fieldSize, FirstMovePlayer firstMove) {
            GameCore.fieldGrid = fieldGrid;
            GameCore.fieldSize = fieldSize;
            linesSpace = fieldGrid.Width / fieldSize;
            cellsCount = fieldSize * fieldSize;
            InitDashPoints();
            switch(firstMove) {
                case FirstMovePlayer.Random:
                    currentPlayer = new Random().Next(2) == 0 ? Player.Red : Player.Blue;
                    break;
                case FirstMovePlayer.Red:
                    currentPlayer = Player.Red;
                    break;
                case FirstMovePlayer.Blue:
                    currentPlayer = Player.Blue;
                    break;
            }
            vertDashes = new Dash[fieldSize + 1, fieldSize];
            horDashes = new Dash[fieldSize, fieldSize + 1];
            CreateBorderDashes();
            cells = new List<Cell>();
            Initialized?.Invoke();
        }

        public static void SwapPlayers() {
            currentPlayer = currentPlayer == Player.Red ? Player.Blue : Player.Red;
            SwapPlayersEvent?.Invoke(currentPlayer);
        }

        private static Dash[,] GetDashesByType(DashType type) {
            return type == DashType.Vertical ? vertDashes : horDashes;
        }

        public static void CreateDash(ref DashPoint point1, ref DashPoint point2) {
            Dash dash = new Dash(ref point1, ref point2);
            DashPoint minPoint = DashPoint.MinPoint(ref point1, ref point2);
            GetDashesByType(dash.Type)[minPoint.IIndex, minPoint.JIndex] = dash;
            Line line = new Line() {
                X1 = point1.Center.X, Y1 = point1.Center.Y,
                X2 = point2.Center.X, Y2 = point2.Center.Y,
                Stroke = new SolidColorBrush(GetCurrentPlayerColor()),
                StrokeThickness = GetLinesWidth() * Consts.DashWidthRatio / 2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            fieldGrid.Children.Add(line);
            if(!HandleClosesCells())
                SwapPlayers();
        }

        public static bool IsThereDash(DashPoint point1, DashPoint point2) {
            foreach(var i in vertDashes) {
                if(i?.HasPoints(point1, point2) ?? false)
                    return true;
            }
            foreach(var i in horDashes) {
                if (i?.HasPoints(point1, point2) ?? false)
                    return true;
            }
            return false;
        }

        public static bool HandleClosesCells() {
            bool found = false;
            for(int i = 0; i < fieldSize; i++) {
                for(int j = 0; j < fieldSize; j++) {
                    if(vertDashes[i, j] != null && horDashes[i, j] != null &&
                       vertDashes[i + 1, j] != null && horDashes[i, j + 1] != null) {
                        double halfLinesSpace = linesSpace / 2;
                        Cell cell = new Cell();
                        cell.Owner = currentPlayer;
                        cell.Center = new Point(i * linesSpace + halfLinesSpace, j * linesSpace + halfLinesSpace);
                        bool dontCreate = false;
                        foreach(var c in cells) {
                            if(c.IsEqualsLocation(cell)) dontCreate = true;
                        }
                        if(dontCreate) continue;

                        found = true;
                        fieldGrid.Children.Add(cell.GetSignImage(linesSpace, linesWidth));
                        cells.Add(cell);

                        ScoreChanged?.Invoke(currentPlayer);

                        if(cells.Count == cellsCount)
                            FieldFilled?.Invoke();
                    }
                }
            }
            return found;
        }
    }
}
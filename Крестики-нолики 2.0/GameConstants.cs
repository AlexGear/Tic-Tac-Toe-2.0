using System.Windows.Media;

namespace Крестики_нолики_2._0 {
    public static class Consts {
        public const int MinFieldSize = 5;
        public const int MaxFieldSize = 15;
        public const double MinFieldLinesWidth = 6.0d;
        public const double MaxFieldLinesWidth = 24.0d;
        public const double DashWidthRatio = 1.6d; // dashWidth = linesWidth * DashWidthRatio
        public const double PointHitboxSizeRatio = 0.95d; // dashPointSize = linesSpace * PointHitboxSizeRatio
        public const double LineCirclesSizeRatio = 0.5d; // lineCircleSize = linesSpace * LineCirclesSizeRatio
        public const double EllipsesAnimationDuration = 180.0d; // milliseconds
        public const double CrossOffsetRatio = 0.2d;
        public const double ZeroOffsetRatio = 0.1d;
        public const string BluePlayerMoveString = "Ходит синий игрок...";
        public const string RedPlayerMoveString = "Ходит красный игрок...";
        public const string YourMoveString = "Ваш ход";
        public const string YouWonFormat = "Вы победили со счетом {0}:{1}!";
        public const string YouLoseFormat = "Вы проиграли со счетом {0}:{1}.";
        public const string DeadHeatString = "Ничья.";
        public const string SureToExit = "Вы уверены, что хотите выйти? Весь прогресс будет утерян.";
        public const string SureToExitTitle = "Выход из игры";
        public const double GridsEmergenceDuration = 950.0d; // milliseconds
        public const int GridEmergenceDelay = 250; // milliseconds
    }

    public static class BluetoothCommands {
        public const byte AcceptInvite = 0x01;
        public const byte DiscardInvite = 0x02;
        public const byte ConnectionVerification = 0x03;
        public const byte VerificationConfirm = 0x04;
        public const byte Leave = 0x05;
        public const byte Pause = 0x06;
        public const byte UnPause = 0x07;
        public const byte FieldSize = 0x08;
    }

    public static class ConstantColors {
        public static readonly Color FieldGridLinesColor = Color.FromArgb(255, 191, 217, 255);
        public static readonly Color FieldGridBorderColor = Colors.Black;
        public static readonly Color RedPlayer = Colors.Red;
        public static readonly Color BluePlayer = Colors.Blue;
        public static readonly Color AvailableMovesEllipses = Color.FromArgb(180, 0, 0, 0);
        public static readonly Color GameOverGridBackground = Color.FromArgb(255, 255, 239, 176);
    }
}
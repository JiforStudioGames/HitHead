namespace Game.Scripts.UI.Core
{
    public readonly struct UIConstants
    {
        private const string R_POPUPS_PATH = @"UI/Popups/";
        private const string R_SCREENS_PATH = @"UI/Screens/";

        public static string Popups(string name) => R_POPUPS_PATH + name;
        public static string Screens(string name) => R_SCREENS_PATH + name;
    }
}
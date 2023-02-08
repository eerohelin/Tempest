using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tempest
{
    internal class Services
    {

        public static event EventHandler? ToolChanged;

        private static int _tool = 1;

        public static Color brushColor = Color.FromRgb(255,255,255);
        public static MapImage mapImage = new();
        public static int tool
        {
            get { return _tool; } set { _tool = value; ToolChanged(new Button(), EventArgs.Empty); }
        }
        public static bool lineSmoothing = true;
        public static double brushSize = 4;
    }
}

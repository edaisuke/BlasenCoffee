using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlasenSignage.Extensions
{
    public static class WindowExtensions
    {
        public static void ToVirtualFullScreen(this Window window)
        {
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }


            var x = SystemParameters.VirtualScreenLeft;
            var y = SystemParameters.VirtualScreenTop;
            var w = SystemParameters.VirtualScreenWidth;
            var h = SystemParameters.VirtualScreenHeight;

            window.Left = x - 10;
            window.Top = y - 10;
            window.Width = w + 20;
            window.Height = h + 20;
        }
    }
}

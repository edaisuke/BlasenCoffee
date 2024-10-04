using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Blasen.Utils
{
    public static class Extensions
    {
        public static T FindAncestor<T>(this DependencyObject dependencyObject)
            where T : DependencyObject
        {
            while (dependencyObject is not null)
            {
                if (dependencyObject is T target)
                {
                    return target;
                }

                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            return null;
        }
    }
}

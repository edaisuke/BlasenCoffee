using System.Windows;
using System.Windows.Media;

namespace BlasenSignage.Extensions
{
    public static class DependencyObjectExtensions
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

using BlasenSignage.Extensions;
using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace BlasenSignage.Behavior
{
    public class NameSpaceInheritBehavior : Behavior<DependencyObject>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject is not FrameworkElement element) { return; }

            element.Dispatcher?.BeginInvoke((Action)(async () =>
            {

                await Task.Delay(100);

                var sourceNameScope = NameScope.GetNameScope(element.FindAncestor<Window>());

                var cm = element.ContextMenu;
                if (cm is not null)
                {
                    NameScope.SetNameScope(cm, sourceNameScope);
                }

                var tt = element.ToolTip;
                if (tt is DependencyObject dependencyObject)
                {
                    NameScope.SetNameScope(dependencyObject, sourceNameScope);
                }

            }));
        }
    }
}

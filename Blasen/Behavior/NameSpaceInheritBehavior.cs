using Blasen.Utils;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Blasen.Behavior
{
    public class NameSpaceInheritBehavior : Behavior<DependencyObject>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject is not FrameworkElement element) { return; }

            element.Dispatcher?.BeginInvoke((Action)( async () => {

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

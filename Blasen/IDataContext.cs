using System.Windows;
using System.Windows.Controls;

namespace Blasen
{
    public interface IDataContext
    {
        void OnAttached(Window window);

        void OnDetached(Window window);
    }
}

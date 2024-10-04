using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Blasen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SetDataContext();

            this.ToMultiFullScreen();
        }


        private void SetDataContext()
        {
            this.DataContextChanged += (s, e) =>
            {
                if (DataContext is IDataContext)
                {
                    ((IDataContext)DataContext).OnDetached(this);
                    ((IDataContext)DataContext).OnAttached(this);
                }
            };
            this.DataContext = new ViewModel();
        }


        private void ToMultiFullScreen()
        {
            var x = SystemParameters.VirtualScreenLeft;
            var y = SystemParameters.VirtualScreenTop;
            var w = SystemParameters.VirtualScreenWidth;
            var h = SystemParameters.VirtualScreenHeight;

            this.Left = x -10;
            this.Top = y -10;
            this.Width = w +20;
            this.Height = h +20;
        }
    }
}
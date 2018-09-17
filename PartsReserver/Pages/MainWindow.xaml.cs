using System.Windows;
using PartsReserver.ViewModels;

namespace PartsReserver.Pages
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainPageViewModel();
        }
    }
}

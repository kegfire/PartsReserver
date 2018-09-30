using System.Collections.Generic;
using System.Data;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;

namespace PartsReserver.Pages
{
    /// <summary>
    /// Interaction logic for AutoRequestResultWindow.xaml
    /// </summary>
    public partial class AutoRequestResultWindow : Window
    {
        public AutoRequestResultWindow(DataTable dt)
        {
            InitializeComponent();
	        ResponseDataGrid.DataContext = dt.DefaultView;
	        ResponseDataGrid.CanUserAddRows = false;
        }
    }
}

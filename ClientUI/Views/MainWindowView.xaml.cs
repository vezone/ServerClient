using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        ViewModels.MainWindowViewModel m_MainWindowViewModel;

        public MainWindowView()
        {
            InitializeComponent();
            m_MainWindowViewModel = new ViewModels.MainWindowViewModel();
            DataContext = m_MainWindowViewModel;
            if (m_MainWindowViewModel.CloseAction == null)
            {
                m_MainWindowViewModel.CloseAction = new Action(Close);
            }
        }

        private void Event_MouseLeftButtonDown(object sender, RoutedEventArgs @event)
        {
            DragMove();
        }
    }
}

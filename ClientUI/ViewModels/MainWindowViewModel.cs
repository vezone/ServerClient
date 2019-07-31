using System;
using System.Windows.Controls;

namespace ClientUI.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private ContentControl m_ContentPage;

        private ContentControl m_ProfilePage;
        private ContentControl m_DialogsPage;

        private RelayCommand ProfileSelectCommand;
        private RelayCommand DialogsSelectCommand;
        private RelayCommand ExitCommand;

        public ContentControl ContentPage
        {
            get
            {
                return m_ContentPage;
            }
            set
            {
                m_ContentPage = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            if (ContentPage == null)
            {
                if (m_ProfilePage == null)
                {
                    m_ProfilePage = new Views.ProfileView();
                }
                ContentPage = m_ProfilePage;
            }

            ProfileSelectCommand = new RelayCommand((object obj) =>
            {
                if (m_ProfilePage == null)
                {
                    m_ProfilePage = new Views.ProfileView();
                }
                ContentPage = m_ProfilePage;
            });
            DialogsSelectCommand = new RelayCommand((object obj) =>
            {
                if (m_DialogsPage == null)
                {
                    m_DialogsPage = new Views.DialogsView();
                }
                ContentPage = m_DialogsPage;
            });
            ExitCommand = new RelayCommand((object obj) =>
            {
                System.Windows.MessageBoxResult result =
                    System.Windows.MessageBox.Show(
                        "Do you really want to close this app?", 
                        "Closing a window", 
                        System.Windows.MessageBoxButton.YesNo);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    CloseAction();
                }
            });
        }

        public Action CloseAction { get; set; }

        public RelayCommand btn_ProfilePageSelect_Click
        {
            get
            {
                return ProfileSelectCommand;
            }
        }

        public RelayCommand btn_DialogsPageSelect_Click
        {
            get
            {
                return DialogsSelectCommand;
            }
        }

        public RelayCommand btn_Exit_Click
        {
            get
            {
                return ExitCommand;
            }
        }
    }
}

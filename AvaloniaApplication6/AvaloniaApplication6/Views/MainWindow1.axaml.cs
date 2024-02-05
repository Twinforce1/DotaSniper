using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace AvaloniaApplication6.Views
{
    public partial class MainWindow1 : Window
    {
        public MainWindow1()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            // ShowMainView();
        }

        public void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // private void OnNavigateToSecondPageClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        // {
        //     ShowSecondPage();
        // }

        // public void ShowMainView()
        // {
        //     var mainPage = new MainView1(this);
        //     this.Content = mainPage;
        // }
        //
        // public void ShowSecondPage()
        // {
        //     var secondPage = new SecondPage(this);
        //     this.Content = secondPage;
        // }
        //
        // public void NavigateToSearchResult()
        // {
        //     var searchResult = new SearchResult(this);
        //     this.Content = searchResult;
        // }
        //
        //
        // public void GoBackToMainPage()
        // {
        //     ShowMainView();
        // }
        //
        // public void GoBackToSecondPage()
        // {
        //     ShowSecondPage();
        // }


        
    }
}
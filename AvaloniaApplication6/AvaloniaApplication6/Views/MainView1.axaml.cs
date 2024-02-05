using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication6.Views;

public partial class MainView1 : UserControl
{

    
    public MainView1()
    {
        InitializeComponent();
        
    }

    // public MainView1(MainView mainWindow)
    // {
    //     throw new System.NotImplementedException();
    // }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
            
            
            
    }
    public void ShowMainView(object? sender, RoutedEventArgs routedEventArgs)
    {
        var mainPage = new MainView1();
        this.Content = mainPage;
    }

    public void ShowSecondPage(object? sender, RoutedEventArgs routedEventArgs)
    {
        var secondPage = new SecondPage(this);
        this.Content = secondPage;
    }

    public void NavigateToSearchResult(object? sender, RoutedEventArgs routedEventArgs)
    {
        var searchResult = new SearchResult(this);
        this.Content = searchResult;
    }

        
    
    
    
    
}
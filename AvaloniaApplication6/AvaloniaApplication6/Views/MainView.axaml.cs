using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication6.ViewModels;

namespace AvaloniaApplication6.Views;

public partial class MainView : UserControl
{
    // создаем экземпляр MainViewModel для связи с MainView
    public MainView()
    {
        InitializeComponent();
        // MainWindow1 mainWindow1 = new MainWindow1();
        // DataContext = mainWindow1;
        // var mainView1 = new MainView1(this);
        // this.Content = mainView1;
        ShowMainView();
    }
    
    
    public void ShowMainView()
    {
        // var mainView1 = new MainWindow1();
        var mainPage = new MainView1();
        this.Content = mainPage;
    }
}
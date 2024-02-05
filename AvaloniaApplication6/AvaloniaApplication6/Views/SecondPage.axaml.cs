using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using AvaloniaApplication6.Views;


namespace AvaloniaApplication6.Views;

public partial class SecondPage : UserControl
{
    private readonly MainView1 _mainWindow1;
    private readonly UserSettingsRepository _userSettingsRepository;
    private TextBox _usernameTextBox;
    private TextBox _emailTextBox;
    private CheckBox _notificationsCheckBox;
    private CheckBox _mailingCheckBox;
    
    

    public SecondPage(MainView1 mainWiew1)
    {
        InitializeComponent();
        
        this._mainWindow1 = mainWiew1;
        _userSettingsRepository = new UserSettingsRepository("Host=localhost;Port=5432;Username=postgres;Password=Qwerty12345;Database=Settings;");
        
    }

    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _usernameTextBox = this.FindControl<TextBox>("UsernameTextBox");
        _emailTextBox = this.FindControl<TextBox>("EmailTextBox");
        _notificationsCheckBox = this.FindControl<CheckBox>("NotificationsCheckBox");
        _mailingCheckBox = this.FindControl<CheckBox>("MailingCheckBox");
    }



    private async void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var email = _emailTextBox.Text;
        var notificationsEnabled = _notificationsCheckBox.IsChecked ?? false;
        var mailingEnabled = _mailingCheckBox.IsChecked ?? false;

        await _userSettingsRepository.SaveSettingsAsync(email, notificationsEnabled, mailingEnabled);

    }
    


    public void OnGoToMainPageClick(object sender, RoutedEventArgs e)
    {
        var mainPage = new MainView1();
        this.Content = mainPage;
    }
    public void OnSearchButtonClick(object? sender, RoutedEventArgs e)
    {
        // Вызывайте метод поиска здесь
        var searchResult = new SearchResult(_mainWindow1);
        this.Content = searchResult;
    }
}
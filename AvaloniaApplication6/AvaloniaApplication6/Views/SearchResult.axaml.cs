using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ExCSS;
using Newtonsoft.Json;
using AvaloniaApplication6;


namespace AvaloniaApplication6.Views
{
    
    public partial class SearchResult : UserControl
    {
        private readonly MainView1 _mainWindow1;
        private TextBox _playerNameTextBox;
        private Image _avatarImage;
        private TextBlock _resultTextBlock;
        private Image _ImagePath;
        private Border _contentBorder;
        private TextBlock _winLossTextBlock;
        private TextBlock _matchesTextBlock;
        private ContentControl _playerPage;
        private TextBlock _killsTextBlock;
        private TextBlock _deathsTextBlock;
        private TextBlock _assistsTextBlock;
        private TextBlock _heroIdTextBlock;
        private Image _heroImage;

        
        
        public SearchResult(MainView1 mainWiew1)
        {
            
            _mainWindow1 = mainWiew1;
            InitializeComponent();
            _contentBorder = this.FindControl<Border>("contentBorder");
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _playerNameTextBox = this.FindControl<TextBox>("playerNameTextBox");
            _avatarImage = this.FindControl<Image>("avatarImage");
            _resultTextBlock = this.FindControl<TextBlock>("resultTextBlock");
            _ImagePath = this.FindControl<Image>("ImagePath");
            _winLossTextBlock = this.FindControl<TextBlock>("winLossTextBlock");
            _matchesTextBlock = this.FindControl<TextBlock>("matchesTextBlock");
            _killsTextBlock = this.FindControl<TextBlock>("KillsTextBlock");
            _deathsTextBlock = this.FindControl<TextBlock>("DeathsTextBlock");
            _assistsTextBlock = this.FindControl<TextBlock>("AssistsTextBlock");
            _heroIdTextBlock = this.FindControl<TextBlock>("HeroIdTextBlock");
            _heroImage = this.FindControl<Image>("HeroImage");
            
            
            

            // Подписываемся на событие клика кнопки
            Button onSearchButtonClick = this.FindControl<Button>("OnSearchButtonClick");
            onSearchButtonClick.Click += OnSearchButtonClick_Click;
            
            // // Добавляем событие клика для кнопки открытия PlayerPage
            // Button openPlayerPageButton = this.FindControl<Button>("OpenPlayerPageButton");
            // openPlayerPageButton.Click += OpenPlayerPageButton_Click;
        }
        

        private async void OnSearchButtonClick_Click(object sender, RoutedEventArgs e)
        {
            
            string playerName = _playerNameTextBox.Text;
            if (!string.IsNullOrEmpty(playerName))
            {
                await SearchPlayer(playerName);
                // Показываем Grid после успешного поиска
                _contentBorder.IsVisible = true;
            }
            else
            {
                _resultTextBlock.Text = "Введите ник игрока.";
            }
        }

        private void OnGoBackMainButtonClick(object sender, RoutedEventArgs e)
        {
            var mainPage = new MainView1();
            this.Content = mainPage;
        }
        
        private void OnGoBackSecondButtonClick(object sender, RoutedEventArgs e)
        {
            var secondPage = new SecondPage(_mainWindow1);
            this.Content = secondPage;
        }
        

       
        
        

        private async Task SearchPlayer(string playerName)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string apiKey = "Ваш API-ключ OpenDota"; // Получите API-ключ на https://www.opendota.com/api-keys
            
                    string apiUrl = $"https://api.opendota.com/api/search?q={playerName}&api_key={apiKey}";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    ParseAndDisplayResults(jsonResponse);
                }
                catch (HttpRequestException ex)
                {
                    _resultTextBlock.Text = $"Ошибка при выполнении HTTP-запроса: {ex.Message}";
                }
            }
        }
        
        
        
        private async void ParseAndDisplayResults(string jsonResponse)
        {
            try
            {
                JArray resultArray = JArray.Parse(jsonResponse);

                if (resultArray.Count > 0)
                {
                    JObject firstResult = (JObject)resultArray[0];
                    //ProfilePlayer profilePlayer = new ProfilePlayer();
                    PlayerPage playerPage = new PlayerPage(_mainWindow1);

                    string personaname = firstResult.Value<string>("personaname");
                    string avatarfull = firstResult.Value<string>("avatarfull");
                    long account_id = firstResult.Value<long>("account_id");
                    _resultTextBlock.Text = personaname;

                    // Отображаем изображение
                    using (HttpClient client = new HttpClient())
                    {
                        byte[] imageData = await client.GetByteArrayAsync(avatarfull);
                        Bitmap bitmap = new Bitmap(new MemoryStream(imageData));
                        _avatarImage.Source = bitmap;
                        
                        // Запрос информации о рейтинге
                        string additionalInfoUrl = $"https://api.opendota.com/api/players/{account_id}";
                        string additionalInfoResponse = await client.GetStringAsync(additionalInfoUrl);
                        
                        
                        try
                        {
                            JToken additionalInfoData = JToken.Parse(additionalInfoResponse);
                        
                            if (additionalInfoData.Type == JTokenType.Object)
                            {
                                JObject additionalInfoObject = (JObject)additionalInfoData;
                        
                                // Проверяем, существует ли поле 'rank_tier' в объекте
                                if (additionalInfoObject.TryGetValue("rank_tier", out JToken rankTier))
                                {
                                    // Преобразуем rank_tier в строку
                                    string rankTierString = rankTier.ToString();
                        
                                    // Проверяем, достаточно ли символов в строке
                                    if (rankTierString.Length >= 2)
                                    {
                                        
                        
                                        // Определяем изображение на основе диапазона значений rank_tier
                                        string imagePathString = GetImagePath(rankTierString);
                        
                                        // Выводим результат
                                        _resultTextBlock.Text =$"{personaname}";
                                        try
                                        {
                                            // Получаем полный путь к корневой директории проекта
                                            string projectRootDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "AvaloniaApplication6");

                                            // Выводим полный путь в консоль
                                            Console.WriteLine($"Корневая директория проекта: {projectRootDirectory}");
                                            

                                            // Формируем относительный путь к изображению
                                            string imagePathRelative = $"Assets\\ranks\\{imagePathString}";

                                            // Формируем полный путь к изображению
                                            string imagePathUri = System.IO.Path.Combine(projectRootDirectory, imagePathRelative);

                                            // Создаем объект Bitmap из файла
                                            var rankTierImage = new Bitmap(imagePathUri);

                                            // Устанавливаем изображение в элемент Image
                                            _ImagePath.Source = rankTierImage;
                                            playerPage.SetPlayerInfo( personaname, new Bitmap(new MemoryStream(imageData)), 
                                                new Bitmap(imagePathUri));
                                        }
                                        catch (Exception ex)
                                        {
                                            // Выводим сообщение об ошибке в консоль
                                            Console.WriteLine($"Ошибка при загрузке изображения: {ex.Message}");
                                        }

                                        // profilePlayer.SetPlayerInfo(personaname, new Bitmap(new MemoryStream(imageData)), 
                                        //     $"Personaname: {personaname}\nRank Tier: {rankTier}", 
                                        // Загружаем изображение героя
                                        // string imagePath = heroInfo["heroImagePath"].ToString();
                                        // string fullPath = Path.Combine("C:\\Users\\vazge\\RiderProjects\\AvaloniaApplication6\\AvaloniaApplication6\\AvaloniaApplication6\\Assets\\heroes\\", imagePath);

                                        //     new Bitmap(imagePathUri));
                                        
                                    }
                                    else
                                    {
                                        _resultTextBlock.Text = "Недостаточно цифр в значении rank_tier.";
                                    }
                                }
                                else
                                {
                                    _resultTextBlock.Text = "Информация о rank_tier отсутствует в полученном JSON.";
                                }
                            }
                            else
                            {
                                _resultTextBlock.Text = "Полученный JSON не является объектом.";
                            }
                        }
                        catch (JsonReaderException)
                        {
                            _resultTextBlock.Text = "Ошибка при чтении JSON. Возможно, получен массив, а не объект.";
                        }
                        catch (Exception ex)
                        {
                            _resultTextBlock.Text = $"Ошибка: {ex.Message}";
                        }
                        _avatarImage.Tapped += async (sender, args) =>
                        {
                            // Добавьте здесь код, который выполнится при клике на изображение
                            // Например, можно открыть большую версию изображения или выполнить другие действия
                            // MessageBox.Show("Изображение было кликнуто!");
                            _mainWindow1.Content = playerPage;
                        };
                        // Additional info about player's matches
                        string matchesUrl = $"https://api.opendota.com/api/players/{account_id}/wl";
                        string matchesResponse = await client.GetStringAsync(matchesUrl);

                        try
                        {
                            JObject matchesData = JObject.Parse(matchesResponse);

                            if (matchesData.TryGetValue("win", out JToken win) && matchesData.TryGetValue("lose", out JToken lose))
                            {
                                int winCount = win.Value<int>();
                                int loseCount = lose.Value<int>();

                                // Calculate win rate
                                double winRate = (double)winCount / (winCount + loseCount) * 100;
                                await GetMatchesInfo(account_id, playerPage);
                                // Display the win rate
                                //_winLossTextBlock.Text = $"Win Rate: {winRate:F2}% \n (Wins: {winCount}, Losses: {loseCount})";
                                //profilePlayer.SetPlayerInfo1($"Win Rate: {winRate:F2}% \n (Wins: {winCount}, Losses: {loseCount}");
                                playerPage.SetPlayerInfo1($"{winRate:F2}%");
                                
                                
                            }
                            else
                            {
                                _winLossTextBlock.Text = "Win and lose information not available.";
                            }
                        }
                        catch (JsonReaderException)
                        {
                            _winLossTextBlock.Text = "Error reading match data JSON.";
                        }
                        // Запрос информации о матчах
                        string matchesInfoUrl = $"https://api.opendota.com/api/players/{account_id}/counts";
                        string matchesInfoResponse = await client.GetStringAsync(matchesInfoUrl);

                        // Парсим информацию о матчах
                        JToken matchesInfoData = JToken.Parse(matchesInfoResponse);

                        if (matchesInfoData.Type == JTokenType.Object)
                        {
                            JObject matchesInfoObject = (JObject)matchesInfoData;

                            // Извлекаем информацию только о leaver_status
                            JToken leaverStatusToken;
                            if (matchesInfoObject.TryGetValue("leaver_status", out leaverStatusToken))
                            {
                                // Извлекаем информацию из "0"
                                JObject leaverStatusObject = (JObject)leaverStatusToken["0"];

                                // Выводим информацию о "0" в TextBlock
                                //_matchesTextBlock.Text = $"Games: {leaverStatusObject.Value<int>("games")}\n";
                                //profilePlayer.SetPlayerInfo2($"Games: {leaverStatusObject.Value<int>("games")}\n");
                                playerPage.SetPlayerInfo2($"{leaverStatusObject.Value<int>("games")}\n");
                            }
                            else
                            {
                                _matchesTextBlock.Text = "Информация о leaver_status отсутствует в полученном JSON.";
                            }
                        }
                        else
                        {
                            _matchesTextBlock.Text = "Полученный JSON не является объектом.";
                        }
                        
                    }
                    //profilePlayer.Show();
                    //_mainWindow.Content = playerPage;
                }
                else
                {
                    _resultTextBlock.Text = "Нет результатов для указанного игрока.";
                }
            }
            catch (Exception ex)
            {
                _resultTextBlock.Text = $"Ошибка при обработке JSON-ответа: {ex.Message}";
            }
        }
        
        private async Task GetMatchesInfo(long accountId, PlayerPage playerPage)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string matchesUrl = $"https://api.opendota.com/api/players/{accountId}/matches";
                    HttpResponseMessage response = await client.GetAsync(matchesUrl);
                    response.EnsureSuccessStatusCode();

                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Парсим JSON
                    JArray matchesArray = JArray.Parse(jsonResponse);
                    

                    // Проверяем, есть ли хотя бы один элемент
                    if (matchesArray.Count > 0)
                    {
                        JObject firstMatch = (JObject)matchesArray[0];
                        // Применение в контексте последнего матча
                        JObject lastMatch = (JObject)matchesArray[0];
                        DisplayKillDeathAssistInfo(lastMatch, playerPage);
                        
                        
                        string matchOutcome = DetermineMatchOutcome(firstMatch);
                        playerPage.ResultMatch(matchOutcome);
    
                        // Дальше можно использовать matchOutcome, например, вывести в консоль
                        Console.WriteLine($"Результат матча: {matchOutcome}");

                        // Дальше можно работать с данными первого матча, например:
                        long matchId = firstMatch.Value<long>("match_id");
                        Console.WriteLine($"ID первого матча: {firstMatch}");
                        
                        
                        
                    }
                    else
                    {
                        Console.WriteLine("Нет данных о матчах для указанного игрока.");
                    }
                    
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Ошибка при выполнении HTTP-запроса: {ex.Message}");
                }
            }
        }
        private string GetHeroImagePath(int heroId)
        {
            // Если hero_id присутствует в словаре, возвращаем соответствующий путь к изображению
            if (heroImageDictionary.ContainsKey(heroId))
            {
                return heroImageDictionary[heroId];
            }

            // Возвращаем путь к изображению по умолчанию, если hero_id не найден
            return "/heroes/default_hero.png";
        }
        
        private void DisplayKillDeathAssistInfo(JObject match, PlayerPage playerPage)
        {
            try
            {
                
                // Извлекаем данные об убийствах, смертях, помощи и hero_id
                int kills = match.Value<int>("kills");
                int deaths = match.Value<int>("deaths");
                int assists = match.Value<int>("assists");
                int heroId = match.Value<int>("hero_id");

                // Получаем информацию о герое
                JObject heroInfo = GetHeroInfo(heroId);
                
                // Выводим изображение героя
                DisplayHeroImage(heroInfo, playerPage);
                // Выводим информацию в элементы управления
                _killsTextBlock.Text = $"Убийства: {kills}";
                _deathsTextBlock.Text = $"Смерти: {deaths}";
                _assistsTextBlock.Text = $"Помощь: {assists}";
                
                // Выводим полученные данные
                playerPage.SetPlayerInfo3(kills, deaths, assists);
                Console.WriteLine($"Убийства: {kills}, Смерти: {deaths}, Помощь: {assists}");
                Console.WriteLine($"Hero ID: {heroInfo["heroId"]}");
                Console.WriteLine($"Изображение героя: {heroInfo["heroImagePath"]}");
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если не удалось получить нужные данные из JSON
                Console.WriteLine($"Ошибка при извлечении информации об убийствах, смертях, помощи и hero_id: {ex.Message}");
            }
            
        }
        
        private void DisplayHeroImage(JObject heroInfo, PlayerPage playerPage)
        {
            try
            {
                // Загружаем изображение героя
                string imagePath = heroInfo["heroImagePath"].ToString();

                // Получаем полный путь к корневой директории проекта
                string projectRootDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "AvaloniaApplication6");

                // Формируем относительный путь к изображению героя
                string imagePathRelative = $"Assets\\heroes\\{imagePath}";

                // Формируем полный путь к изображению героя
                string fullPath = System.IO.Path.Combine(projectRootDirectory, imagePathRelative);

                // Проверка на null перед установкой изображения
                if (_heroImage != null)
                {
                    _heroImage.Source = new Bitmap(fullPath);
                    playerPage.SetPlayerInfo4(new Bitmap(fullPath));
                }
                else
                {
                    Console.WriteLine("_heroImage is null. Unable to set hero image.");
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если не удалось загрузить изображение героя
                Console.WriteLine($"Ошибка при загрузке изображения героя: {ex.Message}");
            }

        }
        private string DetermineMatchOutcome(JObject match)
        {
            try
            {
                int playerSlot = match.Value<int>("player_slot");
                bool radiantWin = match.Value<bool>("radiant_win");

                // Проверяем условия
                if ((playerSlot >= 0 && playerSlot <= 127 && radiantWin) || (playerSlot >= 128 && playerSlot <= 255 && !radiantWin))
                {
                    
                    return "Победа";
                }
                else
                {
                    return "Поражение";
                }
                
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если не удалось получить нужные данные из JSON
                Console.WriteLine($"Ошибка при определении результата матча: {ex.Message}");
                return "Не удалось определить";
            }
        }
        
        Dictionary<int, string> heroImageDictionary = new Dictionary<int, string>
{
    { 1, "Abaddon_icon.png" },
    { 2, "Alchemist_icon.png" },
    { 3, "Ancient_Apparition_icon.png" },
    { 4, "Anti-Mage_icon.png" },
    { 5, "Arc_Warden_icon.png" },
    { 6, "Axe_icon.png" },
    { 7, "Bane_icon.png" },
    { 8, "Batrider_icon.png" },
    { 9, "Beastmaster_icon.png" },
    { 10, "Bloodseeker_icon.png" },
    { 11, "Bounty_Hunter_icon.png" },
    { 12, "Brewmaster_icon.png" },
    { 13, "Bristleback_icon.png" },
    { 14, "Broodmother_icon.png" },
    { 15, "Centaur_Warrunner_icon.png" },
    { 16, "Chaos_Knight_icon.png" },
    { 17, "Chen_icon.png" },
    { 18, "Clinkz_icon.png" },
    { 19, "Clockwerk_icon.png" },
    { 20, "Crystal_Maiden_icon.png" },
    { 21, "Dark_Seer_icon.png" },
    { 22, "Dark_Willow_icon.png" },
    { 23, "Dawnbreaker_icon.png" },
    { 24, "Dazzle_icon.png" },
    { 25, "Death_Prophet_icon.png" },
    { 26, "Disruptor_icon.png" },
    { 27, "Doom_icon.png" },
    { 28, "Dragon_Knight_icon.png" },
    { 29, "Drow_Ranger_icon.png" },
    { 30, "Earth_Spirit_icon.png" },
    { 31, "Earthshaker_icon.png" },
    { 32, "Elder_Titan_icon.png" },
    { 33, "Ember_Spirit_icon.png"},
    { 34, "Enchantress_icon.png" },
    { 35, "Enigma_icon.png" },
    { 36, "Faceless_Void_icon.png" },
    { 37, "Grimstroke_icon.png" },
    { 38, "Gyrocopter_icon.png" },
    { 39, "Hoodwink_icon.png" },
    { 40, "Huskar_icon.png" },
    { 41, "Invoker_icon.png" },
    { 42, "Io_icon.png" },
    { 43, "Jakiro_icon.png" },
    { 44, "Juggernaut_icon.png" },
    { 45, "Keeper_of_the_Light_icon.png" },
    { 46, "Kunkka_icon.png" },
    { 47, "Legion_Commander_icon.png" },
    { 48, "Leshrac_icon.png" },
    { 49, "Lich_icon.png" },
    { 50, "Lifestealer_icon.png" },
    { 51, "Lina_icon.png" },
    { 52, "Lion_icon.png" },
    { 53, "Lone_Druid_icon.png" },
    { 54, "Luna_icon.png" },
    { 55, "Lycan_icon.png" },
    { 56, "Magnus_icon.png" },
    { 57, "Marci_icon.png" },
    { 58, "Mars_icon.png" },
    { 59, "Medusa_icon.png" },
    { 60, "Meepo_icon.png" },
    { 61, "Mirana_icon.png" },
    { 62, "Monkey_King_icon.png" },
    { 63, "Morphling_icon.png" },
    { 64, "Muerta_icon.png" },
    { 65, "Naga_Siren_icon.png" },
    { 66, "Nature's_Prophet_icon.png" },
    { 67, "Necrophos_icon.png" },
    { 68, "Night_Stalker_icon.png" },
    { 69, "Nyx_Assassin_icon.png" },
    { 70, "Ogre_Magi_icon.png" },
    { 71, "Omniknight_icon.png" },
    { 72, "Oracle_icon.png" },
    { 73, "Outworld_Devourer_icon.png" },
    { 74, "Pangolier_icon.png" },
    { 75, "Phantom_Assassin_icon.png" },
    { 76, "Phantom_Lancer_icon.png" },
    { 77, "Phoenix_icon.png" },
    { 78, "Primal_Beast_icon.png" },
    { 79, "Puck_icon.png" },
    { 80, "Pudge_icon.png" },
    { 81, "Pugna_icon.png" },
    { 82, "Queen_of_Pain_icon.png" },
    { 83, "Razor_icon.png" },
    { 84, "Riki_icon.png" },
    { 85, "Rubick_icon.png" },
    { 86, "Sand_King_icon.png" },
    { 87, "Shadow_Demon_icon.png" },
    { 88, "Shadow_Fiend_icon.png" },
    { 89, "Shadow_Shaman_icon.png" },
    { 90, "Silencer_icon.png" },
    { 91, "Skywrath_Mage_icon.png" },
    { 92, "Slardar_icon.png" },
    { 93, "Slark_icon.png" },
    { 94, "Snapfire_icon.png" },
    { 95, "Sniper_icon.png" },
    { 96, "Spectre_icon.png" },
    { 97, "Spirit_Breaker_icon.png" },
    { 98, "Storm_Spirit_icon.png" },
    { 99, "Sven_icon.png" },
    { 100, "Techies_icon.png" },
    { 101, "Templar_Assassin_icon.png" },
    { 102, "Terrorblade_icon.png" },
    { 103, "Tidehunter_icon.png" },
    { 104, "Timbersaw_icon.png" },
    { 105, "Tinker_icon.png" },
    { 106, "Tiny_icon.png" },
    { 107, "Treant_Protector_icon.png" },
    { 108, "Troll_Warlord_icon.png" },
    { 109, "Tusk_icon.png" },
    { 110, "Underlord_icon.png" },
    { 111, "Undying_icon.png" },
    { 112, "Ursa_icon.png" },
    { 113, "Vengeful_Spirit_icon.png" },
    { 114, "Venomancer_icon.png" },
    { 115, "Viper_icon.png" },
    { 116, "Visage_icon.png" },
    { 117, "Void_Spirit_icon.png" },
    { 118, "Warlock_icon.png" },
    { 119, "Weaver_icon.png" },
    { 120, "Windranger_icon.png" },
    { 121, "Winter_Wyvern_icon.png" },
    { 122, "Witch_Doctor_icon.png" },
    { 123, "Wraith_King_icon.png" },
    { 124, "Zeus_icon.png" },
    
}; 


        private JObject GetHeroInfo(int heroId)
        {
            // Проверяем, есть ли информация о герое в словаре
            if (heroImageDictionary.TryGetValue(heroId, out var imagePath))
            {
                return new JObject
                {
                    { "heroId", heroId },
                    { "heroImagePath", imagePath }
                };
            }

            // Возвращаем информацию по умолчанию, если герой не найден
            return new JObject
            {
                { "heroId", heroId },
                { "heroImagePath", "/heroes/default_hero.png" }
            };
        }
        private string GetImagePath(string rankTier)
        {
        
            // Создаем словарь для соответствия между числами и путями к изображениям
            Dictionary<string, string> imagePaths = new Dictionary<string, string>
            {
                { "11", "herald1.png" },
                { "12", "herald2.png" },
                { "13", "herald3.png" },
                { "14", "herald4.png" },
                { "15", "herald5.png" },
                { "21", "guardian1.png" },
                { "22", "guardian2.png" },
                { "23", "guardian3.png" },
                { "24", "guardian4.png" },
                { "25", "guardian5.png" },
                { "31", "crusader1.png" },
                { "32", "crusader2.png" },
                { "33", "crusader3.png" },
                { "34", "crusader4.png" },
                { "35", "crusader5.png" },
                { "41", "archon1.png" },
                { "42", "archon2.png" },
                { "43", "archon3.png" },
                { "44", "archon4.png" },
                { "45", "archon5.png" },
                { "51", "legend1.png" },
                { "52", "legend2.png" },
                { "53", "legend3.png" },
                { "54", "legend4.png" },
                { "55", "legend5.png" },
                { "61", "ancient1.png" },
                { "62", "ancient2.png" },
                { "63", "ancient3.png" },
                { "64", "ancient4.png" },
                { "65", "ancient5.png" },
                { "71", "divine1.png" },
                { "72", "divine2.png" },
                { "73", "divine3.png" },
                { "74", "divine4.png" },
                { "75", "divine5.png" },
                { "80", "immortal.png" },
                // Добавьте соответствия для остальных чисел в диапазонах
            };
        
            // Проверяем, есть ли соответствующий путь в словаре
            if (imagePaths.ContainsKey(rankTier))
            {
                return imagePaths[rankTier];
            }
        
            return "DefaultImage.png"; // Путь к изображению по умолчанию
        }

    }
}

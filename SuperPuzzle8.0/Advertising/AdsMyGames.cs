﻿using Microsoft.Phone.Tasks;
using PhotoPuzzle.Helpers;
using PhotoPuzzle.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoPuzzle.Advertising
{
    public class AdsMyGames
    {
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        List<AdsGame> AdsGames = new List<AdsGame>
        {
            new AdsGame
            {
                AppId = "d240f4cd-e4fc-405f-98a5-ea9002afff6d",
                Icon = new Uri("/Images/games/fw-300.png", UriKind.RelativeOrAbsolute),
                Name = "Филлворды",
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.Филлворды,
                SupportLanguages = SupportLanguages.RU
            },
            new AdsGame
            {
                AppId = "b8d768c7-11b5-4eea-8c3d-f8012148e97d",
                Icon = new Uri("/Images/games/fw-en-300.png", UriKind.RelativeOrAbsolute),
                Name = "FillWords",
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.FillWords,
                SupportLanguages = SupportLanguages.EN
            },
            new AdsGame
            {
                AppId = "0585296f-f9c1-483e-9e99-795219e1410f",
                Icon = new Uri("/Images/games/fw-it-300.png", UriKind.RelativeOrAbsolute),
                Name = "Riempire Parole",
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.RiempireParole,
                SupportLanguages = SupportLanguages.IT
            },
            new AdsGame
            {
                AppId = "0f892535-518b-4199-902c-711130e7c7bd",
                Icon = new Uri("/Images/games/mt-300.png", UriKind.RelativeOrAbsolute),
                Name = AppResources.MemoryTraining,
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.ТренировкаПамяти,
                SupportLanguages = SupportLanguages.All
            },
            new AdsGame
            {
                AppId = "4d4336f6-29a4-4128-935b-5885d67c79ad",
                Icon = new Uri("/Images/games/tp-300.png", UriKind.RelativeOrAbsolute),
                Name = "Talking Puzzle",
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.ГоворящийПазл,
                SupportLanguages = SupportLanguages.All
            },
            new AdsGame
            {
                AppId = "0363c883-6428-4bd6-b599-f11796c69cc0",
                Icon = new Uri("/Images/games/b_300.png", UriKind.RelativeOrAbsolute),
                Name = "Bottle",
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.Бутылочка,
                SupportLanguages = SupportLanguages.All
            },
            new AdsGame
            {
                AppId = "2c1abff6-596d-45e4-8f25-fed9ae780b4b",
                Icon = new Uri("/Images/games/cc-300.png", UriKind.RelativeOrAbsolute),
                Name = "Collect Сat",
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.СобериКота,
                SupportLanguages = SupportLanguages.All
            }
        };

        public void ShowAd(Action cancelAction)
        {
            var currentCulture = CultureInfo.CurrentCulture.ToString().Substring(0, 2).ToUpper();
            var supportGames = AdsGames.Where(x => x.SupportLanguages == SupportLanguages.All || Enum.GetName(typeof(SupportLanguages), x.SupportLanguages) == currentCulture).ToList();
            if (supportGames.Count > 0)
            {
                var rand = new Random();
                int selectIndex = rand.Next(0, supportGames.Count);
                var game = supportGames[selectIndex];
                string isShowAds = String.Format("isShowAds{0}", game.AppId);
                if (!settings.Contains(isShowAds))
                {
                    settings[isShowAds] = true;
                    settings.Save();
                    var image = new Image()
                    {
                        Source = new BitmapImage(game.Icon),
                        Width = 150,
                        Height = 150
                    };
                    image.Tap += delegate
                    {
                        OpenGameInStore(game.AppId);
                    };
                    var title = new TextBlock
                    {
                        Text = game.TitleMessage,
                        Foreground = new SolidColorBrush(Colors.White),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5, 5, 5, 10),
                        TextWrapping = TextWrapping.Wrap
                    };
                    var gameName = new TextBlock
                    {
                        Text = game.Name,
                        Foreground = new SolidColorBrush(Colors.White),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5, 5, 5, 20)
                    };
                    var body = new StackPanel
                    {
                        Orientation = System.Windows.Controls.Orientation.Vertical,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Children =
                    {
                        title,
                        image,
                        gameName
                    }
                    };
                    var popup = new PopupMessage
                    {
                        Body = body
                    };
                    popup.OnOkClick += delegate
                    {
                        OpenGameInStore(game.AppId);
                    };
                    popup.OnCancelClick += delegate
                    {
                        cancelAction.Invoke();
                    };
                    popup.Show("", true);
                }
            }
        }

        private void OpenGameInStore(string appId)
        {
            var marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentIdentifier = appId;
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
            marketplaceDetailTask.Show();
        }
    }

    public class AdsGame
    {
        public string TitleMessage { get; set; }
        public Uri Icon { get; set; }
        public string Name { get; set; }
        public string AppId { get; set; }
        public Game Game { get; set; }
        public SupportLanguages SupportLanguages { get; set; }
    }

    public enum Game
    {
        CompetePeppers,
        Филлворды,
        RiempireParole,
        FillWords,
        ТренировкаПамяти,
        ГоворящийПазл,
        Бутылочка,
        ПазлыМаши,
        СобериКота
    }

    public enum SupportLanguages
    {
        All,
        EN,
        IT,
        RU
    }
}

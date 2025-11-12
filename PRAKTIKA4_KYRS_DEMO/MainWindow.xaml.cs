using PRAKTIKA4_KYRS_DEMO.BD;
using PRAKTIKA4_KYRS_DEMO.BD.MODELS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PRAKTIKA4_KYRS_DEMO
{
    public partial class MainWindow : Window
    {
        Praktika2222Entities db = new Praktika2222Entities();
        private List<EventViewModel> allEvents;
        private List<Direction> allDirections;

        public MainWindow()
        {
            InitializeComponent();
            LoadDirections();
            LoadEvents();
        }

        private void LoadDirections()
        {
            try
            {
                allDirections = db.Direction.ToList();

                cmbDirections.Items.Clear();
                cmbDirections.Items.Add(new ComboBoxItem
                {
                    Content = "Все направления",
                    Tag = "all"
                });

                foreach (var direction in allDirections)
                {
                    cmbDirections.Items.Add(new ComboBoxItem
                    {
                        Content = direction.DirectionName,
                        Tag = direction.ID
                    });
                }

                cmbDirections.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке направлений: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadEvents()
        {
            try
            {
                var eventsList = db.Event.ToList();

                allEvents = eventsList.Select(ev => new EventViewModel
                {
                    LogoPath = GetEventLogoPath(ev.ID),
                    NameEvent = ev.NameEvent,
                    DirectionName = GetEventDirection(ev.ID),
                    Date = ev.DateStart.ToString("dd.MM.yyyy"),
                    DateStart = ev.DateStart,
                }).ToList();

                EventsList.ItemsSource = allEvents;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке мероприятий: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetEventLogoPath(int eventId)
        {
            try
            {
                string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                string imageDir = Path.Combine(projectPath, "Image");

                string[] extensions = { ".jpg", ".png", ".jpeg", ".bmp", ".gif" };

                foreach (string ext in extensions)
                {
                    string fullPath = Path.Combine(imageDir, $"{eventId}{ext}");
                    if (File.Exists(fullPath))
                    {
                        return $"Image/{eventId}{ext}";
                    }
                }

                return "Image/foto.jpg";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске изображения: {ex.Message}");
                return "Image/foto.jpg";
            }
        }

        private string GetEventDirection(int eventId)
        {
            var direction = db.Event_Type_Dictionary
                .Where(etd => etd.IDEvent == eventId)
                .Select(etd => etd.Direction.DirectionName)
                .FirstOrDefault();

            return direction ?? "Не указано";
        }


        private void cmbDirections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cmbDateFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (allEvents == null) return;

            var filtered = allEvents.AsEnumerable(); if (cmbDirections.SelectedItem is ComboBoxItem selectedDirectionItem)
            {
                string selectedDirection = selectedDirectionItem.Content.ToString();
                if (selectedDirection != "Все направления")
                {
                    filtered = filtered.Where(ev => ev.DirectionName == selectedDirection);
                }
            }

            if (cmbDateFilter.SelectedItem is ComboBoxItem selectedDateItem)
            {
                string selectedDateFilter = selectedDateItem.Content.ToString();
                DateTime today = DateTime.Today;

                switch (selectedDateFilter)
                {
                    case "Сегодня":
                        filtered = filtered.Where(ev => ev.DateStart.Date == today);
                        break;

                    case "На этой неделе":
                        DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
                        DateTime endOfWeek = startOfWeek.AddDays(6);
                        filtered = filtered.Where(ev => ev.DateStart.Date >= startOfWeek && ev.DateStart.Date <= endOfWeek);
                        break;

                    case "В этом месяце":
                        DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
                        DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                        filtered = filtered.Where(ev => ev.DateStart.Date >= startOfMonth && ev.DateStart.Date <= endOfMonth);
                        break;

                    case "Предстоящие":
                        filtered = filtered.Where(ev => ev.DateStart.Date > today);
                        break;

                    case "Прошедшие":
                        filtered = filtered.Where(ev => ev.DateStart.Date < today);
                        break;
                }
            }


            EventsList.ItemsSource = filtered.ToList();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            Auth auth = new Auth();
            this.Close();
            auth.Show();
        }
    }
}
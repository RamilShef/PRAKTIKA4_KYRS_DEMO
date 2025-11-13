using PRAKTIKA4_KYRS_DEMO.BD;
using PRAKTIKA4_KYRS_DEMO.BD.MODELS;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace PRAKTIKA4_KYRS_DEMO
{
    /// <summary>
    /// Логика взаимодействия для Moderatop.xaml
    /// </summary>
    public partial class Moderatop : Window
    {
        Praktika2222Entities db = new Praktika2222Entities();
        private List<EventViewModel> allEvents;
        public Moderatop()
        {
            InitializeComponent();
            LoadEvents();
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
                string imageDir = System.IO.Path.Combine(projectPath, "Image");

                string[] extensions = { ".jpg", ".png", ".jpeg", ".bmp", ".gif" };

                foreach (string ext in extensions)
                {
                    string fullPath = System.IO.Path.Combine(imageDir, $"{eventId}{ext}");
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



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OrganizerWindow organizerWindow = new OrganizerWindow();
            this.Close();
            organizerWindow.Show();
        }
    }
}

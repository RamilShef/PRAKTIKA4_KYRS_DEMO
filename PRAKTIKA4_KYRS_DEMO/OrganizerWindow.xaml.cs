using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using PRAKTIKA4_KYRS_DEMO.BD;
using PRAKTIKA4_KYRS_DEMO.BD.MODELS;

namespace PRAKTIKA4_KYRS_DEMO
{
    public partial class OrganizerWindow : Window
    {
        private readonly Praktika2222Entities db = new Praktika2222Entities();

        public OrganizerWindow()
        {
            InitializeComponent();
            InitializeUserData();
            LoadUserPhoto();
        }

        private void InitializeUserData()
        {
            string greeting = GetTimeBasedGreeting();
            string userName = "Организатор";

            if (Session.IsLoggedIn)
                userName = GetUserName(Session.CurrentUser.FIO);

            UpdateGreetingText(greeting, userName);
        }

        private void LoadUserPhoto()
        {
            try
            {
                // Стандартное фото по умолчанию
                string defaultPath = @"C:\Users\ssefy\Desktop\PRAKTIKA4_KYRS_DEMO\PRAKTIKA4_KYRS_DEMO\Image\._11.jpg";
                string userImagePath = defaultPath;

                if (Session.IsLoggedIn && !string.IsNullOrWhiteSpace(Session.CurrentUser.Image))
                {
                    // Если в БД указано имя файла или путь — пытаемся использовать его
                    string possiblePath = Session.CurrentUser.Image;

                    if (!Path.IsPathRooted(possiblePath))
                    {
                        string projectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "Organizator");
                        possiblePath = Path.Combine(projectPath, Path.GetFileName(Session.CurrentUser.Image));
                    }

                    // Если файл существует — используем его
                    if (File.Exists(possiblePath))
                        userImagePath = possiblePath;
                }

                // Если файл существует — устанавливаем картинку
                if (File.Exists(userImagePath))
                {
                    if (Session.IsLoggedIn && !string.IsNullOrWhiteSpace(Session.CurrentUser.Image)) ;
                }
                else
                {
                    // На случай, если даже стандартное фото не найдено
                    MessageBox.Show($"Фото не найдено: {userImagePath}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке фото: {ex.Message}");
            }
        }


        private string GetTimeBasedGreeting()
        {
            int hour = DateTime.Now.Hour;

            if (hour >= 9 && hour <= 11)
                return "Доброе утро";
            else if (hour >= 11 && hour <= 18)
                return "Добрый день";
            else if (hour >= 18 && hour <= 24)
                return "Добрый вечер";
            else
                return "Добро пожаловать";
        }

        private string GetUserName(string fullName)
        {
            if (!string.IsNullOrEmpty(fullName))
            {
                string[] nameParts = fullName.Split(' ');
                if (nameParts.Length > 0)
                    return nameParts[0];
            }
            return "Пользователь";
        }

        private void UpdateGreetingText(string greeting, string userName)
        {
            txtGreeting.Text = greeting;
            txtUserName.Text = userName;
        }

        private void Event_Click(object sender, RoutedEventArgs e)
        {
            Moderatop window = new Moderatop();
            this.Close();
            window.Show();
        }

        private void Player_Click(object sender, RoutedEventArgs e)
        {
            Ychastniki window = new Ychastniki();
            this.Close();
            window.Show();
        }

        private void Juri_Click(object sender, RoutedEventArgs e)
        {
            RegistrationJuryModeratorWindow w = new RegistrationJuryModeratorWindow();
            this.Close();
            w.Show();
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            Profil profil = new Profil();
            this.Close();
            profil.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Session.Logout(); // выход
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }
    }
}

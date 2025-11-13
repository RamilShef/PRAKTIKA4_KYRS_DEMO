using PRAKTIKA4_KYRS_DEMO.BD;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PRAKTIKA4_KYRS_DEMO
{
    public partial class Auth : Window
    {
        int failedAttempts = 0;
        string currentCaptcha = "";
        bool isLocked = false;
        DateTime unlockTime;

        private readonly string credentialsFilePath =
            @"C:\Users\ssefy\Desktop\PRAKTIKA4_KYRS_DEMO\PRAKTIKA4_KYRS_DEMO\hh\saved_credentials.txt";

        public Auth()
        {
            InitializeComponent();
            LoadSavedCredentials();
        }

        private void LoadSavedCredentials()
        {
            try
            {
                if (File.Exists(credentialsFilePath))
                {
                    string[] lines = File.ReadAllLines(credentialsFilePath);
                    if (lines.Length >= 2)
                    {
                        txtIdNumber.Text = lines[0];
                        txtPassword.Password = lines[1];
                        chkRememberMe.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке сохраненных данных: {ex.Message}");
            }
        }

        private void SaveCredentials(string userId, string password)
        {
            try
            {
                string directory = Path.GetDirectoryName(credentialsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (chkRememberMe.IsChecked == true)
                {
                    File.WriteAllLines(credentialsFilePath, new string[] { userId, password });
                    //MessageBox.Show($"Данные сохранены в файл:\n{credentialsFilePath}");
                }
                else
                {
                    if (File.Exists(credentialsFilePath))
                    {
                        File.Delete(credentialsFilePath);
                        MessageBox.Show("Сохраненные данные удалены.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}\n\nПуть: {credentialsFilePath}");
            }
        }

        private string GenerateCaptcha()
        {
            var chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void ShowCaptcha()
        {
            currentCaptcha = GenerateCaptcha();
            captchaText.Text = currentCaptcha;
            captchaInput.Text = "";
            captchaPanel.Visibility = Visibility.Visible;
        }

        private bool CheckCaptcha()
        {
            return captchaInput.Text.Trim().Equals(currentCaptcha, StringComparison.OrdinalIgnoreCase);
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            await PerformLogin(false);
        }

        private async Task PerformLogin(bool isAutoLogin)
        {
            if (isLocked)
            {
                if (DateTime.Now < unlockTime)
                {
                    var secondsLeft = (int)(unlockTime - DateTime.Now).TotalSeconds;
                    MessageBox.Show($"Вы заблокированы! Подождите {secondsLeft} секунд.");
                    return;
                }
                else
                {
                    isLocked = false;
                    failedAttempts = 0;
                }
            }

            string id = txtIdNumber.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                if (!isAutoLogin)
                    MessageBox.Show("Введите ID и пароль!");
                return;
            }

            if (captchaPanel.Visibility == Visibility.Visible)
            {
                if (!CheckCaptcha())
                {
                    MessageBox.Show("Капча введена неверно!");
                    failedAttempts++;
                    if (failedAttempts >= 5)
                    {
                        await LockUser(10);
                    }
                    else
                    {
                        ShowCaptcha();
                    }
                    return;
                }
            }

            try
            {
                if (!int.TryParse(id, out int userId))
                {
                    if (!isAutoLogin)
                        MessageBox.Show("ID должен быть числом!");
                    return;
                }

                var db = Praktika2222Entities.Getcontext();
                if (db == null)
                {
                    MessageBox.Show("Ошибка подключения к базе данных");
                    return;
                }

                var user = db.User.FirstOrDefault(u => u.ID == userId && u.Password == password);

                if (user != null)
                {
                    Session.CurrentUser = user;
                    if (!isAutoLogin)
                        MessageBox.Show("Вход успешен!");

                    failedAttempts = 0;
                    captchaPanel.Visibility = Visibility.Collapsed;

                    SaveCredentials(id, password);

                
                    OpenUserWindow(user);
                }
                else
                {
                    failedAttempts++;
                    if (!isAutoLogin)
                        MessageBox.Show("Неверный логин или пароль!");

                    if (failedAttempts >= 3)
                    {
                        ShowCaptcha();
                        if (!isAutoLogin)
                            MessageBox.Show("Введите капчу для продолжения.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task LockUser(int seconds)
        {
            isLocked = true;
            unlockTime = DateTime.Now.AddSeconds(seconds);
            MessageBox.Show($"Слишком много попыток! Вы заблокированы на {seconds} секунд.");

            await Task.Delay(seconds * 1000);

            isLocked = false;
            failedAttempts = 0;
            captchaPanel.Visibility = Visibility.Collapsed;
        }

        private void OpenUserWindow(User user)
        {
            try
            {
                var db = Praktika2222Entities.Getcontext();
                var userRoles = (from r in db.Role
                                 where r.User.Any(u => u.ID == user.ID)
                                 select r).ToList();

                Window windowToOpen = new emptiness(); // по умолчанию

                if (userRoles.Count > 0)
                {
                    var firstRole = userRoles.First();

                    switch (firstRole.ID)
                    {
                        case 1: // Участник
                            windowToOpen = new emptiness();
                            break;
                        case 2: // Модератор
                            windowToOpen = new emptiness();
                            break;
                        case 3: // Организатор
                            windowToOpen = new OrganizerWindow();
                            break;
                    }
                }

                windowToOpen.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при открытии окна: {ex.Message}");
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выходе: {ex.Message}");
            }
        }

        private void captchaText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowCaptcha();
        }

        private void RefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            ShowCaptcha();
        }
    }
}
//только один сохраненный и его постоянно выкидывает 
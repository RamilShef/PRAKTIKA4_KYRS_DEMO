using PRAKTIKA4_KYRS_DEMO.BD;
using PRAKTIKA4_KYRS_DEMO.BD.MODELS;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Windows.Controls;

namespace PRAKTIKA4_KYRS_DEMO
{
    public partial class RegistrationJuryModeratorWindow : Window
    {
        private string photoPath = ""; // путь к выбранному фото

        public RegistrationJuryModeratorWindow()
        {
            InitializeComponent();
            SetNextUserIDToTextBox();
            LoadRoles();
        }

        // Получить следующий ID
        private int GetNextUserID()
        {
            try
            {
                using (var db = new Praktika2222Entities())
                {
                    return db.User.Any() ? db.User.Max(u => u.ID) + 1 : 1;
                }
            }
            catch
            {
                return 1;
            }
        }

        private void SetNextUserIDToTextBox()
        {
            txtID.Text = GetNextUserID().ToString();
        }

        // Загрузка списка ролей
        private void LoadRoles()
        {
            try
            {
                using (var db = new Praktika2222Entities())
                {
                    var roles = db.Role.ToList();
                    cmbRole.ItemsSource = roles;
                    cmbRole.DisplayMemberPath = "RoleName";
                    cmbRole.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ролей: {ex.Message}");
            }
        }

        // Загрузка фото
        private void BtnUploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFile.ShowDialog() == true)
            {
                photoPath = openFile.FileName;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(photoPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgPhoto.Source = bitmap;
            }
        }

        // Кнопка "Отмена"
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Кнопка "OK" — добавить пользователя
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!ValidateData()) return;

            try
            {
                using (var db = new Praktika2222Entities())
                {
                    string savedImagePath = "";

                    // 📂 Путь к фото (оставь пустым — вставь свой позже)
                    string projectImagesDir = @"C:\Users\ssefy\Desktop\PRAKTIKA4_KYRS_DEMO\PRAKTIKA4_KYRS_DEMO\Image\";

                    if (!string.IsNullOrEmpty(photoPath))
                    {
                        if (!Directory.Exists(projectImagesDir))
                            Directory.CreateDirectory(projectImagesDir);

                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(photoPath)}";
                        string newFilePath = Path.Combine(projectImagesDir, fileName);
                        File.Copy(photoPath, newFilePath, true);
                        savedImagePath = fileName;
                    }

                    string password = pwdPassword.Visibility == Visibility.Visible
                        ? pwdPassword.Password
                        : txtPasswordVisible.Text;

                    // Определяем выбранный пол
                    var genderText = (cmbGender.SelectedItem as ComboBoxItem)?.Content.ToString();
                    var genderFromDb = db.Gender.FirstOrDefault(g => g.GenderName == genderText);

                    // Создаем пользователя
                    User newUser = new User
                    {
                        ID = int.Parse(txtID.Text),
                        FIO = txtFIO.Text,
                        Email = txtEmail.Text,
                        Phone = txtPhone.Text,
                        Image = savedImagePath,
                        Password = HashPassword(password)
                    };

                    // Привязка пола
                    // Привязка пола (для связи M:N)
                    if (genderFromDb != null)
                    {
                        newUser.Gender.Add(genderFromDb);
                    }


                    // Направление
                    if (!string.IsNullOrWhiteSpace(txtDirection.Text))
                    {
                        var dir = db.Direction.FirstOrDefault(d => d.DirectionName == txtDirection.Text);
                        if (dir == null)
                        {
                            dir = new Direction { DirectionName = txtDirection.Text };
                            db.Direction.Add(dir);
                            db.SaveChanges();
                        }
                        newUser.Direction.Add(dir);
                    }

                    // Роль (M:N)
                    var selectedRole = cmbRole.SelectedItem as Role;
                    if (selectedRole != null)
                    {
                        var roleInDb = db.Role.FirstOrDefault(r => r.ID == selectedRole.ID);
                        if (roleInDb != null)
                            newUser.Role.Add(roleInDb);
                    }

                    // Прикрепление к мероприятию (если включено)
                    if (chkAttachEvent.IsChecked == true && cmbEvent.SelectedItem is ComboBoxItem eventItem)
                    {
                        string eventName = eventItem.Content.ToString();
                        var ev = db.Event.FirstOrDefault(eve => eve.NameEvent == eventName);
                        if (ev != null)
                            newUser.Event.Add(ev);
                    }

                    db.User.Add(newUser);
                    db.SaveChanges();

                    MessageBox.Show("Пользователь успешно добавлен!");
                    new OrganizerWindow().Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // 🧩 Показываем реальную причину
                string inner = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"Ошибка при сохранении:\n{inner}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Проверка введённых данных
        private bool ValidateData()
        {
            string password = pwdPassword.Visibility == Visibility.Visible ? pwdPassword.Password : txtPasswordVisible.Text;
            string confirm = pwdConfirm.Visibility == Visibility.Visible ? pwdConfirm.Password : txtConfirmVisible.Text;

            if (string.IsNullOrWhiteSpace(txtFIO.Text))
            {
                MessageBox.Show("Введите ФИО");
                return false;
            }

            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Введите корректный Email");
                return false;
            }

            if (!Regex.IsMatch(txtPhone.Text, @"^\+7\d{10}$"))
            {
                MessageBox.Show("Введите телефон в формате +7XXXXXXXXXX");
                return false;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен быть не менее 6 символов");
                return false;
            }

            if (password != confirm)
            {
                MessageBox.Show("Пароли не совпадают");
                return false;
            }

            return true;
        }

        // Показ / скрытие пароля
        private void chkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            txtPasswordVisible.Text = pwdPassword.Password;
            txtConfirmVisible.Text = pwdConfirm.Password;
            pwdPassword.Visibility = Visibility.Collapsed;
            pwdConfirm.Visibility = Visibility.Collapsed;
            txtPasswordVisible.Visibility = Visibility.Visible;
            txtConfirmVisible.Visibility = Visibility.Visible;
        }

        private void chkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            pwdPassword.Password = txtPasswordVisible.Text;
            pwdConfirm.Password = txtConfirmVisible.Text;
            txtPasswordVisible.Visibility = Visibility.Collapsed;
            txtConfirmVisible.Visibility = Visibility.Collapsed;
            pwdPassword.Visibility = Visibility.Visible;
            pwdConfirm.Visibility = Visibility.Visible;
        }

        // Хеширование пароля
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        private void txtPhone_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtPhone.Text == "+7(___)___-__-__")
            {
                txtPhone.Text = "+7";
                txtPhone.CaretIndex = txtPhone.Text.Length;
            }
        }

        private void txtPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPhone.Text) || txtPhone.Text == "+7")
            {
                txtPhone.Text = "+7(___)___-__-__";
            }
        }

    }
}

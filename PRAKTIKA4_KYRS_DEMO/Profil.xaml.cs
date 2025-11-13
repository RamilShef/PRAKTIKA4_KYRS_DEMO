using PRAKTIKA4_KYRS_DEMO.BD;
using PRAKTIKA4_KYRS_DEMO.BD.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PRAKTIKA4_KYRS_DEMO
{
    public partial class Profil : Window
    {
        public Profil()
        {
            InitializeComponent();

            if (Session.CurrentUser != null)
            {
                LoadProfile(Session.CurrentUser);
            }
            else
            {
                MessageBox.Show("Ошибка: пользователь не найден. Пожалуйста, войдите снова.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void LoadProfile(User user)
        {
            string roles = user.Role != null && user.Role.Any()
                ? string.Join(", ", user.Role.Select(r => r.RoleName))
                : "Нет ролей";

            var profileData = new List<ProfileField>
            {
                new ProfileField { FieldName = "ID пользователя", FieldValue = user.ID.ToString() },
                new ProfileField { FieldName = "ФИО", FieldValue = user.FIO },
                new ProfileField { FieldName = "Email", FieldValue = user.Email },
                new ProfileField { FieldName = "Дата рождения", FieldValue = user.DOB.ToString("dd.MM.yyyy") },
                new ProfileField { FieldName = "Телефон", FieldValue = user.Phone },
                new ProfileField { FieldName = "Роли", FieldValue = roles },
            };

            profileDataGrid.ItemsSource = profileData;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OrganizerWindow organizerWindow = new OrganizerWindow();
            this.Close();
            organizerWindow.Show();
        }
    }
}

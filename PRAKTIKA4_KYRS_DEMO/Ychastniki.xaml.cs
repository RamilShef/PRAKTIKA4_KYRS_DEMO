using PRAKTIKA4_KYRS_DEMO.BD.MODELS;
using PRAKTIKA4_KYRS_DEMO.BD;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для Ychastniki.xaml
    /// </summary>
    public partial class Ychastniki : Window
    {
        public Ychastniki()
        {
            InitializeComponent();
            LoadPlayers();
        }
        private void LoadPlayers()
        {
            try
            {

                using (var db = new Praktika2222Entities())
                {
                    var players = (from u in db.User
                                   where u.Role.Any(r => r.ID == 1)
                                   select u).ToList();


                    var playerViewModels = players.Select(p => new PlayerViewModel
                    {
                        ID = p.ID,
                        FIO = p.FIO,
                        Email = p.Email,
                        Phone = p.Phone,
                        BirthDate = $"Дата рождения: {p.DOB:dd.MM.yyyy}"
                    }).ToList();

                    PlayersList.ItemsSource = playerViewModels;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке участников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OrganizerWindow organizerWindow = new OrganizerWindow();
            this.Close();
            organizerWindow.Show();
        }
    }
}

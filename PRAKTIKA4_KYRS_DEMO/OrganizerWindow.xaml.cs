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
using PRAKTIKA4_KYRS_DEMO.BD;
using PRAKTIKA4_KYRS_DEMO.BD.MODELS;

namespace PRAKTIKA4_KYRS_DEMO
{
    /// <summary>
    /// Логика взаимодействия для OrganizerWindow.xaml
    /// </summary>
    public partial class OrganizerWindow : Window
    {
       // private User currentUser;
        public OrganizerWindow()
        {
            InitializeComponent();
            //currentUser = user;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Moderatop moder = new Moderatop();
            this.Close();
            moder.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Ychastniki moder = new Ychastniki();
            this.Close();
            moder.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            RegistrationJuryModeratorWindow moder = new RegistrationJuryModeratorWindow();
            this.Close();
            moder.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Profil moder = new Profil();
            this.Close();
            moder.Show();
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MainWindow moder = new MainWindow();
            this.Close();
            moder.Show();
        }
    }
    
}

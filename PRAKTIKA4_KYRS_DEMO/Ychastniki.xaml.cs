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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OrganizerWindow organizerWindow = new OrganizerWindow();
            this.Close();
            organizerWindow.Show();
        }
    }
}

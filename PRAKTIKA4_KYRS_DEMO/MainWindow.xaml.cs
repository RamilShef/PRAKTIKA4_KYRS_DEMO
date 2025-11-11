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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PRAKTIKA4_KYRS_DEMO
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            cmbDateFilter.SelectionChanged += CmbDateFilter_SelectionChanged;
        }

        private void CmbDateFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDateFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedFilter = selectedItem.Content.ToString();
                dpSpecificDate.IsEnabled = selectedFilter == "Конкретная дата"; 
            }
        }

        private void Voiti_Click(object sender, RoutedEventArgs e)
        {
            
            Auth auth = new Auth();
            this.Close();
            auth.ShowDialog();
          
        }
    }
}

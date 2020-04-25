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

namespace Biblioteka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Refresh();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddItem AddI = new AddItem();
            AddI.FileOpen();
            Refresh();
        }

        public void Refresh()
        {
            using (var db = new LibraryContext())
            {
                DG.DataContext = db.Songs.ToList<Song>();
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            //Song model = new Song();
            MessageBoxResult dR = MessageBox.Show("Delete EVERYTHING?", "Confirm", MessageBoxButton.YesNo);
            if (MessageBoxResult.Yes == dR)
            {
                using (var db = new LibraryContext())
                {
                    db.Songs.RemoveRange(db.Songs);
                    db.SaveChanges();
                    Refresh();
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Song model = (Song)DG.SelectedItem;
            if (model != null)
            {
                MessageBoxResult dR = MessageBox.Show("Delete \"" + model.Title + "\"?", "Confirm", MessageBoxButton.YesNo);
                if (MessageBoxResult.Yes == dR)
                {
                    using (var db = new LibraryContext())
                    {
                        var entry = db.Entry(model);
                        if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Detached)
                            db.Songs.Attach(model);
                        db.Songs.Remove(model);
                        db.SaveChanges();
                        Refresh();
                    }
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Reflection;
using Vlc.DotNet.Wpf;

namespace Biblioteka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DirectoryInfo vlcLibDirectory;
        private VlcControl control;
        private string currentlyPlaying = null;

        /// <summary>
        /// Skanuje bazę i odświeża widok, inicializuje vlc
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            progBar.IsIndeterminate = true;
            Scan();
            Refresh();

            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            progBar.IsIndeterminate = false;
        }

        /// <summary>
        /// Skanuje pliki z bazy danych i jeśli plik nie istnieje na dysku to zostaje usunięty z bazy danych
        /// </summary>
        private void Scan()
        {
            using var db = new LibraryContext();
            foreach (var item in db.Songs)
            {
                if (File.Exists(item.Location) == false)
                {
                    item.Source = null;
                    Delete(item);
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Usuwa plik z bazy danych i jeśli źródłem pliku jest YouTube to usuwa plik z dysku
        /// </summary>
        /// <param name="item">element bazy danych</param>
        /// <returns>Informuje czy pomyślnie usunięto plik</returns>
        private bool Delete (Song item)
        {
            using var db = new LibraryContext();
            var entry = db.Entry(item);
            if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Detached)
                db.Songs.Attach(item);
            if (item.Source == "YT")
            {
                try { File.Delete(item.Location); }
                catch { return false; }
            }
            db.Songs.Remove(item);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Odświeża DataGrid
        /// </summary>
        public void Refresh()
        {
            using var db = new LibraryContext();
            DG.DataContext = db.Songs.ToList<Song>();
        }

        /// <summary>
        /// Wczytuje plik i go odtwarza
        /// </summary>
        /// <param name="item">element bazy danych</param>
        private void Play(Song item)
        {
            this.control?.Dispose();
            this.control = new VlcControl();
            this.control.SourceProvider.CreatePlayer(this.vlcLibDirectory);

            this.control.SourceProvider.MediaPlayer.Log += (_, args) =>
            {
                string message = $"libVlc : {args.Level} {args.Message} @ {args.Module}";
                System.Diagnostics.Debug.WriteLine(message);
            };

            this.currentlyPlaying = item.Location;
            control.SourceProvider.MediaPlayer.Play(new FileInfo(currentlyPlaying));
            labTitle.Content = item.Title;
            labAuthor.Content = item.Author;
        }
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddItem AddI = new AddItem();
            AddI.FileOpen();
            Refresh();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            //Song model = new Song();
            MessageBoxResult dR = MessageBox.Show("Delete EVERYTHING?", "Confirm", MessageBoxButton.YesNo);
            if (MessageBoxResult.Yes == dR)
            {
                using var db = new LibraryContext();
                db.Songs.RemoveRange(db.Songs);
                db.SaveChanges();
                Refresh();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Song item = (Song)DG.SelectedItem;
            if (item != null)
            {
                MessageBoxResult dR = MessageBox.Show("Delete \"" + item.Title + "\"?", "Confirm", MessageBoxButton.YesNo);
                if (MessageBoxResult.Yes == dR)
                {
                    if (Delete(item) == false) MessageBox.Show("Error deleting file");
                    Refresh();
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DG_Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            Song item = (Song)DG.SelectedItem;
            Play(item);
            this.btnPlayPause.Background = (Brush)FindResource("Pause");
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            Song item = (Song)DG.SelectedItem;
            if (this.control != null && control.SourceProvider.MediaPlayer.IsPlaying()) 
            { 
                control.SourceProvider.MediaPlayer.Pause();
                this.btnPlayPause.Background = (Brush)FindResource("Play");
            }
            else if (this.control != null && control.SourceProvider.MediaPlayer.IsPlaying() != true)
            {
                control.SourceProvider.MediaPlayer.Play();
                this.btnPlayPause.Background = (Brush)FindResource("Pause");
            }
            else if (item != null)
            {
                Play(item);
                this.btnPlayPause.Background = (Brush)FindResource("Pause");
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            this.control?.Dispose();
            this.control = null;
        }

        private void BtnYouTube_Click(object sender, RoutedEventArgs e)
        {
            var yt = new YouTubeSearch(this)
            {
                DataContext = this
            };
            try
            {
                yt.Show();
                Refresh();
            }
            catch { }
        }
    }
}

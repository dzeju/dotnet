using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Biblioteka
{
    /// <summary>
    /// Interaction logic for YouTubeSearch.xaml
    /// </summary>
    public partial class YouTubeSearch : Window
    {
        string loc = null;

        /// <summary>
        /// klasa głównego okna do zarządzania nim w innym oknie
        /// </summary>
        public MainWindow MW { get; }

        /// <summary>
        /// Wywołanie okna wyszukiwania filmów na YT
        /// </summary>
        /// <param name="m">zawartość głównego okna</param>
        public YouTubeSearch(MainWindow m)
        {
            InitializeComponent();
            MW = m;
        }

        /// <summary>
        /// Dodaje plik do bazy danych i wywołuje odświeżenie widoku DataGrid w głównym oknie
        /// </summary>
        /// <param name="vid">element do dodania</param>
        private void AddYTItem(Video vid)
        {
            using var db = new LibraryContext();
            db.Add(
            new Song
            {
                Title = vid.Title,
                Author = vid.Author,
                Album = null,
                Location = loc,
                Source = "YT"
            });
            db.SaveChanges();
        }

        /// <summary>
        /// Wyszukuje filmy z YouTube na podstawie podanej nazwy i wypełnia nimi DataGrid filmów
        /// </summary>
        /// <param name="txtSearch">nazwa filmu do wyszukania</param>
        public async Task<List<Video>> Search(string txtSearch)
        {
            var items = new YoutubeClient();
            List<Video> list = new List<Video>();

            var videos = await items.Search.GetVideosAsync(txtSearch).BufferAsync(15);
            foreach (var item in videos)
            {
                Video vid = new Video();
                vid.Title = item.Title;
                vid.Author = item.Author;
                vid.Url = item.Url;
                vid.Duration = item.Duration.ToString();
                list.Add(vid);
            }
            return list;
        }

        /// <summary>
        /// Pobiera audio z wybranego filmu
        /// </summary>
        /// <param name="url">link wybranego filmu</param>
        /// <param name="name">tytuł filmu</param>
        /// <returns></returns>
        public async Task Download(string url, string name)
        {
            var youtube = new YoutubeClient();

            var streams = await youtube.Videos.Streams.GetManifestAsync(url);

            var streamInfo = streams.GetAudioOnly().WithHighestBitrate();

            if (streamInfo == null)
            {
                /* MessageBox.Show("Error downloading an audio");
                 return;*/
                throw new System.Exception();
            }
            else
            {
                loc = $"Download/{name}.{streamInfo.Container}";
                await youtube.Videos.Streams.DownloadAsync(streamInfo, loc);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            progBar.IsIndeterminate = true;
            try
            {
                List<Video> list = await Search(txtSearch.Text);
                ytDG.DataContext = list;
            }
            catch { MessageBox.Show("Error accured during search"); }
            progBar.IsIndeterminate = false;
        }

        private async void YtDG_Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {

            Video vid = (Video)ytDG.SelectedItem;
            progBar.IsIndeterminate = true;
            try
            {
                await Download(vid.Url, vid.Title);
                AddYTItem(vid);
            }
            catch
            {
                MessageBox.Show("Error downloading an audio");
                return;
            }
            MW.Refresh();
            progBar.IsIndeterminate = false;
            MessageBox.Show("Added");
        }
    }
}

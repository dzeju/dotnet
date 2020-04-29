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

        public MainWindow MW { get; }

        public YouTubeSearch(MainWindow m)
        {
            InitializeComponent();
            MW = m;
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            progBar.IsIndeterminate = true;
            await Search(txtSearch.Text);
            progBar.IsIndeterminate = false;
        }

        private async void YtDG_Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            Video vid = (Video)ytDG.SelectedItem;
            progBar.IsIndeterminate = true;
            try { await AddYTItemAsync(vid); }
            catch { MessageBox.Show("Error downloading an audio"); }
            progBar.IsIndeterminate = false;
            MessageBox.Show("Added");
        }

        private async Task AddYTItemAsync(Video vid)
        {
            await Download(vid.Url, vid.Title);

            using (var db = new LibraryContext())
            {
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
            MW.Refresh();
        }

        public async Task Search(string txtSearch)
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
            this.ytDG.DataContext = list;
        }

        public async Task Download(string url, string name)
        {
            var youtube = new YoutubeClient();

            var streams = await youtube.Videos.Streams.GetManifestAsync(url);

            var streamInfo = streams.GetAudioOnly().WithHighestBitrate();

            if (streamInfo == null)
            {
                MessageBox.Show("Error downloading an audio");
                return;
            }
            else
            {
                loc = $"Download/{name}.{streamInfo.Container}";
                await youtube.Videos.Streams.DownloadAsync(streamInfo, loc);
            }
        }
    }
}

namespace Biblioteka
{
    /// <summary>
    /// klasa video potrzebna do wyświetlenia w datagrid oraz dodania wybranej piosenki
    /// </summary>
    public class Video
    {
        /// <summary>
        /// tytuł
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// autor
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// url filmu
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// czas trwania
        /// </summary>
        public string Duration { get; set; }
    }
}

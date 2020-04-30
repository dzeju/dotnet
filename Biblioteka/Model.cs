using Microsoft.EntityFrameworkCore;

namespace Biblioteka
{
    /// <summary>
    /// Pozwala na wymianę danych pomiędzy aplikacją a plikiem
    /// </summary>
    public class LibraryContext : DbContext
    {
        /// <summary>
        /// Zarządzanie elementami bazy
        /// </summary>
        public DbSet<Song> Songs { get; set; }

        /// <summary>
        /// ustawienie pliku bazy danych
        /// </summary>
        /// <param name="options">opcje</param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=library.db");
    }

    /// <summary>
    /// Struktura elementu bazy danych
    /// </summary>
    public class Song
    {
        /// <summary>
        /// id elementu
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// tutuł
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// autor
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// album
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// ścieżka do pliku
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// źródło
        /// </summary>
        public string Source { get; set; }
    }
}

using System;
using System.Windows.Forms;

namespace Biblioteka
{
    class AddItem
    {
        /// <summary>
        /// Otwiera okno wyboru pliku z którego można wybrać utwór
        /// </summary>
        public void FileOpen()
        {
            var fileDialog = new OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch(result)
            {
                case DialogResult.OK:
                    string file = fileDialog.FileName;
                    try
                    {
                        AddPCItem(file);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("No data in a file :/");
                        break;
                    }
                    break;
                case DialogResult.Cancel:
                default:
                    break;
            }
        }

        /// <summary>
        /// Czyta plik i jeśli może z niego wyczytać dane dodaje do bazy
        /// </summary>
        /// <param name="file"></param>
        private void AddPCItem(string file)
        {
            TagLib.File f = TagLib.File.Create(file);                            //exceptions?
            if (f.Tag.Title == "" || f.Tag.Title == null) throw new Exception();  //TEMPORARY
            using var db = new LibraryContext();
            db.Add(
                new Song
                {
                    Title = f.Tag.Title,
                    Author = f.Tag.FirstPerformer,
                    Album = f.Tag.Album,
                    Location = file,
                    Source = "PC"
                });
            db.SaveChanges();
        }

    }
}

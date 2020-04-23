using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace Biblioteka
{
    class AddItem
    {
        public void FileOpen()
        {
            var fileDialog = new OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch(result)
            {
                case DialogResult.OK:
                    string file = fileDialog.FileName;
                    DataRead(file);
                    break;
                case DialogResult.Cancel:
                default:
                    break;
            }
        }

        private void DataRead(string file)
        {
            TagLib.File f = TagLib.File.Create(file);
            //MessageBox.Show(f.Tag.Title);
            using (var db = new LibraryContext())
            {
                try
                {
                    var song = db.Songs
                    .OrderBy(b => b.Id)
                    .First();
                }
                catch { }//something?
                db.Add(
                    new Song
                    {
                        Title = f.Tag.Title,
                        Author = f.Tag.FirstPerformer,
                        Album = f.Tag.Album,
                        Location = file
                    });
                db.SaveChanges();
            }
        }

    }
}

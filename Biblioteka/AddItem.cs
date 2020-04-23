using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

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
            MessageBox.Show(f.Tag.Title);
            
        }

    }
}

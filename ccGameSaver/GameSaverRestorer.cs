using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace ccGameSaver
{
    public class GameSaverRestorer
    {
        static readonly string AppDataPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString();

        public readonly string saveGamePath = Path.Combine(
            AppDataPath,
            "LocalLow\\AwesomeLand\\CannibalCrossing"
            );
        public readonly string saveGameBackupsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "ccGameSaver"
            );
        bool loaded_local_saves = false;
        List<string> my_local_saves = new List<string>();
        public int preserve_autosave_count { get; set; } = 5;
        public GameSaverRestorer()
        {
        }
        private void ClearPrevAutosaves(int preserve_count = 5)
        {
            if (!loaded_local_saves)
            {
                LoadLocalSaves();
                loaded_local_saves = true;
            }
            var my_local_autosaves = my_local_saves
                .Where(save => save.Substring(0, save.LastIndexOf("-")) == "autosave")
                .OrderByDescending(save => save.Substring(save.LastIndexOf("-") + 1));
            var saves_to_clear = my_local_autosaves.Skip(preserve_count).Take(my_local_autosaves.Count() - preserve_count);
            var preserved_local_autosaves = my_local_autosaves.Take(preserve_count);
            foreach (var save in saves_to_clear)
            {
                var save_full_path = Path.Combine(saveGameBackupsPath, save);
                try
                {
                    Directory.Delete(save_full_path, true);
                }
                catch (DirectoryNotFoundException)
                {
                    break;
                }
                finally
                {
                    my_local_saves.Remove(save);
                }
            }
        }
        public string SaveGame(string save_name_prefix)
        {
            string save_name = save_name_prefix + "-" + DateTime.Now.ToString("yyyyMMddTHHmmss");
            string save_path = Path.Combine(saveGameBackupsPath, save_name);
            try
            {
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(saveGamePath, save_path);
            }
            catch (DirectoryNotFoundException)
            {

                return "No Save Found";
            }
            my_local_saves.Add(save_name);
            ClearPrevAutosaves(preserve_autosave_count);
            return save_name;
        }
        private string RestoreGame(string save_name)
        {
            bool isRunning = Process.GetProcessesByName("CannibalCrossing").Any();
            if(isRunning)
            {
                MessageBox.Show("Close Cannibal Crossing First");
                return save_name;
            }
            try
            {
                Directory.Delete(saveGamePath, true);
            }
            catch (DirectoryNotFoundException)
            {
            }
            string save_path = Path.Combine(saveGameBackupsPath, save_name);
            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(save_path, saveGamePath);
            return save_name;
        }
        public void BackupThenRestoreGame(string save_restore_folder_name)
        {
            //backup current save first
            SaveGame("prerestore");
            //restore a save
            RestoreGame(save_restore_folder_name);
        }
        private void LoadLocalSaves()
        {
            var di = new DirectoryInfo(saveGameBackupsPath);
            try
            {
               my_local_saves.AddRange(
                     di.GetDirectories()
                     .Select(dir_info => dir_info.Name)
                     .ToList());
            }
            catch (DirectoryNotFoundException) // no local saves exist
            {
            }
        }
        public List<string> GetLocalSaveNames()
        {
            if(!loaded_local_saves)
            {
                LoadLocalSaves();
                loaded_local_saves = true;
            }
            my_local_saves = my_local_saves.OrderByDescending
                (
                f_name => f_name.Substring(f_name.LastIndexOf('-') + 1)
                ).ToList();
            return my_local_saves;
        }
    }
}

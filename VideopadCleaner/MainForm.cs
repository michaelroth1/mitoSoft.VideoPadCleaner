using System.Security.Policy;
using System.Web;
using VideopadCleaner.Extensions;

namespace mitoSoft.VideopadCleaner
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.LoadSettings();

            this.toolStripStatusLabel.Text = String.Empty;

            if (string.IsNullOrEmpty(this.textBoxUnusedFiles.Text))
            {
                this.textBoxUnusedFiles.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "VideopadCleaner");
            }
        }

        //Search Videopad file
        private void SearchVideopadFile_Click(object sender, EventArgs e)
        {
            this.openFileDialog.Filter = "*.vpj|";
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var file = new FileInfo(this.openFileDialog.FileName);
                this.textBoxVideopadFile.Text = file.FullName;

                if (string.IsNullOrEmpty(this.textBoxSearchFolder.Text))
                {
                    this.textBoxSearchFolder.Text = file.DirectoryName;
                }
            }
            this.openFileDialog.Filter = string.Empty;
        }

        //Search Folder
        private void SearchFolder_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBoxSearchFolder.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        //Search Unused Folder
        private void SearchUnusedFolder_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBoxSearchFolder.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        //OK
        private void Ok_Click(object sender, EventArgs e)
        {
            var vpfile = this.textBoxVideopadFile.Text.CheckFileExist();
            var searchFolder = this.textBoxSearchFolder.Text.CheckFolderExist();
            var unusedFilesFolder = this.textBoxUnusedFiles.Text.CheckFolderExist(true);

            this.SaveSettings();

            var lines = LoadFile(vpfile);

            var unUsedClips = new SortedDictionary<int, string>();
            bool readClips = false;
            bool readSeq = false;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.StartsWith("clips="))
                {
                    readClips = true;
                }
                else if (line.StartsWith("tracks="))
                {
                    readClips = false;
                }
                else if (line.StartsWith("trackclips="))
                {
                    readSeq = true;
                }
                else if (line.StartsWith("subtitletracks="))
                {
                    readSeq = false;
                }

                if (readClips && line.Contains("&path="))
                {
                    var clipName = line.Between("h=", "&");
                    var clipPpath = line.Between("path=", "&");
                    clipPpath = HttpUtility.UrlDecode(clipPpath);

                    unUsedClips.Add(int.Parse(clipName), clipPpath);
                }

                //Delete clips which are not in sequences
                if (readSeq && line.Contains("h="))
                {
                    var clipName = line.Between("horiginalclip=", "&");

                    if (unUsedClips.ContainsKey(int.Parse(clipName)))
                    {
                        var path = unUsedClips[int.Parse(clipName)];

                        unUsedClips.RemoveByValue(path);
                    }
                }
            }

            //Move unused clips to a specific folder 
            var unused = 0;
            var used = 0;
            foreach (var file in new DirectoryInfo(searchFolder).GetFiles("*.mp4", SearchOption.AllDirectories))
            {
                if (unUsedClips.ContainsValue(file.FullName))
                {
                    System.IO.File.Move(file.FullName, Path.Combine(unusedFilesFolder, file.Name), true);
                    unused++;
                }
                else
                {
                    used++;
                }
            }

            this.toolStripStatusLabel.Text = $"Verwendete Dateien: {used}; verschobene Dateien: {unused}";
            MessageBox.Show(this.toolStripStatusLabel.Text);
        }

        private void LoadSettings()
        {
            this.textBoxVideopadFile.Text = Properties.Settings.Default.VideopadFile;
            this.textBoxUnusedFiles.Text = Properties.Settings.Default.UnusedVideos;
            this.textBoxSearchFolder.Text = Properties.Settings.Default.SearchFolder;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.VideopadFile = this.textBoxVideopadFile.Text;
            Properties.Settings.Default.UnusedVideos = this.textBoxUnusedFiles.Text;
            Properties.Settings.Default.SearchFolder = this.textBoxSearchFolder.Text;
            Properties.Settings.Default.Save();
        }

        private static List<string> LoadFile(string file)
        {
            var lines = System.IO.File.ReadAllLines(file);

            return lines.ToList();
        }
    }
}
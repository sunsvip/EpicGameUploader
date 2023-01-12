
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace EpicGameUploader
{
    public partial class Form1 : Form
    {
        private class SettingData
        {
            public string EpicBPTFile = "";
            public string EpicAppFile = "";
            public string IgnoreList = "";
            public string OrganizationId = "";
            public string ProductId = "";
            public string ArtifactId = "";
            public string BPTClientId = "";
            public string BPTClientSecret = "";
            public string CloudDir = "";
            public string BuildVersion = "1.0";
            public string AppArgs = "";
        }
        const string ConfigJson = "Config.json";
        const string IgnoreUploadFile = "ignore_upload_files.txt";
        SettingData settings;
        public Form1()
        {
            if (File.Exists(ConfigJson))
            {
                settings = LitJson.JsonMapper.ToObject<SettingData>(File.ReadAllText(ConfigJson));
            }
            if (settings == null)
            {
                settings = new SettingData();
            }
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveConfig();
        }

        private void SaveConfig()
        {
            File.WriteAllText(ConfigJson, LitJson.JsonMapper.ToJson(settings));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.tBoxUploadTool.Text = settings.EpicBPTFile;
            this.uploadAppTbox.Text = settings.EpicAppFile;
            this.tBoxIgnore.Text = settings.IgnoreList;
            this.tBoxBuildVersion.Text = settings.BuildVersion;
            this.tBoxOrganizationId.Text = settings.OrganizationId;
            this.tBoxProductId.Text = settings.ProductId;
            this.tBoxArtifactId.Text = settings.ArtifactId;
            this.tBoxBPTClientId.Text = settings.BPTClientId;
            this.tBoxBPTClientSecret.Text = settings.BPTClientSecret;
            this.tBoxCloudDir.Text = settings.CloudDir;
            this.tBoxAppArgs.Text = settings.AppArgs;
        }


        private void UploadGame()
        {
            var buildRoot = Path.GetDirectoryName(settings.EpicAppFile);
            StringBuilder strBuilder = new StringBuilder();
            //strBuilder.AppendFormat(settings.EpicBPTFile);
            strBuilder.AppendFormat(" -OrganizationId=\"{0}\"", settings.OrganizationId);
            strBuilder.AppendFormat(" -ProductId=\"{0}\"", settings.ProductId);
            strBuilder.AppendFormat(" -ArtifactId=\"{0}\"", settings.ArtifactId);
            strBuilder.AppendFormat(" -ClientId=\"{0}\"", settings.BPTClientId);
            strBuilder.AppendFormat(" -ClientSecret=\"{0}\"", settings.BPTClientSecret);
            strBuilder.Append(" -mode=\"UploadBinary\"");
            strBuilder.AppendFormat(" -BuildRoot={0}", buildRoot);
            strBuilder.AppendFormat(" -CloudDir={0}", settings.CloudDir);
            strBuilder.AppendFormat(" -BuildVersion=\"{0}\"", settings.BuildVersion);
            strBuilder.AppendFormat(" -AppLaunch=\"{0}\"", Path.GetFileName(settings.EpicAppFile));
            strBuilder.AppendFormat(" -AppArgs=\"{0}\"", settings.AppArgs);
            if (!string.IsNullOrWhiteSpace(settings.IgnoreList))
            {
                strBuilder.AppendFormat(" -FileIgnoreList=\"{0}\"", Path.Combine(buildRoot, IgnoreUploadFile));
            }

            System.Diagnostics.Process.Start(settings.EpicBPTFile, strBuilder.ToString()).WaitForExit();
        }
        private void RefreshIgnoreFile()
        {
            List<string> ignoreFileList = new List<string>();
            var ignoreList = settings.IgnoreList.Split('\n');
            var uploadDir = Path.GetDirectoryName(settings.EpicAppFile);
            foreach (var ignoreItem in ignoreList)
            {
                var item = ignoreItem.Trim();
                if (string.IsNullOrWhiteSpace(item)) continue;
                if (Path.HasExtension(item))
                {
                    var file = Path.Combine(uploadDir, item);
                    if (File.Exists(file))
                    {
                        var name = item;
                        if (!ignoreFileList.Contains(name))
                            ignoreFileList.Add(name);
                    }

                }
                else
                {
                    var path = Path.Combine(uploadDir, item);
                    if (Directory.Exists(path))
                    {
                        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            var name = file.Substring(uploadDir.Length + 1);
                            if (!ignoreFileList.Contains(name))
                                ignoreFileList.Add(name);
                        }
                    }
                }
            }
            var ignoreFile = Path.Combine(uploadDir, IgnoreUploadFile);
            ignoreFileList.Add(IgnoreUploadFile);
            File.WriteAllLines(ignoreFile, ignoreFileList);

            //this.ignoreListText.AppendText(LitJson.JsonMapper.ToJson(ignoreFileList));
        }


        private void tBoxUploadTool_TextChanged(object sender, EventArgs e)
        {
            settings.EpicBPTFile = this.tBoxUploadTool.Text;
        }

        private void uploadAppTbox_TextChanged(object sender, EventArgs e)
        {
            settings.EpicAppFile = this.uploadAppTbox.Text;
        }

        private void tBoxOrganizationId_TextChanged(object sender, EventArgs e)
        {
            settings.OrganizationId = this.tBoxOrganizationId.Text;
        }

        private void tBoxProductId_TextChanged(object sender, EventArgs e)
        {
            settings.ProductId = this.tBoxProductId.Text;
        }

        private void tBoxArtifactId_TextChanged(object sender, EventArgs e)
        {
            settings.ArtifactId = this.tBoxArtifactId.Text;
        }

        private void tBoxBPTClientId_TextChanged(object sender, EventArgs e)
        {
            settings.BPTClientId = this.tBoxBPTClientId.Text;
        }

        private void tBoxBPTClientSecret_TextChanged(object sender, EventArgs e)
        {
            settings.BPTClientSecret = this.tBoxBPTClientSecret.Text;
        }

        private void tBoxCloudDir_TextChanged(object sender, EventArgs e)
        {
            settings.CloudDir = this.tBoxCloudDir.Text;
        }

        private void tBoxBuildVersion_TextChanged(object sender, EventArgs e)
        {
            settings.BuildVersion = this.tBoxBuildVersion.Text;
        }

        private void tBoxAppArgs_TextChanged(object sender, EventArgs e)
        {
            settings.AppArgs = this.tBoxAppArgs.Text;
        }

        private void uploadToolSelectBt_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "可执行文件(*.exe)|*.exe";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.tBoxUploadTool.Text = dialog.FileName;
            }
        }

        private void tBoxIgnore_TextChanged(object sender, EventArgs e)
        {
            settings.IgnoreList = this.tBoxIgnore.Text;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://dev.epicgames.com/docs/epic-games-store/publishing-tools/store-presence/upload-binaries/bpt-instructions-150?sessionInvalidated=true";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void uploadAppSelectBt_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "可执行文件(*.exe)|*.exe";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.uploadAppTbox.Text = dialog.FileName;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void UploadBt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(settings.EpicBPTFile) || !File.Exists(settings.EpicBPTFile))
            {
                if (MessageBox.Show("Epic上传工具无效，请重新选择。", "错误", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(settings.EpicAppFile) || !File.Exists(settings.EpicAppFile))
            {
                if (MessageBox.Show("游戏exe文件无效，请重新选择。", "错误", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    return;
                }
            }
            RefreshIgnoreFile();
            UploadGame();
        }
    }
}
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace iTunesImport
{

    public partial class frmMain
    {

        #region Fields

        private Importer __Importer;

        private Importer _Importer
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return __Importer;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (__Importer != null)
                {
                    __Importer.StatusChanged -= m_objImporter_StatusChanged;
                    __Importer.Finished -= m_objImporter_Finished;
                }

                __Importer = value;
                if (__Importer != null)
                {
                    __Importer.StatusChanged += m_objImporter_StatusChanged;
                    __Importer.Finished += m_objImporter_Finished;
                }
            }
        }

        public frmMain()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        private void SetEnabledState(bool enabled)
        {
            btnImport.Enabled = enabled;
            btnLogMissingFiles.Enabled = enabled;
            btnClose.Enabled = enabled;
        }

        #endregion

        #region Event Handlers

        private void frmMain_Load(object sender, EventArgs e)
        {
            _Importer = new iTunesImport.Importer();
            lblStatus.AllowDrop = true;
            Text = string.Format("iTunesImport [{0}]", GetVersion());
        }

        private string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string result = fileVersionInfo.ProductVersion;

            return result;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            SetEnabledState(false);
            _Importer.ImportLibrary();
            SetEnabledState(true);
        }

        private void btnLogMissingFiles_Click(object sender, EventArgs e)
        {
            SetEnabledState(false);
            _Importer.FindDuplicates();
            SetEnabledState(true);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void m_objImporter_StatusChanged()
        {
            lblStatus.Text = _Importer.CurrentFolder + Constants.vbCrLf + _Importer.CurrentFile;
            lblStatus.Refresh();
        }

        private void m_objImporter_Finished()
        {
            if (_Importer.SomeFilesMissing)
            {
                lblStatus.Text = "Done (some files missing).";
            }
            else
            {
                lblStatus.Text = "Done.";
            }
            lblStatus.Refresh();
        }

        #endregion

        private void lblStatus_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void lblStatus_DragDrop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent("FileDrop"))
            {

                var files = e.Data.GetData("FileDrop", true);
                string[] filesArray = (string[])files;

                foreach (string playlist in filesArray)
                {
                    if (playlist.ToLower().EndsWith(".m3u"))
                    {
                        _Importer.ImportPlaylist(playlist);
                    }
                }

            }

        }

    }
}
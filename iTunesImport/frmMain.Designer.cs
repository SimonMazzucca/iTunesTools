using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace iTunesImport
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class frmMain : Form
    {

        // Form overrides dispose to clean up the component list.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            btnImport = new Button();
            btnImport.Click += new EventHandler(btnImport_Click);
            btnClose = new Button();
            btnClose.Click += new EventHandler(btnClose_Click);
            lblStatus = new Label();
            lblStatus.DragOver += new DragEventHandler(lblStatus_DragOver);
            lblStatus.DragDrop += new DragEventHandler(lblStatus_DragDrop);
            btnLogMissingFiles = new Button();
            btnLogMissingFiles.Click += new EventHandler(btnLogMissingFiles_Click);
            SuspendLayout();
            // 
            // btnImport
            // 
            btnImport.Location = new Point(12, 54);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(150, 23);
            btnImport.TabIndex = 0;
            btnImport.Text = "&Import";
            btnImport.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.Location = new Point(470, 54);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 23);
            btnClose.TabIndex = 1;
            btnClose.Text = "&Close";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.BackColor = SystemColors.ControlLight;
            lblStatus.BorderStyle = BorderStyle.Fixed3D;
            lblStatus.Location = new Point(12, 9);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(533, 34);
            lblStatus.TabIndex = 2;
            // 
            // btnLogMissingFiles
            // 
            btnLogMissingFiles.Location = new Point(168, 54);
            btnLogMissingFiles.Name = "btnLogMissingFiles";
            btnLogMissingFiles.Size = new Size(150, 23);
            btnLogMissingFiles.TabIndex = 0;
            btnLogMissingFiles.Text = "&Log Missing Files";
            btnLogMissingFiles.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            AcceptButton = btnImport;
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new Size(557, 89);
            Controls.Add(lblStatus);
            Controls.Add(btnClose);
            Controls.Add(btnLogMissingFiles);
            Controls.Add(btnImport);
            MaximizeBox = false;
            Name = "frmMain";
            Load += new EventHandler(frmMain_Load);
            ResumeLayout(false);

        }
        internal Button btnImport;
        internal Button btnClose;
        internal Label lblStatus;
        internal Button btnLogMissingFiles;

    }
}
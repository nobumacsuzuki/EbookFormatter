namespace EbookFormatter
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBoxLog = new TextBox();
            backgroundWorkerEbookFormatter = new System.ComponentModel.BackgroundWorker();
            labelStatus = new Label();
            SuspendLayout();
            // 
            // textBoxLog
            // 
            textBoxLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxLog.Location = new Point(12, 12);
            textBoxLog.Multiline = true;
            textBoxLog.Name = "textBoxLog";
            textBoxLog.ScrollBars = ScrollBars.Vertical;
            textBoxLog.Size = new Size(776, 401);
            textBoxLog.TabIndex = 0;
            // 
            // backgroundWorkerEbookFormatter
            // 
            backgroundWorkerEbookFormatter.WorkerReportsProgress = true;
            backgroundWorkerEbookFormatter.WorkerSupportsCancellation = true;
            backgroundWorkerEbookFormatter.DoWork += OnBgWkrEbookFormatterDoWork;
            backgroundWorkerEbookFormatter.ProgressChanged += OnBgWkrEbookFormatterProgressChanged;
            backgroundWorkerEbookFormatter.RunWorkerCompleted += OnBgWkrEbookFormatterRunWorkerCompleted;
            // 
            // labelStatus
            // 
            labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(12, 416);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(61, 25);
            labelStatus.TabIndex = 1;
            labelStatus.Text = "Ready";
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(labelStatus);
            Controls.Add(textBoxLog);
            Name = "Form1";
            Text = "EbookFormatter";
            DragDrop += OnDragDrop;
            DragEnter += OnDragEnter;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxLog;
        private System.ComponentModel.BackgroundWorker backgroundWorkerEbookFormatter;
        private Label labelStatus;
    }
}

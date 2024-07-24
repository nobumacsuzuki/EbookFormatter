using System.Text;

namespace EbookFormatter
{
    public partial class Form1 : Form
    {
        private EbookFormatEngine ebookFormatter;

        public Form1()
        {
            InitializeComponent();
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop, false) == null)
            {
                textBoxLog.Text = $"Warning: DnD does not contain any files";
                return;
            }
            else
            {
                labelStatus.Text = $"Check file sanities...";

                string[] arrayDndFilenames = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                ebookFormatter = new EbookFormatEngine(arrayDndFilenames);
                ebookFormatter.GetBoarders();

                textBoxLog.Text = ebookFormatter.logger.ToString();

                backgroundWorkerEbookFormatter.RunWorkerAsync();
            }
        }

        private void OnBgWkrEbookFormatterDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ebookFormatter.SliceEbook(sender, e);
        }

        private void OnBgWkrEbookFormatterProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            labelStatus.Text = $"Processing: {e.ProgressPercentage.ToString()}% completed";
        }

        private void OnBgWkrEbookFormatterRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            labelStatus.Text = $"Completed";
            textBoxLog.Text = ebookFormatter.logger.ToString();
        }
    }
}

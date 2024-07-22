using System.Text;

namespace EbookFormatter
{
    public partial class Form1 : Form
    {
        StringBuilder logger;
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
            logger = new StringBuilder();
            textBoxLog.Text = logger.ToString();
            ebookFormatter = new EbookFormatEngine();

            if (e.Data.GetData(DataFormats.FileDrop, false) == null)
            {
                logger.AppendLine($"Warning: DnD does not contain any files");
                return;
            }
            else
            {
                labelStatus.Text = $"Check file sanities...";

                string[] arrayDndFilenames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                string[] arrayInputImageFilenames = CheckFilenameSanity(arrayDndFilenames);

                ebookFormatter.arrayInputImageFilenames = arrayInputImageFilenames;
                ebookFormatter.GetBoarders();
                backgroundWorkerEbookFormatter.RunWorkerAsync();
            }
        }

        private string[] CheckFilenameSanity(string[] filenames)
        {
            List<string> validatedFilenames = new List<string>();
            foreach (string filename in filenames)
            {
                try
                {
                    Bitmap bitmapSanityCheck = new Bitmap(filename);
                    validatedFilenames.Add(filename);
                    bitmapSanityCheck.Dispose();
                }
                catch (Exception e)
                {
                    logger.AppendLine($"Warning, {filename}, {e.Message}, skipped");
                }
            }
            validatedFilenames.Sort();
            return validatedFilenames.ToArray();
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
            logger.AppendLine(ebookFormatter.logger.ToString());
            textBoxLog.Text = logger.ToString();
        }
    }
}

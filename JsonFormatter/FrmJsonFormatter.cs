using System.Text.Json;
using System.Text.RegularExpressions;

namespace JsonFormatter
{
    public partial class FrmJsonFormatter : Form
    {
        public FrmJsonFormatter()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var inputJson = inputTextBox.Text;

                var formattedJson = FormatJson(inputJson);

                HighlightJson(outputTextBox, formattedJson);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

        private string FormatJson(string inputJson)
        {
            var jsonDocument = JsonDocument.Parse(inputJson);
            return JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        private void HighlightJson(RichTextBox outputBox, string json)
        {
            outputBox.Clear();
            outputBox.Text = json;

            var regexPatterns = new Dictionary<Regex, (Color color, FontStyle fontStyle)>
            {
                { new Regex("\"(.*?)\"(?=:)"), (Color.Purple, FontStyle.Regular) },
                { new Regex(":\\s*\"(.*?)\""), (Color.Brown, FontStyle.Regular) },
                { new Regex("\\b\\d+(\\.\\d+)?\\b"), (Color.DarkBlue, FontStyle.Regular) },
                { new Regex("\\b(true|false|null)\\b"), (Color.Blue, FontStyle.Bold) },
                { new Regex("[{}\\[\\],:]"), (Color.Gray, FontStyle.Regular) }
            };

            outputBox.SuspendLayout();

            foreach (var pattern in regexPatterns)
            {
                ApplyStyle(outputBox, pattern.Key, pattern.Value.color, pattern.Value.fontStyle);
            }

            outputBox.ResumeLayout();
        }

        private void ApplyStyle(RichTextBox outputBox, Regex regex, Color color, FontStyle fontStyle)
        {
            foreach (Match match in regex.Matches(outputBox.Text))
            {
                outputBox.Select(match.Index, match.Length);
                outputBox.SelectionColor = color;
                outputBox.SelectionFont = new Font(outputBox.Font, fontStyle);
            }
        }

        private void ShowErrorMessage(Exception ex)
        {
            MessageBox.Show($"Error when format JSON: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

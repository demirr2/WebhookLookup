using System.Diagnostics;
using System.Text.RegularExpressions;
using Mono.Cecil;

namespace WebhookLookup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            textBox1.ReadOnly = true;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                AnalyzeFile(file);
            }
        }

        private void AnalyzeFile(string filePath)
        {
            if (filePath.EndsWith(".exe"))
            {
                ExtractWebhookFromExe(filePath);
            }

            else
            {
                MessageBox.Show("u can only use .exe files!", "if you have any problems contact my discord: tuccarironnn");
            }
        }

        private void ExtractWebhookFromExe(string filePath)
        {
            try
            {
                var assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(filePath);

                string pattern = @"https://discord\.com/api/webhooks/[a-zA-Z0-9-_]+/[a-zA-Z0-9-_]+";

                List<string> webhooks = new List<string>();

                foreach (var module in assembly.Modules)
                {
                    foreach (var type in module.Types)
                    {
                        foreach (var method in type.Methods)
                        {
                            foreach (var body in method.Body.Instructions)
                            {
                                if (body.OpCode.Name == "ldstr")
                                {
                                    string str = body.Operand as string;
                                    if (str != null && Regex.IsMatch(str, pattern))
                                    {
                                        webhooks.Add(str);
                                    }
                                }
                            }
                        }
                    }
                }

                ShowResults(webhooks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "an error has occurred.");
            }
        }

        private void ExtractWebhookFromPy(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);

            string pattern = @"https://discord\.com/api/webhooks/[a-zA-Z0-9-_]+/[a-zA-Z0-9-_]+";

            List<string> webhooks = new List<string>();

            foreach (Match match in Regex.Matches(fileContent, pattern))
            {
                webhooks.Add(match.Value);
            }

            ShowResults(webhooks);
        }

        private void ShowResults(List<string> webhooks)
        {
            if (webhooks.Count > 0)
            {
                foreach (string webhook in webhooks)
                {
                    textBox1.AppendText(webhook + Environment.NewLine);
                }
            }
            else
            {
                MessageBox.Show("no webhook found in the file.", "if you have any problems contact my discord: tuccarironnn");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                Clipboard.SetText(textBox1.Text);
                textBox1.Clear();
                MessageBox.Show("Copied");
            }
            else
            {
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://demirr2.github.io/webhookfucker/",
                UseShellExecute = true
            });
        }
    }
}

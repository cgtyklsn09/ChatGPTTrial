using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ChatGPTTrial2
{
    public partial class MyForm : Form
    {
        const string API_KEY = "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";
        public MyForm()
        {
            InitializeComponent();
        }

        private async void submitButton_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("authorization", "Bearer " + API_KEY);

            var content = new StringContent("{\"model\": \"text-davinci-003\", \"prompt\": \"" + inputTextBox.Text + "\",\"temperature\": 0.5,\"max_tokens\": 1000}",
                        Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);
            string responseString = await response.Content.ReadAsStringAsync();
            var dyData = JsonConvert.DeserializeObject<dynamic>(responseString);

            string guess = dyData.choices[0].text;
            outputTextBox.Text = guess.Substring(2);
        }

        static string GuessCommand(string raw)
        {
            Console.WriteLine("---> GPT-3 API Returned Text:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(raw);

            var lastIndex = raw.LastIndexOf('\n');

            string guess = raw.Substring(lastIndex + 1);

            Console.ResetColor();

            TextCopy.ClipboardService.SetText(guess);

            return guess;
        }
    }
}

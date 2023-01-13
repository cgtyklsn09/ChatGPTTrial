using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ChatGPTTrial2
{
    public partial class MyForm : Form
    {
        private TextBox inputTextBox;
        private TextBox outputTextBox;
        private Button submitButton;

        // Replace YOUR_API_KEY with your actual API key
        const string API_KEY = "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";
        public MyForm()
        {
            InitializeComponent();
            inputTextBox = new TextBox();
            outputTextBox = new TextBox();
            submitButton = new Button();

            inputTextBox.Location = new System.Drawing.Point(10, 10);
            inputTextBox.Size = new System.Drawing.Size(200, 20);

            outputTextBox.Location = new System.Drawing.Point(10, 50);
            outputTextBox.Size = new System.Drawing.Size(200, 20);
            outputTextBox.ReadOnly = true;

            submitButton.Location = new System.Drawing.Point(10, 90);
            submitButton.Size = new System.Drawing.Size(75, 23);
            submitButton.Text = "Submit";
            submitButton.Click += new EventHandler(this.submitButton_Click);

            this.Controls.Add(inputTextBox);
            this.Controls.Add(outputTextBox);
            this.Controls.Add(submitButton);
        }

        private async void submitButton_Click(object sender, EventArgs e)
        {
            using (var client = new HttpClient())
            {
                // Set the API key in the request headers
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + API_KEY);

                // Set the request parameters
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/engines/davinci-codex/completions");
                request.Content = new StringContent("{\"prompt\":\"" + inputTextBox.Text + "\", \"temperature\":0.5}", Encoding.UTF8, "application/json");

                // Send the request
                var response = await client.SendAsync(request);

                // Get the response
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserialize the response
                dynamic json = JsonConvert.DeserializeObject(responseString);

                // Display the response
                outputTextBox.Text = json.choices[0].text;
            }
        }
    }

}

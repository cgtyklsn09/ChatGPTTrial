using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using System.Speech.Recognition;


namespace ChatGPTTrial2
{
    public partial class MyForm : Form
    {
        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        // Replace YOUR_API_KEY with your actual API key
        const string API_KEY = "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";

        bool _isListening = false;
        public MyForm()
        {
            InitializeComponent();
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammar(new DictationGrammar());
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_recognizer_SpeechRecognized);
        }

        private void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            inputTextBox.Invoke(new Action(() => inputTextBox.Text = e.Result.Text));
        }

        private void startStopListeningButton_Click(object sender, EventArgs e)
        {
            if (_isListening)
            {
                _recognizer.RecognizeAsyncStop();
                startStopListeningButton.Text = "Start Listening";
                _isListening = false;
            }
            else
            {
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
                startStopListeningButton.Text = "Stop Listening";
                _isListening = true;
            }

        }

        private async void submitButton_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("authorization", "Bearer " + API_KEY);

            var content = new StringContent("{\"model\": \"text-davinci-003\", \"prompt\": \"" + "Please give me a standalone 'public static void Main()' method in C# to " + inputTextBox.Text + " in current Inventor window. Get an active instance of Inventor by using that:  " + "InvApp.txt" + ". Ignore code descriptions or instructions." + "\",\"temperature\": 0,\"max_tokens\": 1000}", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);

            string responseString = await response.Content.ReadAsStringAsync();

            var dyData = JsonConvert.DeserializeObject<dynamic>(responseString);

            string guess = dyData.choices[0].text;
            outputTextBox.Text = guess.Substring(2);

            string code = guess.Substring(2);

            string input = code;
            int index = input.IndexOf("public static");
            string output = input.Substring(index);


            code = File.ReadAllText("Start.txt") + output + File.ReadAllText("End.txt");
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add(@"C:\Program Files\Autodesk\Inventor 2022\Bin\Public Assemblies\Autodesk.Inventor.Interop.dll");
            parameters.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll");
            parameters.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Windows.Forms.dll");
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);
            if (results.Errors.HasErrors)
            {
                Console.WriteLine("Errors building the code:");
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine(error.ToString());
                    outputTextBox.Text = guess.Substring(2) + error.ToString();
                }
            }
            else
            {
                Assembly assembly = results.CompiledAssembly;
                Type[] types = assembly.GetTypes();
                MethodInfo method = types[0].GetMethod("Main");
                method.Invoke(null, null);

            }
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

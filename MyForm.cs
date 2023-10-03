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
using Newtonsoft.Json.Linq;

namespace ChatGPTTrial2
{
    public partial class MyForm : Form
    {
        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        const string API_KEY = "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";
        bool _isListening = false;
        
        public MyForm()
        {
            InitializeComponent();
        }

        private void MyForm_Load(object sender, EventArgs e)
        {
            try
            {
                _recognizer.SetInputToDefaultAudioDevice();
            }
            catch
            {
                int count = 0;
                string languagePacks = "";
                foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers())
                {
                    count++;
                    languagePacks += count + ")" + ri.Culture.Name + "\n";
                }

                if (count == 0)
                    MessageBox.Show("There is no language pack installed on your computer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                    MessageBox.Show("Language packs installed on your computer:\n" + languagePacks, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Close();
                return;
            }

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
            try
            {
                string request = inputTextBox.Text;
                if (request.Contains("\n"))
                    request = request.Replace("\n", " ");

                string invApp = File.ReadAllText("InvApp.txt");
                request = "Please give me a standalone 'public static void Main()' method in C# to " + request + " in current Inventor window.You this as Inventor.Application object " + invApp + ". Ignore code descriptions or instructions.";

                string endpoint = "https://api.openai.com/v1/chat/completions";
                var messages = new[]
                {
                    new {role = "user", content = request}
                };

                var data = new
                {
                    model = "gpt-3.5-turbo",
                    messages = messages,
                    temperature = 0
                };

                string jsonString = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("authorization", "Bearer " + API_KEY);
                var response2 = await client.PostAsync(endpoint, content);
                string responseContent = await response2.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseContent);
                var response = jsonResponse["choices"][0]["message"]["content"].Value<string>();
                outputTextBox.Text = response;

                string input = response;
                if (input.IndexOf("```csharp") != -1)
                    input = input.Substring(input.IndexOf("```csharp") + 10);
                else if(input.IndexOf("```") != 1)
                    input = input.Substring(input.IndexOf("```") + 4);

                if (input.IndexOf("```") != -1)
                    input = input.Substring(0, input.IndexOf("```"));

                //if (input.StartsWith("using System;"))
                //{
                    
                //}            
                //else
                //{
                //    int index = input.IndexOf("static void Main(string[] args)\n");
                //    if (index == -1)
                //        index = input.IndexOf("static void Main()\n");

                //    string output = input.Substring(index);
                //    if(output.IndexOf("```") != -1)
                //    {
                //        output = output.Substring(0, output.IndexOf("```"));
                //    }

                //    input = File.ReadAllText("Start.txt") + output +"}";// + File.ReadAllText("End.txt");
                //}

                if (!input.Contains("using Inventor;\n"))
                    input = "using Inventor;\n" + input;

                if (!input.Contains("using System;\n"))
                    input = "using System;\n" + input;

                if (input.Contains("Document newDoc = invApp.Documents.Add(DocumentTypeEnum.kPartDocumentObject, false);"))
                    input = input.Replace("Document newDoc = invApp.Documents.Add(DocumentTypeEnum.kPartDocumentObject, false);", "Document newDoc = invApp.Documents.Add(DocumentTypeEnum.kPartDocumentObject);");

                if (!input.Contains("public static void Main()") && input.Contains("static void Main()"))
                    input = input.Replace("static void Main()", "public static void Main()");
                else if (!input.Contains("public static void Main(string[] args)") && input.Contains("static void Main(string[] args)"))
                    input = input.Replace("static void Main(string[] args)", "public static void Main()");

                StreamWriter sw = new StreamWriter("Input.txt");
                sw.WriteLine(input);
                sw.Close();

                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true
                };

                parameters.ReferencedAssemblies.Add(@"C:\Program Files\Autodesk\Inventor 2023\Bin\Public Assemblies\Autodesk.Inventor.Interop.dll");
                parameters.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll");
                parameters.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Windows.Forms.dll");
                CompilerResults results = provider.CompileAssemblyFromSource(parameters, input);
                
                if (results.Errors.HasErrors)
                {
                    Console.WriteLine("Errors building the code:");
                    foreach (CompilerError error in results.Errors)
                    {
                        Console.WriteLine(error.ToString());
                        outputTextBox.Text += "\n" + error.ToString();
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}


/*çalışan örnekler
open a new part document. do not save the document. do not close the document.
open a new part document. do not save the document. do not close the document. create a new sketch.
close inventor
what is the active document name
how many workplanes does the active part document have
how many sketches does the active part document have
*/
//eski
//var content = new StringContent("{\"model\": \"gpt-3\", \"prompt\": \"" + "Please give me a standalone 'public static void Main()' method in C# to " + request + " in current Inventor window. Get an active instance of Inventor by using that:  " + "InvApp.txt" + ". Ignore code descriptions or instructions." + "\",\"temperature\": 0,\"max_tokens\": 1000}", Encoding.UTF8, "application/json");

//HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);

//string responseString = await response.Content.ReadAsStringAsync();

//var dyData = JsonConvert.DeserializeObject<dynamic>(responseString);

//string guess = dyData.choices[0].text;
//outputTextBox.Text = guess.Substring(2);
/*
                 string code = guess.Substring(2);

                string input = code;
                int index = input.IndexOf("public static");
                string output = input.Substring(index);

                code = File.ReadAllText("Start.txt") + output + File.ReadAllText("End.txt");
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateExecutable = false;
                parameters.GenerateInMemory = true;
                parameters.ReferencedAssemblies.Add(@"C:\Program Files\Autodesk\Inventor 2023\Bin\Public Assemblies\Autodesk.Inventor.Interop.dll");
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
 */


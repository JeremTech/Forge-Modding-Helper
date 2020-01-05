using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Forge_Modding_Helper_3
{
    public class GradleTasks
    {
        private TextBox outputTextbox;
        private bool TaskRun = false;

        // Thread vars
        delegate void SetTextCallback(string text);
        Process process;

        // Constructor
        public GradleTasks(TextBox outputTextbox)
        {
            this.outputTextbox = outputTextbox;

            var startinfo = new ProcessStartInfo("cmd.exe")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
            };

            process = new Process { StartInfo = startinfo };
        }

        // Check if a gradle task is already run.
        public bool isTaskRun()
        {
            return this.TaskRun;
        }

        // Update console text - Multi Thread safe
        private void SetText(string text)
        {
            outputTextbox.Text += text + Environment.NewLine;
            outputTextbox.ScrollToEnd();
        }

        // genIntelljRuns task
        public void genIntelljRuns()
        {
            // Create thread
            var thread = new Thread(new ThreadStart(() =>
            {
                process.Start();
                process.StandardInput.WriteLine("gradlew genIntellJRuns");

                var reader = process.StandardOutput;
                while (!reader.EndOfStream)
                {
                    var nextLine = reader.ReadLine();

                    outputTextbox.Dispatcher.Invoke(new SetTextCallback(this.SetText), nextLine);
                }

                process.WaitForExit();
                process.Kill();

            }));

            thread.Start();
        }
    }
}

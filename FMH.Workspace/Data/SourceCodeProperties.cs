using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FMH.Workspace.Data
{
    public class SourceCodeProperties
    {
        /// <summary>
        /// Java files paths list
        /// </summary>
        public List<string> JavaFiles { get; set; }

        /// <summary>
        /// Code lines count
        /// </summary>
        public long CodeLinesCount { get; set; }

        /// <summary>
        /// Workspace path
        /// </summary>
        private string _workspacePath { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workspacePath">Workspace path</param>
        public SourceCodeProperties(string workspacePath)
        {
            this._workspacePath = workspacePath;
            this.JavaFiles = new List<string>();
        }

        /// <summary>
        /// Retrieve all Java files paths of the workspace
        /// </summary>
        public void RetrieveAllJavaFiles()
        {
            string directoryPath = Path.Combine(_workspacePath, "src\\main\\java");
            this.JavaFiles = Directory.EnumerateFiles(directoryPath, string.Empty, SearchOption.AllDirectories).ToList();
        }

        /// <summary>
        /// Return the number of code lines of the workspace <br/>
        /// <c>RetrieveAllJavaFiles()</c> must be called before this method
        /// </summary>
        /// <param name="countBlankCodeLine">Count blank lines ?</param>
        public void CountCodeLines(bool countBlankCodeLine)
        {
            int countResult = 0;

            if (this.JavaFiles == null)
                return;
            
            foreach (string codeFile in this.JavaFiles)
            {
                if (!File.Exists(codeFile))
                    continue;

                using (var reader = File.OpenText(codeFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line) && countBlankCodeLine)
                                countResult++;
                        else
                            countResult++;
                    }
                }
            }

            this.CodeLinesCount = countResult;
        }
    }
}

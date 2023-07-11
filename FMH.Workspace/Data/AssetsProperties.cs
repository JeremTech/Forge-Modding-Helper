using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FMH.Workspace.Data
{
    public class AssetsProperties
    {
        /// <summary>
        /// Textures files paths list
        /// </summary>
        public List<string> TexturesFiles { get; set; }

        /// <summary>
        /// Models files paths list
        /// </summary>
        public List<string> ModelsFiles { get; set; }

        /// <summary>
        /// Blockstates files paths list
        /// </summary>
        public List<string> BlockstatesFiles { get; set; }

        /// <summary>
        /// Workspace path
        /// </summary>
        private string _workspacePath { get; set; }

        /// <summary>
        /// ModId
        /// </summary>
        private string _modId { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="workspacePath">Workspace path</param>
        /// <param name="modId">ModId</param>
        public AssetsProperties(string workspacePath, string modId) 
        {
            this._workspacePath = workspacePath;
            this._modId = modId;
            this.TexturesFiles = new List<string>();
            this.ModelsFiles = new List<string>();
            this.BlockstatesFiles = new List<string>();
        }

        /// <summary>
        /// Retrieve all textures files paths of the workspace
        /// </summary>
        public void RetrieveAllTexturesFiles()
        {
            string directoryPath = Path.Combine(_workspacePath, "src\\main\\resources\\assets", _modId, "textures");
            TexturesFiles = Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories).ToList();
        }

        /// <summary>
        /// Retrieve all models files paths of the workspace
        /// </summary>
        public void RetrieveAllModelsFiles()
        {
            string directoryPath = Path.Combine(_workspacePath, "src\\main\\resources\\assets", _modId, "models");
            ModelsFiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories).ToList();
        }

        /// <summary>
        /// Retrieve all textures files paths of the workspace
        /// </summary>
        public void RetrieveAllBlockstatesFiles()
        {
            string directoryPath = Path.Combine(_workspacePath, "src\\main\\resources\\assets", _modId, "blockstates");
            BlockstatesFiles = Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories).ToList();
        }
    }
}

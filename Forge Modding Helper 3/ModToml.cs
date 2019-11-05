using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge_Modding_Helper_3
{
    public class ModToml
    {
        private Dictionary<string, string> mod_infos = new Dictionary<string, string> { };
        private string folder = "";

        public ModToml(Dictionary<string, string> mod_infos, string generation_foler)
        {
            this.mod_infos = mod_infos;
            this.folder = generation_foler;
        }

        public void generateFile()
        {
            string[] lines = File.ReadAllLines(this.folder + @"\src\main\resources\META-INF\mods.toml");
            string[] output = new string[lines.Length]; 

            for(int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if(line.Contains("issueTrackerURL="))
                {
                    output[i] = line.Replace("http://my.issue.tracker/", this.mod_infos["issue_tracker"]);
                }
                else if (line.Contains("modId="))
                {
                    output[i] = line.Replace("examplemod", this.mod_infos["mod_id"]);
                }
                else if (line.Contains("displayName="))
                {
                    output[i] = line.Replace("Example Mod", this.mod_infos["mod_name"]);
                }
                else if (line.Contains("updateJSONURL="))
                {
                    output[i] = line.Replace("http://myurl.me/", this.mod_infos["update_json"]);
                }
                else if (line.Contains("displayURL="))
                {
                    output[i] = line.Replace("http://example.com/", this.mod_infos["display_url"]);
                }
                else if (line.Contains("logoFile="))
                {
                    output[i] = line.Replace("examplemod.png", "logo.png");
                }
                else if (line.Contains("credits="))
                {
                    output[i] = line.Replace("Thanks for this example mod goes to Java", this.mod_infos["mod_credits"]);
                }
                else if (line.Contains("authors="))
                {
                    output[i] = line.Replace("Love, Cheese and small house plants", this.mod_infos["mod_authors"]);
                }
                else
                {
                    output[i] = line;
                }
            }

            File.Delete(this.folder + @"\src\main\resources\META-INF\mods.toml");
            File.AppendAllLines(this.folder + @"\src\main\resources\META-INF\mods.toml", output);

            string content = File.ReadAllText(this.folder + @"\src\main\resources\META-INF\mods.toml");
            content = content.Replace("This is a long form description of the mod. You can write whatever you want here" + System.Environment.NewLine + System.Environment.NewLine + "Have some lorem ipsum." + System.Environment.NewLine + System.Environment.NewLine + "Lorem ipsum dolor sit amet, consectetur adipiscing elit.Sed mollis lacinia magna.Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.Sed sagittis luctus odio eu tempus.Interdum et malesuada fames ac ante ipsum primis in faucibus.Pellentesque volutpat ligula eget lacus auctor sagittis.In hac habitasse platea dictumst.Nunc gravida elit vitae sem vehicula efficitur.Donec mattis ipsum et arcu lobortis, eleifend sagittis sem rutrum.Cras pharetra quam eget posuere fermentum.Sed id tincidunt justo.Lorem ipsum dolor sit amet, consectetur adipiscing elit.", this.mod_infos["mod_description"]);
            content = content.Replace("dependencies.examplemod", "dependencies." + this.mod_infos["mod_id"]);

            File.Delete(this.folder + @"\src\main\resources\META-INF\mods.toml");
            File.WriteAllText(this.folder + @"\src\main\resources\META-INF\mods.toml", content);
        }
    }
}

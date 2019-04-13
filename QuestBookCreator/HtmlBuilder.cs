using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
namespace QuestBookCreator
{
    class HtmlBuilder : AbstractBuilder
    {
        public string Create(Project curProj)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description =
                "Select the directory that you want to use as the default.";
            folderBrowserDialog1.ShowNewFolderButton = true;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                List<Node> tmp = curProj.get_all();
                HtmlConverter hc = new HtmlConverter();
                string path = folderBrowserDialog1.SelectedPath + "\\";
                string pathToImages = path + "pics";
                System.IO.Directory.CreateDirectory(pathToImages);
                foreach (Node node in tmp)
                {
                    string fullPath;
                    if (node == curProj.startNode)
                        fullPath = path + "start.html";
                    else
                        fullPath = path + node.get_name() + ".html";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(fullPath, false, Encoding.UTF8)) //создал файл
                    {
                        file.Write("<html>");
                        file.Write("<body>");
                        List<string> list_str = hc.get_processed(node, pathToImages);
                        foreach (string s in list_str)
                        {
                            file.WriteLine(s);
                        }
                        file.Write("</body>");
                        file.Write("</html>");
                    }
                }
                // MessageBox.Show("ok");
                return path;
            }
            return "";
        }
    }
}
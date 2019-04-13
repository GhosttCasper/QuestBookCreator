using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
//using Ionic.Zip;
using System.Diagnostics;
namespace QuestBookCreator
{
    class ApkBuilder : AbstractBuilder
    {
        public string Create(Project curProj)
        {
            string pathToTemp = @"C:\signscript";
            if (!System.IO.Directory.Exists(pathToTemp)) //создаём рабочую папку
                System.IO.Directory.CreateDirectory(pathToTemp);
            string pathToAssets = @"C:\signscript\assets";
            string pathToUnsignedApk = @"C:\signscript\sample.apk";
            string pathToSignedApk = @"C:\signscript\SignedFiles\sample.apk";
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "apk files (*.apk)|*.apk|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string pathToApk = saveFileDialog1.FileName;
                if (!System.IO.File.Exists(pathToUnsignedApk))
                    File.WriteAllBytes(pathToUnsignedApk, Properties.Resources.sample);
                //Stream st = QuestBookCreator.Properties.Resources.ResourceManager.GetStream("sample");
                // QuestBookCreator.Properties.Resources.sample.
                //SaveStreamToFile(st, saveFileDialog1.FileName); //копирование apk
                List<Node> tmp = curProj.get_all();
                ApkConverter aConv = new ApkConverter();
                string folder = pathToApk.Substring(0, pathToApk.LastIndexOf(@"\"));
                /////////////////////////////////////////////
                System.IO.Directory.CreateDirectory(pathToAssets);
                foreach (Node node in tmp)
                {
                    string fullPath;
                    if (node == curProj.startNode)
                        fullPath = pathToAssets + @"\start.html";
                    else
                        fullPath = pathToAssets + "\\" + node.get_name() + ".html";
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(fullPath, false, Encoding.UTF8)) //создал файл
                    {
                        file.Write("<html>");
                        file.Write("<body link=\"#0000FF\" vlink=\"#0000FF\" alink=\"#ff0000\" >");
                        List<string> list_str = aConv.get_processed(node, pathToAssets);
                        foreach (string s in list_str)
                        {
                            file.WriteLine(s);
                        }
                        file.Write("</body>");
                        file.Write("</html>");
                    }
                }
                //закончили с барахлом, упаковываем
                using (ZipFile zip = new ZipFile(pathToUnsignedApk)) // Создаем объект для работы с архивом
                {
                    //zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression; // Задаем максимальную степень сжатия
                    zip.AddDirectory(pathToAssets, "assets"); // Кладем в архив папку вместе с содежимым
                                                              // zip.AddFile(@"c:\Temp\Import.csv"); // Кладем в архив одиночный файл
                    zip.Save(pathToUnsignedApk); // Создаем архив
                }
                //Затираем html
                try
                {
                    System.IO.Directory.Delete(pathToAssets, true);
                }
                catch { MessageBox.Show("Проблемы с временными файлами"); }
                //выгружаем скрипт и подпись, если надо
                string pathToScrypt = @"C:\signscript\signfile.vbs";
                if (!System.IO.File.Exists(pathToScrypt))
                    File.WriteAllBytes(pathToScrypt, Properties.Resources.signfile);
                string pathToJar = @"C:\signscript\testsign.jar";
                if (!System.IO.File.Exists(pathToJar))
                    File.WriteAllBytes(pathToJar, Properties.Resources.testsign);
                //запускаем скрипт
                try
                {
                    Process script = Process.Start(pathToScrypt, pathToUnsignedApk);
                    script.WaitForExit();
                }
                catch
                {
                    MessageBox.Show("Не удалось подписать apk файл", "QuestBookCreator");
                    System.IO.File.Move(pathToUnsignedApk, pathToApk);
                    return folder;
                }
                //переносим в нужную папку
                System.IO.File.Move(pathToSignedApk, pathToApk);
                //удаляем неподписанный
                System.IO.File.Delete(pathToUnsignedApk);
                return folder;
            }
            return "";
        }
        public void SaveStreamToFile(Stream input, String filename)
        {
            using (Stream output = File.OpenWrite(filename))
            {
                byte[] buffer = new byte[8 * 1024];
                int len;
                while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, len);
                }
            }
        }
    }
}
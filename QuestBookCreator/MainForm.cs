using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
namespace QuestBookCreator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        Project curProj;
        bool flag_node = false;
        bool flag_click = false;
        int raznX = 0, raznY = 0; //для мыши
        public void Redraw()
        {
            //pictureBox1.Invalidate();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            curProj.Paint(e.Graphics);
        }
        //открывает окно редактирования параграфа
        /*private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            float mouseX = MousePosition.X - this.Location.X - pictureBox1.Location.X - Constants.extraX;
            float mouseY = MousePosition.Y - this.Location.Y - pictureBox1.Location.Y - Constants.extraY;
            Node curN = curProj.getNode(mouseX, mouseY);
            if (curN != null)
            {
                EditForm ef = new EditForm(curN, curProj.get_all());
                ef.ShowDialog();
                curN.refreshChildren();
                Redraw();
            }
        }
        //захват 1 параграфа или всех
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            float mouseX = MousePosition.X - this.Location.X - pictureBox1.Location.X - Constants.extraX;
            float mouseY = MousePosition.Y - this.Location.Y - pictureBox1.Location.Y - Constants.extraY;
            flag_click = true;
            raznX = MousePosition.X; //текущее смещение
            raznY = MousePosition.Y;
            Node curN = curProj.getNode(mouseX, mouseY);
            if (curN != null)
            {
                flag_node = true;
                curProj.curNode = curN;
                Refresh();
            }
        }*/
        //конец перетаскивания
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            flag_node = flag_click = false;
        }
        //перетаскивание
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag_click)
            {
                if (flag_node)
                    movingNode(curProj.curNode);
                else
                {
                    curProj.movingAll(MousePosition.X - raznX, MousePosition.Y - raznY);
                    raznX = MousePosition.X;
                    raznY = MousePosition.Y;
                }
                Redraw();
            }
        }
        void movingNode(Node k)
        {
            k.set_x(k.get_x() + MousePosition.X - raznX);
            k.set_y(k.get_y() + MousePosition.Y - raznY);
            raznX = MousePosition.X;
            raznY = MousePosition.Y;
        }
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "qbc files (*.qbc)|*.qbc|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string s = saveFileDialog1.FileName;
                curProj.saveAs(s);
                this.Text = s;
            }
        }
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkBeforeClosing())
            {
                return;
            }
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "qbc files (*.qbc)|*.qbc|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string s = openFileDialog1.FileName;
                    curProj = curProj.load(s);
                    this.Text = s;
                    Redraw();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void BuildPdfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkBeforeBuilding())
                return;
            curProj.curNode = curProj.startNode;
            AbstractBuilder bb = new PdfBuilder();
            try
            {
                string path = bb.Create(curProj);
                if (path != "")
                {
                    DialogResult dr = MessageBox.Show("Pdf файл успешно сохранён. Открыть сейчас?", "QuestBookCreator", MessageBoxButtons.YesNo);
                    if (dr == System.Windows.Forms.DialogResult.Yes)
                        Process.Start(path);
                }
            }
            catch { MessageBox.Show("Ошибка сборки"); }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            curProj = Project.Instance();
        }
        private void AddNodeButton_Click(object sender, EventArgs e)
        {
            Paragraph p = new Paragraph();
            curProj.addNode(p);
            Redraw();
        }
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (curProj.canSave() == true)
            {
                try
                {
                    curProj.save();
                    MessageBox.Show("Сохранено", "QuestBookCreator");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error" + ex.Message);
                }
            }
            else SaveAsToolStripMenuItem_Click(sender, e);
        }
        private void DeleteNode_Click(object sender, EventArgs e)
        {
            if (curProj.curNode == null)
            {
                MessageBox.Show("Выберите узел", "QuestBookCreator");
                return;
            }
            if (curProj.hasInputEdge(curProj.curNode) == true)
            {
                DialogResult result = MessageBox.Show("На этот параграф есть действующие ссылки. Удалить параграф и все ссылки на него?", "Подтвердите действие",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    curProj.deleteLinksOn(curProj.curNode);
                    curProj.deleteCurNode();
                    Redraw();
                }
            }
            else
            {
                DialogResult result = MessageBox.Show("Вы действительно хотите удалить этот узел?", "Подтвердите действие",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    curProj.deleteCurNode();
                    Redraw();
                }
            }
        }
        private void MakeStartButton_Click(object sender, EventArgs e)
        {
            if (curProj.curNode == null)
                MessageBox.Show("Выбери вершину", "QuestBookCreator");
            else
            {
                curProj.startNode = curProj.curNode;
                curProj.haveStart = true;
                Redraw();
            }
        }
        private void zoomMinus_Click(object sender, EventArgs e)
        {
            if (Constants.minH > curProj.nodeHeight) return; //уже слишком мелко
            curProj.nodeHeight = curProj.nodeHeight / Constants.changingSpeed;
            curProj.nodeWidth = curProj.nodeWidth / Constants.changingSpeed;
            curProj.zoomMinus();
            Redraw();
            //label1.Text = Constants.nodeHeight.ToString();
            //label2.Text = Constants.nodeWidth.ToString();
        }
        private void zoomPlus_Click(object sender, EventArgs e)
        {
            if (Constants.maxH < curProj.nodeHeight) return; //уже слишком крупно
            curProj.nodeHeight = curProj.nodeHeight * Constants.changingSpeed;
            curProj.nodeWidth = curProj.nodeWidth * Constants.changingSpeed;
            curProj.zoomPlus();
            Redraw();
            //label1.Text = Constants.nodeHeight.ToString();
            //label2.Text = Constants.nodeWidth.ToString();
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkBeforeClosing();
            curProj = curProj.load("");
            Redraw();
        }
        private void buildHtmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkBeforeBuilding())
                return;
            curProj.curNode = curProj.startNode;
            AbstractBuilder bb = new HtmlBuilder();
            try
            {
                string path = bb.Create(curProj);
                if (path != "")
                {
                    DialogResult dr = MessageBox.Show("HTML версия книги успешно сохранена. Открыть папку сейчас?", "QuestBookCreator", MessageBoxButtons.YesNo);
                    if (dr == System.Windows.Forms.DialogResult.Yes)
                        Process.Start(path);
                }
            }
            catch { MessageBox.Show("Ошибка сборки"); }
            // string path = bb.Create(curProj);
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N)
                newToolStripMenuItem_Click(sender, e);
            if (e.Control && e.KeyCode == Keys.S)
                SaveToolStripMenuItem_Click(sender, e);
            if (e.Control && e.KeyCode == Keys.O)
                OpenToolStripMenuItem_Click(sender, e);
        }
        /// <summary>
        /// возвращает true, если нажата Отмена
        /// </summary>
        /// <returns></returns>
        private bool checkBeforeClosing()
        {
            if (!curProj.isEmpty())
            {
                DialogResult dr = MessageBox.Show("Сохранить изменения в текущем проекте?", "QuestBookCreator", MessageBoxButtons.YesNoCancel);
                if (dr == System.Windows.Forms.DialogResult.Cancel)
                    return true;
                if (dr == System.Windows.Forms.DialogResult.Yes)
                    SaveToolStripMenuItem_Click(null, null);
            }
            return false;
        }
        /// <summary>
        /// возвращает true, если проверка не пройдена
        /// </summary>
        /// <returns></returns>
        private bool checkBeforeBuilding()
        {
            if (curProj.haveStart == false)
            {
                MessageBox.Show("Необходимо задать стартовый узел");
                return true;
            }
            return false;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBeforeClosing())
                e.Cancel = true;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            //Constants.panelHeight = pictureBox1.Height;
           // Constants.panelWidth = pictureBox1.Width;
        }
        private void buildAPKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkBeforeBuilding())
                return;
            AbstractBuilder bb = new ApkBuilder();
            try
            {
                string path = bb.Create(curProj);
                if (path != "")
                {
                    DialogResult dr = MessageBox.Show("Apk файл успешно собран. Открыть папку с файлом сейчас?", "QuestBookCreator", MessageBoxButtons.YesNo);
                    if (dr == System.Windows.Forms.DialogResult.Yes)
                        Process.Start(path);
                }
            }
            catch (Exception ee) { MessageBox.Show("Ошибка сборки" + ee.Data); }
        }
    }
}
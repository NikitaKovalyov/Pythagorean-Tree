using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Word = Microsoft.Office.Interop.Word;
using Graph = Microsoft.Office.Interop.Graph;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;


namespace coursework
{
    public partial class Form1 : Form
    {
        bool movable; // true, если курсор зажат и перемещается
        float mouse_x; // позиция курсора по x
        float mouse_y; // позици курсора по y
        float mouse_x_offset; // смещение по x
        float mouse_y_offset; // смещение по y
        Graphics gr; // работа с графикой
        Bitmap image; // для работы с изображениями
        float root_x; // начало x на графике (корень)
        float root_y; // начало y на графике (корень)
        int depth;  // глубина
        int length;
        float alpha;

        public Form1()
        {
            InitializeComponent();
            Init();
            openXmlFile();
            Panel_Paint();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Курсовая работа, Никита Ковалёв, гр. 10701320.";
        }

        public void Init()
        {
            image = new Bitmap(Panel_pain.Width, Panel_pain.Height); // создаем растровое изображение
            gr = Graphics.FromImage(image);
            gr.Clear(Color.White);
            Panel_pain.Image = image;
            gr.SmoothingMode = SmoothingMode.AntiAlias;
        }

        // Рисуем дерево.
        private void Panel_Paint()
        {

            gr.Clear(Panel_pain.BackColor);

            try
            {
                depth = (int)nudDepth.Value; // кол-во рекурсивных вызовов, длина дерева.
                length = (int)nudLength.Value; // размер дерева
                if (auto.Checked)
                {
                    depth = length / 10 + 4;
                }
                alpha = (float)((double)nudAlpha.Value * Math.PI / 180.0); // угол поворота дерева
                root_x = Panel_pain.ClientSize.Width / 2 + (float)numericUpDown1.Value * 10 + mouse_x_offset;
                root_y = Panel_pain.ClientSize.Height * 0.95f + (float)numericUpDown2.Value * 10 + mouse_y_offset;
                VectorF v_base = new VectorF(length, 0);
                PointF ll_corner = new PointF(root_x, root_y) - v_base / 2;

                Brush brush = null;
                Brush brush1 = null;
                if (chkFill.Checked)
                {
                    brush = Brushes.Red;
                    brush1 = Brushes.LightGreen;
                }

                DrawBranch(gr, Pens.Black, brush, brush1,
                    depth, ll_corner, v_base, alpha);
            }
            catch
            {

            }
        }

        // Рекурсивная функция отрисовки ветвей.
        private void DrawBranch(Graphics grf, Pen pen, Brush brush, Brush brush1,
            int depth, PointF ll_corner, VectorF v_base, float alpha)
        {
            // находим координаты точек квадрата.
            VectorF v_height = v_base.PerpendicularCCW();
            PointF[] points =
            {
                ll_corner,
                ll_corner + v_base,
                ll_corner + v_base + v_height,
                ll_corner + v_height,
            };

            // Рисуем квадрат.
            if (brush1 != null)
            {
                gr.FillPolygon(brush1, points);
                if (v_base.Length < 15) gr.FillPolygon(brush, points);
            }
            gr.DrawPolygon(pen, points);
            Panel_pain.Image = image;

            // если глубина не 0 рисуем ветви.
            if (depth > 0)
            {
                // ***********
                // Левая ветвь
                // ***********
                // новая длина стороны.
                double w1 = v_base.Length * Math.Cos(alpha);

                //  координаты нового базового вектора.
                float wb1 = (float)(w1 * Math.Cos(alpha));
                float wh1 = (float)(w1 * Math.Sin(alpha));
                VectorF v_base1 = v_base.Scale(wb1) + v_height.Scale(wh1);

                // нижняя левая координата.
                PointF ll_corner1 = ll_corner + v_height;

                // отрисовка.
                DrawBranch(gr, pen, brush, brush1, depth - 1, ll_corner1, v_base1, alpha);

                // ************
                // Правая ветвь
                // ************
                // новая длина.
                double beta = Math.PI / 2.0 - alpha;
                double w2 = v_base.Length * Math.Sin(alpha);

                // новый базовый вектор.
                float wb2 = (float)(w2 * Math.Cos(beta));
                float wh2 = (float)(w2 * Math.Sin(beta));
                VectorF v_base2 = v_base.Scale(wb2) - v_height.Scale(wh2);

                // нижняя левая координата.
                PointF ll_corner2 = ll_corner1 + v_base1;

                // отрисовка.
                DrawBranch(gr, pen, brush, brush1, depth - 1, ll_corner2, v_base2, alpha);
            }
        }

        string xmlPath = @"D:\БНТУ\2курс\лабы\РПВС\4сем\coursework\file.xml";
        string root = "xRoot";
        string title = "dataTree";

        string stepTree = "step";
        string lengthTree = "length";
        string angleTree = "angle";
        string offsetX = "offsetX";
        string offsetY = "offsetY";

        public void saveInXmlFile()
        {
            // Создание XML документа
            XmlDocument document = new XmlDocument();

            // Создание главного узла XML документа
            XmlNode rootNode = document.CreateElement(root);
            XmlNode titleNode = document.CreateElement(title);

            // Добавление узла в документ
            document.AppendChild(rootNode);

            XmlNode stepXNode = document.CreateElement(stepTree);
            XmlText tmp = document.CreateTextNode(nudDepth.Value + "");
            stepXNode.AppendChild(tmp);
            titleNode.AppendChild(stepXNode);
            rootNode.AppendChild(titleNode);

            XmlNode lengthNode = document.CreateElement(lengthTree);
            tmp = document.CreateTextNode(nudLength.Value + "");
            lengthNode.AppendChild(tmp);
            titleNode.AppendChild(lengthNode);
            rootNode.AppendChild(titleNode);

            XmlNode angleNode = document.CreateElement(angleTree);
            tmp = document.CreateTextNode(nudAlpha.Value + "");
            angleNode.AppendChild(tmp);
            titleNode.AppendChild(angleNode);
            rootNode.AppendChild(titleNode);

            XmlNode offsetXNode = document.CreateElement(offsetX);
            tmp = document.CreateTextNode(numericUpDown1.Value + "");
            offsetXNode.AppendChild(tmp);
            titleNode.AppendChild(offsetXNode);
            rootNode.AppendChild(titleNode);

            XmlNode offsetYNode = document.CreateElement(offsetY);
            tmp = document.CreateTextNode(numericUpDown2.Value + "");
            offsetYNode.AppendChild(tmp);
            titleNode.AppendChild(offsetYNode);
            rootNode.AppendChild(titleNode);

            // Сохранение документа по выбранному пути
            document.Save(xmlPath);
        }

        public void openXmlFile()
        {
            // Создание XML документа
            XmlDocument document = new XmlDocument();
            // Загрузка данных из XML в document
            document.Load(xmlPath);
            // Выделяем нужные узлы для записи
            XmlNodeList ListBoxNodes = document.SelectNodes("//" + title + "/" + stepTree);
            foreach (XmlNode ListBoxNode in ListBoxNodes)
            {
                nudDepth.Value = Convert.ToDecimal(ListBoxNode.InnerText);
            }

            ListBoxNodes = document.SelectNodes("//" + title + "/" + lengthTree);
            foreach (XmlNode ListBoxNode in ListBoxNodes)
            {
                nudLength.Value = Convert.ToDecimal(ListBoxNode.InnerText);
            }

            ListBoxNodes = document.SelectNodes("//" + title + "/" + angleTree);
            foreach (XmlNode ListBoxNode in ListBoxNodes)
            {
                nudAlpha.Value = Convert.ToDecimal(ListBoxNode.InnerText);
            }

            ListBoxNodes = document.SelectNodes("//" + title + "/" + offsetX);
            foreach (XmlNode ListBoxNode in ListBoxNodes)
            {
                numericUpDown1.Value = Convert.ToDecimal(ListBoxNode.InnerText);
            }

            ListBoxNodes = document.SelectNodes("//" + title + "/" + offsetY);
            foreach (XmlNode ListBoxNode in ListBoxNodes)
            {
                numericUpDown2.Value = Convert.ToDecimal(ListBoxNode.InnerText);
            }

        }

        private void nudDepth_ValueChanged(object sender, EventArgs e)
        {
            Panel_Paint();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveInXmlFile();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveInXmlFile();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SplashScreen splashScreen = new SplashScreen();
            splashScreen.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenGL form = new OpenGL();
            form.ShowDialog();
        }

        private void сохранитьКартинкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Сохранить изображение как...";
            saveDialog.OverwritePrompt = true; // предупреждение, если пользователь указывает имя уже существующего файла
            saveDialog.CheckPathExists = true; // предупреждение, если пользователь указывает несуществующий путь
            saveDialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG" +
                "|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
            saveDialog.ShowHelp = true; // кнопка "Справка" в диалоговом окне

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image.Save(saveDialog.FileName);
                }
                catch
                {
                    MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveInXmlFile();
            this.Close();
        }

        private void отчетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Word.Application app = new Word.Application();
            app.Visible = true;
            app.Documents.Open(@"D:\БНТУ\2курс\лабы\РПВС\4сем\coursework\Пояснительная записка.docx");
        }

        private void калькуляторToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Calculator calculator = new Calculator();
            calculator.Show();
        }

        private void открытьHelpфайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, @"D:\БНТУ\2курс\лабы\РПВС\4сем\coursework\helpFile.chm");
        }

        PowerPoint.Application objApp;
        PowerPoint.Presentations objPresSet;
        PowerPoint._Presentation objPres;
        PowerPoint.SlideShowWindows objSSWs;
        PowerPoint.SlideShowSettings objSSS;

        private void ShowPresentation() //открытие презентации
        {
            objApp = new PowerPoint.Application();
            objPresSet = objApp.Presentations;
            objPres = objPresSet.Open(@"D:\БНТУ\2курс\лабы\РПВС\4сем\coursework\presentation.pptx"); // что открываем            
            objSSS = objPres.SlideShowSettings;
            objSSS.StartingSlide = 1;
            objSSS.EndingSlide = 3;
            objSSS.Run();
        }

        private void презентацияToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowPresentation();
            GC.Collect(); // очистка памяти
        }

        private void оПрограммеToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            
        }

        private void открытьВOpenGLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenGL form = new OpenGL();
            form.ShowDialog();
        }

        private void шагToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string promptValue = Prompt.ShowDialog("Шаг", "Шаг");
            int depth = Convert.ToInt32(promptValue);
            if (depth >= 0 && depth <= 15)
            {
                nudDepth.Value = depth;
            }
            else
            {
                MessageBox.Show("Введенные данные не входят в допустимый диапазон.");
            }
        }

        private void длинаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string promptValue = Prompt.ShowDialog("Длина", "Длина");
            int length = Convert.ToInt32(promptValue);
            if (length > 0 && length <= 100)
            {
                nudLength.Value = length;
            }
            else
            {
                MessageBox.Show("Введенные данные не входят в допустимый диапазон.");
            }
        }

        private void уголToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string promptValue = Prompt.ShowDialog("Угол", "Угол");
            int angle = Convert.ToInt32(promptValue);
            if (angle >= 0 && angle <= 360)
            {
                nudAlpha.Value = angle;
            }
            else
            {
                MessageBox.Show("Введенные данные не входят в допустимый диапазон.");
            }
        }

        private void oxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string promptValue = Prompt.ShowDialog("Смещение по x", "Смещение по x");
            int offsetX = Convert.ToInt32(promptValue);
            if (offsetX >= -500 && offsetX <= 500)
            {
                numericUpDown1.Value = offsetX;
            }
            else
            {
                MessageBox.Show("Введенные данные не входят в допустимый диапазон.");
            }
        }

        private void oyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string promptValue = Prompt.ShowDialog("Смещение по y", "Смещение по y");
            int offsetY = Convert.ToInt32(promptValue);
            if (offsetY >= -500 && offsetY <= 500)
            {
                numericUpDown2.Value = offsetY;
            }
            else
            {
                MessageBox.Show("Введенные данные не входят в допустимый диапазон.");
            }
        }
    }
}

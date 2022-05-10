using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace coursework
{
    public partial class OpenGL : Form
    {
        ObjectGraph objectGraph; // Объект визуализации модели

        
        /*public int angle;
        public double offsetX, offsetY;
        public int depth;
        public int length;
        public int tmp = 0;*/

        public OpenGL()
        {
            InitializeComponent();
            anT.InitializeContexts(); // Инициализация элемента anT
        }

        private void openGL_Load(object sender, EventArgs e)
        {
            this.Text = "Pythagorean Tree - openGl";
            objectGraph = new ObjectGraph(anT);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            objectGraph.display(Convert.ToDouble(numericUpDown1.Value), Convert.ToDouble(numericUpDown2.Value), Convert.ToDouble(numericUpDown3.Value) * Math.PI / 180, Convert.ToDouble(numericUpDown4.Value) * Math.PI / 180, Convert.ToDouble(numericUpDown5.Value), Convert.ToInt32(numericUpDown6.Value));
        }

        private void anT_Click(object sender, EventArgs e)
        {
            objectGraph.display(Convert.ToDouble(numericUpDown1.Value), Convert.ToDouble(numericUpDown2.Value), Convert.ToDouble(numericUpDown3.Value) * Math.PI / 180, Convert.ToDouble(numericUpDown4.Value) * Math.PI / 180, Convert.ToDouble(numericUpDown5.Value), Convert.ToInt32(numericUpDown6.Value));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Drawing;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace coursework
{
    public class ObjectGraph
    {
        SimpleOpenGlControl openGlControl; // Канва для рисования 3D-модели
        public int angleX;                 // Угол взгляда на 3D-модель по вертикали 
        public int angleY;

        public ObjectGraph(SimpleOpenGlControl openGlControl)
        {
            this.openGlControl = openGlControl;
            Init();
        }

        /* Функция, прорисовывающая изображение в окне */
        public void display(double x, double y, double angleTree, double angle, double len, int depth)
        {
            /* Очистка экрана цветом по-умолчанию */
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

            /* Задание цвета для рисования */
            Gl.glColor3f(1.0f, 0.2f, 0.5f);

            /* Начало рисования. Вершины 2n и 2n+1 будут обьединены в линию*/
            Gl.glBegin(Gl.GL_LINES);

            /* Вызов функции, рисующей фрактал. В функции передаются вершины в буфер */
            //func(50, 33, 1.1, 50, 13);
            func(x, y, angleTree, angle, len, depth);

            openGlControl.SwapBuffers();

            /* Функция, выгружающая буфер в сцену */
            Gl.glEnd();

            /*  Прорисовка текущего буфера */
            Gl.glFlush();
        }

        /* Функция, определённая выше */
        public void func(double x, double y, double angleTree, double angle, double len, int depth)
        {
            //double angp = 1; //изменение угла
            for (int i = -1; i < 5; i += 2)
            {
                //если не достигнута глубина рекурсии - продолжить построение фрактала
                if (depth > 0)
                    //func(x + Math.Cos(angle + i * angp) * len / 2, y + Math.Sin(angle + i * angp) * len / 2, angle + i * angp, len / 2, deph - 1);
                    func(x + Math.Cos(angleTree + i * angle) * len / 2, y + Math.Sin(angleTree + i * angle) * len / 2, angleTree + i * angle, angle, len / 2, depth - 1);
                /* В буфер записываются вершины. Эти две вершины будут соединены в прямые между собой*/
                Gl.glVertex2d(x, y);
                Gl.glVertex2d(x + Math.Cos(angleTree + i * angle) * len / 2, y + Math.Sin(angleTree + i * angle) * len / 2);
            }
        }

        public void Init()
        {
            /* Установка цвета по-умолчанию */
            Gl.glClearColor(0, 0, 0, 0);
            /* Задаются параметры сцены. Матрица преобразований для проекции, координаты сдвигаются в 0 и границы экрана устанавливаются в заданные*/
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0.0, 100.0, 0.0, 100.0, -100.0, 100.0);
        }
    }
}
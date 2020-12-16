﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Курсовая
{
    public partial class Form1 : Form
    {
        Emitter emitter;
        public Color colorPicture=Color.White;
        bool ifRun = true;
        bool ifColor = false;
        bool stepPermission = false;

        public Form1()
        {
            InitializeComponent();
            picDisplay.Image = new Bitmap(picDisplay.Width, picDisplay.Height);

            // а тут теперь вручную создаем
            emitter = new TopEmitter
            {
                Width = picDisplay.Width,
                gravitationY = 5
            };
            emitter.rect = tbSize.Value * 10;
            emitter.Radius = tbSize.Value * 5;
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((tbSpeed.Value != 0 && ifRun) || stepPermission)
            {
                emitter.UpdateState();
            }
            using (var g = Graphics.FromImage(picDisplay.Image))
            {
                g.Clear(colorPicture);
                emitter.Render(g);
                if (ifColor == true)
                {
                    drawPR(g);
                    emitter.MakeColor(picDisplay.Width);
                }
                if (emitter.figure == "square")
                {
                    Particle particle = emitter.ifMouseInSquare();
                    if (particle != null)
                    {
                        if (particle.Form == "square")
                        {
                            particle.size = emitter.rect;

                            drawSquareStroke(g, particle);
                            ShowInfo(g, particle);
                        }
                    }
                }
                else if (emitter.figure == "circle")
                {
                    Particle particle = emitter.ifMouseInCircle();
                    if (particle != null)
                    {
                        if (particle.Form == "circle")
                        {
                            particle.Radius = emitter.Radius;
                            DrawCircle(g, particle);
                            ShowInfo(g, particle);
                        }
                    }
                }
                else {
                    Particle particle = emitter.ifMouseInStar();
                    if (particle != null)
                    {
                        if (particle.Form == "star")
                        {
                            particle.Radius = emitter.Radius;
                            DrawStar(g, particle);
                            ShowInfo(g, particle);
                        }
                    }
                }
            }
            picDisplay.Invalidate();
            stepPermission = false;
        }
        private void drawSquareStroke(Graphics g, Particle particle)
        {
            Pen pen = new Pen(Color.Black);
            g.DrawRectangle(pen, particle.X, particle.Y, particle.size, particle.size);
        }
        public void DrawStar(Graphics g, Particle particle) {
            
            double alpha = 0;        // поворот
            PointF[] points = new PointF[2 * 5 + 1];
            double a = alpha, da = Math.PI / 5, l;
            for (int k = 0; k < 2 * 5 + 1; k++)
            {
                l = k % 2 == 0 ? particle.Radius*2 : particle.Radius;
                points[k] = new PointF((float)(particle.X + l * Math.Cos(a)), (float)(particle.Y + l * Math.Sin(a)));
                a += da;
            }
            g.DrawLines(Pens.Black, points);
        }
        
        private void ChangeTick()
        {
            switch (tbSpeed.Value)
            {
                case 0:
                    ifRun = false;
                    break;
                case 1:
                    emitter.tickRate = 30;
                    break;
                case 2:
                    emitter.tickRate = 25;
                    break;
                case 3:
                    emitter.tickRate = 20;
                    break;
                case 4:
                    emitter.tickRate = 15;
                    break;
                case 5:
                    emitter.tickRate = 10;
                    break;
                case 6:
                    emitter.tickRate = 7;
                    break;
                case 7:
                    emitter.tickRate = 5;
                    break;
                case 8:
                    emitter.tickRate = 3;
                    break;
                case 9:
                    emitter.tickRate = 2;
                    break;
                case 10:
                    emitter.tickRate = 1;
                    break;
            }
        }
        
        public void drawSpeedVector()
        {
            Graphics speedVector = picDisplay.CreateGraphics();

            foreach (var particle in emitter.particles)
            {
                int deviation = (int)(particle.SpeedX * 9);
                Pen pen = new Pen(Brushes.Green);
                speedVector.DrawLine(pen, new Point((int)particle.X, (int)particle.Y),
                    new Point((int)(particle.X + particle.Radius * Math.Cos(deviation - 90)),
                    (int)(particle.Y + particle.Radius * Math.Sin(deviation - 90))));
            }
        }
        private void Start_Click(object sender, EventArgs e)
        {
            ifRun = true;
            ChangeTick();
        }
        private void Stop_Click(object sender, EventArgs e)
        {
            ifRun = false;
        }
        private void Step_Click(object sender, EventArgs e)
        {
            ifRun = false;
            if (emitter.currentHistoryIndex < emitter.particlesHistory.Count - 1 && emitter.currentHistoryIndex != 19)
            {
                //поставить значения дальше по списку
                emitter.particles.RemoveRange(0, emitter.particles.Count);
                foreach (ParticleColorful particle in emitter.particlesHistory[emitter.currentHistoryIndex + 1])
                {
                    ParticleColorful part = new ParticleColorful(particle);
                    part.FromColor = emitter.ColorFrom;
                    part.ToColor = emitter.ColorTo;
                    part.Form = emitter.figure;
                    part.size = emitter.rect;
                    part.Radius = emitter.Radius;
                    emitter.particles.Add(part);
                }
                emitter.currentHistoryIndex++;
            }
            else
            {
                emitter.tickCount += (emitter.tickRate - emitter.tickCount % emitter.tickRate);
                stepPermission = true;
            }
        }
        private void picDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            emitter.X = e.X;
            emitter.Y = e.Y;
        }
        private void DrawCircle(Graphics g, Particle particle)
        {
            Pen pen = new Pen(Brushes.Black);
            g.DrawEllipse(pen, particle.X - particle.Radius, particle.Y - particle.Radius, particle.Radius * 2, particle.Radius * 2);
        }
        private void ShowInfo(Graphics g, Particle particle)
        {
            g.FillRectangle(
                    new SolidBrush(Color.FromArgb(125,Color.White)),
                    particle.X,
                    particle.Y - particle.Radius,
                    60,
                    50
                    );
            g.DrawString(
                $"X : {particle.X}\n" +
                $"Y : {particle.Y}\n" +
                $"Life : {particle.Life}",
                new Font("Verdana", 10),
                new SolidBrush(Color.Black),
                particle.X,
                particle.Y - particle.Radius
                );
        }
        private void StepBack_Click(object sender, EventArgs e)
        {
            ifRun = false;
            if (emitter.currentHistoryIndex >= 2)
            {
                //вернуться на значения из списка
                emitter.particles.RemoveRange(0, emitter.particles.Count);
                foreach (ParticleColorful particle in emitter.particlesHistory[emitter.currentHistoryIndex - 1])
                {
                    
                    ParticleColorful part = new ParticleColorful(particle);
                    if (particle.ifColorBefore) { 
                        part.FromColor = emitter.ColorFrom;
                        part.ToColor = emitter.ColorFrom;
                    }
                    else
                    {
                        particle.FromColor = emitter.ColorFrom;
                        particle.ToColor = emitter.ColorTo;
                    }
                    part.Form = emitter.figure;
                    part.size = emitter.rect;
                    part.Radius = emitter.Radius;
                    emitter.particles.Add(part);
                }
                emitter.currentHistoryIndex--;
            }
        }
        private void tbSpeed_ValueChanged(object sender, EventArgs e)
        {
            ifRun = true;
            ChangeTick();
        }
        private void tbNumber_Scroll_1(object sender, EventArgs e)
        {
            emitter.ParticlesPerTick =  tbNumber.Value;
        }
        private void tbSize_Scroll(object sender, EventArgs e)
        {
            if (emitter.figure == "circle"|| emitter.figure == "star" )
            {
                emitter.Radius = 5 * tbSize.Value;
            }
            else
            {
                emitter.rect = 10 * tbSize.Value;
            }
        }
        private void RandomColorParticles_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int R = random.Next(255);
            int G = random.Next(255);
            int B = random.Next(255);
            emitter.ColorFrom = Color.FromArgb(R, G, B);
            emitter.ColorTo = colorPicture;
        }
        private void RandomColorPictures_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int R = random.Next(255);
            int G = random.Next(255);
            int B = random.Next(255);
            var g = Graphics.FromImage(picDisplay.Image);
            colorPicture = Color.FromArgb(R, G, B);
            emitter.ColorTo = colorPicture;
        }
        private void ColorParticles_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = false;
            MyDialog.Color = emitter.ColorFrom;
            if (MyDialog.ShowDialog() == DialogResult.OK)
                emitter.ColorFrom = MyDialog.Color;
            emitter.ColorTo = colorPicture;
        }
        private void ColorPictures_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = false;
            MyDialog.Color = colorPicture;
            if (MyDialog.ShowDialog() == DialogResult.OK)
                colorPicture = MyDialog.Color;
            emitter.ColorTo = colorPicture;

        }
        private void cmbForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbForm.Text)
            {
                case "Круг":
                    emitter.figure = "circle";
                    break;
                case "Квадрат":
                    emitter.figure = "square";
                    break;
                case "Звезда":
                    emitter.figure = "star";
                    break;
            }
            emitter.removeList();
            tbNumber_Scroll_1(sender, e);
        }
        private void tbLife_Scroll(object sender, EventArgs e)
        {
            emitter.LifeMax =10* tbLife.Value;
            if (emitter.LifeMax <= emitter.LifeMin)
            {
                emitter.LifeMin = 0;
            }
            else emitter.LifeMin = emitter.LifeMax / 2;
        }
        public void drawPR(Graphics g)
        {
            g.DrawRectangle(new Pen(Color.Red,3),0, 100, picDisplay.Width / 7, 40);

            g.DrawRectangle(new Pen(Color.Orange, 3), picDisplay.Width / 7, 110, (picDisplay.Width /7), 40);

            g.DrawRectangle(new Pen(Color.Yellow,3), (picDisplay.Width / 7) *2, 120, (picDisplay.Width /7), 40);
            
            g.DrawRectangle(new Pen(Color.Green, 3), (picDisplay.Width / 7)*3, 130, (picDisplay.Width / 7), 40);

            g.DrawRectangle(new Pen(Color.DodgerBlue, 3), (picDisplay.Width / 7)*4, 120, picDisplay.Width / 7, 40);

            g.DrawRectangle(new Pen(Color.Blue, 3), (picDisplay.Width /  7)* 5, 110, picDisplay.Width / 7, 40);
           
            g.DrawRectangle(new Pen(Color.Violet, 3), (picDisplay.Width /7)*6 , 100, picDisplay.Width / 7, 40);
            
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text) {
                case "Простой":
                    {
                        ifColor = false;
                        colorPicture = Color.White;
                        emitter.ColorFrom = Color.Black;
                        emitter.ColorTo = colorPicture;
                        emitter.LifeMax = 50;
                        emitter.LifeMin = 25;
                        emitter.ParticlesPerTick = 10;
                        if (emitter.figure == "circle" || emitter.figure == "star")
                        {
                            emitter.Radius = 5 * tbSize.Value;
                        }
                        else
                        {
                            emitter.rect = 10 * tbSize.Value;
                        }
                    }
                    break;
                case "Окрашивание":
                    {
                        ifColor = true;
                        colorPicture = Color.Black;
                        emitter.ColorFrom = Color.White;
                        emitter.ColorTo = colorPicture;
                        emitter.LifeMax = 100;
                        emitter.LifeMin =100;
                        emitter.ParticlesPerTick = 10;
                        if (emitter.figure == "circle" || emitter.figure == "star")
                        {
                            emitter.Radius = 5 * 2;
                        }
                        else
                        {
                            emitter.rect = 10 * 2;
                        }
                    }
                    break;
            }
        }
    }
}

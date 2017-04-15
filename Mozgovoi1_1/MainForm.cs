using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mozgovoi1_1
{
    public partial class MainForm : Form
    {
        private static Random rnd = new Random();
        private static Timer timer = new Timer();
        private const double coefficientNotCollisen = 0.1;
        private const int vxmax = 3;
        private const int vymax = 2;
        private const int rmin = 3;
        private const int rmax = 8;

        private Graphics canvas;
        private SolidBrush solidBrush;
        private SolidBrush eraser;

        private List<Molecule> molecules = new List<Molecule>();

        public MainForm()
        {
            InitializeComponent();

            canvas = canvasPanel.CreateGraphics();
            solidBrush = new SolidBrush(Color.Purple);
            eraser = new SolidBrush(canvasPanel.BackColor);

            timer.Interval = 15;
            timer.Tick += delegate(object sender, EventArgs e) { DoStep(); };
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Enabled = false;
                startButton.Text = "Старт";
            }
            else
            {
                timer.Enabled = true;
                startButton.Text = "Стоп";
            }
        }

        private void canvasPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Molecule molecule = new Molecule(e.X, e.Y, rnd.Next(-vxmax, vxmax), rnd.Next(-vymax, vymax), rnd.Next(rmin, rmax), 0);
            molecules.Add(molecule);

            drawMolecule(solidBrush, molecule);
        }

        private void drawMolecule(SolidBrush solidBrush, Molecule molecule)
        {
            canvas.FillEllipse(solidBrush,
                (int)(molecule.X - molecule.Radius), (int)(molecule.Y - molecule.Radius),
                (int)(molecule.Radius * 2), (int)(molecule.Radius * 2));
        }

        private Color ColorEnergy(double internalEnergy)
        {
            double energyMax = rmax * (vxmax * vxmax + vymax * vymax) / 2;
            if ((internalEnergy >= 0) && (internalEnergy < energyMax / 7))
                return Color.Purple;

            if ((internalEnergy >= energyMax / 7) && (internalEnergy < 2 * energyMax / 7))
                return Color.Blue;

            if ((internalEnergy >= 2 * energyMax / 7) && (internalEnergy < 3 * energyMax / 7))
                return Color.Cyan;

            if ((internalEnergy >= 3 * energyMax / 7) && (internalEnergy < 4 * energyMax / 7))
                return Color.Blue;

            if ((internalEnergy >= 4 * energyMax / 7) && (internalEnergy < 5 * energyMax / 7))
                return Color.Yellow;

            if ((internalEnergy >= 5 * energyMax / 7) && (internalEnergy < 6 * energyMax / 7))
                return Color.Orange;

            if (internalEnergy >= 6 * energyMax / 7)
                return Color.Red;

            return Color.Black;
        }

        private void DoStep()
        {
            for (int index1 = 0; index1 < molecules.Count; index1++)
            {
                Molecule molecule = molecules[index1];

                drawMolecule(eraser, molecule);

                molecule.DoStep();

                if (molecule.X <= 0 + molecule.Radius ||
                        molecule.X >= canvasPanel.Width - molecule.Radius)
                    molecule.FlipHorizontally();

                if (molecule.Y <= 0 + molecule.Radius ||
                    molecule.Y >= canvasPanel.Height - molecule.Radius)
                    molecule.FlipVertically();

                for (int index2 = index1 + 1; index2 < molecules.Count; index2++)
                {
                    if (Molecule.HasCollided(molecules[index1], molecules[index2]))
                    {
                        Molecule.ProcessCollision(molecules[index1], molecules[index2], coefficientNotCollisen);
                    }
                }

                drawMolecule(solidBrush, molecule);
            }
        }

    }
}


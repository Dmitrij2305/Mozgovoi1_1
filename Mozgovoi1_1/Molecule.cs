using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mozgovoi1_1
{
    public class Molecule
    {
        private double radius;

        private double x;
        private double y;

        private double vx;
        private double vy;

        private double internalEnergy;

        public double X
        {
            get { return x; }
        }

        public double Y
        {
            get { return y; }
        }

        public double Radius
        {
            get { return radius; }
        }

        public Molecule(double x, double y, double vx, double vy, double radius, double internalEnergy)
        {
            this.radius = radius;

            this.x = x;
            this.y = y;

            this.vx = vx;
            this.vy = vy;

            this.internalEnergy = internalEnergy;
        }

        public Molecule(double x, double y, double radius)
            : this(x, y, 0, 0, radius, 0)
        { }

        public void DoStep()
        {
            x += vx;
            y += vy;
        }

        public void FlipHorizontally()
        {
            vx = -vx;
        }

        public void FlipVertically()
        {
            vy = -vy;
        }

        public static double Distance(Molecule molecule1, Molecule molecule2)
        {
            return Math.Sqrt((molecule2.x - molecule1.x) * (molecule2.x - molecule1.x) + 
                             (molecule2.y - molecule1.y) * (molecule2.y - molecule1.y));
        }

        public static bool HasCollided(Molecule molecule1, Molecule molecule2)
        {
            double squaredDistance = (molecule1.X - molecule2.X) * (molecule1.X - molecule2.X) +
                                  (molecule1.Y - molecule2.Y) * (molecule1.Y - molecule2.Y);

            return squaredDistance <= (molecule1.Radius + molecule2.Radius) * (molecule1.Radius + molecule2.Radius);
        }

        internal static void ProcessCollision(Molecule molecule1, Molecule molecule2, double k)
        {
            Line line = new Line { X1 = molecule1.x, Y1 = molecule1.y, X2 = molecule2.x, Y2 = molecule2.y };
            double v1ProjectionBefore = molecule1.getVelocityProjectionTo(line);
            double v2ProjectionBefore = molecule2.getVelocityProjectionTo(line);

            double m1 = molecule1.Radius;
            double m2 = molecule2.Radius;

            double v1ProjectionAfter = (v1ProjectionBefore * (m1 - m2) + 2 * m2 * v2ProjectionBefore) / (m1 + m2);
            double v2ProjectionAfter = (v2ProjectionBefore * (m2 - m1) + 2 * m1 * v1ProjectionBefore) / (m1 + m2);

            double r = Molecule.Distance(molecule1, molecule2);
            double cosfi = (molecule2.x - molecule1.x) / r;
            double sinfi = (molecule2.y - molecule1.y) / r;

            molecule1.vx -= (1 - k) * (v1ProjectionBefore - v1ProjectionAfter) * cosfi;
            molecule2.vx -= (1 - k) * (v2ProjectionBefore - v2ProjectionAfter) * cosfi;
            molecule1.vy -= (1 - k) * (v1ProjectionBefore - v1ProjectionAfter) * sinfi;
            molecule2.vy -= (1 - k) * (v2ProjectionBefore - v2ProjectionAfter) * sinfi;

            molecule1.internalEnergy += (k * k * m2 * ((molecule2.vx) * (molecule2.vx) + (molecule2.vy) * (molecule2.vy))) /
                (2 * (1 - k) * (1 - k));

            molecule2.internalEnergy += (k * k * m1 * ((molecule1.vx) * (molecule1.vx) + (molecule1.vy) * (molecule1.vy))) /
                (2 * (1 - k) * (1 - k));
        }

        private double getVelocityProjectionTo(Line line)
        {
            return (vx * (line.X2 - line.X1) + vy * (line.Y2 - line.Y1)) / 
                Math.Sqrt((line.X2 - line.X1) * (line.X2 - line.X1) + (line.Y2 - line.Y1) * (line.Y2 - line.Y1));
        }
    }
}

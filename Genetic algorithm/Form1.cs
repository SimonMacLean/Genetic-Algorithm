using System;
using System.Drawing;
using System.Windows.Forms;

namespace Genetic_algorithm
{

    public partial class Form1 : Form
    {
        public Fighter yellowFighter;
        public Fighter blueFighter;
        public Form1()
        {
            InitializeComponent();
        }
    }

    public class Fighter
    {
        public static int fighterSize = 5;
        private static Random mutationRandom = new Random();
        private static int neuronNumber = 10;
        private string DNA;
        public int X, Y;
        short degreesRotated = 180;
        private double seesEnemy, seesEnemyBullet, hasFired, feildOfVisionSize;
        private Neuron[][] neurons = new Neuron[2][] { new Neuron[5], new Neuron[5] };
        public Fighter(Random DNARandom)
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    string currentNeuronDNA = string.Empty;
                    for (int k = 0; k < i + 4; k++)
                    {
                        string currentNum = DNARandom.Next(1000).ToString().PadLeft(3, '0');
                        currentNeuronDNA += currentNum;
                    }
                    DNA += currentNeuronDNA;
                }
            }
            getNeurons();
        }

        public Fighter(Fighter parent1, Fighter parent2, int splittingPlace, bool childNum)
        {
            string[] parent1Sections = { parent1.DNA.Substring(0, splittingPlace * 3), parent1.DNA.Substring(splittingPlace * 3) };
            string[] parent2Sections = { parent2.DNA.Substring(0, splittingPlace * 3), parent2.DNA.Substring(splittingPlace * 3) };
            DNA = childNum ? parent1Sections[0] + parent2Sections[1] : parent1Sections[1] + parent2Sections[0];
            getNeurons();
        }
        private void getNeurons()
        {
            mutate();
            for (int i = 0; i < neurons[0].Length; i++)
            {
                string currentNeuronDNA = DNA.Substring(i * 15, 15);
                neurons[0][i] = new Neuron(currentNeuronDNA, new double[4] { seesEnemy, seesEnemyBullet, hasFired, feildOfVisionSize });
            }
            for (int i = 0; i < neurons[1].Length; i++)
            {
                string currentNeuronDNA = DNA.Substring(neurons[0].Length * 15 + i * (neurons[0].Length + 1) * 3, (neurons[0].Length + 1) * 3);
                double[] inputs = new double[neurons[0].Length];
                for (int j = 0; j < inputs.Length; j++)
                {
                    inputs[j] = neurons[0][j].output;
                }
                neurons[0][i] = new Neuron(currentNeuronDNA, inputs);
            }
        }
        private void mutate()
        {
            int place = mutationRandom.Next(DNA.Length);
            string NewDNA = DNA.Substring(0, place) + mutationRandom.Next(10).ToString() + DNA.Substring(place + 1, DNA.Length - (place + 1));
            DNA = NewDNA;
        }
        private void turnLeft()
        {
            degreesRotated++;
            updateDegrees();
        }
        private void turnRight()
        {
            degreesRotated--;
            updateDegrees();
        }
        private void moveForward()
        {

        }
        private void Shoot()
        {

        }
        private void changeFeildOfView(int direction)
        {

        }
        private void updateDegrees()
        {
            if (degreesRotated > 180)
            {
                degreesRotated -= 360;
                return;
            }
            if (degreesRotated < -180)
            {
                degreesRotated += 360;
                return;
            }
        }
        public static Point target(Point location, int degrees)
        {
            Point result = location;
            double radians = Math.PI * degrees / 360.0;
            double resultX = location.X;
            double resultY = location.Y;
            while (resultX < 1000 && resultX > 0 && resultY < 1000 && resultY > 0)
            {
                resultX += Math.Cos(radians);
                resultY += Math.Sin(radians);
            }
            resultX -= Math.Cos(radians);
            resultY -= Math.Sin(radians);
            result = new Point((int)resultX, (int)resultY);
            return result;
        }
        public static Point nextPoint(Point location, Point target)
        {
            Point closestPoint = location;
            double closestLength = int.MaxValue / 2;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    double distanceFromTarget = Math.Sqrt(Math.Pow(target.X - (location.X + j), 2) + Math.Pow(target.Y - (location.Y + i), 2));
                    if (distanceFromTarget < closestLength)
                        closestPoint = new Point(location.X + j, location.Y + i);
                }
            }
            return closestPoint;
        }
   }

    public class Bullet
    {
        Point location;
        Point targetLocation;
        bool hitSomething;
        Fighter oppositeFighter;
        public Bullet(int X, int Y, int degreesRotated, Fighter enemyFighter)
        {
            location = new Point(X, Y);
            targetLocation = Fighter.target(location, degreesRotated);
            oppositeFighter = enemyFighter;
        }
        public void Update()
        {
            if (location.X < 0 || location.X > 1000 || location.Y < 0 || location.Y > 1000)
            {
                hitSomething = true;
            }
            if (Math.Sqrt(Math.Pow(location.X - oppositeFighter.X, 2) + Math.Pow(location.Y - oppositeFighter.Y, 2)) <= 5)
            {
                hitSomething = true;
                // TODO: Add a point to score
            }
        }
    }

    public class Neuron
    {
        public byte threshold;
        public int inputNumber;
        public byte[] weights;
        private double[] inputs;
        public Neuron(string gene, double[] inputs)
        {
            this.inputs = inputs;
            inputNumber = inputs.Length;
            weights = new byte[inputNumber];
            threshold = (byte)int.Parse(gene.Substring(0, 2));
            for (int i = 0; i < inputNumber; i++)
            {
                gene = gene.Substring(2);
                weights[i] = (byte)int.Parse(gene.Substring(0, 2));
            }
        }

        public int output
        {
            get
            {
                double total = 0;
                for (int i = 0; i < inputNumber; i++)
                {
                    total += inputs[i] * weights[i];
                }
                return total > threshold ? 1 : 0;
            }
        }
    }
}

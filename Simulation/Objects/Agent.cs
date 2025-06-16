using NeuralLife.NeuralNetwork;
using System;
using static TorchSharp.torch;

namespace NeuralLife.Simulation.Objects
{
    public class Agent : SimulationObject
    {
        private const float FoodSatiety = 20f;
        private AgentBrains Brains;

        private float Health = 100f;
        private float Energy = 100f;
        private float Satiety = 100f;

        private const float LowEnergyDamage = 10;
        private const byte RequiredColonialismNeighbors = 3;

        private Random Random;

        public Agent()
        {
            Color = new Color(0, 220, 0);
            Brains = new AgentBrains(CPU);
            Random = new Random();
        }

        private Agent(Color color, AgentBrains brains)
        {
            Color = color;
            Brains = brains;
            Random = new Random();
        }

        public void Damage(float damage)
        {
            Health -= damage;

            if(Health < 0)
            {
                Simulation.Destroy(this);
            }
        }

        protected override void OnUpdate()
        {
            UpdateStats();
            if(Satiety <= 0f || Energy <= 0f)
            {
                if(!SimulationSettings.AllowColonialism)
                {
                    Damage(LowEnergyDamage);
                }
                else
                {
                    byte neighborsCount = 0;
                    for(int i = -1; i < 2; i++)
                    {
                        for(int j = -1; j < 2; j++)
                        {
                            if(Simulation.GetAtPosition(
                                new(Position.X + i,
                                Position.Y + j)) is Agent)
                            {

                                neighborsCount++;
                            }
                        }
                    }

                    if(neighborsCount >= RequiredColonialismNeighbors)
                    {
                        Damage(LowEnergyDamage / 2);
                    }
                }
            }

            BrainsInputData inputData = new BrainsInputData();
            inputData.Health = Health;
            inputData.Energy = Energy;

            inputData.NeighborCellsColors = Simulation.GetNeighborsColors(Position);

            BrainsOutputData outputData = Brains.Forward(inputData);

            var movePosition = new Vector2(Position.X + outputData.MoveBias.X,
            Position.Y + outputData.MoveBias.Y);

            var objectAtMovePosition = Simulation.GetAtPosition(movePosition);

            if(outputData.MoveBias == Vector2.Zero)
            {
                Satiety--;
                return;
            }

            if(objectAtMovePosition is Agent || objectAtMovePosition is Spike)
            {
                Simulation.Destroy(this);
                return;
            }
            else if(objectAtMovePosition is Food)
            {
                Satiety += FoodSatiety;
                Simulation.Destroy(objectAtMovePosition);
            }
            else
            {
                Energy--;
                Simulation.Swap
                (
                    Position,
                    movePosition
                );
            }

            Satiety--;

            if(SimulationSettings.EnableAutoDivide)
            {
                if(outputData.WillDivide)
                {
                    Divide();
                }
            }
            else
            {
                if(Health > 50)
                {
                    Divide();
                }
            }
        }

        private void UpdateStats()
        {
            if(Satiety > 0)
            {
                Satiety -= 2f;
                Energy += 5f;
                Health += 2f;
            }
        }

        private void Divide()
        {
            Color childColor = Color;

            const int maxMutations = 5;
            const float mutationsScope = 0.5f;
            const int mutationsPerSample = 5;

            var mutatesCount = Random.Next(0, maxMutations);

            var brainsClone = Brains.Clone();

            for(int i = 0; i < mutatesCount; i++)
            {
                brainsClone.MutateWeights(mutationsPerSample, Random.NextSingle() * mutationsScope);
            }

            childColor = MutateColor(childColor, (byte)mutatesCount);
            Agent child = new Agent(childColor, brainsClone);

            child.Energy = Energy / 2;
            child.Satiety = Satiety / 2;
            child.Health = Health / 2;
            child.Simulation = Simulation;
            Energy /= 2;
            Satiety /= 2;
            Health /= 2;
            Simulation.SetAtPosition(child, new(Position.X + 1, Position.Y));
        }

        private Color MutateColor(Color childColor, byte colorChange)
        {
            switch(Random.Next(0, 3))
            {
                case 0:
                    childColor.R += (byte)Random.Next(-colorChange, colorChange);
                    break;
                case 1:
                    childColor.G += (byte)Random.Next(-colorChange, colorChange);
                    break;
                case 2:
                    childColor.B += (byte)Random.Next(-colorChange, colorChange);
                    break;
                default:
                    break;
            }

            return childColor;
        }
    }
}

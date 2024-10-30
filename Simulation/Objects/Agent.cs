﻿using NeuralLife.NeuralNetwork;
using System;
using System.Numerics;
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

        protected override void OnUpdate(ObjectEnvironmentData data)
        {
            UpdateStats();
            if(Satiety <= 0f || Energy <= 0f)
            {
                Health--;
            }

            if(Health <= 0f)
            {
                data.Simulation.DestroyAtPosition(data.Position);
                return;
            }

            BrainsInputData inputData = new BrainsInputData();
            inputData.Health = Health;
            inputData.Energy = Energy;

            inputData.NeighborCellsColors = data.Simulation.GetNeighborsColors(data.Position);

            BrainsOutputData outputData = Brains.Forward(inputData);

            var movePosition = new Vector2(data.Position.X + outputData.MoveBias.X,
        data.Position.Y + outputData.MoveBias.Y);

            var objectAtMovePosition = data.Simulation.GetAtPosition(movePosition);

            if(!outputData.WillMove)
            {
                Satiety--;
                return;
            }
            if(objectAtMovePosition is Agent)
            {
                Satiety--;
                return;
            }
            else if(objectAtMovePosition is Food)
            {
                Satiety += FoodSatiety;
                data.Simulation.DestroyAtPosition(movePosition);
            }
            else
            {
                Energy--;
                data.Simulation.Swap
                (
                    data.Position,
                    movePosition
                );
            }

            if(Health >= 50f)
            {
                Divide(data.Position, data.Simulation);
            }

            Satiety--;
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

        private void Divide(Vector2 selfPosition, Simulation simulation)
        {
            Color childColor = Color;

            const int maxMutations = 16;
            const float mutationsScope = 100f;

            var mutatesCount = Random.Next(0, maxMutations);

            var brainsClone = Brains.Clone();

            for(int i = 0; i < mutatesCount; i++)
            {
                brainsClone.MutateWeights(5, Random.NextSingle() * mutationsScope);
            }
            childColor = MutateColor(childColor, (byte)mutatesCount);
            Agent child = new Agent(childColor, brainsClone);

            child.Energy = Energy / 2;
            child.Satiety = Satiety / 2;
            child.Health = Health / 2;
            Energy /= 2;
            Satiety /= 2;
            Health /= 2;
            selfPosition.X += 1;
            simulation.SetAtPosition(child, selfPosition);
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


using System;
using System.Collections.Generic;
using TorchSharp.Modules;
using static TorchSharp.torch;

namespace NeuralLife.NeuralNetwork
{
    public class AgentBrains : nn.Module<Tensor, Tensor>
    {
        public AgentBrains(Device device) : base(nameof(AgentBrains))
        {
            Device = device;

            var layers = new List<nn.Module<Tensor, Tensor>>();

            layers.Add(nn.Linear(InputNeuronsCount, HiddenNeuronCounts[0], device: Device));

            for(int i = 0; i < HiddenNeuronCounts.Length - 1; i++)
            {
                layers.Add(nn.Linear(HiddenNeuronCounts[i], HiddenNeuronCounts[i + 1], device: Device));
            }

            layers.Add(nn.Linear(HiddenNeuronCounts[^1], OutputNeuronsCount, device:device));
            layers.Add(nn.Tanh());

            Model = nn.Sequential(layers.ToArray());

            RegisterComponents();
        }

        public const int InputNeuronsCount = 74;
        public readonly int[] HiddenNeuronCounts = { 28 };
        public const int OutputNeuronsCount = 4;

        private Device Device;
        private nn.Module<Tensor, Tensor> Model;

        public override Tensor forward(Tensor input)
        {
            return Model.forward(input);
        }

        public AgentBrains Clone()
        {
            return (AgentBrains)MemberwiseClone();
        }

        public void MutateWeights(int mutationCount, double mutationStrength = 0.1)
        {
            var random = new Random();

            foreach(var layer in Model.modules())
            {
                if(layer is Linear linearLayer)
                {
                    var weights = linearLayer.weight;
                    int totalWeights = (int)weights.numel();

                    var flatWeights = weights.flatten();
                    for(int i = 0; i < mutationCount; i++)
                    {
                        int index = random.Next(0, totalWeights);
                        flatWeights[i].add((random.NextSingle() * 2 - 1) * mutationStrength);
                    }
                }
            }
        }

        public unsafe BrainsOutputData Forward(BrainsInputData input)
        {
            BrainsOutputData outputData = new();

            //we don`t use alfa channel, so sizeof(Simulation.Color) - 1.
            int tensorValuesSize = input.NeighborCellsColors.Length * (sizeof(Simulation.Color) - 1) + 2;

            float[] tensorValues = new float[tensorValuesSize];
            for(int i = 0; i < tensorValuesSize - 2; i += 3)
            {
                tensorValues[i] = input.NeighborCellsColors[i / 3].R;
                tensorValues[i + 1] = input.NeighborCellsColors[i / 3].G;
                tensorValues[i + 2] = input.NeighborCellsColors[i / 3].B;
            }

            tensorValues[^2] = input.Energy;
            tensorValues[^1] = input.Health;

            Tensor inputData = tensor
                (
                    tensorValues,
                    device: Device
                );

            var result = forward(inputData);

            outputData.WillMove = result[0].item<float>() > 0.5f ? true : false;

            var xBias = result[1].item<float>();
            if(xBias < 0f)
            {
                outputData.MoveBias.X = -1;
            }
            else if(xBias == 0f)
            {
                outputData.MoveBias.X = 0;
            }
            else if(xBias > 0f)
            {
                outputData.MoveBias.X = 1;
            }

            var yBias = result[2].item<float>();
            if(yBias < 0f)
            {
                outputData.MoveBias.Y = -1;
            }
            else if(yBias == 0f)
            {
                outputData.MoveBias.Y = 0;
            }
            else if(yBias > 0f)
            {
                outputData.MoveBias.Y = 1;
            }

            var willDivide = result[3].item<float>();
            if(willDivide < 0f)
            {
                outputData.WillDivide = false;
            }
            if(willDivide > 0f)
            {
                outputData.WillDivide = true;
            }

            return outputData;
        }
    }
}

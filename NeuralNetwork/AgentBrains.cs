
using System;
using System.Collections.Generic;
using TorchSharp.Modules;
using static TorchSharp.torch;

namespace NeuralLife.NeuralNetwork
{
    public class AgentBrains : nn.Module<Tensor, Tensor>
    {
        private enum ActivateFunctions
        { 
            Tanh = -1,
            None = 0
        }
        public AgentBrains(Device device) : base(nameof(AgentBrains))
        {
            Device = device;

            var layers = new List<nn.Module<Tensor, Tensor>>();

            layers.Add(nn.Linear(InputNeuronsCount, HiddenNeuronCounts[0], device: Device));

            for(int i = 0; i < HiddenNeuronCounts.Length - 1; i++)
            {
                int nextLayerCountsIndex = HiddenNeuronCounts[i + 1];
                if(nextLayerCountsIndex <= 0)
                {
                    nextLayerCountsIndex = HiddenNeuronCounts[i + 2];
                }
                if(HiddenNeuronCounts[i] == (int)ActivateFunctions.Tanh)
                {
                    layers.Add(nn.Tanh());
                    i++; //strange, but we use "HiddenNeuronCounts[i + 1]" in line 33,
                    //this will avoid torch exception
                    continue;
                }
                layers.Add(nn.Linear(HiddenNeuronCounts[i], nextLayerCountsIndex, device: Device));
            }

            layers.Add(nn.Linear(HiddenNeuronCounts[^1], OutputNeuronsCount, device:device));
            layers.Add(nn.Tanh());

            Model = nn.Sequential(layers.ToArray());

            RegisterComponents();
        }

        public const int InputNeuronsCount = 74;
        public readonly int[] HiddenNeuronCounts = { 20, -1, 20, 20 };
        public const int OutputNeuronsCount = 10;

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

        public BrainsOutputData Forward(BrainsInputData input)
        {
            const int additionalInfoSize = 2;
            const int availableCellsToEnterCount = 9;
            BrainsOutputData outputData = new();

            int tensorValuesSize = input.NeighborCellsColors.Length * Simulation.Color.SelfSize + additionalInfoSize;

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
                        
            //Bias with the max weight will be the move direction.             

            float maxWeight = float.NegativeInfinity;
            byte indexOfMaxWeight = 10;

            for(byte i = 0; i < availableCellsToEnterCount; i++)
            {
                if(result[i].item<float>() > maxWeight)
                {
                    indexOfMaxWeight = i;
                    maxWeight = result[i].item<float>();
                }
            }

            //oh god, this is govnocode
            switch(indexOfMaxWeight)
            {
                case 0:
                    outputData.MoveBias = new(-1, -1);
                    break;
                case 1:
                    outputData.MoveBias = new(-1, 0);
                    break;
                case 2:
                    outputData.MoveBias = new(-1, 1);
                    break;
                case 3:
                    outputData.MoveBias = new(0, -1);
                    break;
                case 4:
                    outputData.MoveBias = new(0, 0);
                    break;
                case 5:
                    outputData.MoveBias = new(0, 1);
                    break;
                case 6:
                    outputData.MoveBias = new(1, -1);
                    break;
                case 7:
                    outputData.MoveBias = new(1, 0);
                    break;
                case 8:
                    outputData.MoveBias = new(1, 1);
                    break;
                default:
                    throw new IndexOutOfRangeException($"index value {indexOfMaxWeight} was greater than 8!");
            }

            //currently unused
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

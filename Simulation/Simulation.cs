
using System;
using System.Numerics;

namespace NeuralLife.Simulation
{
    public class Simulation
    {

        public Simulation(int xResolution, int yResolution)
        {
            GameField = new SimulationObject[xResolution, yResolution];
            NextFrame = (SimulationObject[,])GameField.Clone();
        }

        private SimulationObject[,] GameField;
        private SimulationObject[,] NextFrame;

        private bool IsFieldUpdated;

        public void Fill<T>() where T : SimulationObject, new()
        {
            for(int i = 0; i < GameField.GetLength(0); i++)
            {
                for(int j = 0; j < GameField.GetLength(1); j++)
                {
                    SetAtPosition(new T(), new Vector2(i, j));
                }
            }
        }

        public void FillIfType<TStart, TResult>(float fillChance)
            where TResult : SimulationObject, new()
        {
            Random random = new Random();

            for(int i = 0; i < GameField.GetLength(0); i++)
            {
                for(int j = 0; j < GameField.GetLength(1); j++)
                {
                    if(GetAtPosition(new Vector2(i, j)) is not TStart)
                    {
                        continue;
                    }
                    if(random.NextSingle() < fillChance)
                    {
                        SetAtPosition(new TResult(), new Vector2(i, j));
                    }
                }
            }
        }

        public void RandomFillIfNull<T>(float fillChance) where T : SimulationObject, new()
        {
            Random random = new Random();

            for(int i = 0; i < GameField.GetLength(0); i++)
            {
                for(int j = 0; j < GameField.GetLength(1); j++)
                {
                    if(GetAtPosition(new Vector2(i, j)) != null)
                    {
                        continue;
                    }
                    if(random.NextSingle() < fillChance)
                    {
                        SetAtPosition(new T(), new Vector2(i, j));
                    }
                }
            }
        }

        public void RandomFill<T>(float fillChance) where T : SimulationObject, new()
        {
            Random random = new Random();

            for(int i = 0; i < GameField.GetLength(0); i++)
            {
                for(int j = 0; j < GameField.GetLength(1); j++)
                {
                    if(random.NextSingle() < fillChance)
                    {
                        SetAtPosition(new T(), new Vector2(i, j));
                    }
                }
            }
        }

        public Color[,] AsColors()
        {
            Color[,] colors = new Color[GameField.GetLength(0), GameField.GetLength(1)];

            for(int i = 0; i < GameField.GetLength(0); i++)
            {
                for(int j = 0; j < GameField.GetLength(1); j++)
                {
                    if(GameField[i, j] == null)
                    {
                        colors[i, j] = new Color(0, 0, 0);
                        continue;
                    }

                    colors[i, j] = GetAtPosition(new Vector2(i, j)).Color;
                }
            }

            return colors;
        }

        public void Update()
        {
            int xResolution = GameField.GetLength(0);
            int yResolution = GameField.GetLength(1);

            for(int i = 0; i < xResolution; i++)
            {
                for(int j = 0; j < yResolution; j++)
                {
                    if(GameField[i, j] == null)
                    {
                        continue;
                    }
                    ObjectEnvironmentData data = new();
                    data.Simulation = this;
                    data.Position.X = i;
                    data.Position.Y = j;

                    GameField[i, j].Update(data);
                }
            }

            if(IsFieldUpdated)
            {
                GameField = (SimulationObject[,])NextFrame.Clone();
            }
            IsFieldUpdated = false;
        }

        public void Swap(Vector2 selfPosition, Vector2 position)
        {
            SimulationObject temp = GetAtPosition(position);
            SimulationObject self = GetAtPosition(selfPosition);

            SetAtPosition(self, position);
            SetAtPosition(temp, selfPosition);
        }

        public void DestroyAtPosition(Vector2 position)
        {
            SetAtPosition(null, position);
        }

        public Vector2 GetNeighborPositionOfType<T>(Vector2 position) where T : SimulationObject
        {
            const int neighborsCount = 15;  //we are get not only neighboring cells, but their neighbors too
            SimulationObject[] neighbors = new SimulationObject[neighborsCount];

            int neighborIterator = 0;
            for(int i = -2; i <= 2; i++)
            {
                for(int j = -2; j <= 2; j++)
                {
                    if(GetAtPosition(new Vector2(i, j)) is T)
                    {
                        return new(position.X + i, position.Y + j);
                    }
                }
            }

            return new Vector2(-1, -1);
        }

        public Color[] GetNeighborsColors(Vector2 position)
        {
            const int neighborsCount = 24;  //we are get not only neighboring cells, but their neighbors too
            Color[] neighborsColors = new Color[neighborsCount];

            var neighbors = GetNeighbors(position);

            for(int i = 0; i < neighbors.Length; i++)
            {
                if(neighbors[i] == null)
                {
                    neighborsColors[i] = new Color(0, 0, 0);
                    continue;
                }
                neighborsColors[i] = neighbors[i].Color;
            }

            return neighborsColors;
        }

        public SimulationObject[] GetNeighbors(Vector2 position)
        {
            const int neighborsCount = 24;  //we are get not only neighboring cells, but their neighbors too
            SimulationObject[] neighbors = new SimulationObject[neighborsCount];

            int neighborIterator = 0;
            for(int i = -2; i <= 2; i++)
            {
                for(int j = -2; j <= 2; j++)
                {
                    if(i == 0 && j == 0)
                    {
                        continue;
                    }
                    neighbors[neighborIterator] = GetAtPosition(new Vector2(position.X + i, position.Y + j));
                    neighborIterator++;
                }
            }

            return neighbors;
        }

        public SimulationObject GetAtPosition(Vector2 position)
        {
            int newX = (int)position.X;
            int newY = (int)position.Y;
            ToGameFieldBounds(ref newX, ref newY);

            return GameField[newX, newY];
        }

        private void ToGameFieldBounds(ref int xPosition, ref int yPosition)
        {
            if(xPosition < 0)
            {
                xPosition = GameField.GetLength(0) - 1;
            }
            else if(xPosition >= GameField.GetLength(0))
            {
                xPosition = 0;
            }

            if(yPosition < 0)
            {
                yPosition = GameField.GetLength(1) - 1;
            }
            else if(yPosition >= GameField.GetLength(1))
            {
                yPosition = 0;
            }
        }

        public void SetAtPosition(SimulationObject obj, Vector2 position)
        {
            int newX = (int)position.X;
            int newY = (int)position.Y;
            ToGameFieldBounds(ref newX, ref newY);
            NextFrame[newX, newY] = obj;

            IsFieldUpdated = true;
        }
    }
}

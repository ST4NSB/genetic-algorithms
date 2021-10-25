using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{
    public class MagicSquare
    {
        private const int _searchSmallGridThreshold = 10;
        private int _fullGridSize, _smallGridSize, _population, _selection;

        private struct ChromosomeInformation
        {
            public List<int> Gene;
            public int Fitness;

            public ChromosomeInformation(List<int> gene, int fitness = int.MaxValue)
            {
                Gene = gene;
                Fitness = fitness;
            }
        }

        public MagicSquare(int n = 10, int m = 3, int population = 900000, int selection = 90000)
        {
            _fullGridSize = n;
            _smallGridSize = m;
            _population = population;
            _selection = selection;
        }

        public void ShowValidBoard()
        {
            Console.WriteLine(" *** Wait a few minutes .. This make take some time ..");
            var chromosomes = new List<ChromosomeInformation>();
            for (int i = 0; i < _population; i++)
            {
                var chromosome = GenerateRandomChromosome();
                var fitness = GetFitnessValue(chromosome);
                chromosomes.Add(new ChromosomeInformation(chromosome, fitness));
            }

            var generation = 1;
            while (chromosomes[0].Fitness >= 0)
            {
                Console.WriteLine(" ~~~~~");
                Console.WriteLine($" Generation number: {generation++}");
                Console.WriteLine($" Fitness: {chromosomes[0].Fitness}");
                PrintBoard(chromosomes[0].Gene);

                this.Shuffle(chromosomes);
                var childs = ApplyCrossover(chromosomes);
                Console.WriteLine(" Done crossover");
                var mutatedChilds = MutateChilds(childs);
                Console.WriteLine(" ~~~~~");
                chromosomes = new List<ChromosomeInformation>(mutatedChilds);
                chromosomes = chromosomes.OrderBy(x => x.Fitness).ToList();
                chromosomes = chromosomes.Take(_selection).ToList();
            }

            Console.WriteLine(" *** Solution: ");
            PrintBoard(chromosomes[0].Gene);
        }

        private List<ChromosomeInformation> ApplyCrossover(List<ChromosomeInformation> parents)
        {
            var rand = new Random();
            var childs = new List<ChromosomeInformation>();

            // swapped inverted crossover (2-point SIC)
            for (int i = 0; i < parents.Count; i += 2)
            {
                var startingPoint = rand.Next(_fullGridSize, (_fullGridSize * _fullGridSize) - (_fullGridSize * 2));
                while (startingPoint % _fullGridSize != 0)
                {
                    startingPoint = rand.Next(_fullGridSize, (_fullGridSize * _fullGridSize) - (_fullGridSize * 2));
                }

                var parentOneADN = parents[i].Gene.Skip(startingPoint).Take(_fullGridSize);
                var parentTwoADN = parents[i + 1].Gene.Skip(startingPoint).Take(_fullGridSize);

                var parentOneFirstpart = parents[i].Gene.Take(startingPoint);
                var parentTwoFirstpart = parents[i + 1].Gene.Take(startingPoint);

                var parentOneSecondpart = parents[i].Gene.Skip(startingPoint + _fullGridSize);
                var parentTwoSecondpart = parents[i + 1].Gene.Skip(startingPoint + _fullGridSize);

                var offspringOne = parentOneSecondpart.Concat(parentOneADN).Concat(parentOneFirstpart).ToList();
                childs.Add(new ChromosomeInformation(offspringOne));
                var offspringTwo = parentTwoSecondpart.Concat(parentTwoADN).Concat(parentTwoFirstpart).ToList();
                childs.Add(new ChromosomeInformation(offspringTwo));
                var offspringThree = parentOneFirstpart.Concat(parentOneADN.Reverse()).Concat(parentOneSecondpart).ToList();
                childs.Add(new ChromosomeInformation(offspringThree));
                var offspringFour = parentTwoFirstpart.Concat(parentTwoADN.Reverse()).Concat(parentTwoSecondpart).ToList();
                childs.Add(new ChromosomeInformation(offspringFour));
                var offspringFive = parentOneFirstpart.Reverse().Concat(parentOneADN).Concat(parentOneSecondpart.Reverse()).ToList();
                childs.Add(new ChromosomeInformation(offspringFive));
                var offspringSix = parentTwoFirstpart.Reverse().Concat(parentTwoADN).Concat(parentTwoSecondpart.Reverse()).ToList();
                childs.Add(new ChromosomeInformation(offspringSix));
            }

            Console.WriteLine($" Number of children: {childs.Count}");

            return childs;
        }

        private List<ChromosomeInformation> MutateChilds(List<ChromosomeInformation> childs)
        {
            var rand = new Random();
            var mutatedChilds = new List<ChromosomeInformation>();

            foreach (var child in childs)
            {
                var mutationType = rand.Next(10) + 1;

                // 0 - mutation
                if (mutationType <= 4)
                {
                    var pointOne = rand.Next(_fullGridSize * _fullGridSize);
                    var pointTwo = rand.Next(_fullGridSize * _fullGridSize);

                    var t = child.Gene[pointOne];
                    child.Gene[pointOne] = child.Gene[pointTwo];
                    child.Gene[pointTwo] = t;
                }

                mutatedChilds.Add(new ChromosomeInformation(child.Gene, GetFitnessValue(child.Gene)));
            }
            Console.WriteLine(" Done mutating & calculating fitness");

            return mutatedChilds;
        }

        private void Shuffle<T>(List<T> list)
        {
            var rand = new Random();
            var n = list.Count;
            for (int i = 0; i < (n - 1); i++)
            {
                var r = i + rand.Next(n - i);
                var t = list[r];
                list[r] = list[i];
                list[i] = t;
            }
        }

        private List<int> GenerateRandomChromosome()
        {
            var rand = new Random();
            var chr = new List<int>();

            for (int i = 0; i < _fullGridSize * _fullGridSize; i++)
            {
                chr.Add(i + 1);
            }

            this.Shuffle(chr);
            return chr;
        }

        private int GetMagicConstant(int n)
        {
            return (n * (n * n + 1)) / 2;
        }

        private void PrintBoard(List<int> chromosome)
        {
            var k = 0;
            var board = new int[_fullGridSize, _fullGridSize];

            Console.WriteLine($" Valid Magic square board (sum = {GetMagicConstant(_fullGridSize)}): ");
            for (int i = 0; i < _fullGridSize; i++)
            {
                for (int j = 0; j < _fullGridSize; j++)
                {
                    board[i, j] = chromosome[k++];
                    Console.Write($" {board[i, j]}");
                    if (board[i,j].ToString().Length == 1)
                    {
                        Console.Write("   ");
                    }
                    else if (board[i, j].ToString().Length == 2)
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
        }

        private int GetFitnessValue(List<int> chromosome)
        {
            int k = 0, fitnessValue = 0;
            var board = new int[_fullGridSize, _fullGridSize];

            for (int i = 0; i < _fullGridSize; i++)
            {
                for (int j = 0; j < _fullGridSize; j++)
                {
                    board[i, j] = chromosome[k++];
                }
            }

            // full grid (10x10) fitness
            fitnessValue += CalculateFitnessByBoardSize(board, _fullGridSize);

            // small grid (3x3) fitness
            if (fitnessValue <= _searchSmallGridThreshold)
            {
                for (int i = 0; i <= _fullGridSize - _smallGridSize; i++)
                {
                    for (int j = 0; j <= _fullGridSize - _smallGridSize; j++)
                    {
                        var smallBoard = new int[_smallGridSize, _smallGridSize];
                        var gaussSum = 0;
                        for (int row = 0; row < _smallGridSize; row++)
                        {
                            for (int col = 0; col < _smallGridSize; col++)
                            {
                                smallBoard[row, col] = board[row + i, col + j];
                                gaussSum += smallBoard[row, col];
                            }
                        }

                        if (gaussSum == ((_smallGridSize * _smallGridSize) * (_smallGridSize * _smallGridSize + 1) / 2))
                            fitnessValue += CalculateFitnessByBoardSize(smallBoard, _smallGridSize);
                    }
                }
            }

            return fitnessValue;
        }

        private int CalculateFitnessByBoardSize(int[,] board, int size)
        {
            var taskCols = Task<int>.Factory.StartNew(() =>
            {
                var fitnessValue = 0;
                // sum cols
                for (int i = 0; i < size; i++)
                {
                    int sum = 0;
                    for (int j = 0; j < size; j++)
                    {
                        sum += board[i, j];
                    }
                    fitnessValue += Math.Abs(sum - GetMagicConstant(size));
                }
                return fitnessValue;
            });

            var taskRows = Task<int>.Factory.StartNew(() =>
            {
                var fitnessValue = 0;
                // sum rows
                for (int j = 0; j < size; j++)
                {
                    int sum = 0;
                    for (int i = 0; i < size; i++)
                    {
                        sum += board[i, j];
                    }
                    fitnessValue += Math.Abs(sum - GetMagicConstant(size));
                }
                return fitnessValue;
            });

            var taskDiag1 = Task<int>.Factory.StartNew(() =>
            {
                var fitnessValue = 0;
                // sum diagonal 1
                int sumDiag1 = 0;
                int line = 0;
                for (int j = 0; j < size; j++)
                {
                    sumDiag1 += board[line, j];
                    line++;
                }
                fitnessValue += Math.Abs(sumDiag1 - GetMagicConstant(size));
                return fitnessValue;
            });

            var taskDiag2 = Task<int>.Factory.StartNew(() =>
            {
                var fitnessValue = 0;
                // sum diagonal 2
                int sumDiag2 = 0;
                int line2 = 0;
                for (int j = 0; j < size; j++)
                {
                    sumDiag2 += board[line2, (size - 1) - j];
                    line2++;
                }
                fitnessValue += Math.Abs(sumDiag2 - GetMagicConstant(size));
                return fitnessValue;
            });

            Task.WaitAll(taskCols, taskRows, taskDiag1, taskDiag2);

            var fitnessValue = taskCols.Result + taskRows.Result + taskDiag1.Result + taskDiag2.Result;

            if (size == _smallGridSize)
            {
                if (fitnessValue > 0)
                {
                    fitnessValue = 0;
                }
                else
                {
                    fitnessValue--; // decrease to a negative value when we found a valid 3x3 grid, so it will come first when sorted
                }
            }

            return fitnessValue;
        }
    }
}

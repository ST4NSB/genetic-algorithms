using System;
using System.Collections.Generic;
using System.Linq;

namespace NQueens_GeneticAlgorithm
{
    public class NQueens
    {
        private int _boardSize, _initialPopulationSize, _selectionFactor, _crossover;

        public NQueens(int n = 8, int initialPopulation = 100, int selectionFactor=20)
        {
            _boardSize = n;
            _initialPopulationSize = initialPopulation;
            _selectionFactor = selectionFactor;
            _crossover = _boardSize / 2;
        }

        public void ShowQueensOnBoard()
        {
            var generations = new List<(List<int> generation, int fitness)>(); // list of tuples (list + int)
            for (int i = 0; i < _initialPopulationSize; i++) 
            {
                var gen = GetRandomGeneration();
                var fitness = GetUnderAttackFitness(gen);
                generations.Add((gen, fitness));
            }

            generations = generations.OrderBy(x => x.fitness).ToList();
            generations = generations.Take(_selectionFactor).ToList();

            while (generations[0].fitness != 0)
            {
                var newGenerations = new List<(List<int> generation, int fitness)>();
                var rand = new Random();
                for (int i = 0; i < _selectionFactor; i += 2) 
                {
                    var firstChild = generations[i].generation.Take(_crossover).ToList();
                    firstChild.AddRange(generations[i + 1].generation.Skip(_crossover).ToList());

                    var secondChild = generations[i].generation.Skip(_crossover).ToList();
                    secondChild.AddRange(generations[i + 1].generation.Take(_crossover).ToList());


                    var mutatingIndexFirstChild = rand.Next(_boardSize);
                    var mutatingIndexSecondChild = rand.Next(_boardSize);

                    firstChild[mutatingIndexFirstChild] = rand.Next(_boardSize);
                    secondChild[mutatingIndexSecondChild] = rand.Next(_boardSize);

                    newGenerations.Add((firstChild, GetUnderAttackFitness(firstChild)));
                    newGenerations.Add((secondChild, GetUnderAttackFitness(secondChild)));
                }

                generations = new List<(List<int> generation, int fitness)>(newGenerations);
                generations = generations.OrderBy(x => x.fitness).ToList();
            }

            ShowValidBoard(generations[0].generation);
        }

        private void ShowValidBoard(List<int> generation)
        {
            var board = new int[_boardSize, _boardSize];
            for (var col = 0; col < generation.Count; col++)
            {
                board[generation[col], col] = 1;
            }

            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    Console.Write($" {board[i, j]} ");
                }
                Console.WriteLine();
            }
        }

        private List<int> GetRandomGeneration()
        {
            var rand = new Random();
            var gen = new List<int>();

            for (int i = 0; i < _boardSize; i++) 
            {
                gen.Add(rand.Next(_boardSize));
            }

            return gen;
        }

        private int GetUnderAttackFitness(List<int> generation)
        {
            int underAttack = 0;

            var genSet = new HashSet<int>(generation); // verifies that generation list isn't on same line
            underAttack += (_boardSize - genSet.Count);

            var board = new int[_boardSize, _boardSize];
            for(var col = 0; col < generation.Count; col++)
            {
                board[generation[col], col] = 1;
            }

            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++) 
                {
                    if (board[i, j] == 1)
                        underAttack += SearchDiagonals(board, i, j);
                }
            }

            return underAttack;
        }

        private int SearchDiagonals(int[,] board, int y, int x)
        {
            int underAttack = 0;

            var dirY = new int[4] { -1, -1, 1, 1 };
            var dirX = new int[4] { -1, 1, 1, -1 };

            for (int k = 0; k < 4; k++)
            {
                var searchY = y + dirY[k];
                var searchX = x + dirX[k];

                while (searchY >= 0 && searchY < _boardSize && searchX >= 0 && searchX < _boardSize)
                {
                    if (board[searchY, searchX] == 1)
                        underAttack += 1;

                    searchY += dirY[k];
                    searchX += dirX[k];
                }
            }

            return underAttack;
        }
    }
}

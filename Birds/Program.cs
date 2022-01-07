using System;
using System.Diagnostics;

namespace Birds
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var puzzle = new PicturePuzzle(_tiles, Comparer, 3);

            puzzle.FindTileArrangement();
            
            if (puzzle.SolutionFound)
            {
                for (int i = 0; i < 9; i++)
                    Console.WriteLine($"Slot {i}: Tile {puzzle.TileInSlot[i]} : Side {puzzle.TileOrientation[i]}");
            }
            else
                Console.WriteLine("No solution found");
        }

        static readonly int[][] _tiles = {
            new[] {(int)Birds.BlueBirdBottom, (int)Birds.RainbowBirdTop, (int)Birds.CardinalTop, (int)Birds.BlueBirdTop},
            new[] {(int)Birds.RainbowBirdBottom, (int)Birds.YellowBirdTop, (int)Birds.CardinalTop, (int)Birds.BlueBirdBottom},
            new[] {(int)Birds.BlueBirdBottom, (int)Birds.RainbowBirdBottom, (int)Birds.CardinalBottom, (int)Birds.YellowBirdBottom},
            new[] {(int)Birds.BlueBirdTop, (int)Birds.RainbowBirdBottom, (int)Birds.YellowBirdTop, (int)Birds.CardinalBottom},
            new[] {(int)Birds.CardinalTop, (int)Birds.YellowBirdTop, (int)Birds.RainbowBirdTop, (int)Birds.BlueBirdBottom},
            new[] {(int)Birds.RainbowBirdBottom, (int)Birds.BlueBirdTop, (int)Birds.YellowBirdTop, (int)Birds.YellowBirdBottom},
            new[] {(int)Birds.CardinalTop, (int)Birds.BlueBirdTop, (int)Birds.RainbowBirdBottom, (int)Birds.YellowBirdTop},
            new[] {(int)Birds.BlueBirdTop, (int)Birds.CardinalBottom, (int)Birds.YellowBirdBottom, (int)Birds.RainbowBirdTop},
            new[] {(int)Birds.RainbowBirdBottom, (int)Birds.CardinalBottom, (int)Birds.YellowBirdTop, (int)Birds.BlueBirdBottom},
        };
        
        static bool Comparer(int imageA, int imageB)
        {
            return imageB == imageA - 100 || imageA == imageB - 100;
        }

        private enum Birds
        {
            BlueBirdBottom = 0,
            BlueBirdTop = 100,
            YellowBirdBottom = 1,
            YellowBirdTop = 101,
            RainbowBirdBottom = 2,
            RainbowBirdTop = 102,
            CardinalBottom = 3,
            CardinalTop = 103
        }
    }

    internal class PicturePuzzle
    {
        public bool SolutionFound;
        public readonly int[] TileInSlot;
        public readonly int[] TileOrientation;
        
        private readonly int[][] _tiles;
        private readonly int _width;
        private readonly Func<int, int, bool> _comparer;

        public PicturePuzzle(int[][] tiles, Func<int, int, bool> comparer, int width)
        {
            _width = width;
            _tiles = tiles;
            _comparer = comparer;

            TileInSlot = new int[_tiles.Length];
            TileOrientation = new int[_tiles.Length];
        }
        
        public void FindTileArrangement()
        {
            FindTileArrangementRecursive(0);
        }
        
        void FindTileArrangementRecursive(int slot)
        {
            if (slot >= _tiles.Length)
            {
                Console.WriteLine("SOLUTION FOUND");
                SolutionFound = true;
                return;
            }
            
            // For this slot try all the tiles
            for (TileInSlot[slot] = 0; TileInSlot[slot] < _tiles.Length; TileInSlot[slot]++)
            {
                bool cont = false;
                for (int i = slot - 1; i >= 0; i--)
                {
                    if (TileInSlot[slot] == TileInSlot[i])
                    {
                        cont = true;
                        break;
                    }
                }

                if (cont)
                    continue;
                
                // For the tile in this slot, try all the possible orientations
                for (TileOrientation[slot] = 0; TileOrientation[slot] < 4; TileOrientation[slot]++)
                {
                    // Check if the tile in the slot to the left is compatible with the tile in this slot
                    if (slot % _width > 0)
                    {
                        int thisSlotLeftImage = _tiles[TileInSlot[slot]][(3 + TileOrientation[slot]) % 4];
                        int leftSlotRightImage = _tiles[TileInSlot[slot - 1]][(1 + TileOrientation[slot - 1]) % 4];
                        if (_comparer(thisSlotLeftImage, leftSlotRightImage) == false)
                            continue;
                    }

                    // Check if the tile in the slot above is compatible with the tile in this slot
                    if (slot >= _width)
                    {
                        int thisSlotUpImage = _tiles[TileInSlot[slot]][(0 + TileOrientation[slot]) % 4];
                        int aboveSlotDownImage = _tiles[TileInSlot[slot - _width]][(2 + TileOrientation[slot - _width]) % 4];
                        if (_comparer(thisSlotUpImage, aboveSlotDownImage) == false)
                            continue;
                    }
                    
                    // The selected tile with the given orientation is compatible
                    // check the next slot for a compatible tile
                    FindTileArrangementRecursive(slot + 1);
                    
                    if (SolutionFound)
                        return;
                }
            }
        }
    }
}
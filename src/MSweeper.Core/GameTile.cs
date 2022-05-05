using System.Collections.Generic;

namespace MSweeper.Core
{
    public class GameTile 
    {
        public ICollection<GameTile> Neighbors { get; } = new List<GameTile>();
        public int Value { get; internal set; } = -1;
        public TilePosition Position { get; internal set; }  
        internal bool IsMine { get; set; }  
    }
}

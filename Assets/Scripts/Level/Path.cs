using System.Collections.Generic;
namespace Level
{
    public class Path
    {
        public Queue<Transfer> Connection { get; set; }
        public Dictionary<Cell,Cell> InvolvedCells; 
        
        public Path()
        {
            Connection = new();
            InvolvedCells = new();
        }
    }
}
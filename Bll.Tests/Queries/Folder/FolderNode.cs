using System.Collections.Generic;

namespace Bll.Tests.Queries.Folder
{
    class FolderNode : Node
    {
        public FolderNode(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public List<Node> Nodes { get; set; } = new List<Node>();
    }
}
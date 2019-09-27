namespace Bll.Tests.Queries.Folder
{
    class FileNode : Node
    {
        public FileNode(int length)
        {
            Length = length;
        }
        public int Length { get; set; }
    }
}
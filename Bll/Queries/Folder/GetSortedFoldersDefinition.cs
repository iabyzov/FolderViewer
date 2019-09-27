using System.Collections.Generic;
using MediatR;

namespace Bll.Queries.Folder
{
    public class GetSortedFoldersDefinition : IRequest<IEnumerable<FolderInfo>>
    {
        public string Path { get; set; }
        public int? Limit { get; set; }
    }
}
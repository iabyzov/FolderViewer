using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Bll.Queries.Folder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebHost.ClientApi.Folder
{
    [ApiController]
    [Route("api/[controller]")]
    public class FolderController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public FolderController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("{folder}/orderBySize")]
        [ResponseCache(NoStore = true)]
        public async Task<IEnumerable<FolderDto>> GetSortedFolders(string folder, CancellationToken cancellationToken, [Range(0, int.MaxValue)]int? limit = null)
        {
            var definition = new GetSortedFoldersDefinition {Path = folder, Limit = limit};
            var folders = await _mediator.Send(definition, cancellationToken);
            return _mapper.Map<FolderDto[]>(folders);
        }
    }
}

using System;
using System.Linq;
using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SampleWebApiAspNetCore.Models;
using SampleWebApiAspNetCore.Helpers;
using System.Text.Json;
using System.Linq.Dynamic.Core;

namespace SampleWebApiAspNetCore.v1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/funcionarioMarcacao_marcacao")]
    //[Route("api/[controller]")]
    public class FuncionarioMarcacaoController : ControllerBase
    {
        private readonly testePAPContext _context;
        private readonly IUrlHelper _urlHelper;
        private readonly IMapper _mapper;

        public FuncionarioMarcacaoController(
            IUrlHelper urlHelper,
            testePAPContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        private IQueryable<FuncionarioMarcacao> GetAll(QueryParameters queryParameters)
        {

            IQueryable<FuncionarioMarcacao> _allItems = _context.FuncionarioMarcacao.OrderBy(queryParameters.OrderBy,
              queryParameters.IsDescending());

            
            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.IdFuncionarioMarcacao.ToString().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }


        [HttpGet(Name = nameof(GetAllFuncionarioMarcacao))]
        public ActionResult GetAllFuncionarioMarcacao(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<FuncionarioMarcacao> funcionarioMarcacao = GetAll(queryParameters).ToList();

            var allItemCount = _context.FuncionarioMarcacao.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForCollection(queryParameters, allItemCount, version);

            var toReturn = funcionarioMarcacao.Select(x => ExpandSingleFuncionarioMarcacaoItem(x, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleFuncionarioMarcacao))]
        public ActionResult GetSingleFuncionarioMarcacao(ApiVersion version, int id)
        {
            FuncionarioMarcacao funcionarioMarcacao = _context.FuncionarioMarcacao.FirstOrDefault(x => x.IdFuncionarioMarcacao == id); 

            if (funcionarioMarcacao == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleFuncionarioMarcacaoItem(funcionarioMarcacao, version));
        }

        [HttpPost(Name = nameof(AddFuncionarioMarcacao))]
        public ActionResult<FuncionarioMarcacaoDto> AddFuncionarioMarcacao(ApiVersion version, [FromBody] FuncionarioMarcacaoCreateDto funcionarioMarcacaoCreateDto)
        {
            if (funcionarioMarcacaoCreateDto == null)
            {
                return BadRequest();
            }

            FuncionarioMarcacao toAdd = _mapper.Map<FuncionarioMarcacao>(funcionarioMarcacaoCreateDto);

            _context.Add(toAdd);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Creating a funcionarioMarcacao failed on save.");
            }

            FuncionarioMarcacao newFuncionarioMarcacaoItem = _context.FuncionarioMarcacao.FirstOrDefault(x => x.IdFuncionarioMarcacao == toAdd.IdFuncionarioMarcacao);

            return CreatedAtRoute(nameof(GetSingleFuncionarioMarcacao),
                new { version = version.ToString(), id = newFuncionarioMarcacaoItem.IdFuncionarioMarcacao },
                _mapper.Map<FuncionarioMarcacao>(newFuncionarioMarcacaoItem));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFuncionarioMarcacao))]
        public ActionResult<FuncionarioMarcacaoDto> PartiallyUpdateFuncionarioMarcacao(int id, [FromBody] JsonPatchDocument<FuncionarioMarcacaoUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            FuncionarioMarcacao existingEntity = _context.FuncionarioMarcacao.FirstOrDefault(x => x.IdFuncionarioMarcacao == id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            FuncionarioMarcacaoUpdateDto funcionarioMarcacaoUpdateDto = _mapper.Map<FuncionarioMarcacaoUpdateDto>(existingEntity);
            patchDoc.ApplyTo(funcionarioMarcacaoUpdateDto);

            TryValidateModel(funcionarioMarcacaoUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(funcionarioMarcacaoUpdateDto, existingEntity);
            _context.FuncionarioMarcacao.Update(existingEntity);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a funcionarioMarcacaoitem failed on save.");
            }

            return Ok(_mapper.Map<FuncionarioMarcacao>(existingEntity));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveFuncionarioMarcacao))]
        public ActionResult RemoveFuncionarioMarcacao(int id)
        {
            FuncionarioMarcacao funcionarioMarcacaoItem = _context.FuncionarioMarcacao.FirstOrDefault(x => x.IdFuncionarioMarcacao == id);

            if (funcionarioMarcacaoItem == null)
            {
                return NotFound();
            }

            _context.FuncionarioMarcacao.Remove(funcionarioMarcacaoItem);

            if (_context.SaveChanges() == 0 )
            {
                throw new Exception("Deleting a funcionarioMarcacaoitem failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateFuncionarioMarcacao))]
        public ActionResult<FuncionarioMarcacaoDto> UpdateFuncionarioMarcacao(int id, [FromBody] FuncionarioMarcacaoUpdateDto funcionarioMarcacaoUpdateDto)
        {
            if (funcionarioMarcacaoUpdateDto == null)
            {
                return BadRequest();
            }

            var existingFuncionarioMarcacaoItem = _context.FuncionarioMarcacao.FirstOrDefault(x => x.IdFuncionarioMarcacao == id);

            if (existingFuncionarioMarcacaoItem == null)
            {
                return NotFound();
            }

            _mapper.Map(funcionarioMarcacaoUpdateDto, existingFuncionarioMarcacaoItem);

            _context.Update(existingFuncionarioMarcacaoItem);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a fooditem failed on save.");
            }

            return Ok(_mapper.Map<FuncionarioMarcacaoDto>(existingFuncionarioMarcacaoItem));
        }

        private List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncionarioMarcacao), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.Page,
                orderby = queryParameters.OrderBy
            }), "self", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncionarioMarcacao), new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            }), "first", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncionarioMarcacao), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            }), "last", "GET"));

            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncionarioMarcacao), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                }), "next", "GET"));
            }

            if (queryParameters.HasPrevious())
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncionarioMarcacao), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                }), "previous", "GET"));
            }

            var posturl = _urlHelper.Link(nameof(AddFuncionarioMarcacao), new { version = version.ToString() });

            links.Add(
               new LinkDto(posturl,
               "create_funcionarioMarcacao",
               "POST"));

            return links;
        }

        private dynamic ExpandSingleFuncionarioMarcacaoItem(FuncionarioMarcacao funcionarioMarcacao, ApiVersion version)
        {
            var links = GetLinks(funcionarioMarcacao.IdFuncionarioMarcacao, version);
            FuncionarioMarcacao item = _mapper.Map<FuncionarioMarcacao>(funcionarioMarcacao);

            var resourceToReturn = item.ToDynamic() as IDictionary<string, object>;
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }

        private IEnumerable<LinkDto> GetLinks(int id, ApiVersion version)
        {
            var links = new List<LinkDto>();

            var getLink = _urlHelper.Link(nameof(GetSingleFuncionarioMarcacao), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(getLink, "self", "GET"));

            var deleteLink = _urlHelper.Link(nameof(RemoveFuncionarioMarcacao), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(deleteLink,
              "delete_funcionarioMarcacao",
              "DELETE"));

            var createLink = _urlHelper.Link(nameof(AddFuncionarioMarcacao), new { version = version.ToString() });

            links.Add(
              new LinkDto(createLink,
              "create_funcionarioMarcacao",
              "POST"));

            var updateLink = _urlHelper.Link(nameof(UpdateFuncionarioMarcacao), new { version = version.ToString(), id = id });

            links.Add(
               new LinkDto(updateLink,
               "update_funcionarioMarcacao",
               "PUT"));

            return links;
        }
    }
}

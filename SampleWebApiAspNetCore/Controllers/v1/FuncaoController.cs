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
using Microsoft.AspNetCore.WebUtilities;

namespace SampleWebApiAspNetCore.v1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/funcao")]
    //[Route("api/[controller]")]
    public class FuncaoController : ControllerBase
    {
        private readonly testePAPContext _context;
        private readonly IUrlHelper _urlHelper;
        private readonly IMapper _mapper;

        public FuncaoController(
            IUrlHelper urlHelper,
            testePAPContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        private IQueryable<Funcao> GetAll(QueryParameters queryParameters)
        {
            IQueryable<Funcao> _allItems = _context.Funcao;

            if (queryParameters.OrderBy != "") {
                _allItems = _allItems.OrderBy(queryParameters.OrderBy,
                  queryParameters.IsDescending());
            }


            if (queryParameters.HasQuery()) {
                var queryString = QueryHelpers.ParseQuery(queryParameters.Query);
                // queryString.GetType() --> typeof(Dictionary<String,StringValues>)


                foreach (var query in queryString) {
                    var filter = query.Value.Count > 0 ? query.Value[0] : "";
                    if (filter != "") {
                        switch (query.Key.ToLower()) {
                            case "idfuncao":
                                _allItems = _allItems
                                    .Where(x => x.IdFuncao.ToString() == filter);
                                break;
                        }
                    }
                }

            }

            return _allItems
                .OrderByDescending(x => x.IdFuncao)
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }


        [HttpGet(Name = nameof(GetAllFuncao))]
        public ActionResult GetAllFuncao(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<Funcao> funcao = GetAll(queryParameters).ToList();

            var allItemCount = _context.Funcao.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForCollection(queryParameters, allItemCount, version);

            var toReturn = funcao.Select(x => ExpandSingleFuncaoItem(x, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleFuncao))]
        public ActionResult GetSingleFuncao(ApiVersion version, int id)
        {
            Funcao funcao = _context.Funcao.FirstOrDefault(x => x.IdFuncao == id); 

            if (funcao == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleFuncaoItem(funcao, version));
        }

        [HttpPost(Name = nameof(AddFuncao))]
        public ActionResult<FuncaoDto> AddFuncao(ApiVersion version, [FromBody] FuncaoCreateDto funcaoCreateDto)
        {
            if (funcaoCreateDto == null)
            {
                return BadRequest();
            }

            Funcao toAdd = _mapper.Map<Funcao>(funcaoCreateDto);

            _context.Add(toAdd);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Creating a funcao failed on save.");
            }

            Funcao newFuncaoItem = _context.Funcao.FirstOrDefault(x => x.IdFuncao == toAdd.IdFuncao);

            return CreatedAtRoute(nameof(GetSingleFuncao),
                new { version = version.ToString(), id = newFuncaoItem.IdFuncao },
                _mapper.Map<Funcao>(newFuncaoItem));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFuncao))]
        public ActionResult<FuncaoDto> PartiallyUpdateFuncao(int id, [FromBody] JsonPatchDocument<FuncaoUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            Funcao existingEntity = _context.Funcao.FirstOrDefault(x => x.IdFuncao == id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            FuncaoUpdateDto funcaoUpdateDto = _mapper.Map<FuncaoUpdateDto>(existingEntity);
            patchDoc.ApplyTo(funcaoUpdateDto);

            TryValidateModel(funcaoUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(funcaoUpdateDto, existingEntity);
            _context.Funcao.Update(existingEntity);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a funcaoitem failed on save.");
            }

            return Ok(_mapper.Map<Funcao>(existingEntity));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveFuncao))]
        public ActionResult RemoveFuncao(int id)
        {
            Funcao funcaoItem = _context.Funcao.FirstOrDefault(x => x.IdFuncao == id);

            if (funcaoItem == null)
            {
                return NotFound();
            }

            _context.Funcao.Remove(funcaoItem);

            if (_context.SaveChanges() == 0 )
            {
                throw new Exception("Deleting a funcaoitem failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateFuncao))]
        public ActionResult<FuncaoDto> UpdateFuncao(int id, [FromBody] FuncaoUpdateDto funcaoUpdateDto)
        {
            if (funcaoUpdateDto == null)
            {
                return BadRequest();
            }

            var existingFuncaoItem = _context.Funcao.FirstOrDefault(x => x.IdFuncao == id);

            if (existingFuncaoItem == null)
            {
                return NotFound();
            }

            _mapper.Map(funcaoUpdateDto, existingFuncaoItem);

            _context.Update(existingFuncaoItem);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a fooditem failed on save.");
            }

            return Ok(_mapper.Map<FuncaoDto>(existingFuncaoItem));
        }

        private List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncao), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.Page,
                orderby = queryParameters.OrderBy
            }), "self", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncao), new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            }), "first", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncao), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            }), "last", "GET"));

            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncao), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                }), "next", "GET"));
            }

            if (queryParameters.HasPrevious())
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncao), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                }), "previous", "GET"));
            }

            var posturl = _urlHelper.Link(nameof(AddFuncao), new { version = version.ToString() });

            links.Add(
               new LinkDto(posturl,
               "create_funcao",
               "POST"));

            return links;
        }

        private dynamic ExpandSingleFuncaoItem(Funcao funcao, ApiVersion version)
        {
            var links = GetLinks(funcao.IdFuncao, version);
            Funcao item = _mapper.Map<Funcao>(funcao);

            var resourceToReturn = item.ToDynamic() as IDictionary<string, object>;
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }

        private IEnumerable<LinkDto> GetLinks(int id, ApiVersion version)
        {
            var links = new List<LinkDto>();

            var getLink = _urlHelper.Link(nameof(GetSingleFuncao), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(getLink, "self", "GET"));

            var deleteLink = _urlHelper.Link(nameof(RemoveFuncao), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(deleteLink,
              "delete_funcao",
              "DELETE"));

            var createLink = _urlHelper.Link(nameof(AddFuncao), new { version = version.ToString() });

            links.Add(
              new LinkDto(createLink,
              "create_funcao",
              "POST"));

            var updateLink = _urlHelper.Link(nameof(UpdateFuncao), new { version = version.ToString(), id = id });

            links.Add(
               new LinkDto(updateLink,
               "update_funcao",
               "PUT"));

            return links;
        }
    }
}

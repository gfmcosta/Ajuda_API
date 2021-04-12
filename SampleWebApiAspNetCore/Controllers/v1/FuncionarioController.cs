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
    [Route("api/v{version:apiVersion}/funcionario")]
    //[Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly testePAPContext _context;
        private readonly IUrlHelper _urlHelper;
        private readonly IMapper _mapper;

        public FuncionarioController(
            IUrlHelper urlHelper,
            testePAPContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

            private IQueryable<Funcionario> GetAll(QueryParameters queryParameters)
            {

            IQueryable<Funcionario> _allItems = _context.Funcionario;

            if (queryParameters.OrderBy != "")
            {
                _allItems = _allItems.OrderBy(queryParameters.OrderBy,
                  queryParameters.IsDescending());
            }


            if (queryParameters.HasQuery())
            {
                var queryString = QueryHelpers.ParseQuery(queryParameters.Query);
                // queryString.GetType() --> typeof(Dictionary<String,StringValues>)


                foreach (var query in queryString)
                {
                    var filter = query.Value.Count > 0 ? query.Value[0] : "";
                    if (filter != "")
                    {
                        switch (query.Key.ToLower())
                        {
                            case "nif":
                                _allItems = _allItems
                                    .Where(x => x.Nif == filter);
                                break;
                        }
                    }
                }

            }
            return _allItems
               .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
               .Take(queryParameters.PageCount);
        }


        [HttpGet(Name = nameof(GetAllFuncionario))]
        public ActionResult GetAllFuncionario(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<Funcionario> funcionario = GetAll(queryParameters).ToList();

            var allItemCount = _context.Funcionario.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForCollection(queryParameters, allItemCount, version);

            var toReturn = funcionario.Select(x => ExpandSingleFuncionarioItem(x, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleFuncionario))]
        public ActionResult GetSingleFuncionario(ApiVersion version, int id)
        {
            Funcionario funcionario = _context.Funcionario.FirstOrDefault(x => x.IdFuncionario == id); 

            if (funcionario == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleFuncionarioItem(funcionario, version));
        }

        [HttpPost(Name = nameof(AddFuncionario))]
        public ActionResult<FuncionarioDto> AddFuncionario(ApiVersion version, [FromBody] FuncionarioCreateDto funcionarioCreateDto)
        {
            if (funcionarioCreateDto == null)
            {
                return BadRequest();
            }

            Funcionario toAdd = _mapper.Map<Funcionario>(funcionarioCreateDto);

            _context.Add(toAdd);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Creating a funcionario failed on save.");
            }

            Funcionario newFuncionarioItem = _context.Funcionario.FirstOrDefault(x => x.IdFuncionario == toAdd.IdFuncionario);

            return CreatedAtRoute(nameof(GetSingleFuncionario),
                new { version = version.ToString(), id = newFuncionarioItem.IdFuncionario },
                _mapper.Map<Funcionario>(newFuncionarioItem));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFuncionario))]
        public ActionResult<FuncionarioDto> PartiallyUpdateFuncionario(int id, [FromBody] JsonPatchDocument<FuncionarioUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            Funcionario existingEntity = _context.Funcionario.FirstOrDefault(x => x.IdFuncionario == id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            FuncionarioUpdateDto funcionarioUpdateDto = _mapper.Map<FuncionarioUpdateDto>(existingEntity);
            patchDoc.ApplyTo(funcionarioUpdateDto);

            TryValidateModel(funcionarioUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(funcionarioUpdateDto, existingEntity);
            _context.Funcionario.Update(existingEntity);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a funcionarioitem failed on save.");
            }

            return Ok(_mapper.Map<Funcionario>(existingEntity));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveFuncionario))]
        public ActionResult RemoveFuncionario(int id)
        {
            Funcionario funcionarioItem = _context.Funcionario.FirstOrDefault(x => x.IdFuncionario == id);

            if (funcionarioItem == null)
            {
                return NotFound();
            }

            _context.Funcionario.Remove(funcionarioItem);

            if (_context.SaveChanges() == 0 )
            {
                throw new Exception("Deleting a funcionarioitem failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateFuncionario))]
        public ActionResult<FuncionarioDto> UpdateFuncionario(int id, [FromBody] FuncionarioUpdateDto funcionarioUpdateDto)
        {
            if (funcionarioUpdateDto == null)
            {
                return BadRequest();
            }

            var existingFuncionarioItem = _context.Funcionario.FirstOrDefault(x => x.IdFuncionario == id);

            if (existingFuncionarioItem == null)
            {
                return NotFound();
            }

            _mapper.Map(funcionarioUpdateDto, existingFuncionarioItem);

            _context.Update(existingFuncionarioItem);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a fooditem failed on save.");
            }

            return Ok(_mapper.Map<FuncionarioDto>(existingFuncionarioItem));
        }

        private List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncionario), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.Page,
                orderby = queryParameters.OrderBy
            }), "self", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncionario), new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            }), "first", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncionario), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            }), "last", "GET"));

            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncionario), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                }), "next", "GET"));
            }

            if (queryParameters.HasPrevious())
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllFuncionario), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                }), "previous", "GET"));
            }

            var posturl = _urlHelper.Link(nameof(AddFuncionario), new { version = version.ToString() });

            links.Add(
               new LinkDto(posturl,
               "create_funcionario",
               "POST"));

            return links;
        }

        private dynamic ExpandSingleFuncionarioItem(Funcionario funcionario, ApiVersion version)
        {
            var links = GetLinks(funcionario.IdFuncionario, version);
            Funcionario item = _mapper.Map<Funcionario>(funcionario);

            var resourceToReturn = item.ToDynamic() as IDictionary<string, object>;
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }

        private IEnumerable<LinkDto> GetLinks(int id, ApiVersion version)
        {
            var links = new List<LinkDto>();

            var getLink = _urlHelper.Link(nameof(GetSingleFuncionario), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(getLink, "self", "GET"));

            var deleteLink = _urlHelper.Link(nameof(RemoveFuncionario), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(deleteLink,
              "delete_funcionario",
              "DELETE"));

            var createLink = _urlHelper.Link(nameof(AddFuncionario), new { version = version.ToString() });

            links.Add(
              new LinkDto(createLink,
              "create_funcionario",
              "POST"));

            var updateLink = _urlHelper.Link(nameof(UpdateFuncionario), new { version = version.ToString(), id = id });

            links.Add(
               new LinkDto(updateLink,
               "update_funcionario",
               "PUT"));

            return links;
        }
    }
}

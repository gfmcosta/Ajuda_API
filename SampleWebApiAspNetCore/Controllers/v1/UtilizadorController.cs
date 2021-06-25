using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Helpers;
using SampleWebApiAspNetCore.Models;
using SampleWebApiAspNetCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace SampleWebApiAspNetCore.v1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/utilizador")]
    //[Route("api/[controller]")]
    public class UtilizadorController : ControllerBase
    {
        private readonly testePAPContext _context;
        private readonly IUrlHelper _urlHelper;
        private readonly IMapper _mapper;

        public UtilizadorController(
            IUrlHelper urlHelper,
            testePAPContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        private IQueryable<Utilizador> GetAll(QueryParameters queryParameters)
        {
            IQueryable<Utilizador> _allItems = _context.Utilizador;

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
                            case "login":
                                _allItems = _allItems
                                    .Where(x => x.Login==filter);
                                break;
                            case "senha":
                                _allItems = _allItems
                                    .Where(x => x.Senha == filter);
                                break;
                            case "idutilizador":
                                _allItems = _allItems
                                    .Where(x => x.IdUtilizador.ToString() == filter);
                                break;
                        }
                    }
                }

            }

            return _allItems
                .OrderByDescending(x => x.Login)
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }


        [HttpGet(Name = nameof(GetAllUtilizador))]
        public ActionResult GetAllUtilizador(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<Utilizador> utilizador = GetAll(queryParameters).ToList();

            var allItemCount = _context.Utilizador.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForCollection(queryParameters, allItemCount, version);

            var toReturn = utilizador.Select(x => ExpandSingleUtilizadorItem(x, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

      

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleUtilizador))]
        public ActionResult GetSingleUtilizador(ApiVersion version, int id)
        {
            Utilizador utilizador = _context.Utilizador.FirstOrDefault(x => x.IdUtilizador == id); 

            if (utilizador == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleUtilizadorItem(utilizador, version));
        }

        [HttpPost(Name = nameof(AddUtilizador))]
        public ActionResult<UtilizadorDto> AddUtilizador(ApiVersion version, [FromBody] UtilizadorCreateDto utilizadorCreateDto)
        {
            if (utilizadorCreateDto == null)
            {
                return BadRequest();
            }

            Utilizador toAdd = _mapper.Map<Utilizador>(utilizadorCreateDto);

            _context.Add(toAdd);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Creating a utilizador failed on save.");
            }

            Utilizador newUtilizadorItem = _context.Utilizador.FirstOrDefault(x => x.IdUtilizador == toAdd.IdUtilizador);

            return CreatedAtRoute(nameof(GetSingleUtilizador),
                new { version = version.ToString(), id = newUtilizadorItem.IdUtilizador },
                _mapper.Map<Utilizador>(newUtilizadorItem));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateUtilizador))]
        public ActionResult<UtilizadorDto> PartiallyUpdateUtilizador(int id, [FromBody] JsonPatchDocument<UtilizadorUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            Utilizador existingEntity = _context.Utilizador.FirstOrDefault(x => x.IdUtilizador == id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            UtilizadorUpdateDto utilizadorUpdateDto = _mapper.Map<UtilizadorUpdateDto>(existingEntity);
            patchDoc.ApplyTo(utilizadorUpdateDto);

            TryValidateModel(utilizadorUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(utilizadorUpdateDto, existingEntity);
            _context.Utilizador.Update(existingEntity);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a utilizadoritem failed on save.");
            }

            return Ok(_mapper.Map<Utilizador>(existingEntity));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveUtilizador))]
        public ActionResult RemoveUtilizador(int id)
        {
            Utilizador utilizadorItem = _context.Utilizador.FirstOrDefault(x => x.IdUtilizador == id);

            if (utilizadorItem == null)
            {
                return NotFound();
            }

            _context.Utilizador.Remove(utilizadorItem);

            if (_context.SaveChanges() == 0 )
            {
                throw new Exception("Deleting a utilizadoritem failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateUtilizador))]
        public ActionResult<UtilizadorDto> UpdateUtilizador(int id, [FromBody] UtilizadorUpdateDto utilizadorUpdateDto)
        {
            if (utilizadorUpdateDto == null)
            {
                return BadRequest();
            }

            var existingUtilizadorItem = _context.Utilizador.FirstOrDefault(x => x.IdUtilizador == id);

            if (existingUtilizadorItem == null)
            {
                return NotFound();
            }

            _mapper.Map(utilizadorUpdateDto, existingUtilizadorItem);

            _context.Update(existingUtilizadorItem);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a utilizadoritem failed on save.");
            }

            return Ok(_mapper.Map<UtilizadorDto>(existingUtilizadorItem));
        }

        private List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllUtilizador), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.Page,
                orderby = queryParameters.OrderBy
            }), "self", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllUtilizador), new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            }), "first", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllUtilizador), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            }), "last", "GET"));

            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllUtilizador), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                }), "next", "GET"));
            }

            if (queryParameters.HasPrevious())
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllUtilizador), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                }), "previous", "GET"));
            }

            var posturl = _urlHelper.Link(nameof(AddUtilizador), new { version = version.ToString() });

            links.Add(
               new LinkDto(posturl,
               "create_utilizador",
               "POST"));

            return links;
        }

        private dynamic ExpandSingleUtilizadorItem(Utilizador utilizador, ApiVersion version)
        {
            var links = GetLinks(utilizador.IdUtilizador, version);
            Utilizador item = _mapper.Map<Utilizador>(utilizador);

            var resourceToReturn = item.ToDynamic() as IDictionary<string, object>;
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }

        private IEnumerable<LinkDto> GetLinks(int id, ApiVersion version)
        {
            var links = new List<LinkDto>();

            var getLink = _urlHelper.Link(nameof(GetSingleUtilizador), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(getLink, "self", "GET"));

            var deleteLink = _urlHelper.Link(nameof(RemoveUtilizador), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(deleteLink,
              "delete_utilizador",
              "DELETE"));

            var createLink = _urlHelper.Link(nameof(AddUtilizador), new { version = version.ToString() });

            links.Add(
              new LinkDto(createLink,
              "create_utilizador",
              "POST"));

            var updateLink = _urlHelper.Link(nameof(UpdateUtilizador), new { version = version.ToString(), id = id });

            links.Add(
               new LinkDto(updateLink,
               "update_utilizador",
               "PUT"));

            return links;
        }
    }
}

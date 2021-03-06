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
using SampleWebApiAspNetCore.Services;

namespace SampleWebApiAspNetCore.v1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/marcacao")]
    //[Route("api/[controller]")]
    public class MarcacaoController : ControllerBase
    {
        private readonly testePAPContext _context;
        private readonly IUrlHelper _urlHelper;
        private readonly IMapper _mapper;

        public MarcacaoController(
            IUrlHelper urlHelper,
            testePAPContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        private IQueryable<Marcacao> GetAll(QueryParameters queryParameters) {

            IQueryable<Marcacao> _allItems = _context.Marcacao;

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
                            case "idmarcacao":
                                _allItems = _allItems
                                    .Where(x => x.IdMarcacao.ToString() == filter);
                                break;
                            case "data":
                                _allItems = _allItems
                                    .Where(x => x.Data.ToString() == filter);
                                break;
                            case "hora":
                                _allItems = _allItems
                                    .Where(x => x.Hora.ToString() == filter);
                                break;
                            case "idfuncionario":
                                _allItems = _allItems
                                    .Where(x => x.IdFuncionario.ToString() == filter);
                                break;
                            case "idpaciente":
                                _allItems = _allItems
                                    .Where(x => x.IdPaciente.ToString() == filter);
                                break;
                            case "idtecnico":
                                _allItems = _allItems
                                    .Where(x => x.IdTecnico.ToString() == filter);
                                break;
                        }
                     }
                }

                
            }

            return _allItems.OrderByDescending(x => x.IdMarcacao)
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        private IList<Marcacao> GetFilter(FilterDTO filter) {

            IQueryable<Marcacao> _allItems = _context.Marcacao;
            _allItems = _allItems.ToFilterView(filter);
            return _allItems.ToList();
        }

        [HttpPost]
        [Route("filter")]
        public ActionResult GetFilterMarcacao(ApiVersion version, FilterDTO filter) {
            List<Marcacao> marcacao = GetFilter(filter).ToList();
            var allItemCount = _context.Marcacao.Count();

            var toReturn = marcacao.Select(x => ExpandSingleMarcacaoItem(x, version));

            return Ok(new {
                value = toReturn
            });
        }


        [HttpGet(Name = nameof(GetAllMarcacao))]
        public ActionResult GetAllMarcacao(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<Marcacao> marcacao = GetAll(queryParameters).ToList();

            var allItemCount = _context.Marcacao.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForCollection(queryParameters, allItemCount, version);

            var toReturn = marcacao.Select(x => ExpandSingleMarcacaoItem(x, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleMarcacao))]
        public ActionResult GetSingleMarcacao(ApiVersion version, int id)
        {
            Marcacao marcacao = _context.Marcacao.FirstOrDefault(x => x.IdMarcacao == id); 

            if (marcacao == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleMarcacaoItem(marcacao, version));
        }

        [HttpPost(Name = nameof(AddMarcacao))]
        public ActionResult<MarcacaoDto> AddMarcacao(ApiVersion version, [FromBody] MarcacaoCreateDto marcacaoCreateDto)
        {
            if (marcacaoCreateDto == null)
            {
                return BadRequest();
            }

            Marcacao toAdd = _mapper.Map<Marcacao>(marcacaoCreateDto);

            if (toAdd.IdFuncionario == 0) {
                toAdd.IdFuncionario = null;
            }

            _context.Add(toAdd);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Creating a marcacao failed on save.");
            }

          //  Marcacao newMarcacaoItem = _context.Marcacao.FirstOrDefault(x => x.IdMarcacao == toAdd.IdMarcacao);

            return CreatedAtRoute(nameof(GetSingleMarcacao),
                new { version = version.ToString(), id = toAdd.IdMarcacao },
                _mapper.Map<MarcacaoCreateDto>(toAdd));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateMarcacao))]
        public ActionResult<MarcacaoDto> PartiallyUpdateMarcacao(int id, [FromBody] JsonPatchDocument<MarcacaoUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            Marcacao existingEntity = _context.Marcacao.FirstOrDefault(x => x.IdMarcacao == id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            MarcacaoUpdateDto marcacaoUpdateDto = _mapper.Map<MarcacaoUpdateDto>(existingEntity);
            patchDoc.ApplyTo(marcacaoUpdateDto);

            TryValidateModel(marcacaoUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(marcacaoUpdateDto, existingEntity);
            _context.Marcacao.Update(existingEntity);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a marcacaoitem failed on save.");
            }

            return Ok(_mapper.Map<Marcacao>(existingEntity));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveMarcacao))]
        public ActionResult RemoveMarcacao(int id)
        {
            Marcacao marcacaoItem = _context.Marcacao.FirstOrDefault(x => x.IdMarcacao == id);

            if (marcacaoItem == null)
            {
                return NotFound();
            }

            _context.Marcacao.Remove(marcacaoItem);

            if (_context.SaveChanges() == 0 )
            {
                throw new Exception("Deleting a marcacaoitem failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateMarcacao))]
        public ActionResult<MarcacaoDto> UpdateMarcacao(int id, [FromBody] MarcacaoUpdateDto marcacaoUpdateDto)
        {
            if (marcacaoUpdateDto == null)
            {
                return BadRequest();
            }
           

            var existingMarcacaoItem = _context.Marcacao.FirstOrDefault(x => x.IdMarcacao == id);

            if (existingMarcacaoItem == null)
            {
                return NotFound();
            }

            _mapper.Map(marcacaoUpdateDto, existingMarcacaoItem);

            if (existingMarcacaoItem.IdFuncionario == 0) {
                existingMarcacaoItem.IdFuncionario = null;
            }

            _context.Update(existingMarcacaoItem);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a fooditem failed on save.");
            }

            return Ok(_mapper.Map<MarcacaoDto>(existingMarcacaoItem));
        }

        private List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllMarcacao), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.Page,
                orderby = queryParameters.OrderBy
            }), "self", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllMarcacao), new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            }), "first", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllMarcacao), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            }), "last", "GET"));

            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllMarcacao), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                }), "next", "GET"));
            }

            if (queryParameters.HasPrevious())
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllMarcacao), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                }), "previous", "GET"));
            }

            var posturl = _urlHelper.Link(nameof(AddMarcacao), new { version = version.ToString() });

            links.Add(
               new LinkDto(posturl,
               "create_marcacao",
               "POST"));

            return links;
        }

        private dynamic ExpandSingleMarcacaoItem(Marcacao marcacao, ApiVersion version)
        {
            var links = GetLinks(marcacao.IdMarcacao, version);
            Marcacao item = _mapper.Map<Marcacao>(marcacao);
            if (item.IdFuncionario == null) {
                item.IdFuncionario = 0;
            }

            var resourceToReturn = item.ToDynamic() as IDictionary<string, object>;
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }

        private IEnumerable<LinkDto> GetLinks(int id, ApiVersion version)
        {
            var links = new List<LinkDto>();

            var getLink = _urlHelper.Link(nameof(GetSingleMarcacao), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(getLink, "self", "GET"));

            var deleteLink = _urlHelper.Link(nameof(RemoveMarcacao), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(deleteLink,
              "delete_marcacao",
              "DELETE"));

            var createLink = _urlHelper.Link(nameof(AddMarcacao), new { version = version.ToString() });

            links.Add(
              new LinkDto(createLink,
              "create_marcacao",
              "POST"));

            var updateLink = _urlHelper.Link(nameof(UpdateMarcacao), new { version = version.ToString(), id = id });

            links.Add(
               new LinkDto(updateLink,
               "update_marcacao",
               "PUT"));

            return links;
        }
    }
}

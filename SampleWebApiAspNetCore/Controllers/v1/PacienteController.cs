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
using AutoMapper.QueryableExtensions;
using SampleWebApiAspNetCore.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SampleWebApiAspNetCore.v1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/paciente")]
    //[Route("api/[controller]")]
    public class PacienteController : ControllerBase
    {
        private readonly testePAPContext _context;
        private readonly IUrlHelper _urlHelper;
        private readonly IMapper _mapper;

        public PacienteController(
            IUrlHelper urlHelper,
            testePAPContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _urlHelper = urlHelper;
        }

        private IQueryable<Paciente> GetAll(QueryParameters queryParameters)
        {

            IQueryable<Paciente> _allItems = _context.Paciente;

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
                                    .Where(x => x.Nif==filter);
                                break;
                        }
                    }
                }

            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }
        private IList<Paciente> GetFilter(FilterDTO filter)
        {

            IQueryable<Paciente> _allItems = _context.Paciente;
            _allItems = _allItems.ToFilterView(filter);
            return _allItems.ToList();
        }

        [HttpPost]
        [Route("filter")]
        public ActionResult GetFilterPaciente(ApiVersion version, FilterDTO filter)
        {
            List<Paciente> paciente = GetFilter(filter).ToList();
            var allItemCount = _context.Paciente.Count();

            var toReturn = paciente.Select(x => ExpandSinglePacienteItem(x, version));

            return Ok(new
            {
                value = toReturn
            });
        }

        [HttpGet(Name = nameof(GetAllPaciente))]
        public ActionResult GetAllPaciente(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<Paciente> paciente = GetAll(queryParameters).ToList();

            var allItemCount = _context.Paciente.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForCollection(queryParameters, allItemCount, version);

            var toReturn = paciente.Select(x => ExpandSinglePacienteItem(x, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }


        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSinglePaciente))]
        public ActionResult GetSinglePaciente(ApiVersion version, int id)
        {
            Paciente paciente = _context.Paciente.FirstOrDefault(x => x.IdPaciente == id); 

            if (paciente == null)
            {
                return NotFound();
            }

            return Ok(ExpandSinglePacienteItem(paciente, version));
        }

        [HttpPost(Name = nameof(AddPaciente))]
        public ActionResult<PacienteDto> AddPaciente(ApiVersion version, [FromBody] PacienteCreateDto pacienteCreateDto)
        {
            if (pacienteCreateDto == null)
            {
                return BadRequest();
            }

            Paciente toAdd = _mapper.Map<Paciente>(pacienteCreateDto);

            _context.Add(toAdd);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Creating a paciente failed on save.");
            }

            Paciente newPacienteItem = _context.Paciente.FirstOrDefault(x => x.IdPaciente == toAdd.IdPaciente);

            return CreatedAtRoute(nameof(GetSinglePaciente),
                new { version = version.ToString(), id = newPacienteItem.IdPaciente },
                _mapper.Map<Paciente>(newPacienteItem));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdatePaciente))]
        public ActionResult<PacienteDto> PartiallyUpdatePaciente(int id, [FromBody] JsonPatchDocument<PacienteUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            Paciente existingEntity = _context.Paciente.FirstOrDefault(x => x.IdPaciente == id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            PacienteUpdateDto pacienteUpdateDto = _mapper.Map<PacienteUpdateDto>(existingEntity);
            patchDoc.ApplyTo(pacienteUpdateDto);

            TryValidateModel(pacienteUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pacienteUpdateDto, existingEntity);
            _context.Paciente.Update(existingEntity);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a pacienteitem failed on save.");
            }

            return Ok(_mapper.Map<Paciente>(existingEntity));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemovePaciente))]
        public ActionResult RemovePaciente(int id)
        {
            Paciente pacienteItem = _context.Paciente.FirstOrDefault(x => x.IdPaciente == id);

            if (pacienteItem == null)
            {
                return NotFound();
            }

            _context.Paciente.Remove(pacienteItem);

            if (_context.SaveChanges() == 0 )
            {
                throw new Exception("Deleting a pacienteitem failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdatePaciente))]
        public ActionResult<PacienteDto> UpdatePaciente(int id, [FromBody] PacienteUpdateDto pacienteUpdateDto)
        {
            if (pacienteUpdateDto == null)
            {
                return BadRequest();
            }

            var existingPacienteItem = _context.Paciente.FirstOrDefault(x => x.IdPaciente == id);

            if (existingPacienteItem == null)
            {
                return NotFound();
            }

            _mapper.Map(pacienteUpdateDto, existingPacienteItem);

            _context.Update(existingPacienteItem);

            if (_context.SaveChanges() == 0)
            {
                throw new Exception("Updating a pacienteitem failed on save.");
            }

            return Ok(_mapper.Map<PacienteDto>(existingPacienteItem));
        }

        private List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllPaciente), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.Page,
                orderby = queryParameters.OrderBy
            }), "self", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllPaciente), new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            }), "first", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllPaciente), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            }), "last", "GET"));

            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllPaciente), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                }), "next", "GET"));
            }

            if (queryParameters.HasPrevious())
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetAllPaciente), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                }), "previous", "GET"));
            }

            var posturl = _urlHelper.Link(nameof(AddPaciente), new { version = version.ToString() });

            links.Add(
               new LinkDto(posturl,
               "create_paciente",
               "POST"));

            return links;
        }

        private dynamic ExpandSinglePacienteItem(Paciente paciente, ApiVersion version)
        {
            var links = GetLinks(paciente.IdPaciente, version);
            Paciente item = _mapper.Map<Paciente>(paciente);

            var resourceToReturn = item.ToDynamic() as IDictionary<string, object>;
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }

        private IEnumerable<LinkDto> GetLinks(int id, ApiVersion version)
        {
            var links = new List<LinkDto>();

            var getLink = _urlHelper.Link(nameof(GetSinglePaciente), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(getLink, "self", "GET"));

            var deleteLink = _urlHelper.Link(nameof(RemovePaciente), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(deleteLink,
              "delete_paciente",
              "DELETE"));

            var createLink = _urlHelper.Link(nameof(AddPaciente), new { version = version.ToString() });

            links.Add(
              new LinkDto(createLink,
              "create_paciente",
              "POST"));

            var updateLink = _urlHelper.Link(nameof(UpdatePaciente), new { version = version.ToString(), id = id });

            links.Add(
               new LinkDto(updateLink,
               "update_paciente",
               "PUT"));

            return links;
        }
    }
}

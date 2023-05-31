using AutoMapper;
using MagicVilla_WebAPI.Data;
using MagicVilla_WebAPI.Logging;
using MagicVilla_WebAPI.Models.Dto;
using MagicVilla_WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MagicVilla_WebAPI.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace MagicVilla_WebAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class VillaController : ControllerBase
    {

        private readonly ILogging _logger;
        private readonly IMapper _mapper;
        private readonly IVillaRepository _villaRepository;
        protected APIResponse _apiResponse;

        public VillaController(ILogging logger, IMapper mapper, IVillaRepository villaRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _villaRepository = villaRepository;
            _apiResponse = new();
        }

        [HttpGet]
        //[Authorize]
        //[ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VillaDTO>))]
        public async Task<ActionResult<APIResponse>> Villas([FromQuery(Name = "filter occupancy")] int? Occupancy, [FromQuery] string? search,
            [FromQuery] string? sortBy, [FromQuery] string? sortOrder, int pageSize = 0, int pageNo = 1)
        {
            try
            {
                IEnumerable<Villa> villas;

                if (Occupancy > 0)
                {
                    villas = await _villaRepository.GetAllAsync(v => v.Occupancy == Occupancy, pageSize: pageSize, pageNo: pageNo);
                }
                else
                {
                    _logger.Log("Getting all Villas", "");
                    villas = await _villaRepository.GetAllAsync(pageSize: pageSize, pageNo: pageNo);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    villas = villas.Where(v => v.Name.ToLower().Contains(search)
                    || v.Amenity.ToLower().Contains(search));
                }
                if (!string.IsNullOrEmpty(sortBy))
                {
                    if (typeof(VillaDTO).GetProperty(sortBy) != null)
                    {

                        villas = await _villaRepository.SortbyParam(sortBy, sortOrder);
                    }
                }
                Pagination pagination = new Pagination() { PageNo = pageNo, PageSize = pageSize };
                //add pagination to header
                Response.Headers.Add("X-pagination", JsonSerializer.Serialize(pagination));
                _apiResponse.Result = _mapper.Map<List<VillaDTO>>(villas);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _apiResponse;
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [Authorize(Roles = "admin")]
        //after 30 sec our cache will expire
        //[ResponseCache(Duration = 30)]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // every id will have diffrent chache
        //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<APIResponse>> Villa(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.Log("Get Villa Error with id: " + id, "Error");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                var villa = await _villaRepository.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                _apiResponse.Result = _mapper.Map<VillaDTO>(villa);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _apiResponse;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> Villa([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (await _villaRepository.GetAsync(x => x.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa already Exist");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                if (ModelState.IsValid)
                {
                    Villa villa = _mapper.Map<Villa>(createDTO);
                    await _villaRepository.CreateAsync(villa);
                    _apiResponse.Result = _mapper.Map<VillaDTO>(villa);
                    _apiResponse.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetVilla", new { id = villa.Id }, _apiResponse);
                }
                else
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _apiResponse;

        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Custom")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                var villa = await _villaRepository.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                await _villaRepository.RemoveAsync(villa);
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _apiResponse;

        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || updateDTO.Id != id)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                Villa villa = _mapper.Map<Villa>(updateDTO);

                await _villaRepository.UpdateAsync(villa);
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _apiResponse;
        }

        [HttpPatch("{id}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            try
            {
                if (id == 0 || patchDTO == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                var villa = await _villaRepository.GetAsync(v => v.Id == id, false);

                if (villa == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                if (!ModelState.IsValid)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                VillaUpdateDTO villaUpdate = _mapper.Map<VillaUpdateDTO>(villa);

                patchDTO.ApplyTo(villaUpdate, ModelState);

                Villa model = _mapper.Map<Villa>(villaUpdate);

                await _villaRepository.UpdateAsync(model);
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _apiResponse;

        }
    }
}

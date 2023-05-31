using AutoMapper;
using MagicVilla_WebAPI.Models;
using MagicVilla_WebAPI.Models.Dto;
using MagicVilla_WebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace MagicVilla_WebAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    // when we are calling APi we need to pass what version we are looking for
    [ApiVersion("1.0",Deprecated =true)]
    public class VillaNumberController : ControllerBase
    {
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IMapper _mapper;
        private readonly IVillaRepository _villaRepository;
        protected APIResponse _apiResponse;

        public VillaNumberController(IVillaNumberRepository villaNumberRepository, IMapper mapper, IVillaRepository villaRepository)
        {
            _villaNumberRepository = villaNumberRepository;
            _mapper = mapper;
            _villaRepository = villaRepository;
            _apiResponse = new();
        }

        // v1 of this mehtod is going to be depriciated in one month
        [HttpGet("GetString")]
        public IEnumerable<string> Get()
        {
            return new string[] { "V1", "value2" };
        }

        //[MapToApiVersion("1.0")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VillaNumberDTO>))]
        public async Task<ActionResult<APIResponse>> VillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumbers = await _villaNumberRepository.GetAllAsync();
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbers);
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _apiResponse;
        }

        [HttpGet("{villaNo}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaNumberDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> VillaNumber(int villaNo)
        {
            try
            {
                if (villaNo == 0)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                VillaNumber villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo);

                if (villaNumber == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _apiResponse;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> VillaNumber([FromBody] VillaNumberCreateDTO villaNumberCreateDTO)
        {
            try
            {
                if (villaNumberCreateDTO == null || villaNumberCreateDTO.VillaNo == 0)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                if (await _villaRepository.GetAsync(v => v.Id == villaNumberCreateDTO.VillaId) == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    ModelState.AddModelError("CustomError", "VillaId is Invalid!");
                    return BadRequest(ModelState);
                }

                if (ModelState.IsValid)
                {
                    VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberCreateDTO);
                    await _villaNumberRepository.CreateAsync(villaNumber);
                    _apiResponse.StatusCode = HttpStatusCode.Created;
                    _apiResponse.Result = villaNumber;
                    return CreatedAtRoute("GetVillaNumber", new { villaNo = villaNumber.VillaNo }, villaNumber);
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
                _apiResponse.ErrorMessages = new List<string> { ex.Message };
            }

            return _apiResponse;

        }

        [HttpDelete("{villaNo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int villaNo)
        {
            try
            {
                if (villaNo == 0)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                VillaNumber villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo);

                if (villaNumber == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);

                }

                await _villaNumberRepository.RemoveAsync(villaNumber);
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.Message };
            }

            return _apiResponse;
        }

        [HttpPut("{villaNo}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, VillaNumberUpdateDTO villaNumberUpdateDTO)
        {
            try
            {
                if (villaNo == 0)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                if (await _villaRepository.GetAsync(v => v.Id == villaNumberUpdateDTO.VillaId) == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    ModelState.AddModelError("CustomError", "VillaId is Invalid");
                    return BadRequest(_apiResponse);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberUpdateDTO);
                await _villaNumberRepository.UpdateAsync(villaNumber);
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.Message };
            }

            return _apiResponse;

        }

        [HttpPatch("villaNo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVillaNumber(int villaNo, JsonPatchDocument<VillaNumberUpdateDTO> patchDTO)
        {
            try
            {
                if (patchDTO == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                VillaNumber villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo, false);

                if (villaNumber == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                VillaNumberUpdateDTO villaNumberUpdate = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);

                patchDTO.ApplyTo(villaNumberUpdate, ModelState);

                VillaNumber model = _mapper.Map<VillaNumber>(villaNumberUpdate);

                await _villaNumberRepository.UpdateAsync(model);

                _apiResponse.StatusCode = HttpStatusCode.NoContent;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.Message };
            }

            return _apiResponse;


        }

    }


}


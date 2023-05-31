using AutoMapper;
using MagicVilla_WebAPI.Models;
using MagicVilla_WebAPI.Models.Dto;
using MagicVilla_WebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace MagicVilla_WebAPI.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
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

        //[MapToApiVersion("1.0")]
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VillaNumberDTO>))]
        //public async Task<ActionResult<APIResponse>> VillaNumbers()
        //{
        //    try
        //    {
        //        IEnumerable<VillaNumber> villaNumbers = await _villaNumberRepository.GetAllAsync();
        //        _apiResponse.StatusCode = HttpStatusCode.OK;
        //        _apiResponse.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbers);
        //        return Ok(_apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _apiResponse.IsSuccess = false;
        //        _apiResponse.ErrorMessages = new List<string> { ex.Message };
        //    }
        //    return _apiResponse;
        //}

        //[MapToApiVersion("2.0")]
        [HttpGet("GetString")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

    }


}


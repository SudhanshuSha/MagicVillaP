using MagicVilla_WebAPI.Models;
using MagicVilla_WebAPI.Models.Dto;
using MagicVilla_WebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_WebAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    // its same for v1 and v2
    [ApiVersionNeutral]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        protected APIResponse _response;

        public UserController(IUserRepository repository)
        {
            _repository = repository;
            _response = new APIResponse();
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = await _repository.Login(loginRequestDTO);

            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_response);
            }

            _response.Result = loginResponse;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }


        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Registration([FromBody] RegisterationRequestDTO registerationRequestDTO)
        {
            bool isUserUnique = _repository.IsUniqueUser(registerationRequestDTO.UserName);

            if (!isUserUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exist!");
                return BadRequest(_response);
            }

            var user = await _repository.Register(registerationRequestDTO);

            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error While Registering");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

    }
}

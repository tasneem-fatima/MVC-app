using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;
        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _responseDto = new();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDtO registrationRequestDtO)
        {
            var errorMessage = await _authService.Register(registrationRequestDtO);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);
            }

            return Ok(_responseDto);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _authService.Login(loginRequestDto);
            if (loginResponse.User == null)
            {
                _responseDto.IsSuccess = false ;
                _responseDto.Message = "Username or password is invalid";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponse;
            return Ok(_responseDto);
        }
        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDtO registrationRequestDtO)
        {
            var assignRole = await _authService.AssignRole(registrationRequestDtO.Email, registrationRequestDtO.Role.ToUpper());
            if (!assignRole)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error encountered";
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }
    }
}

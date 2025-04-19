using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using System.Diagnostics;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDtO registrationRequestDtO)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = Utility.SD.ApiType.POST,
                Data = registrationRequestDtO,
                Url = SD.AuthAPIBase + "/api/auth/assignRole"
            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDtO registrationRequestDtO)
        {
            string apiUrl = SD.AuthAPIBase + "/api/auth/register";
            Debug.WriteLine("Registering user with API URL: {ApiUrl}", apiUrl);

            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDtO,
                Url = apiUrl
            }, withBearer: false);
        }
    }
}

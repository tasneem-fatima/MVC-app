using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDtO registrationRequestDtO);
        Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDtO registrationRequestDtO);
    }
}

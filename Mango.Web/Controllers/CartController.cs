using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IShoppingCartService _cartService;
        public CartController(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedonLoggedInUser());
        }
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            ResponseDto responseDto = await _cartService.ApplyCoupon(cartDto);
            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader!.CouponCode = "";
            ResponseDto responseDto = await _cartService.ApplyCoupon(cartDto);
            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        private async Task<CartDto> LoadCartDtoBasedonLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)
                ?.FirstOrDefault()?.Value;

            ResponseDto response = await _cartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>
                    (Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDto();
        }
    }
}

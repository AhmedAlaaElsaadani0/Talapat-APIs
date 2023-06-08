using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Dtos;
using Talabat.API.Errors;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;

namespace Talabat.API.Controllers
{
    public class BasketsController : BaseAPIController
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketsController(IBasketRepository basketRepository_, IMapper mapper)
        {
            basketRepository = basketRepository_;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBaskeyByID(string id)
        {
            var basket = await basketRepository.GetBasketAsync(id);
            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var mappedBasket = mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var updatedOrCreatedBasket = await basketRepository.UpdateBasketAsync(mappedBasket);
            if (updatedOrCreatedBasket is null) return BadRequest(new ApiResponse(400));
            return Ok(updatedOrCreatedBasket);
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        { 
            var deleted = await basketRepository.DeleteBasketAsync(id);
            return Ok(deleted);
        }
    }
}

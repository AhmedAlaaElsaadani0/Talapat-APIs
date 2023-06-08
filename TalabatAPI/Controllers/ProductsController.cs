using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Talabat.API.Dtos;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;
using Talabat.Core.Specifications.Products;

namespace Talabat.API.Controllers
{
    public class ProductsController : BaseAPIController
    {
        ///private readonly IGenericRepository<Product> productsRepo;
        ///private readonly IGenericRepository<ProductBrand> productBrandsRepo;
        ///private readonly IGenericRepository<ProductType> productTypesRepo;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ProductsController(
            ///IGenericRepository<Product> productsRepo_,
            ///IGenericRepository<ProductBrand> productBrandsRepo_,
            ///IGenericRepository<ProductType> productTypesRepo_,
            IUnitOfWork unitOfWork_,
            IMapper mapper_)
        {
            ///productsRepo = productsRepo_;
            ///productBrandsRepo = productBrandsRepo_;
            ///productTypesRepo = productTypesRepo_;
            unitOfWork = unitOfWork_;
            mapper = mapper_;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(specParams);

            var products = await unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var data = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var CountSpec = new ProductWithFilterationForCountSpecification(specParams);

            var count = await unitOfWork.Repository<Product>().GetCountWithSpecAsync(CountSpec);

            return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex, specParams.PageSize, count, data));
        }

        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(id);

            var product = await unitOfWork.Repository<Product>().GetByIdWithSpecAsync(spec);

            if (product is null)
                return NotFound(new ApiResponse(404));

            return Ok(mapper.Map<Product, ProductToReturnDto>(product));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var types = await unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(types);
        }

    }
}

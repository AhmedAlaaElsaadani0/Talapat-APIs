using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Talabat.Repository.Data;

namespace Talabat.API.Controllers
{
    public class BuggyController : BaseAPIController
    {
        private readonly StoreContext dbContext;

        public BuggyController(StoreContext dbContext_)
        {
            dbContext = dbContext_;
        }

        [HttpGet("notfound")]        // GET : api/buggy/notfound
        public ActionResult GetNotFoundRequest()
        {
            var product = dbContext.Products.Find(100);
            if (product is null)
                return NotFound(new ApiResponse(404));
            return Ok(product);
        }

        [HttpGet("servererror")]     // GET : api/buggy/servererror
        public ActionResult GetServerErorrRequest()
        {
            var product = dbContext.Products.Find(100);
            // Null Reference Exception
            var productToReturn = product.ToString();
            return Ok(productToReturn);
        }

        [HttpGet("badrequest")]      // GET : api/buggy/badrequest
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id}")] // GET : api/buggy/badrequest/one
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }

    }
}

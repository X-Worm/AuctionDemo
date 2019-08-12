using AuctionDemo.DAL.Models;
using AuctionDemo.DAL.Models.AuthRepository;
using AuctionDemo.BLL.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AuctionDemo.Controllers
{
    /// <summary>
    /// Operations on user
    /// </summary>
    public class UserController : BaseController
    {
       public UserController()
        {

        }

        /// <summary>
        /// Register user in system
        /// </summary>

        /// <param name="model"></param>
        /// <response code="200">successful operation</response>
        /// <response code="401">unauthorized</response>
        /// <response code="500">internal server error</response>
        [AllowAnonymous]
        [Route("api/user/register")]
        public async Task<IHttpActionResult> Register(User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var repository = new AuthRepository())
            {
                IdentityResult result = await repository.RegisterUser(model);
                return Created("successful registration", model);
            }
        }
        

        /// <summary>
        /// Logs out current logged in user session
        /// </summary>
        /// <response code="0">successful operation</response>
        [HttpGet]
        [Route("api/user/logout")]
        
        public  IHttpActionResult LogoutUser()
        {
            return Ok();
        }
        

        /// <summary>
        /// Get account info
        /// </summary>
        /// <param name="amount"></param>
        /// <response code="200">successful operation</response>
        /// <response code="400">bad request , invalid Amount</response>
        /// <response code="401">unauthorized</response>
        /// <response code="500">internal server error</response>
        [HttpGet]
        [Authorize]
        [Route("api/user/account/info")]
        public  IHttpActionResult UserAccountGet()
        {
            var result = new UserService().GetAccount(UserId);
            return Ok(result);
        }


        /// <summary>
        /// Withdraw money from the account
        /// </summary>

        /// <param name="amount"></param>
        /// <response code="200">successful operation</response>
        /// <response code="400">bad request , invalid Amount</response>
        /// <response code="401">unauthorized</response>
        /// <response code="500">internal server error</response>
        [HttpGet]
        [Authorize]
        [Route("api/user/account")]
        public IHttpActionResult UserAccountWidthdraw([FromUri]int amount)
        {
            var result = new UserService().WidthdrawFromAccount(UserId, amount);
            return Ok(result);
        }


        /// <summary>
        /// add  to account
        /// </summary>

        /// <param name="amount"></param>
        /// <response code="200">OK</response>
        /// <response code="401">unauthorized</response>
        /// <response code="500">internal server error</response>
        [Authorize]
        [HttpPost]
        [Route("api/user/account")]
        public IHttpActionResult UserAccountPost([FromUri]int amount)
        {
            var result = new UserService().AddMoney(UserId, amount);
            return Ok(result);
        }

        /// <summary>
        /// Get information about User Configurations 
        /// </summary>

        /// <response code="200">Post User_Configuration model that describe users configurations</response>
        /// <response code="401">unauthorized</response>
        /// <response code="500">internal server error</response>
        [HttpGet]
        [Route("api/user/configuration")]
        [Authorize]
        public  IHttpActionResult UserConfigurationGet()
        {
            var result = new UserConfigurationService().GetUserConfigurations(UserId);
            return Json(result);
        }

        /// <summary>
        /// Post User Configurations 
        /// </summary>

        /// <param name="userConfigurations">New User_Configuration object</param>
        /// <response code="201">Created</response>
        /// <response code="401">unauthorized</response>
        /// <response code="500">internal server error</response>
        [HttpPost]
        [Route("api/user/configuration")]
        [Authorize]
        public virtual IHttpActionResult UserConfigurationPost([FromBody]User_Configuration userConfigurations)
        {
            var result = new UserConfigurationService().PostUserConfigurations(UserId , userConfigurations);
            return Created("", result);
        }

        /// <summary>
        /// Put User Configurations 
        /// </summary>

        /// <param name="userConfigurations">New User_Configuration object</param>
        /// <response code="200">OK</response>
        /// <response code="401">unauthorized</response>
        /// <response code="500">internal server error</response>
        [HttpPut]
        [Route("api/user/configuration")]
        [Authorize]
        public virtual IHttpActionResult UserConfigurationPut([FromBody]User_Configuration userConfigurations)
        {
            var result = new UserConfigurationService().PutUserConfigurations(UserId, userConfigurations);
            return Created("", result);
        }

    }
}

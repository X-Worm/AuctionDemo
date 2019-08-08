using AuctionDemo.Models;
using AuctionDemo.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AuctionDemo.Controllers
{
    /// <summary>
    /// Operations on lot
    /// </summary>
    public class LotController : BaseController
    {
        /// <summary>
        /// Create new lot
        /// </summary>
        /// <remarks>This can only be done by the logged in user.</remarks>
        /// <param name="body">New lot object</param>
        /// <response code="201">created</response>
        /// <response code="401">unauthorized</response>
        /// <response code="500">internal server error</response>
        [HttpPost]
        [Authorize]
        [Route("api/lot")]
        public virtual IHttpActionResult PostLot([FromBody]Lot lot)
        {
           
            new LotService().CreateNewLot(lot , UserId);
            return Created("", lot);
        }

        /// <summary>
        /// Delete lot by id
        /// </summary>
        /// <remarks>This can only be done by the logged in user.</remarks>
        /// <param name="lotId">Id of lot to delete</param>
        /// <response code="204">No content</response>
        /// <response code="401">unauthorized</response>
        /// <response code="404">Not found</response>
        /// <response code="500">internal server error</response>
        [HttpDelete]
        [Authorize]
        [Route("api/lot/{id}")]
        public IHttpActionResult DeleteLot(short? id)
        {
            new LotService().DeleteLot(id , UserId);

         return  ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }

        /// <summary>
        /// Get all lots of other users
        /// </summary>
        /// <remarks>This can only be done by the logged in user.</remarks>
        /// <param name="pagesize">Number of lots in page , by default is 5</param>
        /// <param name="pagenumber">Number of page , by default is 1</param>
        /// <param name="sort">sort list by comma separated set parameters , by default is Lot_Id</param>
        /// <param name="field">receive only the fields that you specify in the parameter, separated by a comma , by default is null</param>
        /// <response code="200">successful operation (List of lots or empty page)</response>
        /// <response code="401">unauthorized</response>
        /// <response code="500">internal server error</response>
        [HttpGet]
        [Authorize]
        [Route("api/lot")]
        public virtual IHttpActionResult GetLot([FromUri]int pagesize = 5, [FromUri]int pagenumber = 1, [FromUri]string sort = "")
        {
            var result = new LotService().GetLots(pagesize, pagenumber, sort);
            return Json(result);

        }



        /// <summary>
        /// Get all lots of other users
        /// </summary>
        /// <remarks>This can only be done by the logged in user.</remarks>
        /// <param name="pagesize">Number of lots in page , by default is 5</param>
        /// <param name="pagenumber">Number of page , by default is 1</param>
        /// <param name="sort">sort list by comma separated set parameters , by default is Lot_Id</param>
        /// <param name="field">receive only the fields that you specify in the parameter, separated by a comma , by default is null</param>
        /// <response code="200">successful operation (List of lots or empty page)</response>
        /// <response code="401">unauthorized</response>
        /// <response code="500">internal server error</response>
        [HttpGet]
        [Authorize]
        [Route("api/lot/{id}")]
        public virtual IHttpActionResult GetLot(short? id = 1)
        {
            int i = 1 / (id.Value - 3);
            var result = new LotService().GetLot(id);
            return Json(result);
        }


        /// <summary>
        /// Update lot by id
        /// </summary>
        /// <remarks>This can only be done by the logged in user.</remarks>
        /// <param name="lotId">Id of lot to update</param>
        /// <param name="body">New lot object</param>
        /// <response code="200">OK</response>
        /// <response code="401">unauthorized</response>
        /// <response code="404">Invalid ID , Not found</response>
        /// <response code="500">internal server error</response>
        [HttpPut]
        [Authorize]
        [Route("api/lot/{id}")]
        public  IHttpActionResult PutLot(short? id, [FromBody]Lot body)
        { 

            new LotService().UpdateLot(id, UserId , body);

            return Ok();
        }
    }
}

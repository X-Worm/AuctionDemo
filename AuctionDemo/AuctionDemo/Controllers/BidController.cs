﻿using AuctionDemo.DAL.Models;
using AuctionDemo.BLL.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static AutoMapper.Mapper;

using AuctionDemo.ViewModels.Mappers;

namespace AuctionDemo.Controllers
{
    /// <summary>
    /// Operations about bid
    /// </summary>
    public class BidController : BaseController
    {
        public BidController()
        {

        }

        /// <summary>
        /// Create new bid on user lot
        /// </summary>
        /// <remarks>This can only be done by the logged in user.</remarks>
        /// <param name="body">New bid object</param>
        /// <response code="201">created</response>
        /// <response code="401">unauthorized</response>
        /// <response code="404">Not found , invalid Lot_id</response>
        /// <response code="500">internal server error</response>
        [HttpPost]
        [Authorize]
        [Route("api/bid")]
        public IHttpActionResult BidCreateBidPost([FromBody]Bid bid)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException("Model bid is invalid");
            }
            var bidService = new BidService();
            bidService.CreateNewBid(bid , UserId);

            return Created("", bid);
        }


        /// <summary>
        /// Get all bids of some lot 
        /// </summary>
        /// <remarks>This can only be done by the logged in user.</remarks>
        /// <param name="lotId">Get all bid of some lot</param>
        /// <param name="pagesize">Number of lots in page , by default is 5</param>
        /// <param name="pagenumber">Number of page , by default is 1</param>
        /// <param name="sort">sort list by comma separated set parameters , by default is Lot_Id</param>
        /// <param name="field">receive only the fields that you specify in the parameter, separated by a comma , by default is null</param>
        /// <response code="200">successful operation (List of bids or empty page)</response>
        /// <response code="401">unauthorized</response>
        /// <response code="404">Not found, invalid Lot_Id</response>
        /// <response code="500">internal server error</response>
        [HttpGet]
        [Authorize]
        [Route("api/bid/{lotId}")]
        public IHttpActionResult BidGetBidsGet([FromUri]short lotId = 1, [FromUri]int pagesize = 5, [FromUri]int pagenumber = 1,
              [FromUri]string sort = "", [FromUri]string filterPrice = "", [FromUri]string filterDate = "")
        {
            // Example: filterPrice in format - 50,100 - get Only Price in range (50 , 100) , if formatPrice - 50 - get price in range (0 , 50)
            var result = new BidService().GetAllBids(lotId, sort, filterPrice, filterDate, pagesize, pagenumber);

            if (result != null && result.Count != 0) return Ok(mapper.Map<IEnumerable<BidViewModel>>(result));
            // if (result != null && result.Count != 0) return Ok(mapper.Map<IEnumerable<BidViewModel>>(result));
            else return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
            
        }

    }
}

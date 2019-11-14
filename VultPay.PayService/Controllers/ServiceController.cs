using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace VultPay.PayService.Controllers
{
    public class ServiceController : Controller
    {

        /// <summary>
        /// 
        /// </summary>
        IConfiguration Configuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public ServiceController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Domain.Models.Application.TransactionResult> SinglePay([FromBody]Domain.Models.Application.Transaction transaction)
        {
            try
            {
                return await Application
                                .Transactions
                                .SinglePayment
                                .Create(Configuration)
                                .PayAsync(transaction);
            }
            catch
            {
                throw;
            }
        }

    }
}
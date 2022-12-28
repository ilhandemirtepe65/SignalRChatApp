using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SignalRChatApp.Models;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http.Connections;






//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;

//using Microsoft.Extensions.Caching.Distributed;
//using Newtonsoft.Json;
//using System.Text;
//using Microsoft.AspNetCore.Http;
//using System.Net.Http.Headers;
//using System.IO;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.SignalR.Client;












namespace SignalRChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCache _distributedCache;
        public static List<UserMessage> model = new List<UserMessage>();
        static HubConnection connectionSignalR;
        public HomeController(ILogger<HomeController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }
        public async Task<IActionResult> Index()
        {
            var data = await AddRedisCache(model, 130, "usermessage");
            return View(data);
        }

        public async Task<List<UserMessage>> AddRedisCache(List<UserMessage> allData, int cacheTime, string cacheKey)
        {
            var dataUserMessage = await _distributedCache.GetAsync(cacheKey);
            if (dataUserMessage == null)
            {
                var data = JsonConvert.SerializeObject(allData);
                var dataByte = Encoding.UTF8.GetBytes(data);

                var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(cacheTime));
                option.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheTime);
                await _distributedCache.SetAsync(cacheKey, dataByte, option);
            }
            var newsString = await _distributedCache.GetStringAsync(cacheKey);
            return JsonConvert.DeserializeObject<List<UserMessage>>(newsString);

        }
        [HttpPost]
        public async Task<IActionResult> SaveUserMessage(UserMessage userMessage)
        {

            model.Insert(0, userMessage);
            var data = await AddRedisCache(model, 130, "usermessage");
            //Push SignalR
            //Trigger The Signal
            Connect().Wait();
            await connectionSignalR.InvokeAsync("SendMessage", userMessage);
            return RedirectToAction("Index");
        }
        public static async Task Connect()
        {
            connectionSignalR = new HubConnectionBuilder()
                .WithUrl("http://localhost:5142/chatHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                })

                .Build();
            await connectionSignalR.StartAsync();


            //connectionSignalR = new HubConnectionBuilder().WithUrl("http://localhost:5142/chatHub", options => {
            //    options.Transports = HttpTransportType.WebSockets;
            //}).WithAutomaticReconnect().Build();
            //await connectionSignalR.StartAsync();
        }
    }
}
using Azure_queue.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace Azure_queue.Controllers
{
    public class HomeController : Controller
    {
        MyQueueServiceClient MyQueue = new MyQueueServiceClient();

        List<string> messages = new List<string>();

        List<Lot> lots = new List<Lot>();

        public HomeController()
        {
           
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Lots = await MyQueue.ShowAllAsync();
          // ViewBag.Lots=messages.ToArray();
            return View(messages);
        }
        
        [HttpPost]
        public async Task<IActionResult> Index(string id)//Index for specific currency
        {
            ViewBag.Lots = await MyQueue.ShowTarget(id);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLot(Lot lot)
        {
            if (ModelState.IsValid)
            {
              await MyQueue.AddMessageAsync(JsonSerializer.Serialize(lot));
                return RedirectToAction("Index");
            }

            return View();
        }

        public async Task<IActionResult>BuyLot(string id)
        {
            await MyQueue.DeleteMessage(id);

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public async Task<IActionResult> NewLot()
        {

            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
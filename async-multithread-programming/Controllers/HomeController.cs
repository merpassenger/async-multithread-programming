using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace async_multithread_programming.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : Controller
{
    
    //Asenkron programlamada Task sınıfı ile geriye bir durum kodu dönüleceğinin sözü verilmekte ve işlem başlamakta, işlem bitene kadar bir bekleme veya kitlenme söz konusu değildir.
    //Bu metot devam ederken başka istekler alınabilmektedir
    [HttpGet("GetContentAsync")]
    public async Task<IActionResult> GetContentWithTask()
    {
        var task = new HttpClient().GetStringAsync("https://www.google.com");

        var data = await task;

        return Ok(data.Length);
    }
    
    //Burada senkron bir metot olduğundan içeride thread tarafından metot çağrıldığında burada Ok ile data dönülünceye kadar başka bir istğin yapılması engellenecektir.
    [HttpGet("GetContent")]
    public IActionResult GetContentWithoutTask()
    {
        var client = new WebClient();
        var task =  client.DownloadString("https://www.google.com");

        var data = task;

        return Ok(data.Length);
    }
}
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace async_multithread_programming.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    /*
    Asenkron programlamada Task sınıfı ile geriye bir durum kodu dönüleceğinin sözü verilmekte ve işlem başlamakta, işlem bitene kadar bir bekleme veya kitlenme söz konusu değildir.
    Bu metot devam ederken başka istekler alınabilmektedir
    */
    [HttpGet("GetContentAsync")]
    public async Task<IActionResult> GetContentWithTask()
    {
        var task = new HttpClient().GetStringAsync("https://www.google.com");

        var data = await task;

        return Ok(data.Length);
    }

    /*
     * Burada senkron bir metot olduğundan içeride thread tarafından metot çağrıldığında burada Ok ile data dönülünceye kadar başka bir istğin yapılması engellenecektir.
     */
    [HttpGet("GetContent")]
    public IActionResult GetContentWithoutTask()
    {
        var client = new WebClient();
        var task = client.DownloadString("https://www.google.com");

        var data = task;

        return Ok(data.Length);
    }

    /*
     * Her async method async-await ikilisine sahip olmak zorunda değildir. Async, metot içerisinde async method kullanılacaksa gereklidir.
     * Bir metot async-await ikisilisi olmadan da text dönebileceğini gösterir.
     * Burada metot benden Task istemekte ve kullandığım async metot da geriye bir Task return etmekte olduğundan metot içerisinde bir patlama olmadı
     */
    [HttpGet("GetFileText")]
    public Task<string> ReadFileAsync()
    {
        var s = new StreamReader("file.txt");
        return s.ReadToEndAsync();
    }
    
    [HttpGet("GetDataTaskContinueWith")]
    public async Task<IActionResult> GetDataTaskContinueWithAsync()
    {
        Console.WriteLine("etc1");

        var task = new HttpClient().GetStringAsync("https://www.google.com")
            .ContinueWith(data => { Console.WriteLine(data.Result.Length); });

        Console.WriteLine("etc2");

        await task;

        return Ok(task);
        
        /*
         * etc1
         * etc2
         * task data 
         */
    }
}
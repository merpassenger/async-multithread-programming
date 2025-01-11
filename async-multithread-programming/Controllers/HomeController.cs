using System.Net;
using async_multithread_programming.Helpers;
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

    [HttpGet("GetDataTaskWhenAll")]
    public async Task<IActionResult> GetDataTaskWhenAllAsync()
    {
        Console.WriteLine("Main thread:" + Thread.CurrentThread.ManagedThreadId);
        List<string> urlList = new List<string>()
        {
            "https://www.google.com",
            "https://www.microsoft.com",
            "https://www.amazon.com",
        };

        List<Task<Content>> tasksList = new List<Task<Content>>();

        ContentMethod contentMethod = new ContentMethod();
        
        urlList.ToList().ForEach(x =>
        {
            tasksList.Add(contentMethod.GetContentAsync(x));
        });

        var contents = await Task.WhenAll(tasksList.ToArray());

        contents.ToList().ForEach(x =>
        {
            Console.WriteLine(x);
        });

        return Ok(contents);

    }
    
    [HttpGet("GetDataTaskWaitAll")]
    public async Task<IActionResult> GetDataTaskWaitAllAsync()
    {
        Console.WriteLine("Main thread:" + Thread.CurrentThread.ManagedThreadId);
        List<string> urlList = new List<string>()
        {
            "https://www.google.com",
            "https://www.microsoft.com",
            "https://www.amazon.com",
        };

        List<Task<Content>> tasksList = new List<Task<Content>>();

        ContentMethod contentMethod = new ContentMethod();
        
        urlList.ToList().ForEach(x =>
        {
            tasksList.Add(contentMethod.GetContentAsync(x));
        });

        Console.WriteLine("Before WaitAll Method:" + Thread.CurrentThread.ManagedThreadId);
        
        Task.WaitAll(tasksList.ToArray());

        Console.WriteLine("After WaitAll Method:" + Thread.CurrentThread.ManagedThreadId);
        
        /*
           Main thread:4
           Before WaitAll Method:4
           After WaitAll Method:4
           Main thread:25
           Before WaitAll Method:25
           Content thread:26
           Content thread:4
           Content thread:26
           After WaitAll Method:25
           
         */
        
        return Ok();

    }
    
   
}
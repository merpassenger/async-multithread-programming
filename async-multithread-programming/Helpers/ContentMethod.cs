namespace async_multithread_programming.Helpers;

public class ContentMethod
{
    public async Task<Content> GetContentAsync(string url)
    {
        Content c = new Content();

        var data = await new HttpClient().GetStringAsync(url);

        c.Website = url;
        c.Lnegth = data.Length;
        Console.WriteLine("Content thread:" +Thread.CurrentThread.ManagedThreadId);

        return c;
    }
}
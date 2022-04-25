namespace MinimalApi
{
    interface IStreamVideos
    {
        Task StreamVideo(HttpRequest request, HttpResponse response, CancellationToken cancellationToken);
    }

    public class VideoStreamer : IStreamVideos
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public VideoStreamer(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        async Task IStreamVideos.StreamVideo(HttpRequest request, HttpResponse response, CancellationToken cancellationToken)
        {
            string videosPath = _configuration.GetValue<string>("MyConfiguration:VideosPath");
            string filepath = Path.Join(_environment.WebRootPath, videosPath, "Chris-Do.mp4");

            _ = Path.GetFileName(filepath);

            Stream? stream = null;
            byte[] buffer = new byte[4096];
            int length;
            long dataToRead;

            try
            {
                stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);

                dataToRead = stream.Length;

                response.Headers["Accept-Ranges"] = "bytes";
                response.ContentType = "application/octet-stream";

                int startByte = 0;
                if (!String.IsNullOrEmpty(request.Headers["Range"]))
                {
                    string[] range = request.Headers["Range"].ToString().Split(new char[] { '=', '-' });
                    startByte = Int32.Parse(range[1]);
                    stream.Seek(startByte, SeekOrigin.Begin);

                    response.StatusCode = 206;
                    response.Headers["Content-Range"] = $" bytes {startByte}-{dataToRead - 1}/{dataToRead}";
                }

                var outputStream = response.Body;
                while (dataToRead > 0)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        length = await stream.ReadAsync(buffer, cancellationToken);

                        await outputStream.WriteAsync(buffer, cancellationToken);
                        await outputStream.FlushAsync(cancellationToken);

                        buffer = new byte[buffer.Length];
                        dataToRead -= buffer.Length;
                    }
                    else
                        dataToRead = -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }        
    }
}

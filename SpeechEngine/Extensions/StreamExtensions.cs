namespace SpeechEngine.Extensions;

public static class StreamExtensions
{
    public static async Task CopyToAsync(
        this Stream source,
        Stream destination,
        long contentLength,
        Action<double> updateProgressPercentages = default,
        int bufferSize = 104858,
        CancellationToken cancellationToken = default)
    {
        int bytesRead;
        long totalRead = 0;
        var buffer = new byte[bufferSize];

        while ((bytesRead = await source.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            totalRead += bytesRead;

            if (updateProgressPercentages != default)
                updateProgressPercentages(Math.Round(totalRead / (double)contentLength * 100d, 1));
        }
    }
}

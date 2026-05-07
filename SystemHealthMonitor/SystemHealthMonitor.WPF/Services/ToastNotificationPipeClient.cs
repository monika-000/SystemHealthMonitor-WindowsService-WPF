using System.IO;
using System.IO.Pipes;

namespace SystemHealthMonitor.WPF.Services
{
    internal static class ToastNotificationPipeClient
    {
        private const string _pipeName = "ToastNotificationPipe";
        public static async Task SendMessageAsync(string arg)
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out))
                {
                    await pipeClient.ConnectAsync();

                    using (StreamWriter sw = new StreamWriter(pipeClient))
                    {
                        await sw.WriteLineAsync(arg);
                        await sw.FlushAsync();
                    }

                }
            }
            catch (Exception ex) when (ex is IOException or ObjectDisposedException or TimeoutException or EndOfStreamException)
            {
                // Client disconnected or pipe broke log exception
            }
            catch (Exception ex)
            {
                //Log the error but do not break out of the loop. Otherwise the pipe will stop listening
                throw; 
            }

        }
    }
}

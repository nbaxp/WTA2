using System.Runtime.InteropServices;
using WTA.Application.Abstractions;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

/// <summary>
/// 单例
/// </summary>
public class LinuxService : IMonitorService
{
    [DllImport("libc.so.6")]
    private static extern int getpid();

    public LinuxService()
    {
    }

    public void Dispose()
    {
    }

    public MonitorModel GetStatus()
    {
        var model = Monitor.CreateModel();
        try
        {
            //Syscall.getpid
            //Debug.WriteLine(model.CpuUsage.ToString("F2") + "," +
            //    model.MemoryUsage.ToString("F2") + "," +
            //    (model.SpeedReceived / 1024).ToString("F2") + "," +
            //    (model.SpeedSent / 1024).ToString("F2") + "," +
            //    (model.DiskRead / 1024).ToString("F2") + "," +
            //    (model.DiskWrite / 1024).ToString("F2"));
            Console.WriteLine("linux test:" + getpid());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return model;
    }
}

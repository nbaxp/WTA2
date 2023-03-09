using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using CZGL.SystemInfo;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

public class MonitorService2 : IMonitorService, IDisposable
{
    private readonly bool _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    private readonly Stopwatch _stopWatch;
    private readonly System.Timers.Timer _timer;
    private double? _cpuTime;
    private double _cpuUsage;
    private double? _processCpuLoad;
    private double? _processCpuTime;
    private Rate? _rate;
    private SizeInfo _speedReceived;
    private SizeInfo _speedSent;
    protected static PerformanceCounter CPUCounter;
    public MonitorService2()
    {
        this._stopWatch = new Stopwatch();
        this._timer = new System.Timers.Timer(TimeSpan.FromSeconds(1));
        this._timer.Elapsed += Callback;
        this._stopWatch.Start();
        this._timer.Start();
    }

    public void Dispose()
    {
        this._timer?.Stop();
        this._timer?.Dispose();
    }

    public MonitorModel GetStatus()
    {
        var memory = _isWindows ? MemoryHelper.GetMemoryValue() : LinuxMemory.GetMemory();
        using var process = Process.GetCurrentProcess();
        var model = new MonitorModel
        {
            ServerTime = DateTimeOffset.UtcNow,
            UserName = SystemPlatformInfo.UserName,
            OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
            OSDescription = RuntimeInformation.OSDescription,
            ProcessCount = Process.GetProcesses().Length,
            ThreadCount = Process.GetProcesses().Sum(o => o.Threads.Count),
            ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            ProcessorCount = Environment.ProcessorCount,
            CpuUsage = this._cpuUsage,
            TotalPhysicalMemory = memory.TotalPhysicalMemory,
            MemoryUsage = memory.UsedPhysicalMemory * 1.0 / memory.TotalPhysicalMemory,
            SpeedSent = this._speedSent.Size,
            SpeedReceived = this._speedReceived.Size,
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            ProcessName = process.ProcessName,
            ProcessCpuLoad = _processCpuLoad,
            TotalSeconds = process.TotalProcessorTime.TotalSeconds,
            ProcessTotalMemory = process.PrivateMemorySize64
        };
        return model;
    }

    private void Callback(object? sender, ElapsedEventArgs e)
    {
        using var process = Process.GetCurrentProcess();
        //if (this._cpuTime == null)
        //{
        //    this._cpuTime = process.TotalProcessorTime.TotalMicroseconds;
        //}
        //else
        //{
        //    var cpuTime2 = process.TotalProcessorTime.TotalMicroseconds;
        //    this._cpuUsage = (cpuTime2 - _cpuTime.Value) / elapsed.TotalMicroseconds;
        //    this._cpuTime = cpuTime2;
        //}
        if (this._rate == null)
        {
            this._rate = NetworkInfo.TryGetRealNetworkInfo()!.GetIpv4Speed();
        }
        else
        {
            var rate2 = NetworkInfo.TryGetRealNetworkInfo()!.GetIpv4Speed();
            var speed = NetworkInfo.GetSpeed(this._rate.Value, rate2);
            this._speedSent = speed.Sent;
            this._speedReceived = speed.Received;
        }
        _stopWatch.Stop();
        var elapsed = _stopWatch.Elapsed;
        if (this._processCpuTime == null)
        {
            this._processCpuTime = process.UserProcessorTime.TotalMicroseconds;
        }
        else
        {
            var processCpuTime2 = process.UserProcessorTime.TotalMicroseconds;
            this._processCpuLoad = (processCpuTime2 - _processCpuTime) / elapsed.TotalMicroseconds * Environment.ProcessorCount;
            this._processCpuTime = processCpuTime2;
        }
        this._stopWatch.Start();
    }

    private CPUTime GetCpuTime()
    {
        return _isWindows ? CPUHelper.GetCPUTime() : LinuxCPU.GetCPUTime();
    }
}

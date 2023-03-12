using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;
using WTA.Application.Services.Monitor;

namespace WTA.Infrastructure.Monitor;

/// <summary>
/// 单例
/// </summary>
[Service<IMonitorService>(ServiceLifetime.Singleton, PlatformType.Linux)]
[SupportedOSPlatform("linux")]
public class LinuxService : IMonitorService
{
    private readonly CancellationTokenSource _cts;
    private readonly Stopwatch _stopWatch;
    private double _cpuUsage;
    private LinuxStatusModel? _prevStatus;

    public LinuxService()
    {
        this._cts = new CancellationTokenSource();
        this._stopWatch = new Stopwatch();
        Task.Run(() =>
        {
            while (!this._cts.IsCancellationRequested)
            {
                if (!this._stopWatch.IsRunning)
                {
                    this._stopWatch.Start();
                    this._prevStatus = this.DoWorkInternal();
                }
                else
                {
                    this._stopWatch.Stop();
                    this.DoWork();
                    this._stopWatch.Start();
                }
                Task.Delay(1000);
            }
        }, this._cts.Token);
    }

    public void Dispose()
    {
        this._cts.Cancel();
    }

    public MonitorModel GetStatus()
    {
        var model = Monitor.CreateModel();
        try
        {
            model.CpuUsage = this._cpuUsage;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return model;
    }

    private static double GetCpuUsage()
    {
        var stats = File.ReadAllLines("/proc/stat");
        var values = stats.First()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(o => int.Parse(o, CultureInfo.InvariantCulture)).ToArray();
        var cpuUsage = 1 - 1.0 * values[3] / values.Sum(o => o);
        return cpuUsage;
    }

    //curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/vsdb
    [DllImport("libc")]
    private static extern int getpid();

    private void DoWork()
    {
        if (_prevStatus != null)
        {
            var elapsed = this._stopWatch.Elapsed;
            var status = this.DoWorkInternal();
            this._cpuUsage = 1 - 1.0 * (status.CpuIdle - this._prevStatus.CpuIdle) / (status.CpuTotal - this._prevStatus.CpuTotal);
        }
    }

    private LinuxStatusModel DoWorkInternal()
    {
        var stats = File.ReadAllLines($"/proc/stat");
        var values = stats.First().Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(o => int.Parse(o, CultureInfo.InvariantCulture))
            .ToArray();
        return new LinuxStatusModel
        {
            CpuTotal = values.Sum(),
            CpuIdle = values[3]
        };
    }
}

public class LinuxStatusModel
{
    public int CpuTotal { get; set; }
    public int CpuIdle { get; set; }
}

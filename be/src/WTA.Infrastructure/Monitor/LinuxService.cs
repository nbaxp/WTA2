using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;
using WTA.Application.Extensions;
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
    private MonitorModel _model;
    private LinuxStatusModel? _prevStatus;

    public LinuxService()
    {
        this._model = MonitorHelper.CreateModel();
        this._cts = new CancellationTokenSource();
        this._stopWatch = new Stopwatch();
        Task.Run(async () =>
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
                    this._stopWatch.Reset();
                    this._stopWatch.Start();
                }
                await Task.Delay(1000 * 1).ConfigureAwait(false);
            }
        }, this._cts.Token);
    }

    public void Dispose()
    {
        this._cts.Cancel();
    }

    public MonitorModel GetStatus()
    {
        return this._model;
    }

    [DllImport("libc")]
    public static extern ulong TimevalToJiffies(uint input);

    private void DoWork()
    {
        if (_prevStatus != null)
        {
            Debug.WriteLine("timer:" + this._stopWatch.ElapsedMilliseconds);
            var microseconds = this._stopWatch.Elapsed.TotalMicroseconds;
            this._model = MonitorHelper.CreateModel();
            var status = this.DoWorkInternal();
            this._model.CpuUsage = 1 - 1.0 * (status.CpuIdle - this._prevStatus.CpuIdle) / (status.CpuTotal - this._prevStatus.CpuTotal);
            this._model.ProcessCpuUsage = 1.0 * (status.ProcessCpuUsed - this._prevStatus.ProcessCpuUsed) / (status.CpuTotal - this._prevStatus.CpuTotal);
            this._model.TotalMemory = status.TotalMemory;
            this._model.MemoryUsage = 1 - 1.0 * status.MemoryFree / status.TotalMemory;
            this._model.NetReceived = (float)(1.0f * (status.NetReceived - this._prevStatus.NetReceived) / (microseconds / 1_000_000));
            this._model.NetSent = (float)(1.0f * (status.NetSent - this._prevStatus.NetSent) / (microseconds / 1_000_000));
        }
    }

    protected virtual LinuxStatusModel DoWorkInternal()
    {
        var status = new LinuxStatusModel();
        // cpu
        var procStat = File.ReadAllLines("/proc/stat");
        var statValues = procStat.First().Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(o => int.Parse(o, CultureInfo.InvariantCulture))
            .ToArray();
        status.CpuTotal = statValues.Sum();
        status.CpuIdle = statValues[3];
        // process cpu
        var processProcStat = File.ReadAllLines($"/proc/{MonitorHelper.CurrentProcess.Id}/stat");
        var processStatValues = processProcStat.First().Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Skip(13)
            .Take(9)
            .Select(o => int.Parse(o, CultureInfo.InvariantCulture))
            .ToArray();
        status.ProcessCpuTotal = processStatValues.Last();
        status.ProcessCpuUsed = processStatValues.Take(2).Sum();
        // memory
        var procMeminfo = File.ReadAllLines("/proc/meminfo");
        var memoryValues = procMeminfo
            .Take(2)
            .Select(o => Regex.Match(o, @"(\S+):\s+(\d+)").Groups.Values.Skip(1).Select(o => o.Value.Trim()).ToList())
            .Select(o => new KeyValuePair<string, long>(o.First(), long.Parse(o.Last(), CultureInfo.InvariantCulture)))
            .ToDictionary(o => o.Key, o => o.Value);
        status.TotalMemory = memoryValues["MemTotal"] * 1024;
        status.MemoryFree = memoryValues["MemFree"] * 1024;
        // network
        var procNetDev = File.ReadAllLines("/proc/net/dev");
        var netValues = procNetDev.Skip(2)
            .Select(o => Regex.Match(o, @"\s*(\S+):\s*(\d+)\s*\d+\s*\d+\s*\d+\s*\d+\s*\d+\s*\d+\s*\d+\s*(\d+)").Groups)
            .Where(o => o.Count == 4 && o[2].Value != "lo")
            .Select(o => o.Values.Skip(2).Select(o => o.Value.Trim()).ToList())
            .Select(o => new { Receive = long.Parse(o.First(), CultureInfo.InvariantCulture), Transmit = long.Parse(o.Last(), CultureInfo.InvariantCulture) })
            .ToList();
        var inBytes = netValues.Sum(o => o.Receive);
        var outBytes = netValues.Sum(o => o.Transmit);
        status.NetReceived = inBytes;
        status.NetSent = outBytes;
        //
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = "bash",
                    Arguments = "-c ps -eo nlwp | tail -n +2 | awk '{ num_threads += $1 } END { print num_threads }'",
                },
                EnableRaisingEvents = true
            };
            process.OutputDataReceived += (s, e) =>
            {
                Debug.WriteLine(e.Data);
            };
            process.ErrorDataReceived += (s, e) =>
            {
                Debug.WriteLine(e.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
        }
        //
        return status;
    }
}

public class LinuxStatusModel
{
    public int CpuIdle { get; set; }
    public int CpuTotal { get; set; }
    public long MemoryFree { get; set; }
    public long TotalMemory { get; set; }
    public long NetReceived { get; set; }
    public long NetSent { get; set; }
    public int ProcessCpuTotal { get; internal set; }
    public int ProcessCpuUsed { get; internal set; }
}

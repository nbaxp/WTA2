using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.CPU;

namespace WTA.Infrastructure.Monitor;

public class SystemMonitor
{
    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware)
            {
                subHardware.Accept(this);
            }
        }

        public void VisitSensor(ISensor sensor)
        {
        }

        public void VisitParameter(IParameter parameter)
        {
        }
    }

    //private static readonly IHardwareInfo hardwareInfo = new HardwareInfo();
    private readonly Computer _computer;

    public SystemMonitor()
    {
        this._computer = new Computer
        {
            IsCpuEnabled = true,
            //IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsNetworkEnabled = true,
            IsStorageEnabled = true
        };
        _computer.Open();
        _computer.Accept(new UpdateVisitor());
        this.CpuCount = _computer.Hardware.Count(o => o.HardwareType == HardwareType.Cpu);
        //_computer.Hardware.
        _computer.Close();
    }

    public long CpuCount { get; }
    //public ulong MemoryTotalPhysical => hardwareInfo.MemoryStatus.TotalPhysical;
    //public ulong MemoryAvailablePhysical => hardwareInfo.MemoryStatus.AvailablePhysical;
}

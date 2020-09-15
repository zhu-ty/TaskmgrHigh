using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using OpenHardwareMonitor.Hardware;
using System.Collections;

namespace TaskmgrHigh
{
    class GPUTemp
    {
        public List<GPUStates> Update()
        {
            GPUlist.Clear();
            CheckGPU();
            
            return GPUlist;
        }
        public class GPUStates
        {
            public string type;
            public string speed;
            public string temp;

        }
        List<GPUStates> GPUlist = new List<GPUStates>();
        //ArrayList GPUlist = new ArrayList() { };
        private void GPUinfo(string name, string Type, string tempvalue, string fanvalue)
        {
            GPUStates GP = new GPUStates();
            if (Type == "GpuAti" || Type == "GpuNvidia")
            {
                GP.type = name;
                GP.temp = tempvalue + " °C";
                GP.speed = fanvalue + " RPM";
            }
            GPUlist.Add(GP);
        }
        private OpenHardwareMonitor.Hardware.Computer computerHardware = new OpenHardwareMonitor.Hardware.Computer();
        //private OpenHardwareMonitor.Hardware.Computer computerHardware = null;
        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }
        private void CheckGPU()
        {
            //computerHardware = new OpenHardwareMonitor.Hardware.Computer();
            string name = string.Empty;
            string gpuType = string.Empty;
            string sensortype = string.Empty;
            string tempvalue = string.Empty;
            string fanvalue = string.Empty;
            int x, y;
            int hardwareCount;
            int sensorcount;
            computerHardware.FanControllerEnabled = true;
            computerHardware.GPUEnabled = true;
            computerHardware.Open();
            UpdateVisitor updateVisitor = new UpdateVisitor();
            computerHardware.Accept(updateVisitor);
            hardwareCount = computerHardware.Hardware.Count();
            for (x = 0; x < hardwareCount; x++)
            {
                name = computerHardware.Hardware[x].Name;
                gpuType = computerHardware.Hardware[x].HardwareType.ToString();//判断是A卡还是N卡条件              
                sensorcount = computerHardware.Hardware[x].Sensors.Count();
                if (sensorcount > 0)
                {
                    for (y = 0; y < sensorcount; y++)
                    {
                        if (computerHardware.Hardware[x].Sensors[y].SensorType.ToString() == "Temperature")
                        {
                            tempvalue = computerHardware.Hardware[x].Sensors[y].Value.ToString();
                        }
                        if (computerHardware.Hardware[x].Sensors[y].SensorType.ToString() == "Fan")
                        {
                            fanvalue = computerHardware.Hardware[x].Sensors[y].Value.ToString();
                        }
                    }

                }
                GPUinfo(name, gpuType, tempvalue, fanvalue);
            }
        }
    }
}

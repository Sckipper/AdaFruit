
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Medic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            StartBluetoothSearch();
            InitializeComponent();
        }

        private void StartBluetoothSearch()
        {
            // New watcher
            var watcher = new DnaBluetoothLEAdvertisementWatcher(new GattServiceIds());

            // Hook into events
            watcher.StartedListening += () =>
            {
                Debug.WriteLine("Started listening");
            };

            watcher.StoppedListening += () =>
            {
                Debug.WriteLine("Stopped listening");
            };

            watcher.NewDeviceDiscovered += (device) =>
            {
                listBox1.Invoke(new Action(() => listBox1.Items.Add(device.Name + " (" + device.SignalStrengthInDB + ")")));
            };

            watcher.DeviceNameChanged += (device) =>
            {
                Debug.WriteLine($"Device name changed: {device}");
            };

            watcher.DeviceTimeout += (device) =>
            {
                Debug.WriteLine($"Device timeout: {device}");
            };

            watcher.StartListening();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            

            this.Enabled = true;
        }
    }
}

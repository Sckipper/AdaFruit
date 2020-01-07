
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Medic
{
    public partial class MainWindow : Form
    {
        private DnaBluetoothLEAdvertisementWatcher watcher;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            StartBluetoothSearch();
        }

        private void StartBluetoothSearch()
        {
            // New watcher
            watcher = new DnaBluetoothLEAdvertisementWatcher(new GattServiceIds());

            // Hook into events
            watcher.StartedListening += () =>
            {
                log("Bluetooth started listening");
            };

            watcher.StoppedListening += () =>
            {
                log("Bluetooth stoped listening");
            };

            watcher.NewDeviceDiscovered += (device) =>
            {
                listBoxDevices.Invoke(new Action(() => 
                    listBoxDevices.Items.Add(device)
                ));
            };

            watcher.DeviceNameChanged += (device) =>
            {
                log("Device name changed: " + device);
                listBoxDevices.Invoke(new Action(() =>
                {
                    for (int i=0; i< listBoxDevices.Items.Count; i++)
                    {
                        if (((DnaBluetoothLEDevice)listBoxDevices.Items[i]).DeviceId == device.DeviceId)
                        {
                            listBoxDevices.Items.RemoveAt(i); //remove the element
                            listBoxDevices.Items.Insert(i, device);
                        }
                    }
                }));
            };

            watcher.DeviceTimeout += (device) =>
            {
                log("Device timeout: " + device);
            };

            watcher.StartListening();
        }

        private void buttonPair_Click(object sender, EventArgs e)
        {
            DnaBluetoothLEDevice device = (DnaBluetoothLEDevice)listBoxDevices.SelectedItem;
            Task.Run(async () =>
            {
                try
                {
                    // Try and connect
                    log("Pairing with " + device);
                    await watcher.PairToDeviceAsync(device.DeviceId);
                }
                catch (Exception ex)
                {
                    // Log it out
                    log("Failed to pair with " + device);
                    Console.WriteLine("Failed to pair to Contour device.");
                    Console.WriteLine(ex);
                }
            });
        }

        private void listBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonPair.Enabled = true;
        }

        public void log(String message)
        {
            labelConsole.Invoke((MethodInvoker)delegate {
                labelConsole.Text = message;
            });
        }
    }
}

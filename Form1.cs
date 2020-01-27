
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

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
                listViewDevices.Invoke(new Action(() =>
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = device.Name + "   (" + device.SignalStrengthInDB + "dB)"; // Or whatever display text you need
                    item.Tag = device;

                    for (int i = 0; i < listViewDevices.Items.Count; i++)
                    {
                        if (((DnaBluetoothLEDevice)listViewDevices.Items[i].Tag).DeviceId == device.DeviceId)
                        {
                            listViewDevices.Items.RemoveAt(i);
                            listViewDevices.Items.Insert(i, item);
                            return;
                        }
                    }

                    if (device.Paired)
                        listViewDevices.Items.Add(item).BackColor = Color.GreenYellow;
                    else listViewDevices.Items.Add(item);
                }));
            };

            watcher.DeviceNameChanged += (device) =>
            {
                log("Device name changed: " + device);
                listViewDevices.Invoke(new Action(() =>
                {
                    for (int i=0; i< listViewDevices.Items.Count; i++)
                    {
                        if ( ((DnaBluetoothLEDevice)listViewDevices.Items[i].Tag).DeviceId == device.DeviceId)
                        {
                            listViewDevices.Items.RemoveAt(i); //remove the element
                            ListViewItem item = new ListViewItem();
                            item.Text = device.Name + "   (" + device.SignalStrengthInDB + "dB)"; // Or whatever display text you need
                            item.Tag = device;
                            listViewDevices.Items.Insert(i, item);
                            return;
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

        private void listViewDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewDevices.SelectedItems.Count > 0 && listViewDevices.SelectedItems[0].BackColor == Color.GreenYellow)
            {
                buttonPair.Enabled = false;
                buttonUnpair.Enabled = true;
            }
            else
            {
                buttonPair.Enabled = true;
                buttonUnpair.Enabled = false;
            }
        }

        private void buttonPair_Click(object sender, EventArgs e)
        {
            if (listViewDevices.SelectedItems.Count == 0)
                return;

            DnaBluetoothLEDevice device = (DnaBluetoothLEDevice)listViewDevices.SelectedItems[0].Tag;
            DevicePairingResultStatus result = DevicePairingResultStatus.Failed;
            Task.Run(async () =>
            {
                try
                {
                    // Try and connect
                    log("Pairing with " + device);
                    result = await watcher.PairToDeviceAsync(device.DeviceId);
                    if (result == DevicePairingResultStatus.Paired)
                    {
                        log("Pairing successful with " + device);
                        listViewDevices.Invoke(new Action(() =>
                        {
                            listViewDevices.SelectedItems[0].BackColor = Color.GreenYellow;
                            listViewDevices.SelectedIndices.Clear();
                        }));

                        var dev = await BluetoothLEDevice.FromIdAsync(device.DeviceId).AsTask();
                        logCaracteristics(dev);
                    }
                    else
                        log("Pairing failed: " + result);

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

        private async void buttonUnpair_Click(object sender, EventArgs e)
        {
            if (listViewDevices.SelectedItems.Count == 0)
                return;

            DnaBluetoothLEDevice device = (DnaBluetoothLEDevice)listViewDevices.SelectedItems[0].Tag;
            log("Unpairing " + device);
            var dev = await BluetoothLEDevice.FromIdAsync(device.DeviceId).AsTask();
            var result = await dev.DeviceInformation.Pairing.UnpairAsync();
            if (result == null || result.Status == DeviceUnpairingResultStatus.Unpaired)
                log("Unpairing successful with" + device);
            else
                log("Unpairing unsuccessful with" + device);

            listViewDevices.Invoke(new Action(() =>
            {
                listViewDevices.SelectedItems[0].BackColor = default(Color);
                listViewDevices.SelectedIndices.Clear();
            }));
        }

        public void log(String message)
        {
            labelConsole.Invoke((MethodInvoker)delegate {
                labelConsole.Text = message;
            });
        }

        protected async void logCaracteristics(BluetoothLEDevice device)
        {
            var services = await device.GetGattServicesAsync();

            foreach (var service in services.Services)
            {
                if (service.Uuid.ToString().Equals("00001809-0000-1000-8000-00805f9b34fb"))
                {
                    var characteristics = await service.GetCharacteristicsAsync();
                    foreach (var curCharacteristic in characteristics.Characteristics)
                    {
                        if (curCharacteristic.Uuid.ToString().Equals("00002a1c-0000-1000-8000-00805f9b34fb"))
                        {
                            //if (curCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                            //{
                                var result = await curCharacteristic.ReadValueAsync();
                                var reader = DataReader.FromBuffer(result.Value);
                                var input = new byte[reader.UnconsumedBufferLength];
                                reader.ReadBytes(input);
                                Console.WriteLine(BitConverter.ToString(input));
                            //}
                        }
                            
                    }
                }
                //Console.WriteLine($"Service: {service.Uuid}");
                //var characteristics = await service.GetCharacteristicsAsync();
                //foreach (var curCharacteristic in characteristics.Characteristics)
                //{
                //    Console.WriteLine($"Characteristic: {curCharacteristic.Uuid}");
                //}
            }
        }

    }
}


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
        private DnaBluetoothLEDevice greenBLE = null;
        private DnaBluetoothLEDevice purpleBLE = null;

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
                    item.Text = device.Name + "   (" + device.SignalStrengthInDB + "dB)";
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
                    {
                        listViewDevices.Items.Add(item).BackColor = Color.GreenYellow;
                        findDeviceTypeAsync(device);
                    }
                    else listViewDevices.Items.Add(item);
                }));
            };

            watcher.DeviceNameChanged += (device) =>
            {
                log("Device name changed: " + device);
                listViewDevices.Invoke(new Action(() =>
                {
                    for (int i = 0; i < listViewDevices.Items.Count; i++)
                    {
                        if (((DnaBluetoothLEDevice)listViewDevices.Items[i].Tag).DeviceId == device.DeviceId)
                        {
                            listViewDevices.Items.RemoveAt(i);
                            ListViewItem item = new ListViewItem();
                            item.Text = device.Name + "   (" + device.SignalStrengthInDB + "dB)";
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

                        findDeviceTypeAsync(device);
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
            labelConsole.Invoke((MethodInvoker)delegate
            {
                labelConsole.Text = message;
            });
        }

        private async void findDeviceTypeAsync(DnaBluetoothLEDevice device)
        {
            var dev = await BluetoothLEDevice.FromIdAsync(device.DeviceId).AsTask();
            try
            {
                var services = await dev.GetGattServicesAsync();

                foreach (var service in services.Services)
                {
                    if (service.Uuid.ToString().Equals(GattService.FloraAccelerometerService))
                    {
                        var characteristics = await service.GetCharacteristicsAsync();
                        foreach (var curCharacteristic in characteristics.Characteristics)
                        {
                            if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraAccelerometerCharacteristicX))
                                device.accelerometerX = curCharacteristic;
                            else if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraAccelerometerCharacteristicY))
                                device.accelerometerY = curCharacteristic;
                            else if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraAccelerometerCharacteristicZ))
                                device.accelerometerZ = curCharacteristic;
                        }
                    }
                    else if (service.Uuid.ToString().Equals(GattService.FloraMagnetometerService))
                    {
                        var characteristics = await service.GetCharacteristicsAsync();
                        foreach (var curCharacteristic in characteristics.Characteristics)
                        {
                            if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraMagnetometerCharacteristicX))
                                device.magnetometerX = curCharacteristic;
                            else if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraMagnetometerCharacteristicY))
                                device.magnetometerY = curCharacteristic;
                            else if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraMagnetometerCharacteristicZ))
                                device.magnetometerZ = curCharacteristic;
                        }
                    }
                    else if (service.Uuid.ToString().Equals(GattService.FloraGyroscopeService))
                    {
                        var characteristics = await service.GetCharacteristicsAsync();
                        foreach (var curCharacteristic in characteristics.Characteristics)
                        {
                            if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraGyroscopeCharacteristicX))
                            {
                                if (curCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                                {
                                    var result = await curCharacteristic.ReadValueAsync();
                                    var reader = DataReader.FromBuffer(result.Value);
                                    byte[] input = new byte[reader.UnconsumedBufferLength];
                                    reader.ReadBytes(input);
                                    float gyroX = BitConverter.ToSingle(input, 0);
                                    Console.WriteLine(gyroX);
                                    if (gyroX != 0) //isGreen
                                    {
                                        buttonGreen.BackColor = Color.DarkGreen;
                                        buttonGreen.Text = device.Name;
                                    }
                                    else //isPurple
                                    {
                                        buttonPurple.BackColor = Color.MediumPurple;
                                        buttonPurple.Text = device.Name;
                                    }
                                }
                                device.gyroscopeX = curCharacteristic;
                            }
                            else if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraGyroscopeCharacteristicY))
                                device.gyroscopeY = curCharacteristic;
                            else if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraGyroscopeCharacteristicZ))
                                device.gyroscopeZ = curCharacteristic;
                        }
                    }
                }

                if(buttonGreen.BackColor == Color.DarkGreen && greenBLE == null)
                    greenBLE = device;
                else if (buttonPurple.BackColor == Color.MediumPurple && purpleBLE == null)
                    purpleBLE = device;

                if (greenBLE != null && purpleBLE != null)
                {
                    buttonStart.Enabled = true;
                    buttonStop.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                labelConsole.Text = "Error reading Services and Characteristics";
            }
        }

        public async void GatherGreenData()
        {
            try
            {
                if (greenBLE != null)
                {
                    
                }
            }
            catch (Exception ex)
            {
                // log errors
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            bool exit = false;
            Color errorColor = Color.DarkRed;
            if (String.IsNullOrWhiteSpace(textBoxName.Text))
            {
                textBoxName.BackColor = errorColor;
                exit = true;
            }
            else if (String.IsNullOrWhiteSpace(textBoxSurname.Text))
            {
                textBoxSurname.BackColor = errorColor;
                exit = true;
            }else if (String.IsNullOrWhiteSpace(textBoxAge.Text))
            {
                textBoxAge.BackColor = errorColor;
                exit = true;
            }
            else if (String.IsNullOrWhiteSpace(textBoxHeight.Text))
            {
                textBoxHeight.BackColor = errorColor;
                exit = true;
            }
            else if (String.IsNullOrWhiteSpace(textBoxWeight.Text))
            {
                textBoxWeight.BackColor = errorColor;
                exit = true;
            }

            if (exit)
                return;

            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {

        }


    }
}

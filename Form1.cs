
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading;

namespace Medic
{
    public partial class MainWindow : Form
    {
        private DnaBluetoothLEAdvertisementWatcher watcher;
        private DnaBluetoothLEDevice greenBLE = null;
        private DnaBluetoothLEDevice purpleBLE = null;
        private Thread readGreenDataThread = null;
        private Thread readPurpleDataThread;
        private Color greenColor = Color.GreenYellow;
        private Color purpleColor = Color.Violet;

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
                        listViewDevices.Items.Add(item).Selected = true;
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
            if (listViewDevices.SelectedItems.Count > 0 && listViewDevices.SelectedItems[0].BackColor != SystemColors.Window)
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
                if (listViewDevices.SelectedItems[0].BackColor == greenColor)
                {
                    buttonGreen.BackColor = default(Color);
                    buttonGreen.Text = String.Empty;
                    greenBLE = null;
                }
                else if (listViewDevices.SelectedItems[0].BackColor == purpleColor)
                {
                    buttonPurple.BackColor = default(Color);
                    buttonPurple.Text = String.Empty;
                    purpleBLE = null;
                }

                listViewDevices.SelectedItems[0].BackColor = SystemColors.Window;
                listViewDevices.SelectedIndices.Clear();
            }));
        }

        public void log(String message, Color color = default(Color))
        {
            labelConsole.Invoke((MethodInvoker)delegate
            {
                labelConsole.ForeColor = color;
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
                    if (service.Uuid.ToString().Equals(GattService.FloraSensorService))
                    {
                        var characteristics = await service.GetCharacteristicsAsync();
                        foreach (var curCharacteristic in characteristics.Characteristics)
                        {
                            if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraAccelerometerCharacteristic))
                                device.accelerometer = curCharacteristic;
                            else if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraMagnetometerCharacteristic))
                                device.magnetometer = curCharacteristic;
                            else if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraGyroscopeCharacteristic))
                                device.gyroscope = curCharacteristic;
                        }

                        if (characteristics.Characteristics.Count == 3 && device.accelerometer != null && device.magnetometer != null && device.gyroscope != null) //isGreen
                        {
                            buttonGreen.Invoke(new Action(() =>
                            {
                                buttonGreen.BackColor = greenColor;
                                buttonGreen.Text = device.Name;
                            }));
                            listViewDevices.Invoke(new Action(() =>
                            {
                                if (listViewDevices.SelectedItems.Count > 0)
                                {
                                    listViewDevices.SelectedItems[0].BackColor = greenColor;
                                    listViewDevices.SelectedIndices.Clear();
                                }
                            }));

                        }
                        else if (characteristics.Characteristics.Count == 2 && device.accelerometer != null && device.magnetometer != null)//isPurple
                        {
                            buttonPurple.Invoke(new Action(() =>
                            {
                                buttonPurple.BackColor = purpleColor;
                                buttonPurple.Text = device.Name;
                            }));
                            listViewDevices.Invoke(new Action(() =>
                            {
                                if (listViewDevices.SelectedItems.Count > 0)
                                {
                                    listViewDevices.SelectedItems[0].BackColor = purpleColor;
                                    listViewDevices.SelectedIndices.Clear();
                                }
                            }));
                        }
                    }
                }

                if (buttonGreen.BackColor == greenColor && greenBLE == null)
                    greenBLE = device;
                else if (buttonPurple.BackColor == purpleColor && purpleBLE == null)
                    purpleBLE = device;

                if (greenBLE != null)//&& purpleBLE != null)
                {
                    buttonStart.Invoke(new Action(() =>
                    {
                        buttonStart.Enabled = true;
                    }));
                    buttonStop.Invoke(new Action(() =>
                    {
                        buttonStop.Enabled = false;
                    }));
                }

                listViewDevices.Invoke(new Action(() =>
                {
                    if (listViewDevices.SelectedItems.Count > 0 && listViewDevices.SelectedItems[0].BackColor == SystemColors.Window)
                    {
                        listViewDevices.SelectedItems[0].BackColor = Color.LightSkyBlue;
                        listViewDevices.SelectedIndices.Clear();
                    }
                }));
            }
            catch (Exception ex)
            {
                log(ex.Message, Color.Red);
            }
        }

        public async void readGreenData(object dev)
        {
            var device = (DnaBluetoothLEDevice)dev;
            try
            {
                if (device != null)
                {
                    SensorValue accel = new SensorValue();
                    SensorValue gyro = new SensorValue();
                    SensorValue magneto = new SensorValue();
                    GattReadResult result;
                    byte[] input = new byte[12];

                    while (true)
                    {
                        //Accelerometer
                        if (device.accelerometer != null && device.accelerometer.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                        {
                            result = await greenBLE.accelerometer.ReadValueAsync(BluetoothCacheMode.Uncached);
                            if (result.Status == GattCommunicationStatus.Success)
                            {
                                DataReader.FromBuffer(result.Value).ReadBytes(input);
                                //Console.WriteLine(BitConverter.ToString(input));
                                accel.x = BitConverter.ToSingle(input, 0);
                                accel.y = BitConverter.ToSingle(input, 4);
                                accel.z = BitConverter.ToSingle(input, 8);
                            }
                        }

                        ////Magnetometer
                        if (device.magnetometer != null && device.magnetometer.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                        {
                            result = await greenBLE.magnetometer.ReadValueAsync(BluetoothCacheMode.Uncached);
                            if (result.Status == GattCommunicationStatus.Success)
                            {
                                DataReader.FromBuffer(result.Value).ReadBytes(input);
                                //Console.WriteLine(BitConverter.ToString(input));
                                magneto.x = BitConverter.ToSingle(input, 0);
                                magneto.y = BitConverter.ToSingle(input, 4);
                                magneto.z = BitConverter.ToSingle(input, 8);
                            }
                        }

                        ////Gyroscope
                        if (device.gyroscope != null && device.gyroscope.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                        {
                            result = await greenBLE.gyroscope.ReadValueAsync(BluetoothCacheMode.Uncached);
                            if (result.Status == GattCommunicationStatus.Success)
                            {
                                DataReader.FromBuffer(result.Value).ReadBytes(input);
                                //Console.WriteLine(BitConverter.ToString(input));
                                gyro.x = BitConverter.ToSingle(input, 0);
                                gyro.y = BitConverter.ToSingle(input, 4);
                                gyro.z = BitConverter.ToSingle(input, 8);
                            }
                        }

                        Console.WriteLine("Accel " + accel.toString());
                        Console.WriteLine("Magneto " + magneto.toString());
                        Console.WriteLine("Gyro " + gyro.toString());
                    }
                }
            }
            catch (Exception ex)
            {
                log(ex.Message, Color.Red);
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            bool exit = false;
            Color errorColor = Color.OrangeRed;
            if (String.IsNullOrWhiteSpace(textBoxName.Text))
            {
                textBoxName.BackColor = errorColor;
                exit = true;
            }
            if (String.IsNullOrWhiteSpace(textBoxSurname.Text))
            {
                textBoxSurname.BackColor = errorColor;
                exit = true;
            }
            if (String.IsNullOrWhiteSpace(textBoxAge.Text))
            {
                textBoxAge.BackColor = errorColor;
                exit = true;
            }
            if (String.IsNullOrWhiteSpace(textBoxHeight.Text))
            {
                textBoxHeight.BackColor = errorColor;
                exit = true;
            }
            if (String.IsNullOrWhiteSpace(textBoxWeight.Text))
            {
                textBoxWeight.BackColor = errorColor;
                exit = true;
            }

            //if (exit)
            //    return;

            buttonStart.Enabled = false;
            buttonStop.Enabled = true;

            readGreenDataThread = new Thread(new ParameterizedThreadStart(readGreenData));
            readGreenDataThread.Start(purpleBLE);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (readGreenDataThread != null)
            {
                readGreenDataThread.Abort();
                readGreenDataThread = null;
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
            }

        }
    }
}

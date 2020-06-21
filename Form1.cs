
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
        private Thread readPurpleDataThread = null;
        private Color greenColor = Color.GreenYellow;
        private Color purpleColor = Color.Violet;
        private Color errorColor = Color.OrangeRed;
        private double MaxPitch = 0;

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
                        listViewDevices.Items.Add(item);
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
                        log("Pairing failed: " + result, Color.Red);

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

            if (buttonStop.Enabled == true && ((greenBLE != null && dev.DeviceId == greenBLE.DeviceId) || (purpleBLE != null && dev.DeviceId == purpleBLE.DeviceId)))
                buttonStop.PerformClick();

            var result = await dev.DeviceInformation.Pairing.UnpairAsync();
            if (result == null || result.Status == DeviceUnpairingResultStatus.Unpaired)
                log("Unpairing successful with" + device);
            else
                log("Unpairing unsuccessful with" + device, Color.Red);

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
                            if (curCharacteristic.Uuid.ToString().Equals(GattService.FloraAllrCharacteristics))
                            {
                                device.caracteristic = curCharacteristic;
                                var result = await curCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
                                if (result.Status == GattCommunicationStatus.Success)
                                {
                                    byte[] input = new byte[18];
                                    DataReader.FromBuffer(result.Value).ReadBytes(input);
                                    Console.WriteLine(BitConverter.ToString(input));
                                    if (BitConverter.ToInt16(input, 0) != 0 && BitConverter.ToInt16(input, 12) != 0 && greenBLE == null) //isGreen
                                    {
                                        buttonGreen.Invoke(new Action(() =>
                                        {
                                            buttonGreen.BackColor = greenColor;
                                            buttonGreen.Text = device.Name;
                                        }));
                                        listViewDevices.Invoke(new Action(() =>
                                        {
                                            for (int i = 0; i < listViewDevices.Items.Count; i++)
                                            {
                                                if (((DnaBluetoothLEDevice)listViewDevices.Items[i].Tag).DeviceId == device.DeviceId)
                                                {
                                                    listViewDevices.Items[i].BackColor = greenColor;
                                                    listViewDevices.SelectedIndices.Clear();
                                                }
                                            }
                                        }));

                                        greenBLE = device;
                                    }
                                    else if (BitConverter.ToInt16(input, 12) == 0 && BitConverter.ToInt16(input, 0) != 0 && purpleBLE == null) //isPurple
                                    {
                                        buttonPurple.Invoke(new Action(() =>
                                        {
                                            buttonPurple.BackColor = purpleColor;
                                            buttonPurple.Text = device.Name;
                                        }));
                                        listViewDevices.Invoke(new Action(() =>
                                        {
                                            for (int i = 0; i < listViewDevices.Items.Count; i++)
                                            {
                                                if (((DnaBluetoothLEDevice)listViewDevices.Items[i].Tag).DeviceId == device.DeviceId)
                                                {
                                                    listViewDevices.Items[i].BackColor = purpleColor;
                                                    listViewDevices.SelectedIndices.Clear();
                                                }
                                            }
                                        }));

                                        purpleBLE = device;
                                    }
                                }
                            }
                        }
                    }
                }

                if (greenBLE != null && purpleBLE != null)
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
            }
            catch (Exception ex)
            {
                log(ex.Message, Color.Red);
            }
            finally
            {
                listViewDevices.Invoke(new Action(() =>
                {
                    for (int i = 0; i < listViewDevices.Items.Count; i++)
                    {
                        if (((DnaBluetoothLEDevice)listViewDevices.Items[i].Tag).DeviceId == device.DeviceId && listViewDevices.Items[i].BackColor == SystemColors.Window)
                        {
                            listViewDevices.Items[i].BackColor = Color.Cyan;
                            listViewDevices.SelectedIndices.Clear();
                        }
                    }
                }));
            }
        }

        public async void readData(object dev)
        {
            MaxPitch = 0;
            var device = (DnaBluetoothLEDevice)dev;
            try
            {
                if (device != null)
                {
                    SensorValue accel = new SensorValue();
                    SensorValue gyro = new SensorValue();
                    SensorValue magneto = new SensorValue();
                    GattReadResult result;
                    byte[] input = new byte[18];

                    //Calculus
                    double Pitch_f;

                    while (buttonStop.Enabled == true)
                    {
                        if (device.caracteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                        {
                            result = await device.caracteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
                            if (result.Status == GattCommunicationStatus.Success)
                            {
                                DataReader.FromBuffer(result.Value).ReadBytes(input);
                                //Console.WriteLine(BitConverter.ToString(input));
                                accel.x = (float)BitConverter.ToInt16(input, 0) / (float)1000;
                                accel.y = (float)BitConverter.ToInt16(input, 2) / (float)1000;
                                accel.z = (float)BitConverter.ToInt16(input, 4) / (float)1000;
                                magneto.x = (float)BitConverter.ToInt16(input, 6) / (float)1000;
                                magneto.y = (float)BitConverter.ToInt16(input, 8) / (float)100 / (float)10000;
                                magneto.z = (float)BitConverter.ToInt16(input, 10) / (float)1000;
                                gyro.x = (float)BitConverter.ToInt16(input, 12) / (float)1000;
                                gyro.y = (float)BitConverter.ToInt16(input, 14) / (float)1000;
                                gyro.z = (float)BitConverter.ToInt16(input, 16) / (float)1000;
                            }
                        }

                        Console.WriteLine("Accel " + accel.toString());
                        //Console.WriteLine("Magneto " + magneto.toString());
                        //Console.WriteLine("Gyro " + gyro.toString());

                        var AxN = (accel.x + 10) / 20;
                        var AyN = (accel.y + 10) / 20;
                        var AzN = (accel.z + 10) / 20;
                        var Pitch = Math.Asin(-AxN);
                        var Pitch_grad = Pitch * 180 / Math.PI;
                        if (accel.x < 0)
                            Pitch_f = Math.Abs(Pitch_grad);
                        else
                            Pitch_f = Math.Abs(Pitch_grad) + 100;

                        var Roll = Math.Asin(AyN / Math.Cos(Pitch));
                        var Roll_grad = Roll * 180 / Math.PI;
                        if (MaxPitch < Pitch_f)
                            MaxPitch = Pitch_f;

                        Console.WriteLine("Pitch: " + Pitch_f + "    Roll: " + Roll_grad);
                        Console.WriteLine("");
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
            if (String.IsNullOrWhiteSpace(buttonPurple.Text) || String.IsNullOrWhiteSpace(buttonGreen.Text))
            {
                buttonStart.Enabled = false;
                buttonStop.Enabled = false;
                return;
            }

            bool exit = false;

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

            if (exit)
                return;

            buttonStart.Enabled = false;
            buttonStop.Enabled = true;

            if (purpleBLE != null)
            {
                ThreadStart start = () => {
                    readData(purpleBLE);
                };

                readPurpleDataThread = new Thread(start);
                readPurpleDataThread.Start();
            }
            
            if (greenBLE != null)
            {
                ThreadStart start = () => {
                    readData(greenBLE);
                };

                readGreenDataThread = new Thread(start);
                readGreenDataThread.Start();
            }

            listViewDevices.Invoke(new Action(() =>
            {
                labelResultValue.Text = "";
            }));
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(buttonPurple.Text) || String.IsNullOrWhiteSpace(buttonGreen.Text))
            {
                buttonStart.Enabled = false;
                buttonStop.Enabled = false;
            }else
            {
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
            }

            if (readGreenDataThread != null)
                readGreenDataThread = null;
            
            if (readPurpleDataThread != null)
                readPurpleDataThread = null;

            if (String.IsNullOrWhiteSpace(buttonPurple.Text) || String.IsNullOrWhiteSpace(buttonGreen.Text))
            {
                buttonStart.Enabled = false;
                buttonStop.Enabled = false;
            }

            FlexieExtensieBrat_END();
        }

        private void FlexieExtensieBrat_END()
        {
            listViewDevices.Invoke(new Action(() =>
            {
                if (MaxPitch < 100)
                {
                    labelResultValue.Text = "Exercitiul nu a fost realizata corect";
                    labelResultValue.ForeColor = Color.Red;
                }
                else if (MaxPitch > 100 && MaxPitch < 160)
                {
                    labelResultValue.Text = "Exercitiul trebuie imbunatatit";
                    labelResultValue.ForeColor = Color.Blue;
                }
                else if (MaxPitch > 160)
                {
                    labelResultValue.Text = "Exercitiul a fost realizat corect";
                    labelResultValue.ForeColor = Color.Green;
                }
            }));
        }
    }
}

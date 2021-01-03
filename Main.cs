using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kymacros
{
    public partial class Main : Form
    {

        private IconButton selectedButton;
        private List<Device> devices;
        private int keybCount = 0;

        public Main()
        {
            InitializeComponent();
            EditTopBar();
            ActivateButton(btnHome);
            GetDevicesList();
        }

        private void GetDevicesList()
        {
            devices = new List<Device>();
            uint deviceCount = 0;
            int dwSize = (Marshal.SizeOf(typeof(RawInputDeviceList)));
            if (RawInputHandler.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) == 0)
            {
                IntPtr rawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
                RawInputHandler.GetRawInputDeviceList(rawInputDeviceList, ref deviceCount, (uint)dwSize);
                
                for (int i = 0; i < deviceCount; i++)
                {
                    Device device = new Device();

                    RawInputDeviceList rid = (RawInputDeviceList)Marshal.PtrToStructure(new IntPtr(rawInputDeviceList.ToInt64() + (dwSize * i)), typeof(RawInputDeviceList));

                    uint pcbSize = 0;
                    RawInputHandler.GetRawInputDeviceInfo(rid.hDevice, (uint)RawInputDeviceInfo.RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);
                    IntPtr pData = Marshal.AllocHGlobal((int)pcbSize);

                    string deviceName;
                    RawInputHandler.GetRawInputDeviceInfo(rid.hDevice, (uint)RawInputDeviceInfo.RIDI_DEVICENAME, pData, ref pcbSize);
                    deviceName = (string)Marshal.PtrToStringAnsi(pData);

                    deviceName = deviceName.Substring(4);
                    device.Name = deviceName;

                    if(rid.dwType == DeviceType.RimTypekeyboard || rid.dwType == DeviceType.RimTypeHid)
                    {
                        device.Description = GetDeviceDescription(deviceName);
                        device.IntType = rid.dwType;
                        device.Handle = rid.hDevice;
                        devices.Add(device);
                        Console.WriteLine(device.ToString());
                        keybCount++;
                    }

                    Marshal.FreeHGlobal(pData);
                }
                lblKeybCount.Text = string.Format("{0}", keybCount);

                Marshal.FreeHGlobal(rawInputDeviceList);
            }
            else
            {
                MessageBox.Show("YEP COCK");
            }
        }

        public static string GetDeviceDescription(string device)
        {
            string deviceDesc = null;
            try
            {
                var deviceKey = RegistryAccess.GetDeviceKey(device);
                if(deviceKey != null)
                {
                    deviceDesc = deviceKey.GetValue("DeviceDesc").ToString();
                    deviceDesc = deviceDesc.Substring(deviceDesc.IndexOf(';') + 1);
                }
            }
            catch (Exception)
            {
                deviceDesc = "Device is malformed unable to look up in the registry";
            }
            return deviceDesc;
        }

        private void RegisterDevice()
        {
            RawInputDevice[] rid = new RawInputDevice[1];
            rid[0].UsagePage = HidUsagePage.GENERIC;
            rid[0].Usage = HidUsage.Keyboard;
            rid[0].Flags = RawInputDeviceFlags.INPUTSINK;
            //rid[0].Target = IntPtr.Zero;
            rid[0].Target = this.Handle;

            if (RawInputHandler.RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])))
            {
                MessageBox.Show("Registered");
            }
            else
            {
                MessageBox.Show("cum");
            }
        }

        private void EditTopBar()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            mainTabControl.Appearance = TabAppearance.FlatButtons;
            mainTabControl.ItemSize = new Size(0, 1);
            mainTabControl.SizeMode = TabSizeMode.Fixed;
        }

        private void ActivateButton(object senderBtn)
        {
            if (senderBtn != null)
            {
                DeactivateButton(selectedButton);
                selectedButton = (IconButton)senderBtn;
                selectedButton.BackColor = Color.FromArgb(67, 76, 94);
            }
        }

        private void DeactivateButton(object senderBtn)
        {
            if (senderBtn != null)
            {
                selectedButton = (IconButton)senderBtn;
                selectedButton.BackColor = Color.FromArgb(59, 66, 82);
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            mainTabControl.SelectedIndex = 0;
        }

        private void btnProfiles_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            mainTabControl.SelectedIndex = 1;
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            mainTabControl.SelectedIndex = 2;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            mainTabControl.SelectedIndex = 3;
        }

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                WindowState = FormWindowState.Maximized;
            else
                WindowState = FormWindowState.Normal;
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (Int64.Parse(lblIndex.Text) == keybCount)
                lblIndex.Text = "1";
            else
                lblIndex.Text = (Int64.Parse(lblIndex.Text) + 1).ToString();
            var selectedDevice = devices.ElementAt(Int32.Parse(lblIndex.Text)-1);
            lblKeybName.Text = selectedDevice.Handle.ToString();
            txtDescription.Text = selectedDevice.Description;
            txtType.Text = selectedDevice.getTypeString();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (Int64.Parse(lblIndex.Text) == 1)
                lblIndex.Text = keybCount.ToString();
            else
                lblIndex.Text = (Int64.Parse(lblIndex.Text) - 1).ToString();
            var selectedDevice = devices.ElementAt(Int32.Parse(lblIndex.Text)-1);
            lblKeybName.Text = selectedDevice.Handle.ToString();
            txtDescription.Text = selectedDevice.Description;
            txtType.Text = selectedDevice.getTypeString();
        }
    }
}

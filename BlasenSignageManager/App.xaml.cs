using OpenSource.UPnP;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Net.Sockets;
using System.Net;
using System.Windows;

namespace BlasenSignageManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var icon = GetResourceStream(new Uri("Icon.ico", UriKind.Relative)).Stream;
            var menu = new ContextMenuStrip();
            menu.Items.Add("終了", null, OnExitClicked);
            var notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = new Icon(icon),
                Text = "ブラセン デジタルサイネージ",
                ContextMenuStrip = menu,
            };

            notifyIcon.MouseClick += new MouseEventHandler(OnNotifyIconClicked);

            TestDlna();
        }



        private void OnNotifyIconClicked(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

            }
        }


        private void OnExitClicked(object? sender, EventArgs e)
        {
            Shutdown(0);
        }



        private IPAddress? GetIPAddress()
        {
            var hostname = Dns.GetHostName();

            var addresses = Dns.GetHostAddresses(hostname);
            foreach (var address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address;
                }
            }
            return null;
        }


        private void TestDlna()
        {
            var ipaddress = GetIPAddress();
            var uriBuilder = new UriBuilder("http", ipaddress?.ToString(), 8080);
            var url = uriBuilder.ToString();


            var localDevice = UPnPDevice.CreateRootDevice(3600, 1, null);
            localDevice.StandardDeviceType = "urn:e-frontier:device:controllee";
            localDevice.UniqueDeviceName = "DegitalSignage-59927499-D892-4CE8-8C7C-7805563F4703";
            localDevice.FriendlyName = "Degital Signage";
            localDevice.Major = 1;
            localDevice.Minor = 0;
            localDevice.SerialNumber = "1234567890";
            localDevice.ModelNumber = "3.1234";
            localDevice.ModelURL = uriBuilder.Uri;

            localDevice.Manufacturer = "e-frontier systems.";
            localDevice.ManufacturerURL = "https://e-frontier.systems/";
            localDevice.ModelName = "Degital Signage";
            localDevice.ModelDescription = "Blasen Degital Signage 1.0";
            localDevice.UserAgentTag = "efrontier";
            localDevice.PresentationURL = url;

            dynamic instance = new ExpandoObject();

            var service = new UPnPService(
                // Version
                1.0,
                // Service ID
                "urn:Belkin:serviceId:basicevent1",
                // Service Type
                "urn:Belkin:service:basicevent:1",
                // Standard Service?
                true,
                // Service Object Instance
                instance
            );
            service.ControlURL = "/upnp/control/basicevent1";
            service.EventURL = "/upnp/event/basicevent1";
            service.SCPDURL = "/eventservice.xml";

            string stateVarName = "BinaryState";
            var stateVariable = new UPnPStateVariable(stateVarName, typeof(bool), true);
            stateVariable.AddAssociation("GetBinaryState", stateVarName);
            stateVariable.AddAssociation("SetBinaryState", stateVarName);
            stateVariable.Value = false;
            service.AddStateVariable(stateVariable);

            instance.GetBinaryState = new Func<bool>(() => (bool)service.GetStateVariable(stateVarName));
            instance.SetBinaryState = new Action<int>((BinaryState) => {
                Console.WriteLine("SetBinaryState({0})", BinaryState);
                service.SetStateVariable(stateVarName, BinaryState != 0);
            });

            // Add the methods
            service.AddMethod("GetBinaryState", stateVarName);
            service.AddMethod("SetBinaryState", stateVarName);

            // Add the service
            localDevice.AddService(service);
            // Start the WeMo switch device UPnP simulator
            localDevice.StartDevice();
        }
    }
}

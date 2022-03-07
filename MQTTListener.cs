using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MonitorSwitcher
{
    internal class MQTTListener
    {
        public MqttClient client = null;
        public string MQTT_BROKER_ADDRESS = "192.168.1.104";
        bool lastValue = false;

        public void StartListening()
        {
            client = new MqttClient(IPAddress.Parse(MQTT_BROKER_ADDRESS));

            // register to message received
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            string clientId = Guid.NewGuid().ToString();
            var res = client.Connect(clientId);
            var c = client.IsConnected;

            // subscribe to the topic "/home/temperature" with QoS 2
            client.Subscribe(new string[] { "pc/monitor" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        public void StopListening()
        {            
            client.Disconnect();
        }

        private void Client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            if (e.Message[0] == (byte)'7')
            {
                
                // ProcessStartInfo psi = new ProcessStartInfo(fileName, arg + " 15");
                //var t = Process.Start(psi);
                lastValue = true;
                SwitchTo();

            }
            else if (e.Message[0] == (byte)'6')
            {
                lastValue = false;
                SwitchTo();
                //PInvoke.ChangeInputSelect(true);
                ////ProcessStartInfo psi = new ProcessStartInfo(fileName, arg + " 17");
                ////var t = Process.Start(psi);
                //client.Publish("sensor2/ir_special", new byte[] { (byte)'6' });
                //Thread.Sleep(100);
                //client.Publish("sensor2/ir_special", new byte[] { (byte)'7' });
            }
        }

        public void SwitchTo()
        {
            PInvoke.ChangeInputSelect(lastValue);
            client.Publish("sensor2/ir_special", new byte[] { (byte)'6' });
            Thread.Sleep(500);
            client.Publish("sensor2/ir_special", new byte[] { (byte)'7' });
            lastValue = !lastValue;
        }
    }
}

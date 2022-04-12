using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json;

public class Evironment_Data 
{
    public string temperature { get; set; }
    public string humidity { get; set; }
}

public class ControlDevice_Data
{
    public string device { get; set; }
    public string status { get; set; }
}

public class M2MqttUnityTest : M2MqttUnityClient
{
    public InputField addressInputField;
    public InputField userName;
    public InputField password;
    public Slider Layer;

    public List<string> topics = new List<string>();
    private List<string> eventMessages = new List<string>();

    public Text Temperature;
    public Text Humidity;

    public Slider setLed;
    public Slider setPump;
    private ControlDevice_Data _controlLed_data;
    private ControlDevice_Data _controlPump_data;

    public void SetBrokerAddress(string s)
    {
        if (addressInputField)
        {
            this.brokerAddress = addressInputField.text;
            this.timeoutOnConnection = 5;
        }
    }

    public void SetUsername(string s)
    {
        if (userName)
        {
            this.mqttUserName = userName.text;
        }
    }

    public void SetPassword(string s)
    {
        if (password)
        {
            this.mqttPassword = password.text;
        }
    }

    public void SetEncrypted(bool isEncrypted)
    {
        this.isEncrypted = isEncrypted;
    }

    public void AddUiMessage(string msg)
    {
    }

    protected override void OnConnecting()
    {
        base.OnConnecting();
    }

    protected override void OnConnected()
    {
        base.OnConnected();
        Layer.value = 1;
        SubscribeTopics();
    }

    public void SetLed()
    {
        _controlLed_data = new ControlDevice_Data();
        _controlLed_data.device = "LED";
        if (setLed.value == 0)
            _controlLed_data.status = "OFF";
        else
            _controlLed_data.status = "ON";
        string msg_config = JsonConvert.SerializeObject(_controlLed_data);
        client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        Debug.Log("publish led");
    }

    public void SetPump()
    {
        _controlPump_data = new ControlDevice_Data();
        _controlPump_data.device = "PUMP";
        if (setPump.value == 0)
            _controlPump_data.status = "OFF";
        else
            _controlPump_data.status = "ON";
        string msg_config = JsonConvert.SerializeObject(_controlPump_data);
        client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        Debug.Log("publish pump");
    }

    protected override void SubscribeTopics()
    {
        if (topics[0]!="")
        {
            client.Subscribe(new string[] { topics[0] }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }
    }

    protected override void UnsubscribeTopics()
    {
        client.Unsubscribe(new string[] { topics[0] });
    }

    protected override void OnConnectionFailed(string errorMessage)
    {
        Layer.value = 0.5f;
        AddUiMessage("CONNECTION FAILED! " + errorMessage);
    }

    protected override void OnDisconnected()
    {
        AddUiMessage("Disconnected.");
    }

    protected override void OnConnectionLost()
    {
        AddUiMessage("CONNECTION LOST!");
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void DecodeMessage(string topic, byte[] message)
    {
        string msg = System.Text.Encoding.UTF8.GetString(message);
        Debug.Log("Received: " + msg);
        StoreMessage(msg);
        if (topic == topics[0])
        {
            Evironment_Data stuff = JsonConvert.DeserializeObject<Evironment_Data>(msg);

            Temperature.text = stuff.temperature;
            Humidity.text = stuff.humidity;
        }
    }

    private void StoreMessage(string eventMsg)
    {
        eventMessages.Add(eventMsg);
    }

    private void ProcessMessage(string msg)
    {
        AddUiMessage("Received: " + msg);
    }

    protected override void Update()
    {
        base.Update();

        if (eventMessages.Count > 0)
        {
            foreach (string msg in eventMessages)
            {
                ProcessMessage(msg);
            }
            eventMessages.Clear();
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    private void OnValidate()
    {
        autoConnect = false;
    }
}

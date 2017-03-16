using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

// Check it is hololens and proceed with the operations!
#if !UNITY_EDITOR && UNITY_METRO
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

public class RemoteLeap : MonoBehaviour {

    // Leap Motion Server settings
    bool background = true;
    bool optimizeHMD = true;
    bool focused = true;
    //string connection = "ws://localhost:6437/v6.json";
    //string connection = "ws://192.168.0.10:6437/v6.json"; // Stationær
    string connection = "ws://192.168.0.15:6437/v6.json"; // Bærbar
    //string connection = "ws://127.0.0.1:6437/v6.json";
    //string connection = "ws://192.168.137.1:6437/v6.json";
    public GameObject text;
    public Text QueueCounter;
    private Text textcomp;
    private string debug = "Start debugging";

    private Queue<RootObject> _leapData = new Queue<RootObject>();
    private object _leapDataLock = new object();


    public Slider slider;
    public GameObject cube;

    // Use this for initialization
    void Start () {

        textcomp = text.GetComponent<Text>();

#if !UNITY_EDITOR && UNITY_METRO
        
        debug = "UWP TEST!";
        initConnection();

#endif

    }

#if !UNITY_EDITOR && UNITY_METRO
    private async Task initConnection(){
        
        debug = "Initiating Connection!";

        MessageWebSocket webSock = new MessageWebSocket();

        //In this case we will be sending/receiving a string so we need to set the MessageType to Utf8.
        webSock.Control.MessageType = SocketMessageType.Utf8;

        //Add the MessageReceived event handler.
        webSock.MessageReceived += WebSock_MessageReceived;

        //Add the Closed event handler.
        webSock.Closed += WebSock_Closed;

        //Uri serverUri = new Uri("wss://echo.websocket.org");
        Uri serverUri = new Uri(connection);
    
        try{

            debug = "Trying to send!";
            //Connect to the server.
            await webSock.ConnectAsync(serverUri);

  
            //Send a message to the server.
            await WebSock_SendMessage(webSock, "version");
            //await WebSock_SendMessage(webSock, "{enableGestures: true}");
            //await WebSock_SendMessage(webSock, "Hello world i am coming back!");
        } catch (Exception ex){
            debug = ex.Message;
            //Add code here to handle any exceptions
            Debug.Log(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

    }

    private async Task WebSock_SendMessage(MessageWebSocket webSock, string message){

        debug = "Message: " + message; 
        DataWriter messageWriter = new DataWriter(webSock.OutputStream);
        messageWriter.WriteString(message);
        await messageWriter.StoreAsync();

    }

    private void WebSock_Closed(IWebSocket sender, WebSocketClosedEventArgs args){
        throw new NotImplementedException();
    }


    private async void WebSock_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args){

        DataReader messageReader = args.GetDataReader();
        messageReader.UnicodeEncoding = UnicodeEncoding.Utf8;
        string messageString = messageReader.ReadString(messageReader.UnconsumedBufferLength);

        //JsonObject jsonObject = JsonObject.Parse(messageString);

        //Debug.WriteLine(jsonObject.ToString());
        //Debug.WriteLine(messageString);

        //messageString = jsonObject.GetNamedValue("version").ToString();
        //JToken leapmessage = JObject.Parse(messageString);

        //var test = leapmessage.Value<string>("version") ?? "100";
        //Debug.WriteLine(test);
         
        var check = JsonUtility.FromJson<RootObject>(messageString);

        
        System.Diagnostics.Debug.WriteLine(check);

        debug = "# of Hands: " + check.hands.Count + "\n";
        debug += "¤ of pointables: " + check.pointables.Count + "\n";
        debug += " Position of tip with type = 1: ";

        try
        {            
            foreach(Pointable e in check.pointables)
            {
                if(e.type == 1)
                {
                    var tip = e.tipPosition;
                    debug += " X = " + tip[0] + " Y = " + tip[1] + " Z = " + tip[2];
                    break;
                }
            }
        }catch(Exception e)
        {
            Debug.Log(e.Message);
        }

        if(check.hands.Count > 0){
            lock( _leapDataLock)
            {
                _leapData.Enqueue(check);
            }
        }

    }

#endif


    // Update is called once per frame
    void FixedUpdate () {

        // Update the hololens text!
        textcomp.text = debug;

        // Dequeue
        RootObject obj = null;

        lock (_leapDataLock)
        {
            if (_leapData.Count > 0) obj = _leapData.Dequeue();
            else obj = null;
            QueueCounter.text = "Queue Msgs: " + _leapData.Count;            
        }

        if(obj != null)
        {
            foreach (Pointable e in obj.pointables)
            {
                if (e.type == 1)
                {
                    var tip = e.tipPosition;
                    //cube.transform.position = new Vector3((float)tip[0], (float)tip[1], (float)tip[2]);
                    //cube.transform.Rotate(0, (float)(tip[1] / 10), 0);

                    cube.transform.rotation = Quaternion.AngleAxis((float)tip[0], Vector3.up);    

                }
            }

        }


    }

    [Serializable] 
    public class Hand
    {
        public List<List<double>> armBasis;
        public double armWidth;
        public double confidence;
        public List<double> direction;
        public List<double> elbow;
        public double grabAngle;
        public double grabStrength;
        public int id;
        public List<double> palmNormal;
        public List<double> palmPosition;
        public List<double> palmVelocity;
        public double palmWidth;
        public double pinchDistance;
        public double pinchStrength;
        public List<List<double>> r;
        public double s;
        public List<double> sphereCenter;
        public double sphereRadius;
        public List<double> stabilizedPalmPosition;
        public List<double> t;
        public double timeVisible;
        public string type;
        public List<double> wrist;
    }

    [Serializable]
    public class InteractionBox
    {
        public List<double> center;
        public List<double> size;
    }

    [Serializable]
    public class Pointable
    {
        public List<List<List<double>>> bases;
        public List<double> btipPosition;
        public List<double> carpPosition;
        public List<double> dipPosition;
        public List<double> direction;
        public bool extended;
        public int handId;
        public int id;
        public double length;
        public List<double> mcpPosition;
        public List<double> pipPosition;
        public List<double> stabilizedTipPosition;
        public double timeVisible;
        public List<double> tipPosition;
        public List<double> tipVelocity;
        public bool tool;
        public double touchDistance;
        public string touchZone;
        public int type;
        public double width;
    }

    [Serializable]
    public class RootObject
    {
        public double currentFrameRate;
        public List<object> devices;
        public List<object> gestures;
        public List<Hand> hands;
        public int id;
        public InteractionBox interactionBox;
        public List<Pointable> pointables;
        public List<List<double>> r;
        public double s;
        public List<double> t;
        public long timestamp;
    }

}

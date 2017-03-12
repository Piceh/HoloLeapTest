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
    string connection = "ws://192.168.0.10:6437/v6.json";
    //string connection = "ws://192.168.137.1:6437/v6.json";
    public GameObject text;
    private Text textcomp;
    private string debug = "Start debugging";

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
         
         var check = JsonUtility.FromJson<LeapData>(messageString);

         System.Diagnostics.Debug.WriteLine(check);

        //         debug = messageString;
        debug = "r: " + check.r + " s: " + check.s + " hands: " + check.hands;

    }
#endif


    // Update is called once per frame
    void FixedUpdate () {

        // Update the hololens text!
        textcomp.text = debug;

	}

    [Serializable]
    public class LeapData{

        public float currentFrameRate;
        public float id;
        public float r;
        public float s;

        public List<gesture> gestures;
        public List<hand> hands;
        public interactionBox interactionBox;
        public List<pointable> pointables;

    }

    [Serializable]
    public class gesture{

        public Vector3 center; // Circle
        public Vector3 direction;
        public int duration; // Microsconds
        public List<int> handIds;
        public int id;
        public List<float> normal;
        public List<int> pointtableIds;
        public Vector3 position;
        public float progress;
        public float radius;
        public float speed;
        public Vector3 startPosition;
        public string state;
        public string type;

    }

    [Serializable]
    public class hand{

        public List<Vector3> arms;
        public float armWidth;
        public float confidence;
        public Vector3 direction;
        public Vector3 elbow;
        public float grabStrength;
        public int id;
        public Vector3 palmNormal;
        public Vector3 palmPosition;
        public Vector3 palmVelocity;
        public float pinchStrength;
        public Matrix4x4 r;
        public float s;
        public Vector3 sphereCenter;
        public float sphereRadius;
        public Vector3 stabilizedPalmPosition;
        public Vector3 t;
        public float timeVisible;
        public string type;
        public Vector3 wrist;
    }

    public class interactionBox{
        public Vector3 center;
        public Vector3 size;
    }

    public class pointable{
        public List<Vector3> bases;
        public Vector3 btipPosition;
        public Vector3 carpPosition;
        public Vector3 dipPosition;
        public Vector3 direction;
        public int handId;
        public int id;
        public float length;
        public Vector3 mcPosition;
        public Vector3 pipPosition;
        public Vector3 stabilizedTipPosition;
        public float timeVisble;
        public Vector3 tipPosition;
        public Vector3 tipVelocity;
        public bool tool;
        public float touchDistance;
        public string touchZone;
        public int type;
        public float width;

    }

}

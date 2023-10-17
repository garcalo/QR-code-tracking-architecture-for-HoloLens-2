// GAA
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.SampleQRCodes
{
    [RequireComponent(typeof(SpatialGraphNodeTracker))]
    public class QRCode : MonoBehaviour, IMixedRealityPointerHandler
    {
        public Microsoft.MixedReality.QR.QRCode qrCode;
        private GameObject qrCodeCube;

        public float PhysicalSize { get; private set; }
        public string CodeText { get; private set; }

        private TextMesh QRID;
        private TextMesh QRNodeID;
        private TextMesh QRText;
        private TextMesh QRVersion;
        private TextMesh QRTimeStamp;
        private TextMesh QRSize;
        private GameObject QRInfo;
        private bool validURI = false;
        private bool launch = false;
        private System.Uri uriResult;
        private long lastTimeStamp = 0;

        // Use this for initialization
        void Start()
        {
            // Set initial values
            PhysicalSize = 0.1f;
            CodeText = "Dummy";
            // Check if QR code is empty
            if (qrCode == null)
            {
                throw new System.Exception("QR Code Empty");
            }

            // Assign values from QR code
            PhysicalSize = qrCode.PhysicalSideLength;
            CodeText = qrCode.Data;

            // Find and assign references to text meshes and game objects || GAA: this is only useful if we use QRCode prefab
            qrCodeCube = gameObject.transform.Find("Cube").gameObject;
            QRInfo = gameObject.transform.Find("QRInfo").gameObject;
            QRID = QRInfo.transform.Find("QRID").gameObject.GetComponent<TextMesh>();
            QRNodeID = QRInfo.transform.Find("QRNodeID").gameObject.GetComponent<TextMesh>();
            QRText = QRInfo.transform.Find("QRText").gameObject.GetComponent<TextMesh>();
            QRVersion = QRInfo.transform.Find("QRVersion").gameObject.GetComponent<TextMesh>();
            QRTimeStamp = QRInfo.transform.Find("QRTimeStamp").gameObject.GetComponent<TextMesh>();
            QRSize = QRInfo.transform.Find("QRSize").gameObject.GetComponent<TextMesh>();

            // Update text mesh values
            QRID.text = "Id:" + qrCode.Id.ToString();
            QRNodeID.text = "NodeId:" + qrCode.SpatialGraphNodeId.ToString();
            QRText.text = CodeText;

            // Check if the code is a valid URI and update color accordingly
            if (System.Uri.TryCreate(CodeText, System.UriKind.Absolute, out uriResult))
            {
                validURI = true;
                QRText.color = Color.blue;
            }

            QRVersion.text = "Ver: " + qrCode.Version;
            QRSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";
            QRTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
            QRTimeStamp.color = Color.yellow;

            // Log QR code information
            Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " QRVersion = " + qrCode.Version + " QRData = " + CodeText);
        }

        void UpdatePropertiesDisplay()
        {
            // Update properties that change
            if (qrCode != null && lastTimeStamp != qrCode.SystemRelativeLastDetectedTime.Ticks)
            {
                QRSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";

                QRTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
                QRTimeStamp.color = QRTimeStamp.color == Color.yellow ? Color.white : Color.yellow;
                PhysicalSize = qrCode.PhysicalSideLength;

                // Log updated QR code information
                Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " Time = " + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff"));


                // Update cube position and scale
                qrCodeCube.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
                qrCodeCube.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, 0.005f);

                // Update last timestamp and scale of QR info game object
                lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;
                QRInfo.transform.localScale = new Vector3(PhysicalSize / 0.2f, PhysicalSize / 0.2f, PhysicalSize / 0.2f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdatePropertiesDisplay();
            if (launch)  // Check if launch flag is set and launch URI if valid
            {
                launch = false;
                LaunchUri();
            }
        }

        void LaunchUri()
        {
#if WINDOWS_UWP
            // Launch the URI
            UnityEngine.WSA.Launcher.LaunchUri(uriResult.ToString(), true);
#endif
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (validURI)  // Check if the URI is valid and set launch flag
            {
                launch = true;
            }
            // eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
}

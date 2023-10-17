// GAA

using Microsoft.MixedReality.QR;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.SampleQRCodes
{
    public static class QRCodeEventArgs  // Helper class to create QRCodeEventArgs instances
    {
        public static QRCodeEventArgs<TData> Create<TData>(TData data)
        {
            return new QRCodeEventArgs<TData>(data);
        }
    }

    [Serializable]
    public class QRCodeEventArgs<TData> : EventArgs // Event arguments for QR code events
    {
        public TData Data { get; private set; }

        public QRCodeEventArgs(TData data)
        {
            Data = data;
        }
    }

    public class QRCodesManager : Singleton<QRCodesManager>
    {
        [Tooltip("Determines if the QR codes scanner should be automatically started.")]
        public bool AutoStartQRTracking = true;

        //public string QRCodeIDToTrack ;

        public bool IsTrackerRunning { get; private set; }

        public bool IsSupported { get; private set; }

        // Events for QR code tracking
        public event EventHandler<bool> QRCodesTrackingStateChanged;
        public event EventHandler<QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode>> QRCodeAdded;
        public event EventHandler<QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode>> QRCodeUpdated;
        public event EventHandler<QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode>> QRCodeRemoved;

        private System.Collections.Generic.SortedDictionary<System.Guid, Microsoft.MixedReality.QR.QRCode> qrCodesList = new SortedDictionary<System.Guid, Microsoft.MixedReality.QR.QRCode>();

        private QRCodeWatcher qrTracker;
        private bool capabilityInitialized = false;
        private QRCodeWatcherAccessStatus accessStatus;
        private System.Threading.Tasks.Task<QRCodeWatcherAccessStatus> capabilityTask;


        public System.Guid GetIdForQRCode(string qrCodeData)
        {
            Debug.Log("linea 54" + qrCodeData);
            lock (qrCodesList)
            {
                foreach (var ite in qrCodesList)
                {
                    if (ite.Value.Data == qrCodeData)
                    {
                        return ite.Key;
                    }
                }
            }
            return new System.Guid();
        }

        public System.Collections.Generic.IList<Microsoft.MixedReality.QR.QRCode> GetList()
        {
            lock (qrCodesList)
            {
                return new List<Microsoft.MixedReality.QR.QRCode>(qrCodesList.Values);
            }
        }
        protected void Awake()
        {
            // No code in this method
        }

        // Use this for initialization
        async protected virtual void Start()
        {
            IsSupported = QRCodeWatcher.IsSupported();
            capabilityTask = QRCodeWatcher.RequestAccessAsync();
            accessStatus = await capabilityTask;
            capabilityInitialized = true;
        }

        private void SetupQRTracking()
        {
            try
            {
                // Create a new instance of QRCodeWatcher
                qrTracker = new QRCodeWatcher();
                IsTrackerRunning = false;

                // Subscribe to QR code watcher events
                qrTracker.Added += QRCodeWatcher_Added;
                qrTracker.Updated += QRCodeWatcher_Updated;
                qrTracker.Removed += QRCodeWatcher_Removed;
                qrTracker.EnumerationCompleted += QRCodeWatcher_EnumerationCompleted;
            }
            catch (Exception ex)
            {
                Debug.Log("QRCodesManager : exception starting the tracker (QRCodeWatcher) " + ex.ToString()); // Log an exception if there's an error starting the tracker
            }

            if (AutoStartQRTracking)
            {
                StartQRTracking(); // Start QR code tracking if AutoStartQRTracking is enabled | change this in the inspector of the script in Unity
            }
        }

        public void StartQRTracking()
        {
            // the objective of this commented part is to be able to identify the qr code in a future implementation
            //Debug.Log("entro en el loop de setup tracking para primero comprobar id");
            //Debug.Log("que hay dentro de qrTracker que es un QRCodeWatcher  " + qrTracker.Id);
            //Debug.Log("que sale de GetIdForQRCode  " + GetIdForQRCode(qrTracker.ToString()));
            //if (GetIdForQRCode(qrTracker.ToString())) ==QRCodeIDToTrack)
            //{
            //Debug.Log("he comprobado id y coincide");
            if (qrTracker != null && !IsTrackerRunning)
            {
                Debug.Log("QRCodesManager starting QRCodeWatcher");
                try
                {
                    // Start the QR code tracker
                    qrTracker.Start();
                    IsTrackerRunning = true;
                    QRCodesTrackingStateChanged?.Invoke(this, true);
                }
                catch (Exception ex)
                {
                    Debug.Log("QRCodesManager (StartQRTracking) starting QRCodeWatcher Exception:" + ex.ToString());
                }
            }
            //}
            //else
            //{
            //    Debug.Log("he comprobado id y no coincide");
            //}
        }

        public void StopQRTracking()
        {
            if (IsTrackerRunning)
            {
                IsTrackerRunning = false;
                if (qrTracker != null)
                {
                    // Stop the QR code tracker and clear the QR codes list
                    qrTracker.Stop();
                    qrCodesList.Clear();
                }

                var handlers = QRCodesTrackingStateChanged;
                if (handlers != null)
                {
                    // Invoke QR code tracking state changed event
                    handlers(this, false);
                }
            }
        }

        private void QRCodeWatcher_Removed(object sender, QRCodeRemovedEventArgs args)
        {
            Debug.Log("QRCodesManager QRCodeWatcher_Removed: initialized method to remove QR code from QR codes list");

            bool found = false;
            lock (qrCodesList)
            {
                if (qrCodesList.ContainsKey(args.Code.Id))
                {
                    // Remove the QR code from the QR codes list
                    qrCodesList.Remove(args.Code.Id);
                    found = true;
                }
            }
            if (found)
            {
                var handlers = QRCodeRemoved;
                if (handlers != null)
                {
                    // Invoke QR code removed event
                    handlers(this, QRCodeEventArgs.Create(args.Code));
                }
            }
        }

        private void QRCodeWatcher_Updated(object sender, QRCodeUpdatedEventArgs args)
        {
            Debug.Log("QRCodesManager QRCodeWatcher_Updated: initialized method to update QR codes list");

            bool found = false;
            lock (qrCodesList)
            {
                if (qrCodesList.ContainsKey(args.Code.Id))
                {
                    // Update the QR code in the QR codes list
                    found = true;
                    qrCodesList[args.Code.Id] = args.Code;
                }
            }
            if (found)
            {
                var handlers = QRCodeUpdated;
                if (handlers != null)
                {
                    // Invoke QR code updated event
                    handlers(this, QRCodeEventArgs.Create(args.Code));
                }
            }
        }


        private void QRCodeWatcher_Added(object sender, QRCodeAddedEventArgs args)
        {
            Debug.Log("QRCodesManager QRCodeWatcher_Added: initialized method to add QR code to QR codes list");

            lock (qrCodesList)
            {
                // Add the QR code to the QR codes list
                qrCodesList[args.Code.Id] = args.Code;
                Debug.Log("QR code added to QR codes list");
            }
            var handlers = QRCodeAdded;
            if (handlers != null)
            {
                // Invoke QR code added event
                handlers(this, QRCodeEventArgs.Create(args.Code));
            }
        }

        private void QRCodeWatcher_EnumerationCompleted(object sender, object e)
        {
            Debug.Log("QRCodesManager QrTracker_EnumerationCompleted");
        }

        private void Update()
        {

            if (qrTracker == null && capabilityInitialized && IsSupported)  // Check if the QR code tracker needs to be set up
            {
                if (accessStatus == QRCodeWatcherAccessStatus.Allowed)  // Set up QR code tracking if access is allowed
                {
                    Debug.Log("Setting up QR code tracking");
                    SetupQRTracking();
                }
                else
                {
                    // Log the capability access status if access is not allowed
                    Debug.Log("Capability access status : " + accessStatus);
                }
            }
        }
    }
}

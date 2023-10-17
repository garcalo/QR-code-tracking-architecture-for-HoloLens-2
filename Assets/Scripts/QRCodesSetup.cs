// GAA

using UnityEngine;

namespace Microsoft.MixedReality.SampleQRCodes
{
    /// <summary>
    /// Manages the setup and configuration of QR code scanning.
    /// </summary>
    public class QRCodesSetup : MonoBehaviour
    {
        [Tooltip("Determines if the QR codes scanner should be automatically started.")]
        public bool AutoStartQRTracking = true;

        [Tooltip("Visualize the detected QRCodes in the 3d space.")]
        public bool VisualizeQRCodes = true;

        QRCodesManager qrCodesManager = null;

        void Awake()
        {
            qrCodesManager = QRCodesManager.Instance;  // Get the instance of the QR code manager
            if (AutoStartQRTracking) // Start QR code tracking if AutoStartQRTracking is enabled
            {
                qrCodesManager.StartQRTracking();
            }
            if (VisualizeQRCodes) // Add QRCodesVisualizer component to visualize QR codes if VisualizeQRCodes is enabled
            {
                gameObject.AddComponent(typeof(QRCodesVisualizer));
            }
        }
    }
}

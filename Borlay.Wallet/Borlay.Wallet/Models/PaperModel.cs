using Borlay.Wallet.Storage;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Borlay.Wallet.Models
{
    public class PaperModel : ModelBase
    {
        private readonly SecureString walletPrivateKey;

        public PaperModel(SecureString walletPrivateKey)
        {
            this.walletPrivateKey = walletPrivateKey ?? throw new ArgumentNullException(nameof(walletPrivateKey));

            string qrText = walletPrivateKey.GetString();
            ShowQR(qrText);

            this.OpenConfigButton = new TextButtonModel((b) => Process.Start(ConfigurationStorage.GetDirectory()))
            {
                Content = "Open configs"
            };
            this.CopyWalletKeyButton = new TextButtonModel((b) => Clipboard.SetText(walletPrivateKey.GetString()))
            {
                Content = "Copy seed"
            };
        }

        public void ShowQR(string qrText)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(qrText, out qrCode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(4, QuietZoneModules.Four));

            Stream memoryStream = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);

            // very important to reset memory stream to a starting position, otherwise you would get 0 bytes returned
            memoryStream.Position = 0;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = memoryStream;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();
            QRImage = bi; //A WPF Image control
        }

        private object qrImage;
        public object QRImage
        {
            get => qrImage;
            set
            {
                this.qrImage = value;
                NotifyPropertyChanged();
            }
        }


        public ButtonModel OpenConfigButton { get; private set; }
        public ButtonModel CopyWalletKeyButton { get; private set; }
    }
}

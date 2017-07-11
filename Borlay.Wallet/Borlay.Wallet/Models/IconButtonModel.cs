using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public class IconButtonModel
    {
        public IconButtonModel(Func<IconButtonModel, Task> click, IconType iconType, ColorType colorType = ColorType.Gray)
            : this(iconType, colorType)
        {
            this.IconClick = new ActionCommandAsync(o => click(this));
        }

        public IconButtonModel(Action<IconButtonModel> click, IconType iconType, ColorType colorType = ColorType.Gray)
            : this(iconType, colorType)
        {
            this.IconClick = new ActionCommand(o => click(this));
        }

        public IconButtonModel(IconType iconType, ColorType colorType = ColorType.Gray)
        {
            IconSource = GetIconPath(iconType, colorType); // "../Resources/Icons/restart-40.png";
        }

        private string GetIconPath(IconType iconType, ColorType colorType)
        {
            var path = "../Resources/Icons/";
            var iconName = iconType.ToString().ToLower();
            if(colorType != ColorType.Gray)
            {
                var fix = colorType.ToString().Substring(0, 1).ToLower();
                iconName = $"{iconName}-{fix}";
            }
            iconName += "-40.png";
            var iconPath = Path.Combine(path, iconName);
            return iconPath;
        }

        public string IconSource { get; set; }

        public ICommand IconClick { get; set; }
    }

    public enum ColorType
    {
        Gray,
        Red,
        Green,
        Blue
    }


    public enum IconType
    {
        Plus,
        Minus,
        Restart,
        Checkmark,
        Settings,
    }
}

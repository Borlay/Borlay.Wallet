﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Borlay.Wallet.Models
{
    public class IconButtonModel : ButtonModel
    {
        public IconButtonModel(Func<ButtonModel, Task> click, IconType iconType, ColorType colorType = ColorType.Gray)
            : base(click)
        {
            Initialize(iconType, colorType);
        }

        public IconButtonModel(Action<ButtonModel> click, IconType iconType, ColorType colorType = ColorType.Gray)
            : base(click)
        {
            Initialize(iconType, colorType);
        }

        public IconButtonModel(IconType iconType, ColorType colorType = ColorType.Gray)
        {
            Initialize(iconType, colorType);
        }

        private void Initialize(IconType iconType, ColorType colorType)
        {
            Content = GetIconPath(iconType, colorType);
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
        Sent,
    }
}

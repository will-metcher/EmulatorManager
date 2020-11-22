using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmulatorManager {
    class FormUtils {

        public static Label CreateLabel(string text, int width, int height, int x, int y, Color fontColor) {
            Label label = new Label();
            label.Text = text;
            label.Size = new System.Drawing.Size(width, height);
            label.Location = new System.Drawing.Point(x,y);
            label.ForeColor = fontColor;
            return label;
        }
    }
}

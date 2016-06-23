using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GMScoreboard.UI
{
    public class TransparentPanel : Panel
    {
        public TransparentPanel() 
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.Opaque, true);
            this.BackColor = Color.Transparent;  
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

    }
}

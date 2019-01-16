using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bishop {
    public partial class SlopeShapeForm : Form {
        public SlopeShapeForm() {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                Setting.ShareClass.Ax = System.Convert.ToSingle(txtAy.Text);
                Setting.ShareClass.Ay = System.Convert.ToSingle(txtAy.Text);
                Setting.ShareClass.Bx = System.Convert.ToSingle(txtBx.Text);
                Setting.ShareClass.By = System.Convert.ToSingle(txtBy.Text);
                Setting.ShareClass.Cx = System.Convert.ToSingle(txtCx.Text);
                Setting.ShareClass.Cy = System.Convert.ToSingle(txtCy.Text);
                Setting.ShareClass.Dx = System.Convert.ToSingle(txtDx.Text);
                Setting.ShareClass.Dy = System.Convert.ToSingle(txtDy.Text);
                Setting.ShareClass.Ex = System.Convert.ToSingle(txtEx.Text);
                Setting.ShareClass.Ey = System.Convert.ToSingle(txtEy.Text);
                Setting.ShareClass.Fx = System.Convert.ToSingle(txtFx.Text);
                Setting.ShareClass.Fy = System.Convert.ToSingle(txtFy.Text);
                this.Close();
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void SlopeShapeForm_Load(object sender, EventArgs e) {
            txtAx.Text = Setting.ShareClass.Ax.ToString();
            txtAy.Text = Setting.ShareClass.Ay.ToString();
            txtBx.Text = Setting.ShareClass.Bx.ToString();
            txtBy.Text = Setting.ShareClass.By.ToString();
            txtCx.Text = Setting.ShareClass.Cx.ToString();
            txtCy.Text = Setting.ShareClass.Cy.ToString();
            txtDx.Text = Setting.ShareClass.Dx.ToString();
            txtDy.Text = Setting.ShareClass.Dy.ToString();
            txtEx.Text = Setting.ShareClass.Ex.ToString();
            txtEy.Text = Setting.ShareClass.Ey.ToString();
            txtFx.Text = Setting.ShareClass.Fx.ToString();
            txtFy.Text = Setting.ShareClass.Fy.ToString();
        }
    }
}

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
    public partial class SoilParameterForm : Form {
        public SoilParameterForm() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            try{
                Setting.ShareClass.EffectiveC = System.Convert.ToSingle(txtEffectiveC.Text);
                Setting.ShareClass.EffectivePhi = System.Convert.ToSingle(txtEffectivePhi.Text);
                Setting.ShareClass.Gamma = System.Convert.ToSingle(txtGamma.Text);
                this.Close();
            } 
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            } 
        }

        private void button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void SoilParameterForm_Load(object sender, EventArgs e) {
            txtEffectiveC.Text = Setting.ShareClass.EffectiveC.ToString();
            txtEffectivePhi.Text = Setting.ShareClass.EffectivePhi.ToString();
            txtGamma.Text = Setting.ShareClass.Gamma.ToString();
        }
    }
}

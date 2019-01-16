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
    public partial class SetSeedForm : Form {
        public SetSeedForm() {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            try{
                Setting.ShareClass.N = System.Convert.ToInt32(txtN.Text);
                Setting.ShareClass.CenterX = System.Convert.ToSingle(txtCenterX.Text);
                Setting.ShareClass.CenterY = System.Convert.ToSingle(txtCenterY.Text);
                Setting.ShareClass.CenterWidth = System.Convert.ToSingle(txtCenterWidth.Text);
                Setting.ShareClass.CenterHeight = System.Convert.ToSingle(txtCenterHeight.Text);
                Setting.ShareClass.SeedHeight = System.Convert.ToInt32(txtSeedHeight.Text);
                Setting.ShareClass.SeedWidth = System.Convert.ToInt32(txtSeedWidth.Text);
                Setting.ShareClass.RadiusDelta = System.Convert.ToSingle(txtRadiusDelta.Text);
                Setting.ShareClass.RadiusRange = System.Convert.ToSingle(txtRadiusRange.Text);
                this.Close();
            } 
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }  
        }

        private void SetSeedForm_Load(object sender, EventArgs e) {
            if (Setting.ShareClass.N.ToString() != "0") {
                txtN.Text = Setting.ShareClass.N.ToString();
            }
            if (Setting.ShareClass.CenterX.ToString() != "0"){ 
                txtCenterX.Text = Setting.ShareClass.CenterX.ToString();
            }
            if(Setting.ShareClass.CenterY.ToString() != "0"){
                txtCenterY.Text = Setting.ShareClass.CenterY.ToString();
            }
            if(Setting.ShareClass.CenterWidth.ToString() != "0"){ 
                txtCenterWidth.Text = Setting.ShareClass.CenterWidth.ToString();
            }
            if (Setting.ShareClass.CenterHeight.ToString() != "0") {
                txtCenterHeight.Text = Setting.ShareClass.CenterHeight.ToString();
            }
            if (Setting.ShareClass.SeedHeight.ToString() != "0"){
                txtSeedHeight.Text = Setting.ShareClass.SeedHeight.ToString();
            }
            if (Setting.ShareClass.SeedWidth.ToString() != "0") {
                 txtSeedWidth.Text = Setting.ShareClass.SeedWidth.ToString();
            }
            if (Setting.ShareClass.RadiusDelta.ToString() != "0") {
                txtRadiusDelta.Text = Setting.ShareClass.RadiusDelta.ToString();
            }
            if (Setting.ShareClass.RadiusRange.ToString() != "0") {
                txtRadiusRange.Text = Setting.ShareClass.RadiusRange.ToString() ;
            }
            
        }
    }
}

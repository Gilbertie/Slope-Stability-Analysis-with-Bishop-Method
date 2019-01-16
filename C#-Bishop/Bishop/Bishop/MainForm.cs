using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Bishop {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }
        // 全局变量
        int xPos;   // 鼠标位置
        int yPos;   // 鼠标位置
        bool MoveFlag;  // 判断鼠标是否被按下
        float LargeRatio = 15; // 图形放大系数
        int calc_num;   // 计算次数标记 
        double Fs_min;    // 最小Fs
        List<double> Fs_min_list = new List<double>(); // 为了统计各点Fs 画等势线
        // 记录最小的Fs中的参数
        double center_x_min;
        double center_y_min;
        double radius_min;
        List<double> slice_x_min = new List<double>();
        List<double> t_min = new List<double>();
        List<double> b_min = new List<double>();

        

        // 初始化分隔条的位置
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e) {
            this.dataGridView1.Width = this.splitContainer1.Panel1.Width;
        }
        // MainForm初始化
        private void MainForm_Load(object sender, EventArgs e) {
            this.dataGridView1.Width = this.splitContainer1.Panel1.Width;
            this.MouseWheel += PictureBox1_MouseWheel; 
        }
        // 图片的放大和缩小
        void PictureBox1_MouseWheel(object sender, MouseEventArgs e){ //判断上滑还是下滑
            if (e.Delta < 0){ //计算缩放大小
                this.pictureBox1.Width = this.pictureBox1.Width * 9 / 10;
                this.pictureBox1.Height = this.pictureBox1.Height * 9 / 10;
            }else{
                this.pictureBox1.Width = this.pictureBox1.Width * 11 / 10;
                this.pictureBox1.Height = this.pictureBox1.Height * 11 / 10;
            }//设置图片在窗体居中
            
        }
  
        private void timer1_Tick(object sender, EventArgs e) {
            toolStripStatusLabel1.Text = DateTime.Now.ToShortDateString();
            toolStripStatusLabel2.Text = DateTime.Now.ToLongTimeString();
        }

        private void 边坡形状ToolStripMenuItem_Click(object sender, EventArgs e) {
            SlopeShapeForm slopeShapeForm = new SlopeShapeForm();
            slopeShapeForm.ShowDialog();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
            //鼠标已经抬起
            MoveFlag = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
            //只在鼠标按下时绘制移动
            if (MoveFlag){
                pictureBox1.Left += Convert.ToInt16(e.X - xPos);//设置x坐标.
                pictureBox1.Top += Convert.ToInt16(e.Y - yPos);//设置y坐标.
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            this.pictureBox1.Focus();
            MoveFlag = true;//已经按下.
            xPos = e.X;//当前x坐标.
            yPos = e.Y;//当前y坐标.
        }
        // 位图的坐标转换
        public void ConvertPoint(ref float x,ref float y){
            float Height = this.pictureBox1.Height;
            x = x + 10; // 10与边界的距离
            y = Height - y - 10;
        }
        
        // 读取csv
        private void toolStripButton1_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();//首先根据打开文件对话框，选择excel表格
            ofd.Filter = "表格|*.csv";//打开文件对话框筛选器
            string strPath;//文件完整的路径名
            if (ofd.ShowDialog() == DialogResult.OK) 
            {
                try
                {
                    strPath = ofd.FileName.Replace("\\", "\\\\"); 
                    MessageBox.Show(strPath);
                    string filePath = strPath.Substring(0,strPath.LastIndexOf("\\")+1);
                    MessageBox.Show(filePath);
                    //filePath = "D:\\Coding\\Github\\Little-ExE\\翟钱的条分法计算边坡稳定\\C#-Bishop\\";
                    string cnstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=\"text;HDR=Yes;FMT=Delimited\";";
                    string fileName =  strPath.Remove(0,strPath.LastIndexOf("\\") + 1) ;
                    OleDbConnection cn = new OleDbConnection(cnstring);
                    string aSQL = "select * from [" + fileName + "]";
                    cn.Open();
                    OleDbDataAdapter da = new OleDbDataAdapter(aSQL, cn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cn.Close();
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);//捕捉异常
                }
            }
        }
        // 圆心点列表 X方向
        public List<double> Center_X() {
            List<double> result = new List<double>();
            double temp_x = Setting.ShareClass.CenterX;
            result.Add(Setting.ShareClass.CenterX);
            double delta_x = Setting.ShareClass.CenterWidth / Setting.ShareClass.SeedWidth;
            for (int i = 1; i < Setting.ShareClass.SeedWidth; i++) {
                temp_x += delta_x;
                result.Add(temp_x);
            }
            return result;
        }
        // 圆心点列表 Y方向
        public List<double> Center_Y() {
            List<double> result = new List<double>();
            double temp_y = Setting.ShareClass.CenterY;
            result.Add(Setting.ShareClass.CenterY);
            double delta_y = Setting.ShareClass.CenterHeight / Setting.ShareClass.SeedHeight;
            for (int i = 1; i < Setting.ShareClass.SeedHeight; i++) {
                temp_y += delta_y;
                result.Add(temp_y);
            }
            return result;
        }
        // 总的绘图程序 ///////////////////////////////////////////

        public void DrawSlope_1() {
            int Height = this.pictureBox1.Height;
            int Width = this.pictureBox1.Width;
            //创建位图
            Bitmap bitmap = new Bitmap(Width, Height);
            DrawSlope(bitmap);
            this.pictureBox1.Image = DrawSlope(bitmap);
        }
        public void DrawCircleCenter_2() {
            int Height = this.pictureBox1.Height;
            int Width = this.pictureBox1.Width;
            //创建位图
            Bitmap bitmap = new Bitmap(Width, Height);
            bitmap = DrawSlope(bitmap);
            bitmap = DrawCircleCenter(bitmap);
            this.pictureBox1.Image = bitmap;
        }
        public void DrawResult_3() {
            int Height = this.pictureBox1.Height;
            int Width = this.pictureBox1.Width;
            //创建位图
            Bitmap bitmap = new Bitmap(Width, Height);
            bitmap = DrawSlope(bitmap);
            bitmap = DrawCircleCenter(bitmap);
            bitmap = DrawResult(bitmap);
            this.pictureBox1.Image = bitmap;
        }
        ////////////////////////////////////////////
        
        // 绘制边坡
        public Bitmap DrawSlope(Bitmap bitmap) {
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            Pen mypen = new Pen(Color.Black, 2);
            // 读取各点值
            float Ax = Setting.ShareClass.Ax * LargeRatio;
            float Ay = Setting.ShareClass.Ay * LargeRatio;
            float Bx = Setting.ShareClass.Bx * LargeRatio;
            float By = Setting.ShareClass.By * LargeRatio;
            float Cx = Setting.ShareClass.Cx * LargeRatio;
            float Cy = Setting.ShareClass.Cy * LargeRatio;
            float Dx = Setting.ShareClass.Dx * LargeRatio;
            float Dy = Setting.ShareClass.Dy * LargeRatio;
            float Ex = Setting.ShareClass.Ex * LargeRatio;
            float Ey = Setting.ShareClass.Ey * LargeRatio;
            float Fx = Setting.ShareClass.Fx * LargeRatio;
            float Fy = Setting.ShareClass.Fy * LargeRatio;
            ConvertPoint(ref Ax, ref Ay );
            ConvertPoint(ref Bx, ref By );
            ConvertPoint(ref Cx, ref Cy );
            ConvertPoint(ref Dx, ref Dy );
            ConvertPoint(ref Ex, ref Ey );
            ConvertPoint(ref Fx, ref Fy );
            g.DrawLine(mypen, Ax, Ay, Bx, By);
            g.DrawLine(mypen, Bx, By, Cx, Cy);
            g.DrawLine(mypen, Cx, Cy, Dx, Dy);
            g.DrawLine(mypen, Dx, Dy, Ex, Ey);
            g.DrawLine(mypen, Ex, Ey, Fx, Fy);
            g.DrawLine(mypen, Fx, Fy, Ax, Ay);
            return bitmap;
        }
        // 绘制圆心点
        public Bitmap DrawCircleCenter(Bitmap bitmap) {
            Graphics g = Graphics.FromImage(bitmap);
            List<double> C_X = Center_X();
            List<double> C_Y = Center_Y();
            foreach(float cx in C_X) {
                foreach(float cy in C_Y) {
                    float x = cx * LargeRatio;
                    float y = cy * LargeRatio;
                    ConvertPoint(ref x, ref y);
                    g.FillEllipse(Brushes.Blue, x, y, 5, 5);
                }
            }
            return bitmap;
        }
        // 绘制计算结果 = Fs_min圆弧 + N个条分 + 各点的Fs值
        public Bitmap DrawResult(Bitmap bitmap) {
            Graphics g = Graphics.FromImage(bitmap);
            //选择字体、字号、风格
            Font font = new Font("Microsoft YaHei", 6, FontStyle.Bold);
            Pen mypen = new Pen(Color.Green, 2);
            Pen mypen_thin = new Pen(Color.Green, 1);
            // 获取圆心坐标
            int i = 0;
            List<double> C_X = Center_X();
            List<double> C_Y = Center_Y();
            try {
                // 绘制各个圆心
                foreach(float cx in C_X) {
                    foreach(float cy in C_Y) {
                        float x = cx * LargeRatio;
                        float y = cy * LargeRatio;
                        ConvertPoint(ref x, ref y);
                        string str = Fs_min_list[i].ToString();
                        g.DrawString(str, font, Brushes.Black, x, y-10);
                        i++;                  
                    }
                } 
                // 绘制圆心+半径
                float x_min = (float)(center_x_min *  LargeRatio);
                float y_min = (float)(center_y_min *  LargeRatio);
                ConvertPoint(ref x_min, ref y_min );
                g.FillEllipse(Brushes.Red, x_min, y_min, 6, 6);
                float p_left_x = (float)(slice_x_min[0]*LargeRatio);
                float p_right_x = (float)(slice_x_min[Setting.ShareClass.N]*LargeRatio);
                float p_left_y = (float)(t_min[0]*LargeRatio);
                float p_right_y = (float)(t_min[Setting.ShareClass.N]*LargeRatio);
                ConvertPoint(ref p_left_x, ref p_left_y );
                ConvertPoint(ref p_right_x, ref p_right_y );
                g.DrawLine(mypen, p_left_x, p_left_y, x_min, y_min);
                g.DrawLine(mypen, p_right_x, p_right_y, x_min, y_min);
                // 绘制圆弧
                for(int k = 0; k < slice_x_min.Count-1; k++) {
                    float x1 = (float)(slice_x_min[k]*LargeRatio);
                    float y1 = (float)(b_min[k]*LargeRatio);
                    float x2 = (float)(slice_x_min[k+1]*LargeRatio);
                    float y2 = (float)(b_min[k+1]*LargeRatio);
                    ConvertPoint(ref x1, ref y1 );
                    ConvertPoint(ref x2, ref y2 );
                    g.DrawLine(mypen, x1, y1, x2, y2);
                }
                // 绘制土条
                for(int m = 1; m < slice_x_min.Count-1; m++) { 
                    float a1 = (float)(slice_x_min[m]*LargeRatio);
                    float b1 = (float)(t_min[m]*LargeRatio);
                    float a2 = (float)(slice_x_min[m]*LargeRatio);
                    float b2 = (float)(b_min[m]*LargeRatio);
                    ConvertPoint(ref a1, ref b1 );
                    ConvertPoint(ref a2, ref b2 );
                    g.DrawLine(mypen_thin, a1, b1, a2, b2);
                }
            } catch {
                MessageBox.Show("请先进行计算");
                return bitmap;
            }
                    
            return bitmap;
        }
        // 触发绘制边坡
        private void toolStripButton3_Click(object sender, EventArgs e) {
            DrawSlope_1();
        }
        // 图片复位
        private void toolStripButton4_Click(object sender, EventArgs e) {
            int y = splitContainer1.Panel2.Top;
            pictureBox1.Width = splitContainer1.Panel2.Width;
            pictureBox1.Height = splitContainer1.Panel2.Height;
            pictureBox1.Location = new Point(0, y);
        }

        

        private void toolStripButton5_Click(object sender, EventArgs e) {        
            // 开始计算
            ComputeBishop();
        }
        public void ComputeBishop() {
            // 初始化计数
            calc_num = 1;
            Fs_min = 999;
            Fs_min_list.Clear();
            // 清空上一次的数据
            dataGridView1.Rows.Clear();
            //////////////////////////////////////////////////////////////  
            // 分条的宽度
            double Slice_width(double point_left,double point_right) {
                double width = Math.Abs(point_right-point_left);
                return width / Setting.ShareClass.N;
            }
            // 分条的x坐标 维度是N+1
            List<double> Slice_x_axis(double point_left, double point_right) {
                List<double> result = new List<double>();
                double single_width = Slice_width(point_left, point_right);
                double temp = point_left;
                for (int i=0; i < Setting.ShareClass.N + 1; i++) {
                    result.Add(temp);
                    temp += single_width;
                }
                return result;
            }
            // 计算面积 将四边形分成两个三角形，根据海伦公式
            double CalcArea(double x1,double y1,double x2,double y2,double x3,double y3,double x4,double y4) {
                double calcDistance(double a1,double b1,double a2,double b2) {
                    return Math.Sqrt(Math.Pow((a1-a2),2.0) + Math.Pow((b1-b2),2.0));
                }
                double d12 = calcDistance(x1, y1, x2, y2);
	            double d23 = calcDistance(x2, y2, x3, y3);
	            double d34 = calcDistance(x3, y3, x4, y4);
	            double d41 = calcDistance(x4, y4, x1, y1);
	            double d24 = calcDistance(x2, y2, x4, y4);
                double k1 = (d12+d41+d24)/2;
	            double k2 = (d23+d34+d24)/2;
	            double s1 = Math.Sqrt((k1*(k1-d12)*(k1-d41)*(k1-d24)));
	            double s2 = Math.Sqrt((k2*(k2-d23)*(k2-d34)*(k2-d24)));
                double s = s1+s2;
                return s;
            }

            // 解二元一次方程组
            double Equations1_2(double a, double b, double c) {
                double de = b * b - 4 * a * c;
                double jie = -1;
                if (a != 0){
                    if (de >= 0){
                        double x1 = ((-b) + Math.Sqrt(de)) / (2 * a);
                        double x2 = ((-b) - Math.Sqrt(de)) / (2 * a);
                        jie = Math.Max(x1,x2);
                    }
                    else{
                        jie = -1; // -1 表示无解
                    }
                }
                return jie;
            }

            // 根据圆心坐标 计算OD长度为参考半径
            double Circle_radius(double center_x, double center_y){ 
                return Math.Sqrt(Math.Pow((Setting.ShareClass.Dx-center_x),2.0)+Math.Pow((Setting.ShareClass.Dy-center_y),2.0));
            }

            // 点到斜坡面的距离
            double GetMinDistance(double Ax, double Ay){
                double dis = 0;
                double Cx = Setting.ShareClass.Cx;
                double Cy = Setting.ShareClass.Cy;
                double Dx = Setting.ShareClass.Dx;
                double Dy = Setting.ShareClass.Dy;
                double lineK = (Dy - Cy) / (Dx - Cx);
                double lineC = (Dx * Cy - Cx * Dy) / (Dx - Cx);
                dis = Math.Abs(lineK * Ax - Ay + lineC) / (Math.Sqrt(lineK * lineK + 1));
                return dis;
            }
	        //  圆弧与左侧坡上地面的交点 横坐标值
            double Point_left(double center_x,double center_y, double r) {
                double y = Setting.ShareClass.Cy;
                double temp_x = Math.Sqrt(r*r-Math.Pow((y-center_y),2.0));
                return center_x-temp_x;
            }

            // 圆弧与右侧坡下地面的交点 横坐标值
            double Point_Right(double center_x, double center_y, double r, bool bottom) {
                double result = -1;
                double Cx = Setting.ShareClass.Cx;
                double Cy = Setting.ShareClass.Cy;
                double Dx = Setting.ShareClass.Dx;
                double Dy = Setting.ShareClass.Dy;
                
                if ( r <= GetMinDistance(center_x,center_y) ) {
                    // MessageBox.Show("没有交点");
                    return result;
                }
                if (bottom == true){
                    result = Math.Sqrt(r*r-Math.Pow((Dy-center_y),2.0))+center_x;
                } 
                else {
                    double K = (Dy-Cy)/(Dx-Cx);
                    double B = -K*Cx+Cy-center_y;
                    double a = K*K+1;
                    double b = -2*center_x+2*B*K;
                    double c = center_x*center_x+B*B-r*r;
                    // MessageBox.Show(K.ToString()+"\n"+B.ToString()+"\n"+a.ToString()+"\n"+b.ToString()+"\n"+c.ToString());
                    result = Equations1_2(a,b,c);
                }
                return result;
            }
            // 圆弧分段交点 维度是N+1 
            List<double> Side_bottom(List<double> slice_x, double center_x, double center_y, double r) {
                List<double> side_bottom = new List<double>();
                foreach(double x in slice_x) {
                    double result = center_y-Math.Sqrt(r*r-Math.Pow((x-center_x),2.0));
                    side_bottom.Add(result);
                }
                return side_bottom;
            }
            // 上边界的分段函数表达式
            List<double> Side_top(List<double> x) {
                List<double> y_top = new List<double>();
                foreach(double t in x) {
                    double y = 0.0;
                    if (t <= Setting.ShareClass.Cx){
                        y = Setting.ShareClass.Cy;
                    }
                    else if(t > Setting.ShareClass.Cx && t <= Setting.ShareClass.Dx){
                        y = ((Setting.ShareClass.Dy-Setting.ShareClass.Cy)/(Setting.ShareClass.Dx-Setting.ShareClass.Cx))*(t-Setting.ShareClass.Cx)+Setting.ShareClass.Cy;
                    }
                    else if( t > Setting.ShareClass.Dx){
                        y = Setting.ShareClass.Dy;
                    }
                    y_top.Add(y);
                }
                return y_top;
            }
   
            // 半径变化列表
            List<double> Radius_Range() {
                List<double> result = new List<double>();
                double r_range = Setting.ShareClass.RadiusRange;
                double temp_r = r_range;
                while(temp_r >= -r_range) {
                    result.Add(temp_r);
                    temp_r -= Setting.ShareClass.RadiusDelta;
                }
                return result;
                
            }

            // 分条的重度 已经*了gamma 维度是N
            List<double> slice_W(List<double> x, List<double> t,  List<double> b) {
                List<double> result = new List<double>();
                for (int i = 0; i < Setting.ShareClass.N; i++) {
                    double W = CalcArea(x[i],b[i],x[i],t[i],x[i+1],t[i+1],x[i+1],b[i+1])*Setting.ShareClass.Gamma;
                    result.Add(W);
                }
                return result;
            }

            // 分条中点与圆心点的夹角 维度是N
            List<double> slice_theta(List<double> x, List<double> b, double center_x, double center_y) {
                List<double> slice_mid_x = new List<double>();
                List<double> slice_mid_y = new List<double>();
                List<double> result = new List<double>();
                for (int i = 0; i < Setting.ShareClass.N; i++) {
                    slice_mid_x.Add((x[i]+x[i+1])/2);
                    slice_mid_y.Add((b[i]+b[i+1])/2);
                }
                for (int i = 0; i < Setting.ShareClass.N; i++) {
                    double theta = Math.Atan((center_x-slice_mid_x[i])/(center_y-slice_mid_y[i]));
                    result.Add(theta);
                }
                return result; // 返回的是弧度制
            }
            // 角度制转弧度制
            double DegToRad(double deg){ //角度化弧度
                double rad = deg / 180 * Math.PI;
                return rad;
            }

            ///////测试代码///////////////////////////////////////////////// 

            // 主计算程序
            List<double> C_X = Center_X();
            List<double> C_Y = Center_Y();

            foreach (double center_x in C_X) {
                foreach (double center_y in C_Y) {
                    double length_OD = Circle_radius(center_x, center_y);
                    double Fs_min_each = 999;
                    List<double> R_List = Radius_Range();
                    foreach (double r in R_List) {
                        // 在OD的基础上对R变化
                        double R = length_OD + r;
                        // 与坡顶地面交点
                        double point_l = Point_left(center_x,center_y,R);
                        // 与坡面or底地面交点 根据R判断是坡面还是坡底
                        bool bottom;
                        if (r >= 0) {
                            bottom = true;
                        } else {
                            bottom = false;
                        }
                        double point_r = Point_Right(center_x,center_y,R,bottom);
                        if (point_r == -1) { // 若没有交点 退出本次循环
                            break;
                        }
                        // 根据N 推算的分条横坐标值 维度N+1
                        List<double> slice_x = Slice_x_axis(point_l,point_r);
                        // 上边界的y值 维度N+1
                        List<double> side_t = Side_top(slice_x);
                        // 圆弧下边界的y值 维度N+1
                        List<double> side_b = Side_bottom(slice_x,center_x,center_y,R);
                        // 分条宽度
                        double b = Slice_width(point_l,point_r);
                        // 分条中点与圆心点的夹角 维度N
                        List<double> theta = slice_theta(slice_x,side_b,center_x,center_y);
                        // 分条的重度 分条面积*gamma 维度N
                        List<double> W = slice_W(slice_x,side_t,side_b);
                        // 求和W_i*sin(theta_i) Fs的分母
                        double sum_W_theta = 0;
                        for (int j = 0; j < W.Count; j++) {
                            sum_W_theta += W[j] * Math.Sin(theta[j]);
                        }
                        // 迭代Fs至与上次之差小于0.1 
                        double Fs = 1;
		                double Fs_last = 0;
                        while (Math.Abs(Fs - Fs_last) > 0.1) {
                            double sum_Fs_numerator = 0;
                            for (int i = 0; i < Setting.ShareClass.N; i++) {
                                double m_theta = Math.Cos(theta[i]) + Math.Tan(DegToRad(Setting.ShareClass.EffectivePhi)) * Math.Sin(theta[i]) / Fs;
                                sum_Fs_numerator += (1 / m_theta) * (Setting.ShareClass.EffectiveC * b + W[i] * Math.Tan(DegToRad(Setting.ShareClass.EffectivePhi)));
                            }
                            Fs_last = Fs;
                            Fs = Math.Round(sum_Fs_numerator / sum_W_theta, 3);
                        }
                        // 添加入dataGridView1 逐行显示在Data中
                        int index = this.dataGridView1.Rows.Add();
                        this.dataGridView1.Rows[index].Cells[0].Value = calc_num;
                        this.dataGridView1.Rows[index].Cells[1].Value = Math.Round(center_x,2);
                        this.dataGridView1.Rows[index].Cells[2].Value = Math.Round(center_y,2);
                        this.dataGridView1.Rows[index].Cells[3].Value = Math.Round(R,2);
                        this.dataGridView1.Rows[index].Cells[4].Value = Fs;
                        calc_num += 1;
                        // 找到Fs_min
                        if (Fs < Fs_min) {
                            Fs_min = Math.Round(Fs,3);
                            center_x_min = Math.Round(center_x,2);
                            center_y_min = Math.Round(center_y,2);
                            radius_min = Math.Round(R,2);
                            slice_x_min = slice_x;
                            t_min = side_t;
                            b_min = side_b;
                        }
                        if (Fs < Fs_min_each){  // 为了统计各点Fs 画等势线
			                Fs_min_each = Fs;
                        }
                    }
                    Fs_min_list.Add(Fs_min_each);
                }
            }
            
            // 有一个标签显示最危险滑动面+提示计算完成
            toolStripStatusLabel3.Text = "最危险滑动面为: "+"圆心坐标=("+center_x_min.ToString()+","+center_y_min.ToString()+") "+"半径="+radius_min.ToString()+". "+"安全系数Fs="+Fs_min.ToString();
            MessageBox.Show("最危险滑动面为: "+"圆心坐标=("+center_x_min.ToString()+","+center_y_min.ToString()+") "+"半径="+radius_min.ToString()+".\n"+"安全系数Fs="+Fs_min.ToString(),"计算完成");
        }

        private void 计算布种ToolStripMenuItem_Click(object sender, EventArgs e) {
            SetSeedForm setSeedForm = new SetSeedForm();
            setSeedForm.ShowDialog();
        }

        private void toolStripButton6_Click(object sender, EventArgs e) {
            double l = Math.Tan(Setting.ShareClass.EffectivePhi);
            double tt = Math.Tan(30);
            MessageBox.Show("l"+l.ToString());
            MessageBox.Show("tt"+tt.ToString());
        }
        // 触发绘制圆心点
        private void toolStripButton2_Click(object sender, EventArgs e) {
            DrawCircleCenter_2();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e) {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void 土体参数ToolStripMenuItem_Click(object sender, EventArgs e) {
            SoilParameterForm soilParameterForm = new SoilParameterForm();
            soilParameterForm.ShowDialog();
        }

        private void toolStripButton7_Click(object sender, EventArgs e) {
            DrawResult_3();
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e) {
             ComputeBishop();
        }
    }
}

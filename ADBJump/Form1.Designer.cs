namespace ADBJump
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rtbCmd = new System.Windows.Forms.RichTextBox();
            this.btnDebugCapture = new System.Windows.Forms.Button();
            this.tbxPressTimeValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxRotate180 = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 679);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(725, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(62, 17);
            this.toolStripStatusLabel1.Text = "Andriod :";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(80, 17);
            this.toolStripStatusLabel2.Text = "未检测到设备";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(0, 640);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(360, 39);
            this.panel1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(109, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(138, 39);
            this.button2.TabIndex = 1;
            this.button2.Text = "Home";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(247, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(113, 39);
            this.button3.TabIndex = 2;
            this.button3.Text = "返回";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 39);
            this.button1.TabIndex = 0;
            this.button1.Text = "菜单";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(360, 640);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(553, 650);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(51, 21);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "1000";
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(499, 655);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "跳后等待";
            // 
            // rtbCmd
            // 
            this.rtbCmd.Location = new System.Drawing.Point(364, 0);
            this.rtbCmd.Name = "rtbCmd";
            this.rtbCmd.Size = new System.Drawing.Size(360, 640);
            this.rtbCmd.TabIndex = 5;
            this.rtbCmd.Text = "";
            this.rtbCmd.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rtbCmd_MouseClick);
            // 
            // btnDebugCapture
            // 
            this.btnDebugCapture.Location = new System.Drawing.Point(631, 656);
            this.btnDebugCapture.Name = "btnDebugCapture";
            this.btnDebugCapture.Size = new System.Drawing.Size(90, 23);
            this.btnDebugCapture.TabIndex = 7;
            this.btnDebugCapture.Text = "无画面调试";
            this.btnDebugCapture.UseVisualStyleBackColor = true;
            this.btnDebugCapture.Click += new System.EventHandler(this.btnDebugCapture_Click);
            // 
            // tbxPressTimeValue
            // 
            this.tbxPressTimeValue.Location = new System.Drawing.Point(368, 658);
            this.tbxPressTimeValue.Name = "tbxPressTimeValue";
            this.tbxPressTimeValue.Size = new System.Drawing.Size(105, 21);
            this.tbxPressTimeValue.TabIndex = 8;
            this.tbxPressTimeValue.Text = "4.00";
            this.tbxPressTimeValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxPressTimeValue_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(366, 643);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "跳跃参数(小近大远）";
            // 
            // cbxRotate180
            // 
            this.cbxRotate180.AutoSize = true;
            this.cbxRotate180.Location = new System.Drawing.Point(631, 640);
            this.cbxRotate180.Name = "cbxRotate180";
            this.cbxRotate180.Size = new System.Drawing.Size(90, 16);
            this.cbxRotate180.TabIndex = 10;
            this.cbxRotate180.Text = "画面旋转180";
            this.cbxRotate180.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 701);
            this.Controls.Add(this.btnDebugCapture);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cbxRotate180);
            this.Controls.Add(this.tbxPressTimeValue);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rtbCmd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "辅助跳一跳";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtbCmd;
        private System.Windows.Forms.Button btnDebugCapture;
        private System.Windows.Forms.TextBox tbxPressTimeValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbxRotate180;
    }
}


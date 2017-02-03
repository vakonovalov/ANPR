namespace NumberPlateDetector
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Button play;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.streamBox = new Emgu.CV.UI.ImageBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.crop = new Emgu.CV.UI.ImageBox();
            this.symbols = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pr1 = new Emgu.CV.UI.ImageBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pr2 = new Emgu.CV.UI.ImageBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.angleBox = new System.Windows.Forms.TextBox();
            this.normBox = new Emgu.CV.UI.ImageBox();
            this.label7 = new System.Windows.Forms.Label();
            this.nr2Box = new Emgu.CV.UI.ImageBox();
            this.label8 = new System.Windows.Forms.Label();
            this.nr1Box = new Emgu.CV.UI.ImageBox();
            this.label9 = new System.Windows.Forms.Label();
            this.stop = new System.Windows.Forms.Button();
            this.pause = new System.Windows.Forms.Button();
            this.sourcePlay = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.actionLog = new System.Windows.Forms.ListView();
            play = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.streamBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pr1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pr2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.normBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nr2Box)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nr1Box)).BeginInit();
            this.SuspendLayout();
            // 
            // play
            // 
            play.BackColor = System.Drawing.SystemColors.ControlLightLight;
            play.FlatAppearance.BorderColor = System.Drawing.Color.White;
            play.FlatAppearance.BorderSize = 0;
            play.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            play.Image = ((System.Drawing.Image)(resources.GetObject("play.Image")));
            play.Location = new System.Drawing.Point(950, 490);
            play.Name = "play";
            play.Size = new System.Drawing.Size(65, 63);
            play.TabIndex = 8;
            play.UseVisualStyleBackColor = false;
            play.Click += new System.EventHandler(this.play_Click);
            // 
            // streamBox
            // 
            this.streamBox.Location = new System.Drawing.Point(12, 30);
            this.streamBox.Name = "streamBox";
            this.streamBox.Size = new System.Drawing.Size(523, 481);
            this.streamBox.TabIndex = 2;
            this.streamBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Входной поток";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(541, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Поворот";
            // 
            // crop
            // 
            this.crop.Location = new System.Drawing.Point(544, 46);
            this.crop.Name = "crop";
            this.crop.Size = new System.Drawing.Size(247, 86);
            this.crop.TabIndex = 5;
            this.crop.TabStop = false;
            // 
            // symbols
            // 
            this.symbols.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.symbols.Location = new System.Drawing.Point(544, 466);
            this.symbols.Name = "symbols";
            this.symbols.Size = new System.Drawing.Size(244, 45);
            this.symbols.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(541, 450);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Распознанные символы\r\n";
            // 
            // pr1
            // 
            this.pr1.Location = new System.Drawing.Point(544, 193);
            this.pr1.Name = "pr1";
            this.pr1.Size = new System.Drawing.Size(247, 86);
            this.pr1.TabIndex = 11;
            this.pr1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(541, 177);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(248, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Размытие, морф. раскрытие Ч/Б изображения";
            // 
            // pr2
            // 
            this.pr2.Location = new System.Drawing.Point(544, 317);
            this.pr2.Name = "pr2";
            this.pr2.Size = new System.Drawing.Size(247, 86);
            this.pr2.TabIndex = 13;
            this.pr2.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(541, 301);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(197, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Оператор Собеля + морф. раскрытие";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(541, 147);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Rotate angle: ";
            // 
            // angleBox
            // 
            this.angleBox.Location = new System.Drawing.Point(611, 144);
            this.angleBox.Name = "angleBox";
            this.angleBox.Size = new System.Drawing.Size(58, 20);
            this.angleBox.TabIndex = 15;
            // 
            // normBox
            // 
            this.normBox.Location = new System.Drawing.Point(846, 46);
            this.normBox.Name = "normBox";
            this.normBox.Size = new System.Drawing.Size(247, 86);
            this.normBox.TabIndex = 17;
            this.normBox.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(843, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Нормализация:";
            // 
            // nr2Box
            // 
            this.nr2Box.Location = new System.Drawing.Point(846, 317);
            this.nr2Box.Name = "nr2Box";
            this.nr2Box.Size = new System.Drawing.Size(247, 86);
            this.nr2Box.TabIndex = 21;
            this.nr2Box.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(843, 301);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(99, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Промежуточный 2";
            // 
            // nr1Box
            // 
            this.nr1Box.Location = new System.Drawing.Point(846, 193);
            this.nr1Box.Name = "nr1Box";
            this.nr1Box.Size = new System.Drawing.Size(247, 86);
            this.nr1Box.TabIndex = 19;
            this.nr1Box.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(843, 177);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Промежуточный 1";
            // 
            // stop
            // 
            this.stop.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.stop.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.stop.FlatAppearance.BorderSize = 0;
            this.stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stop.Image = ((System.Drawing.Image)(resources.GetObject("stop.Image")));
            this.stop.Location = new System.Drawing.Point(857, 490);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(72, 63);
            this.stop.TabIndex = 22;
            this.stop.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.stop.UseVisualStyleBackColor = false;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // pause
            // 
            this.pause.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pause.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.pause.FlatAppearance.BorderSize = 0;
            this.pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pause.Image = ((System.Drawing.Image)(resources.GetObject("pause.Image")));
            this.pause.Location = new System.Drawing.Point(1028, 490);
            this.pause.Name = "pause";
            this.pause.Size = new System.Drawing.Size(65, 63);
            this.pause.TabIndex = 23;
            this.pause.UseVisualStyleBackColor = false;
            this.pause.Click += new System.EventHandler(this.pause_Click);
            // 
            // sourcePlay
            // 
            this.sourcePlay.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.sourcePlay.FlatAppearance.BorderSize = 0;
            this.sourcePlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sourcePlay.Image = ((System.Drawing.Image)(resources.GetObject("sourcePlay.Image")));
            this.sourcePlay.Location = new System.Drawing.Point(1028, 426);
            this.sourcePlay.Name = "sourcePlay";
            this.sourcePlay.Size = new System.Drawing.Size(62, 58);
            this.sourcePlay.TabIndex = 24;
            this.sourcePlay.UseVisualStyleBackColor = true;
            this.sourcePlay.Click += new System.EventHandler(this.sourcePlay_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 522);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 13);
            this.label10.TabIndex = 26;
            this.label10.Text = "Лог действий:";
            // 
            // actionLog
            // 
            this.actionLog.Location = new System.Drawing.Point(15, 538);
            this.actionLog.Name = "actionLog";
            this.actionLog.Size = new System.Drawing.Size(520, 62);
            this.actionLog.TabIndex = 27;
            this.actionLog.UseCompatibleStateImageBehavior = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ClientSize = new System.Drawing.Size(1165, 612);
            this.Controls.Add(this.actionLog);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.sourcePlay);
            this.Controls.Add(this.pause);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.nr2Box);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.nr1Box);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.normBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.angleBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pr2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pr1);
            this.Controls.Add(this.label4);
            this.Controls.Add(play);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.symbols);
            this.Controls.Add(this.crop);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.streamBox);
            this.Name = "Form1";
            this.Text = "Тестовый интерфейс";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.streamBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pr1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pr2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.normBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nr2Box)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nr1Box)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Emgu.CV.UI.ImageBox streamBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public Emgu.CV.UI.ImageBox crop;
        private System.Windows.Forms.TextBox symbols;
        private System.Windows.Forms.Label label3;
        public Emgu.CV.UI.ImageBox pr1;
        private System.Windows.Forms.Label label4;
        public Emgu.CV.UI.ImageBox pr2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.TextBox angleBox;
        public Emgu.CV.UI.ImageBox normBox;
        private System.Windows.Forms.Label label7;
        public Emgu.CV.UI.ImageBox nr2Box;
        private System.Windows.Forms.Label label8;
        public Emgu.CV.UI.ImageBox nr1Box;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.Button pause;
        private System.Windows.Forms.Button sourcePlay;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ListView actionLog;
    }
}


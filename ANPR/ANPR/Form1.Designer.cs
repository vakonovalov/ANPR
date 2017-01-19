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
            this.VideoImage = new Emgu.CV.UI.ImageBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.crop = new Emgu.CV.UI.ImageBox();
            this.symbols = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.pr1 = new Emgu.CV.UI.ImageBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pr2 = new Emgu.CV.UI.ImageBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.normBox = new Emgu.CV.UI.ImageBox();
            this.label7 = new System.Windows.Forms.Label();
            this.nr2Box = new Emgu.CV.UI.ImageBox();
            this.label8 = new System.Windows.Forms.Label();
            this.nr1Box = new Emgu.CV.UI.ImageBox();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.VideoImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pr1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pr2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.normBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nr2Box)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nr1Box)).BeginInit();
            this.SuspendLayout();
            // 
            // VideoImage
            // 
            this.VideoImage.Location = new System.Drawing.Point(12, 30);
            this.VideoImage.Name = "VideoImage";
            this.VideoImage.Size = new System.Drawing.Size(523, 481);
            this.VideoImage.TabIndex = 2;
            this.VideoImage.TabStop = false;
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(950, 505);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 48);
            this.button1.TabIndex = 8;
            this.button1.Text = "Старт";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1056, 505);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 48);
            this.button2.TabIndex = 9;
            this.button2.Text = "Стоп";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
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
            this.label4.Click += new System.EventHandler(this.label4_Click);
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
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(611, 144);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(58, 20);
            this.textBox1.TabIndex = 15;
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1165, 565);
            this.Controls.Add(this.nr2Box);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.nr1Box);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.normBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pr2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pr1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.symbols);
            this.Controls.Add(this.crop);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.VideoImage);
            this.Name = "Form1";
            this.Text = "Тестовый интерфейс";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.VideoImage)).EndInit();
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

        private Emgu.CV.UI.ImageBox VideoImage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public Emgu.CV.UI.ImageBox crop;
        private System.Windows.Forms.TextBox symbols;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        public Emgu.CV.UI.ImageBox pr1;
        private System.Windows.Forms.Label label4;
        public Emgu.CV.UI.ImageBox pr2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
        public Emgu.CV.UI.ImageBox normBox;
        private System.Windows.Forms.Label label7;
        public Emgu.CV.UI.ImageBox nr2Box;
        private System.Windows.Forms.Label label8;
        public Emgu.CV.UI.ImageBox nr1Box;
        private System.Windows.Forms.Label label9;
    }
}


namespace Interfaz_02
{
    partial class Inicio
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
            this.datos_dataGridView = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.archivo_textBox = new System.Windows.Forms.TextBox();
            this.hojas_comboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.datos_dataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // datos_dataGridView
            // 
            this.datos_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datos_dataGridView.Location = new System.Drawing.Point(21, 125);
            this.datos_dataGridView.Name = "datos_dataGridView";
            this.datos_dataGridView.Size = new System.Drawing.Size(867, 313);
            this.datos_dataGridView.TabIndex = 0;
            this.datos_dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.datos_dataGridView_CellContentClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Archivo";
            // 
            // archivo_textBox
            // 
            this.archivo_textBox.Location = new System.Drawing.Point(55, 19);
            this.archivo_textBox.Name = "archivo_textBox";
            this.archivo_textBox.Size = new System.Drawing.Size(507, 20);
            this.archivo_textBox.TabIndex = 2;
            // 
            // hojas_comboBox
            // 
            this.hojas_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hojas_comboBox.FormattingEnabled = true;
            this.hojas_comboBox.Location = new System.Drawing.Point(773, 18);
            this.hojas_comboBox.Name = "hojas_comboBox";
            this.hojas_comboBox.Size = new System.Drawing.Size(103, 21);
            this.hojas_comboBox.TabIndex = 3;
            this.hojas_comboBox.SelectedValueChanged += new System.EventHandler(this.hojas_comboBox_SelectedValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(738, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Hoja";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.archivo_textBox);
            this.groupBox1.Controls.Add(this.hojas_comboBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(882, 48);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Excel";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(630, 16);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Encabezados";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(568, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Abrir";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Inicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 504);
            this.Controls.Add(this.datos_dataGridView);
            this.Controls.Add(this.groupBox1);
            this.Name = "Inicio";
            this.Text = "Inicio";
            this.Load += new System.EventHandler(this.Inicio_Load);
            ((System.ComponentModel.ISupportInitialize)(this.datos_dataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView datos_dataGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox archivo_textBox;
        private System.Windows.Forms.ComboBox hojas_comboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.BindingSource bindingSource1;
    }
}
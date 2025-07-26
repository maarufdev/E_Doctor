namespace E_Doctor.WindowsFormApp;
partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        main_header_panel = new Panel();
        main_sidebar_panel = new Panel();
        main_context_panel = new Panel();
        man_header_title_label = new Label();
        dashboard_btn = new Button();
        button1 = new Button();
        button2 = new Button();
        button3 = new Button();
        button4 = new Button();
        panel1 = new Panel();
        label1 = new Label();
        label2 = new Label();
        label3 = new Label();
        panel2 = new Panel();
        label4 = new Label();
        label5 = new Label();
        label6 = new Label();
        panel3 = new Panel();
        label7 = new Label();
        label8 = new Label();
        label9 = new Label();
        panel4 = new Panel();
        label10 = new Label();
        main_header_panel.SuspendLayout();
        main_sidebar_panel.SuspendLayout();
        main_context_panel.SuspendLayout();
        panel1.SuspendLayout();
        panel2.SuspendLayout();
        panel3.SuspendLayout();
        panel4.SuspendLayout();
        SuspendLayout();
        // 
        // main_header_panel
        // 
        main_header_panel.BackColor = Color.FromArgb(12, 165, 217);
        main_header_panel.Controls.Add(man_header_title_label);
        main_header_panel.Dock = DockStyle.Top;
        main_header_panel.Location = new Point(0, 0);
        main_header_panel.Margin = new Padding(4, 2, 4, 2);
        main_header_panel.Name = "main_header_panel";
        main_header_panel.Size = new Size(1428, 100);
        main_header_panel.TabIndex = 0;
        // 
        // main_sidebar_panel
        // 
        main_sidebar_panel.BackColor = Color.White;
        main_sidebar_panel.Controls.Add(button4);
        main_sidebar_panel.Controls.Add(button3);
        main_sidebar_panel.Controls.Add(button2);
        main_sidebar_panel.Controls.Add(button1);
        main_sidebar_panel.Controls.Add(dashboard_btn);
        main_sidebar_panel.Dock = DockStyle.Left;
        main_sidebar_panel.Location = new Point(0, 100);
        main_sidebar_panel.Margin = new Padding(4, 2, 4, 2);
        main_sidebar_panel.Name = "main_sidebar_panel";
        main_sidebar_panel.Size = new Size(248, 684);
        main_sidebar_panel.TabIndex = 1;
        // 
        // main_context_panel
        // 
        main_context_panel.BackColor = Color.FromArgb(243, 244, 246);
        main_context_panel.Controls.Add(panel4);
        main_context_panel.Controls.Add(panel3);
        main_context_panel.Controls.Add(panel2);
        main_context_panel.Controls.Add(panel1);
        main_context_panel.Dock = DockStyle.Right;
        main_context_panel.Location = new Point(247, 100);
        main_context_panel.Margin = new Padding(4, 2, 4, 2);
        main_context_panel.Name = "main_context_panel";
        main_context_panel.Size = new Size(1181, 684);
        main_context_panel.TabIndex = 2;
        // 
        // man_header_title_label
        // 
        man_header_title_label.AutoSize = true;
        man_header_title_label.BackColor = Color.Transparent;
        man_header_title_label.Font = new Font("Times New Roman", 30F);
        man_header_title_label.ForeColor = Color.White;
        man_header_title_label.Location = new Point(41, 22);
        man_header_title_label.Margin = new Padding(4, 0, 4, 0);
        man_header_title_label.Name = "man_header_title_label";
        man_header_title_label.Size = new Size(215, 57);
        man_header_title_label.TabIndex = 0;
        man_header_title_label.Text = "E-Doctor";
        // 
        // dashboard_btn
        // 
        dashboard_btn.FlatAppearance.BorderSize = 0;
        dashboard_btn.FlatStyle = FlatStyle.Flat;
        dashboard_btn.Font = new Font("Times New Roman", 14F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
        dashboard_btn.ForeColor = SystemColors.ControlDarkDark;
        dashboard_btn.Location = new Point(41, 47);
        dashboard_btn.Name = "dashboard_btn";
        dashboard_btn.Size = new Size(169, 39);
        dashboard_btn.TabIndex = 0;
        dashboard_btn.Text = "Dashboard";
        dashboard_btn.UseVisualStyleBackColor = true;
        // 
        // button1
        // 
        button1.FlatAppearance.BorderSize = 0;
        button1.FlatStyle = FlatStyle.Flat;
        button1.Font = new Font("Times New Roman", 14F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
        button1.ForeColor = SystemColors.ControlDarkDark;
        button1.Location = new Point(41, 101);
        button1.Name = "button1";
        button1.Size = new Size(169, 39);
        button1.TabIndex = 1;
        button1.Text = "Patients";
        button1.UseVisualStyleBackColor = true;
        // 
        // button2
        // 
        button2.FlatAppearance.BorderSize = 0;
        button2.FlatStyle = FlatStyle.Flat;
        button2.Font = new Font("Times New Roman", 14F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
        button2.ForeColor = SystemColors.ControlDarkDark;
        button2.Location = new Point(41, 161);
        button2.Name = "button2";
        button2.Size = new Size(169, 39);
        button2.TabIndex = 2;
        button2.Text = "Doctors";
        button2.UseVisualStyleBackColor = true;
        // 
        // button3
        // 
        button3.FlatAppearance.BorderSize = 0;
        button3.FlatStyle = FlatStyle.Flat;
        button3.Font = new Font("Times New Roman", 14F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
        button3.ForeColor = SystemColors.ControlDarkDark;
        button3.Location = new Point(41, 215);
        button3.Name = "button3";
        button3.Size = new Size(169, 39);
        button3.TabIndex = 3;
        button3.Text = "Diagnostics";
        button3.UseVisualStyleBackColor = true;
        // 
        // button4
        // 
        button4.FlatAppearance.BorderSize = 0;
        button4.FlatStyle = FlatStyle.Flat;
        button4.Font = new Font("Times New Roman", 14F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
        button4.ForeColor = SystemColors.ControlDarkDark;
        button4.Location = new Point(41, 274);
        button4.Name = "button4";
        button4.Size = new Size(169, 39);
        button4.TabIndex = 4;
        button4.Text = "Settings";
        button4.UseVisualStyleBackColor = true;
        // 
        // panel1
        // 
        panel1.BackColor = Color.White;
        panel1.BorderStyle = BorderStyle.Fixed3D;
        panel1.Controls.Add(label3);
        panel1.Controls.Add(label2);
        panel1.Controls.Add(label1);
        panel1.Font = new Font("Arial", 9F, FontStyle.Strikeout);
        panel1.Location = new Point(52, 34);
        panel1.Name = "panel1";
        panel1.Size = new Size(318, 220);
        panel1.TabIndex = 0;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Font = new Font("Arial", 16F, FontStyle.Bold);
        label1.ForeColor = Color.FromArgb(0, 0, 0, 0);
        label1.Location = new Point(35, 20);
        label1.Name = "label1";
        label1.Size = new Size(193, 32);
        label1.TabIndex = 0;
        label1.Text = "Total Patients";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font("Arial", 20F, FontStyle.Bold);
        label2.ForeColor = Color.FromArgb(37, 99, 235);
        label2.Location = new Point(35, 77);
        label2.Name = "label2";
        label2.Size = new Size(102, 40);
        label2.TabIndex = 1;
        label2.Text = "1,245";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Font = new Font("Arial", 12F, FontStyle.Bold);
        label3.ForeColor = Color.DimGray;
        label3.Location = new Point(35, 150);
        label3.Name = "label3";
        label3.Size = new Size(207, 24);
        label3.TabIndex = 2;
        label3.Text = "Last Updated: Today";
        // 
        // panel2
        // 
        panel2.BackColor = Color.White;
        panel2.BorderStyle = BorderStyle.Fixed3D;
        panel2.Controls.Add(label4);
        panel2.Controls.Add(label5);
        panel2.Controls.Add(label6);
        panel2.Font = new Font("Arial", 9F, FontStyle.Strikeout);
        panel2.Location = new Point(400, 34);
        panel2.Name = "panel2";
        panel2.Size = new Size(318, 220);
        panel2.TabIndex = 3;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Font = new Font("Arial", 12F, FontStyle.Bold);
        label4.ForeColor = Color.DimGray;
        label4.Location = new Point(35, 150);
        label4.Name = "label4";
        label4.Size = new Size(207, 24);
        label4.TabIndex = 2;
        label4.Text = "Last Updated: Today";
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.Font = new Font("Arial", 20F, FontStyle.Bold);
        label5.ForeColor = Color.FromArgb(37, 99, 235);
        label5.Location = new Point(35, 77);
        label5.Name = "label5";
        label5.Size = new Size(102, 40);
        label5.TabIndex = 1;
        label5.Text = "1,245";
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.Font = new Font("Arial", 16F, FontStyle.Bold);
        label6.ForeColor = Color.FromArgb(0, 0, 0, 0);
        label6.Location = new Point(35, 20);
        label6.Name = "label6";
        label6.Size = new Size(200, 32);
        label6.TabIndex = 0;
        label6.Text = "Appointments";
        // 
        // panel3
        // 
        panel3.BackColor = Color.White;
        panel3.BorderStyle = BorderStyle.Fixed3D;
        panel3.Controls.Add(label7);
        panel3.Controls.Add(label8);
        panel3.Controls.Add(label9);
        panel3.Font = new Font("Arial", 9F, FontStyle.Strikeout);
        panel3.Location = new Point(736, 34);
        panel3.Name = "panel3";
        panel3.Size = new Size(318, 220);
        panel3.TabIndex = 4;
        // 
        // label7
        // 
        label7.AutoSize = true;
        label7.Font = new Font("Arial", 12F, FontStyle.Bold);
        label7.ForeColor = Color.DimGray;
        label7.Location = new Point(35, 150);
        label7.Name = "label7";
        label7.Size = new Size(207, 24);
        label7.TabIndex = 2;
        label7.Text = "Last Updated: Today";
        // 
        // label8
        // 
        label8.AutoSize = true;
        label8.Font = new Font("Arial", 20F, FontStyle.Bold);
        label8.ForeColor = Color.FromArgb(37, 99, 235);
        label8.Location = new Point(35, 77);
        label8.Name = "label8";
        label8.Size = new Size(102, 40);
        label8.TabIndex = 1;
        label8.Text = "1,245";
        // 
        // label9
        // 
        label9.AutoSize = true;
        label9.Font = new Font("Arial", 16F, FontStyle.Bold);
        label9.ForeColor = Color.FromArgb(0, 0, 0, 0);
        label9.Location = new Point(35, 20);
        label9.Name = "label9";
        label9.Size = new Size(207, 32);
        label9.TabIndex = 0;
        label9.Text = "Nurse On Duty";
        // 
        // panel4
        // 
        panel4.BackColor = Color.White;
        panel4.BorderStyle = BorderStyle.Fixed3D;
        panel4.Controls.Add(label10);
        panel4.Location = new Point(52, 311);
        panel4.Name = "panel4";
        panel4.Size = new Size(1002, 312);
        panel4.TabIndex = 5;
        // 
        // label10
        // 
        label10.AutoSize = true;
        label10.Font = new Font("Arial", 16F, FontStyle.Bold);
        label10.ForeColor = Color.FromArgb(0, 0, 0, 0);
        label10.Location = new Point(35, 26);
        label10.Name = "label10";
        label10.Size = new Size(233, 32);
        label10.TabIndex = 3;
        label10.Text = "Recent Activities";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1428, 784);
        Controls.Add(main_sidebar_panel);
        Controls.Add(main_context_panel);
        Controls.Add(main_header_panel);
        Font = new Font("Arial", 9F);
        Margin = new Padding(4, 2, 4, 2);
        Name = "MainForm";
        Text = "Form1";
        Load += Form1_Load;
        main_header_panel.ResumeLayout(false);
        main_header_panel.PerformLayout();
        main_sidebar_panel.ResumeLayout(false);
        main_context_panel.ResumeLayout(false);
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        panel2.ResumeLayout(false);
        panel2.PerformLayout();
        panel3.ResumeLayout(false);
        panel3.PerformLayout();
        panel4.ResumeLayout(false);
        panel4.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Panel main_header_panel;
    private Panel main_sidebar_panel;
    private Panel main_context_panel;
    private Label man_header_title_label;
    private Button dashboard_btn;
    private Button button4;
    private Button button3;
    private Button button2;
    private Button button1;
    private Panel panel1;
    private Label label3;
    private Label label2;
    private Label label1;
    private Panel panel4;
    private Label label10;
    private Panel panel3;
    private Label label7;
    private Label label8;
    private Label label9;
    private Panel panel2;
    private Label label4;
    private Label label5;
    private Label label6;
}

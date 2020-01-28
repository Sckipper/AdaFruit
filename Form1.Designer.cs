namespace Medic
{
    partial class MainWindow
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
            this.labelConsole = new System.Windows.Forms.Label();
            this.buttonPair = new System.Windows.Forms.Button();
            this.listViewDevices = new System.Windows.Forms.ListView();
            this.buttonUnpair = new System.Windows.Forms.Button();
            this.buttonGreen = new System.Windows.Forms.Button();
            this.buttonPurple = new System.Windows.Forms.Button();
            this.panelDraw = new System.Windows.Forms.Panel();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.labelResult = new System.Windows.Forms.Label();
            this.labelResultValue = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelAge = new System.Windows.Forms.Label();
            this.labelSurname = new System.Windows.Forms.Label();
            this.textBoxSurname = new System.Windows.Forms.TextBox();
            this.textBoxAge = new System.Windows.Forms.TextBox();
            this.labelHeight = new System.Windows.Forms.Label();
            this.textBoxHeight = new System.Windows.Forms.TextBox();
            this.labelWeight = new System.Windows.Forms.Label();
            this.textBoxWeight = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelConsole
            // 
            this.labelConsole.AutoSize = true;
            this.labelConsole.Location = new System.Drawing.Point(12, 527);
            this.labelConsole.Name = "labelConsole";
            this.labelConsole.Size = new System.Drawing.Size(0, 13);
            this.labelConsole.TabIndex = 2;
            // 
            // buttonPair
            // 
            this.buttonPair.Enabled = false;
            this.buttonPair.Location = new System.Drawing.Point(375, 25);
            this.buttonPair.Name = "buttonPair";
            this.buttonPair.Size = new System.Drawing.Size(88, 32);
            this.buttonPair.TabIndex = 0;
            this.buttonPair.Text = "Pair";
            this.buttonPair.UseVisualStyleBackColor = true;
            this.buttonPair.Click += new System.EventHandler(this.buttonPair_Click);
            // 
            // listViewDevices
            // 
            this.listViewDevices.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.listViewDevices.GridLines = true;
            this.listViewDevices.HideSelection = false;
            this.listViewDevices.Location = new System.Drawing.Point(12, 12);
            this.listViewDevices.MultiSelect = false;
            this.listViewDevices.Name = "listViewDevices";
            this.listViewDevices.Size = new System.Drawing.Size(357, 118);
            this.listViewDevices.TabIndex = 3;
            this.listViewDevices.UseCompatibleStateImageBehavior = false;
            this.listViewDevices.View = System.Windows.Forms.View.List;
            this.listViewDevices.SelectedIndexChanged += new System.EventHandler(this.listViewDevices_SelectedIndexChanged);
            // 
            // buttonUnpair
            // 
            this.buttonUnpair.Enabled = false;
            this.buttonUnpair.Location = new System.Drawing.Point(375, 80);
            this.buttonUnpair.Name = "buttonUnpair";
            this.buttonUnpair.Size = new System.Drawing.Size(88, 32);
            this.buttonUnpair.TabIndex = 4;
            this.buttonUnpair.Text = "Unpair";
            this.buttonUnpair.UseVisualStyleBackColor = true;
            this.buttonUnpair.Click += new System.EventHandler(this.buttonUnpair_Click);
            // 
            // buttonGreen
            // 
            this.buttonGreen.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonGreen.Enabled = false;
            this.buttonGreen.Location = new System.Drawing.Point(12, 145);
            this.buttonGreen.Name = "buttonGreen";
            this.buttonGreen.Size = new System.Drawing.Size(175, 42);
            this.buttonGreen.TabIndex = 5;
            this.buttonGreen.UseVisualStyleBackColor = false;
            // 
            // buttonPurple
            // 
            this.buttonPurple.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonPurple.Enabled = false;
            this.buttonPurple.Location = new System.Drawing.Point(193, 145);
            this.buttonPurple.Name = "buttonPurple";
            this.buttonPurple.Size = new System.Drawing.Size(175, 42);
            this.buttonPurple.TabIndex = 6;
            this.buttonPurple.UseVisualStyleBackColor = false;
            // 
            // panelDraw
            // 
            this.panelDraw.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDraw.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panelDraw.Location = new System.Drawing.Point(516, 12);
            this.panelDraw.Name = "panelDraw";
            this.panelDraw.Size = new System.Drawing.Size(400, 250);
            this.panelDraw.TabIndex = 7;
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonStart.Location = new System.Drawing.Point(516, 268);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(195, 50);
            this.buttonStart.TabIndex = 8;
            this.buttonStart.Text = "START";
            this.buttonStart.UseVisualStyleBackColor = true;
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonStop.Location = new System.Drawing.Point(721, 268);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(195, 50);
            this.buttonStop.TabIndex = 9;
            this.buttonStop.Text = "STOP";
            this.buttonStop.UseVisualStyleBackColor = true;
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Stencil", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResult.Location = new System.Drawing.Point(625, 362);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(168, 44);
            this.labelResult.TabIndex = 10;
            this.labelResult.Text = "Result:";
            // 
            // labelResultValue
            // 
            this.labelResultValue.AutoSize = true;
            this.labelResultValue.Font = new System.Drawing.Font("Arial Rounded MT Bold", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResultValue.Location = new System.Drawing.Point(642, 426);
            this.labelResultValue.Name = "labelResultValue";
            this.labelResultValue.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelResultValue.Size = new System.Drawing.Size(127, 40);
            this.labelResultValue.TabIndex = 11;
            this.labelResultValue.Text = "GOOD";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(22, 247);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(84, 29);
            this.labelName.TabIndex = 12;
            this.labelName.Text = "Nume:";
            // 
            // textBoxName
            // 
            this.textBoxName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxName.Location = new System.Drawing.Point(181, 251);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(173, 26);
            this.textBoxName.TabIndex = 13;
            // 
            // labelAge
            // 
            this.labelAge.AutoSize = true;
            this.labelAge.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAge.Location = new System.Drawing.Point(22, 346);
            this.labelAge.Name = "labelAge";
            this.labelAge.Size = new System.Drawing.Size(86, 29);
            this.labelAge.TabIndex = 14;
            this.labelAge.Text = "Vârstă:";
            // 
            // labelSurname
            // 
            this.labelSurname.AutoSize = true;
            this.labelSurname.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSurname.Location = new System.Drawing.Point(22, 299);
            this.labelSurname.Name = "labelSurname";
            this.labelSurname.Size = new System.Drawing.Size(117, 29);
            this.labelSurname.TabIndex = 15;
            this.labelSurname.Text = "Prenume:";
            // 
            // textBoxSurname
            // 
            this.textBoxSurname.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSurname.Location = new System.Drawing.Point(181, 299);
            this.textBoxSurname.Name = "textBoxSurname";
            this.textBoxSurname.Size = new System.Drawing.Size(173, 26);
            this.textBoxSurname.TabIndex = 16;
            // 
            // textBoxAge
            // 
            this.textBoxAge.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAge.Location = new System.Drawing.Point(181, 346);
            this.textBoxAge.Name = "textBoxAge";
            this.textBoxAge.Size = new System.Drawing.Size(173, 26);
            this.textBoxAge.TabIndex = 17;
            // 
            // labelHeight
            // 
            this.labelHeight.AutoSize = true;
            this.labelHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeight.Location = new System.Drawing.Point(22, 398);
            this.labelHeight.Name = "labelHeight";
            this.labelHeight.Size = new System.Drawing.Size(105, 29);
            this.labelHeight.TabIndex = 18;
            this.labelHeight.Text = "Înălţime:";
            // 
            // textBoxHeight
            // 
            this.textBoxHeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxHeight.Location = new System.Drawing.Point(181, 398);
            this.textBoxHeight.Name = "textBoxHeight";
            this.textBoxHeight.Size = new System.Drawing.Size(173, 26);
            this.textBoxHeight.TabIndex = 19;
            // 
            // labelWeight
            // 
            this.labelWeight.AutoSize = true;
            this.labelWeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWeight.Location = new System.Drawing.Point(22, 450);
            this.labelWeight.Name = "labelWeight";
            this.labelWeight.Size = new System.Drawing.Size(111, 29);
            this.labelWeight.TabIndex = 20;
            this.labelWeight.Text = "Greutate:";
            // 
            // textBoxWeight
            // 
            this.textBoxWeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxWeight.Location = new System.Drawing.Point(181, 450);
            this.textBoxWeight.Name = "textBoxWeight";
            this.textBoxWeight.Size = new System.Drawing.Size(173, 26);
            this.textBoxWeight.TabIndex = 21;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 549);
            this.Controls.Add(this.textBoxWeight);
            this.Controls.Add(this.labelWeight);
            this.Controls.Add(this.textBoxHeight);
            this.Controls.Add(this.labelHeight);
            this.Controls.Add(this.textBoxAge);
            this.Controls.Add(this.textBoxSurname);
            this.Controls.Add(this.labelSurname);
            this.Controls.Add(this.labelAge);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.labelResultValue);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.panelDraw);
            this.Controls.Add(this.buttonPurple);
            this.Controls.Add(this.buttonGreen);
            this.Controls.Add(this.buttonUnpair);
            this.Controls.Add(this.listViewDevices);
            this.Controls.Add(this.buttonPair);
            this.Controls.Add(this.labelConsole);
            this.Name = "MainWindow";
            this.Text = "Titlu";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelConsole;
        private System.Windows.Forms.Button buttonPair;
        private System.Windows.Forms.ListView listViewDevices;
        private System.Windows.Forms.Button buttonUnpair;
        private System.Windows.Forms.Button buttonGreen;
        private System.Windows.Forms.Button buttonPurple;
        private System.Windows.Forms.Panel panelDraw;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Label labelResultValue;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelAge;
        private System.Windows.Forms.Label labelSurname;
        private System.Windows.Forms.TextBox textBoxSurname;
        private System.Windows.Forms.TextBox textBoxAge;
        private System.Windows.Forms.Label labelHeight;
        private System.Windows.Forms.TextBox textBoxHeight;
        private System.Windows.Forms.Label labelWeight;
        private System.Windows.Forms.TextBox textBoxWeight;
    }
}


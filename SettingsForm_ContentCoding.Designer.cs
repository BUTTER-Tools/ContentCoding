namespace ContentCoding
{
    partial class SettingsForm_ContentCoding
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm_ContentCoding));
            this.OKButton = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.SelectedDictionariesCheckedListbox = new System.Windows.Forms.CheckedListBox();
            this.DictionaryDescriptionTextbox = new System.Windows.Forms.TextBox();
            this.CheckAllButton = new System.Windows.Forms.Button();
            this.UncheckAllButton = new System.Windows.Forms.Button();
            this.LoadDictionaryButton = new System.Windows.Forms.Button();
            this.RawCountsCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OKButton.Location = new System.Drawing.Point(611, 482);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(118, 40);
            this.OKButton.TabIndex = 6;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(11, 18);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(166, 20);
            this.label10.TabIndex = 12;
            this.label10.Text = "Dictionaries To Use";
            // 
            // SelectedDictionariesCheckedListbox
            // 
            this.SelectedDictionariesCheckedListbox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectedDictionariesCheckedListbox.FormattingEnabled = true;
            this.SelectedDictionariesCheckedListbox.Location = new System.Drawing.Point(15, 51);
            this.SelectedDictionariesCheckedListbox.Name = "SelectedDictionariesCheckedListbox";
            this.SelectedDictionariesCheckedListbox.ScrollAlwaysVisible = true;
            this.SelectedDictionariesCheckedListbox.Size = new System.Drawing.Size(361, 382);
            this.SelectedDictionariesCheckedListbox.Sorted = true;
            this.SelectedDictionariesCheckedListbox.TabIndex = 13;
            this.SelectedDictionariesCheckedListbox.Click += new System.EventHandler(this.SelectedDictionariesCheckedListbox_Click);
            this.SelectedDictionariesCheckedListbox.SelectedIndexChanged += new System.EventHandler(this.SelectedDictionariesCheckedListbox_SelectedIndexChanged);
            this.SelectedDictionariesCheckedListbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SelectedDictionariesCheckedListbox_KeyPress);
            // 
            // DictionaryDescriptionTextbox
            // 
            this.DictionaryDescriptionTextbox.BackColor = System.Drawing.SystemColors.Info;
            this.DictionaryDescriptionTextbox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DictionaryDescriptionTextbox.Location = new System.Drawing.Point(388, 51);
            this.DictionaryDescriptionTextbox.MaxLength = 2147483647;
            this.DictionaryDescriptionTextbox.Multiline = true;
            this.DictionaryDescriptionTextbox.Name = "DictionaryDescriptionTextbox";
            this.DictionaryDescriptionTextbox.ReadOnly = true;
            this.DictionaryDescriptionTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DictionaryDescriptionTextbox.Size = new System.Drawing.Size(377, 418);
            this.DictionaryDescriptionTextbox.TabIndex = 1000;
            this.DictionaryDescriptionTextbox.TabStop = false;
            // 
            // CheckAllButton
            // 
            this.CheckAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CheckAllButton.Location = new System.Drawing.Point(48, 482);
            this.CheckAllButton.Name = "CheckAllButton";
            this.CheckAllButton.Size = new System.Drawing.Size(118, 40);
            this.CheckAllButton.TabIndex = 1001;
            this.CheckAllButton.Text = "Check All";
            this.CheckAllButton.UseVisualStyleBackColor = true;
            this.CheckAllButton.Click += new System.EventHandler(this.CheckAllButton_Click);
            // 
            // UncheckAllButton
            // 
            this.UncheckAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UncheckAllButton.Location = new System.Drawing.Point(224, 482);
            this.UncheckAllButton.Name = "UncheckAllButton";
            this.UncheckAllButton.Size = new System.Drawing.Size(118, 40);
            this.UncheckAllButton.TabIndex = 1002;
            this.UncheckAllButton.Text = "Uncheck All";
            this.UncheckAllButton.UseVisualStyleBackColor = true;
            this.UncheckAllButton.Click += new System.EventHandler(this.UncheckAllButton_Click);
            // 
            // LoadDictionaryButton
            // 
            this.LoadDictionaryButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadDictionaryButton.Location = new System.Drawing.Point(433, 482);
            this.LoadDictionaryButton.Name = "LoadDictionaryButton";
            this.LoadDictionaryButton.Size = new System.Drawing.Size(118, 40);
            this.LoadDictionaryButton.TabIndex = 1003;
            this.LoadDictionaryButton.Text = "Load External Dictionary";
            this.LoadDictionaryButton.UseVisualStyleBackColor = true;
            this.LoadDictionaryButton.Click += new System.EventHandler(this.LoadDictionaryButton_Click);
            // 
            // RawCountsCheckbox
            // 
            this.RawCountsCheckbox.AutoSize = true;
            this.RawCountsCheckbox.Font = new System.Drawing.Font("MS Reference Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RawCountsCheckbox.Location = new System.Drawing.Point(15, 440);
            this.RawCountsCheckbox.Name = "RawCountsCheckbox";
            this.RawCountsCheckbox.Size = new System.Drawing.Size(318, 20);
            this.RawCountsCheckbox.TabIndex = 1004;
            this.RawCountsCheckbox.Text = "Provide output as raw category frequencies";
            this.RawCountsCheckbox.UseVisualStyleBackColor = true;
            // 
            // SettingsForm_ContentCoding
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 534);
            this.Controls.Add(this.RawCountsCheckbox);
            this.Controls.Add(this.LoadDictionaryButton);
            this.Controls.Add(this.UncheckAllButton);
            this.Controls.Add(this.CheckAllButton);
            this.Controls.Add(this.DictionaryDescriptionTextbox);
            this.Controls.Add(this.SelectedDictionariesCheckedListbox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.OKButton);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm_ContentCoding";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Plugin Name";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckedListBox SelectedDictionariesCheckedListbox;
        private System.Windows.Forms.TextBox DictionaryDescriptionTextbox;
        private System.Windows.Forms.Button CheckAllButton;
        private System.Windows.Forms.Button UncheckAllButton;
        private System.Windows.Forms.Button LoadDictionaryButton;
        private System.Windows.Forms.CheckBox RawCountsCheckbox;
    }
}
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace ContentCoding
{
    internal partial class SettingsForm_ContentCoding : Form
    {


        #region Get and Set Options

        public List<DictionaryMetaObject> DictDataToReturn;
        public Dictionary<string, string> DictDescriptions;
        public Dictionary<string, string> DictPrefixes;
        public bool RawFreqs;

       #endregion



        public SettingsForm_ContentCoding(List<DictionaryMetaObject> Dicts, bool RawFreqs)
        {
            InitializeComponent();

            DictDataToReturn = Dicts;
            DictDescriptions = new Dictionary<string, string>();
            DictPrefixes = new Dictionary<string, string>();

            RawCountsCheckbox.Checked = RawFreqs;

            foreach (DictionaryMetaObject Dict in Dicts)
            {
                SelectedDictionariesCheckedListbox.Items.Add(Dict.DictionaryName);

                if (Dict.UseDictionary) SelectedDictionariesCheckedListbox.SetItemChecked(
                                                    SelectedDictionariesCheckedListbox.Items.IndexOf(Dict.DictionaryName), Dict.UseDictionary);
                try
                { 
                    DictDescriptions.Add(Dict.DictionaryName, Dict.DictionaryDescription);
                    DictPrefixes.Add(Dict.DictionaryName, Dict.DictionaryCategoryPrefix);
                }
                catch
                {
                }
            }

            SelectedDictionariesCheckedListbox.SelectedIndex = 0;

            
        }










        private void OKButton_Click(object sender, System.EventArgs e)
        {
            RawFreqs = RawCountsCheckbox.Checked;
            for (int i = 0; i < DictDataToReturn.Count; i++)
            {
                DictDataToReturn[i].UseDictionary = SelectedDictionariesCheckedListbox.GetItemChecked(SelectedDictionariesCheckedListbox.Items.IndexOf(DictDataToReturn[i].DictionaryName));
            }

            this.DialogResult = DialogResult.OK;

        }

        private void SelectedDictionariesCheckedListbox_Click(object sender, System.EventArgs e)
        {
            UpdateDescription();
        }

        private void UpdateDescription()
        {
            if (SelectedDictionariesCheckedListbox.SelectedItem != null)
            {
                DictionaryDescriptionTextbox.Text = SelectedDictionariesCheckedListbox.SelectedItem.ToString() + Environment.NewLine + Environment.NewLine +
                    "Output Prefix: " + DictPrefixes[SelectedDictionariesCheckedListbox.SelectedItem.ToString()] + Environment.NewLine + Environment.NewLine +
                    DictDescriptions[SelectedDictionariesCheckedListbox.SelectedItem.ToString()];
            }
                
        }

        private void SelectedDictionariesCheckedListbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            UpdateDescription();
        }

        private void SelectedDictionariesCheckedListbox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            UpdateDescription();
        }

        private void CheckAllButton_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < SelectedDictionariesCheckedListbox.Items.Count; i++) SelectedDictionariesCheckedListbox.SetItemChecked(i, true);
        }

        private void UncheckAllButton_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < SelectedDictionariesCheckedListbox.Items.Count; i++) SelectedDictionariesCheckedListbox.SetItemChecked(i, false);
        }







        private void LoadDictionaryButton_Click(object sender, System.EventArgs e)
        {


            using (var dialog = new OpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.ValidateNames = true;
                dialog.Title = "Please choose the Dictionary file that you would like to read";
                dialog.FileName = "Dictionary.dic";
                dialog.Filter = "LIWC Dictionary File|*.dic;*.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {


                    try
                    {

                        if (SelectedDictionariesCheckedListbox.Items.Contains(Path.GetFileName(dialog.FileName)))
                        {
                            MessageBox.Show("There is already a dictionary with this name in your list.", "Error Loading Dictionary", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string DicText = File.ReadAllText(dialog.FileName, Encoding.UTF8).Trim();
                        DictionaryMetaObject InputDictData = new DictionaryMetaObject(Path.GetFileName(dialog.FileName), "User-loaded dictionary", "", DicText);
                        DictParser DP = new DictParser();
                        InputDictData.DictData = DP.ParseDict(InputDictData);
                        DictDataToReturn.Add(InputDictData);
                        SelectedDictionariesCheckedListbox.Items.Add(InputDictData.DictionaryName);
                        SelectedDictionariesCheckedListbox.SetItemChecked(SelectedDictionariesCheckedListbox.Items.IndexOf(InputDictData.DictionaryName), true);
                        DictDescriptions.Add(InputDictData.DictionaryName, InputDictData.DictionaryDescription);
                        DictPrefixes.Add(InputDictData.DictionaryName, "");


                    }

                    catch
                    {
                        MessageBox.Show("There was an error while trying to read/load your dictionary file. Is your dictionary correctly formatted? Have you already added this dictionary to this plugin?", "Error reading dictionary", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                }



            }



        }





    }
}

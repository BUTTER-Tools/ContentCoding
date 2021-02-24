using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using PluginContracts;
using OutputHelperLib;
using System.Xml;


namespace ContentCoding
{
    public partial class ContentCoding : Plugin
    {


        public string[] InputType { get; } = { "Tokens" };
        public string OutputType { get; } = "OutputArray";

        public Dictionary<int, string> OutputHeaderData { get; set; } = new Dictionary<int, string>() { { 0, "TokenCount" } };
        public bool InheritHeader { get; } = false;

        #region Plugin Details and Info

        public string PluginName { get; } = "Content Coding w/ Dictionary";
        public string PluginType { get; } = "Language Analysis";
        public string PluginVersion { get; } = "1.2.1";
        public string PluginAuthor { get; } = "Ryan L. Boyd (ryan@ryanboyd.io)";
        public string PluginDescription { get; } = "Codes your tokenized texts for dictionary-defined words and phrases. Comes with several dictionaries built in, plus allows you to load your own LIWC- and RIOT-formatted dictionaries.";
        public bool TopLevel { get; } = false;
        public string PluginTutorial { get; } = "https://youtu.be/jOkOpJTyG5M";


        List<DictionaryMetaObject> DictionaryList { get; set; }
        HashSet<string> ListOfBuiltInDictionaries { get; set; }
        public bool RawFreqs { get; set; } = false;




        public Icon GetPluginIcon
        {
            get
            {
                return Properties.Resources.icon;
            }
        }

        #endregion



        public void ChangeSettings()
        {

            using (var form = new SettingsForm_ContentCoding(DictionaryList, RawFreqs))
            {

                form.Icon = Properties.Resources.icon;
                form.Text = PluginName;


                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    RawFreqs = form.RawFreqs;
                    DictionaryList = form.DictDataToReturn;
                }
            }

        }


        public Payload RunPlugin(Payload Input)
        {



            Payload pData = new Payload();
            pData.FileID = Input.FileID;
            pData.SegmentID = Input.SegmentID;

            for (int i = 0; i < Input.StringArrayList.Count; i++)
            {

                string[] OutputArray = new string[TotalCatCount];
                //just set a nice clean blank for each column
                for (int prepCount = 0; prepCount < OutputArray.Length; prepCount++) OutputArray[prepCount] = "";

                OutputArray[0] = Input.StringArrayList[i].Length.ToString();
                //for (int j = 0; j < TotalCatCount; j++) OutputArray[j] = "0";

                //if we have more than 0 words
                if (Input.StringArrayList[i].Length > 0) {


                    TextAnalyzer TA = new TextAnalyzer();

                    for (int j = 0; j < DictionaryList.Count; j++)
                    {

                        if (DictionaryList[j].UseDictionary)
                        { 
                            Dictionary<string, int> Results = TA.AnalyzeText(DictionaryList[j].DictData, Input.StringArrayList[i]);

                            for (int k = 0; k < DictionaryList[j].DictData.CatNames.Length; k++)
                            {

                                if (RawFreqs)
                                { 
                                    OutputArray[
                                        OutputDataMap[
                                            DictionaryList[j].DictionaryCategoryPrefix + DictionaryList[j].DictData.CatNames[k]
                                                     ]
                                            ] = Results[DictionaryList[j].DictData.CatValues[k]].ToString(); // we do the +1 here because the indices are offset since the first column is "token count"
                                }
                                else
                                {
                                    OutputArray[
                                        OutputDataMap[
                                            DictionaryList[j].DictionaryCategoryPrefix + DictionaryList[j].DictData.CatNames[k]
                                                     ]
                                            ] = ((Results[DictionaryList[j].DictData.CatValues[k]] / (double)Input.StringArrayList[i].Length) * 100).ToString(); // we do the +1 here because the indices are offset since the first column is "token count"
                                }
                            }
                        }

                    }

                }

                pData.SegmentNumber.Add(Input.SegmentNumber[i]);
                pData.StringArrayList.Add(OutputArray);

              

            }

            return (pData);
        }




        public Dictionary<string, int> OutputDataMap { get; set; }
        private int TotalCatCount { get; set; } = 0;
        public void Initialize()
        {

            TotalCatCount = 1;
            Dictionary<string, int> TempOutputDataMap = new Dictionary<string, int>();
            Dictionary<int, string> TempHeaderData = new Dictionary<int, string>();
            TempHeaderData.Add(0, "TokenCount");

            for (int i = 0; i < DictionaryList.Count; i++)
            {
                if (DictionaryList[i].UseDictionary)
                {

                    DictParser DP = new DictParser();
                    DictionaryList[i].DictData = DP.ParseDict(DictionaryList[i]);

                    //add all of the categories to the header
                    for (int j = 0; j < DictionaryList[i].DictData.NumCats; j++)
                    {

                        

                        int increment = 1;
                        //makes sure that we don't have duplicate category names
                        string CatNameRoot = DictionaryList[i].DictData.CatNames[j];
                        while (TempOutputDataMap.ContainsKey(DictionaryList[i].DictionaryCategoryPrefix + DictionaryList[i].DictData.CatNames[j]))
                        {
                            DictionaryList[i].DictData.CatNames[j] = CatNameRoot + "_" + increment.ToString();
                            increment++;
                        }

                        TempHeaderData.Add(TotalCatCount,
                           DictionaryList[i].DictionaryCategoryPrefix + DictionaryList[i].DictData.CatNames[j]);

                        TempOutputDataMap.Add(DictionaryList[i].DictionaryCategoryPrefix + DictionaryList[i].DictData.CatNames[j],
                            TotalCatCount);
                        TotalCatCount++;
                    }
                }
            }

            OutputHeaderData = TempHeaderData;
            OutputDataMap = TempOutputDataMap;
        }

        public bool InspectSettings()
        {
            return true;
        }

        



        //one of the few plugins thus far where I'm actually using a constructor
        //might not be the most efficient way to handle this (especially at runtime)
        //but I don't suspect that it'll be too bad.
        public ContentCoding()
        {
            DictionaryList = new List<DictionaryMetaObject>();
            ListOfBuiltInDictionaries = new HashSet<string>();

            DictionaryList.Add(new DictionaryMetaObject("Absolutist Words",
                                                  "Al-Mosaiwi, M., & Johnstone, T. (2018). In an Absolute State: Elevated Use of Absolutist Words Is a Marker Specific to Anxiety, Depression, and Suicidal Ideation. Clinical Psychological Science, 2167702617747074. https://doi.org/10.1177/2167702617747074",
                                                  "Abs_",
                                                  Properties.Resources.Absolutist_Words));

            DictionaryList.Add(new DictionaryMetaObject("Agency/Communion Dictionary",
                                                  "Pietraszkiewicz, A., Formanowicz, M., Sendén, M. G., Boyd, R. L., Sikström, S., & Sczesny, S. (2019). The Big Two dictionaries: Capturing agency and communion in natural language. European Journal of Social Psychology, 49(5), 871–887. https://doi.org/10.1002/ejsp.2561",
                                                  "BigTwo_",
                                                  Properties.Resources.AgencyCommunionDict));

            DictionaryList.Add(new DictionaryMetaObject("Age Stereotypes Dictionary",
                                                  "Remedios, J. D., Chasteen, A. L., & Packer, D. J. (2010). Sunny side up: The reliance on positive age stereotypes in descriptions of future older selves. Self and Identity, 9(3), 257–275. https://doi.org/10.1080/15298860903054175",
                                                  "ASD_",
                                                  Properties.Resources.Age_Stereotypes_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Agitation/Dejection Dictionary",
                                                  "Johnsen, J. A. K., Vambheim, S. M., Wynn, R., & Wangberg, S. C. (2014). Language of motivation and emotion in an internet support group for smoking cessation: explorative use of automated content analysis to measure regulatory focus. Psychology Research and Behavior Management, 7, 19–29. https://doi.org/10.2147/PRBM.S54947",
                                                  "AgDj_",
                                                  Properties.Resources.Agitation_Dejection_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Arguing Lexicon",
                                                  "Somasundaran, S., Ruppenhofer, J., & Wiebe, J. (2007). Detecting Arguing and Sentiment in Meetings. In Proceedings of the 8th SIGdial Workshop on Discourse and Dialogue (pp. 26–34). Antwerp: Association for Computational Linguistics." + Environment.NewLine + Environment.NewLine +
                                                  "Expanded / modified from the original RegEx/Macro version using EXREX (https://github.com/asciimoo/exrex)",
                                                  "ArgLex_",
                                                  Properties.Resources.ArgLex));

            DictionaryList.Add(new DictionaryMetaObject("Analogy/Being Dictionary",
                                                  "Martindale, C. (1990). The clockwork muse:  The predictability of artistic change. New York, NY, US: Basic Books.",
                                                  "ABD_",
                                                  Properties.Resources.Analogy_Being_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Attention Focus Dictionary",
                                                  "Gregson, A. H. C. (2018). Under what conditions does CEO attention influence corporate social performance? The moderating effect of span of control, executive committee gender diversity and industry dynamism. (Master’s Thesis). Universiteit van Amsterdam, Amsterdam Business School. Retrieved from http://www.scriptiesonline.uba.uva.nl/document/667864",
                                                  "AFD_",
                                                  Properties.Resources.Anique_Gregson___Attention_Focus));

            DictionaryList.Add(new DictionaryMetaObject("Body Type Dictionary",
                                                  "Wilson, A. (2006). Development and Application of a Content Analysis Dictionary for Body Boundary Research. Literary and Linguistic Computing, 21(1), 105–110. https://doi.org/10.1093/llc/fqi014" + Environment.NewLine + Environment.NewLine +
                                                  "Fisher, S. (1970). Body Experience in Fantasy and Behavior. Irvington Pub." + Environment.NewLine + Environment.NewLine +
                                                  "Fisher, S., & Fisher, R. L. (1964). Body image boundaries and patterns of body perception. The Journal of Abnormal and Social Psychology, 68(3), 255–262. https://doi.org/10.1037/h0044857",
                                                  "BTD_",
                                                  Properties.Resources.Body_Type_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Brand Personality Dictionary",
                                                  "Opoku, R. A., Hultman, M., & Saheli-Sangari, E. (2008). Positioning in Market Space: The Evaluation of Swedish Universities’ Online Brand Personalities. Journal of Marketing for Higher Education, 18(1), 124–144. https://doi.org/10.1080/08841240802100386",
                                                  "BPD_",
                                                  Properties.Resources.Brand_Personality));

            DictionaryList.Add(new DictionaryMetaObject("Concreteness (via Suffixes)",
                                                  "Mergenthaler, E. (1996). Emotion-abstraction patterns in verbatim protocols: A new way of describing psychotherapeutic processes. Journal of Consulting and Clinical Psychology, 64(6), 1306–1315.",
                                                  "EmAb_",
                                                  Properties.Resources.Concreteness___Mergenthaler_Suffix));

            DictionaryList.Add(new DictionaryMetaObject("Connectives",
                                                  "Ko, W.-J., Durrett, G., & Li, J. J. (2019). Domain Agnostic Real-Valued Specificity Prediction. Proceedings of the AAAI Conference on Artificial Intelligence, 33, 6610–6617. https://doi.org/10.1609/aaai.v33i01.33016610",
                                                  "CD_",
                                                  Properties.Resources.Connectives));

            DictionaryList.Add(new DictionaryMetaObject("Controversial Terms Lexicon",
                                                  "Mejova, Y., Zhang, A. X., Diakopoulos, N., & Castillo, C. (2014). Controversy and sentiment in online news. ArXiv:1409.8152 [Cs]. http://arxiv.org/abs/1409.8152",
                                                  "CTL_",
                                                  Properties.Resources.Mejova_et_al_2014_controversial_words));

            DictionaryList.Add(new DictionaryMetaObject("Corporate Social Responsibility Dictionary",
                                                  "Pencle, N., & Mălăescu, I. (2016). What’s in the words? Development and validation of a multidimensional dictionary for CSR and application using prospectuses. Journal of Emerging Technologies in Accounting, 13(2), 109–127. https://doi.org/10.2308/jeta-51615",
                                                  "CSR_",
                                                  Properties.Resources.Corporate_Social_Responsibility));

            DictionaryList.Add(new DictionaryMetaObject("Empath Default Dictionary",
                                                  "Fast, E., Chen, B., & Bernstein, M. S. (2016). Empath: Understanding Topic Signals in Large-Scale Text. Proceedings of the 2016 CHI Conference on Human Factors in Computing Systems, 4647–4657. https://doi.org/10.1145/2858036.2858535",
                                                  "Empath_",
                                                  Properties.Resources.EmpathDefaultDictionary));

            DictionaryList.Add(new DictionaryMetaObject("English Prime",
                                                  "Kellogg III, E. W., & Bourland Jr., D. D. (1990). Working with E-Prime: Some practical notes. ETC: A Review of General Semantics, 47, 376-392." + Environment.NewLine + Environment.NewLine +
                                                  "Boyd, R. L. (2012). To Be or Not To Be: An Empirical Test of English Prime as Theory (Thesis, North Dakota State University). Retrieved from https://library.ndsu.edu/ir/handle/10365/26556",
                                                  "EP_",
                                                  Properties.Resources.English_Prime));

            DictionaryList.Add(new DictionaryMetaObject("Financial Sentiment Dictionary (2018 Version)",
                                                  "Loughran, T., & Mcdonald, B. (2011). When Is a Liability Not a Liability? Textual Analysis, Dictionaries, and 10-Ks. The Journal of Finance, 66(1), 35–65. https://doi.org/10.1111/j.1540-6261.2010.01625.x" + Environment.NewLine + Environment.NewLine +
                                                  "Loughran, T., & Mcdonald, B. (2016). Textual analysis in accounting and finance: A survey. Journal of Accounting Research, 54(4), 1187–1230. https://doi.org/10.1111/1475-679X.12123" + Environment.NewLine + Environment.NewLine +
                                                  "Bodnaruk, A., Loughran, T., & McDonald, B. (2015). Using 10-K text to gauge financial constraints. Journal of Financial and Quantitative Analysis, 50(4), 623–646. https://doi.org/10.1017/S0022109015000411",
                                                  "FSD_",
                                                  Properties.Resources.FinancialSentimentDictionary2018));

            DictionaryList.Add(new DictionaryMetaObject("Forestry Value Dictionary",
                                                  "Bengston, D. N., & Xu, Z. (1995). Changing National Forest Values: a content analysis. Research Paper NC-323. St. Paul, MN: U.S. Dept. of Agriculture, Forest Service, North Central Forest Experiment Station, 323. https://doi.org/10.2737/NC-RP-323",
                                                  "FVD_",
                                                  Properties.Resources.Forest_Value_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Global Citizen Dictionary",
                                                  "Reysen, S., Pierce, L., Mazambani, G., Mohebpour, I., Puryear, C., Snider, J. S., … Blake, M. E. (2014). Construction and initial validation of a dictionary for global citizen linguistic markers. International Journal of Cyber Behavior, Psychology and Learning, 4(4), 1–15. https://doi.org/10.4018/ijcbpl.2014100101",
                                                  "GCD_",
                                                  Properties.Resources.Global_Citizen_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Grant Evaluation Dictionary",
                                                  "Kaatz, A., Magua, W., Zimmerman, D. R., & Carnes, M. (2015). A quantitative linguistic analysis of National Institutes of Health R01 application critiques from investigators at one institution. Academic Medicine : Journal of the Association of American Medical Colleges, 90(1), 69–75. https://doi.org/10.1097/ACM.0000000000000442",
                                                  "GE_",
                                                  Properties.Resources.GrantEvalDic));

            DictionaryList.Add(new DictionaryMetaObject("Grievance Dictionary",
                                                  "Vegt, I. van der, Kleinberg, B., & Gill, P. (2018). Threat assessment dictionary. https://osf.io/szvm7" + Environment.NewLine + Environment.NewLine + 
                                                  "Note: You will want to stem or lemmatize your texts prior to analysis with this dictionary.",
                                                  "Grievance_",
                                                  Properties.Resources.GrievanceDictionary));

            DictionaryList.Add(new DictionaryMetaObject("Harvard General Inquirer IV",
                                                  "Stone, P. J., Dunphy, D. C., Smith, M. S., & Ogilvie, D. M. (1966). The General Inquirer: A computer approach to content analysis. Cambridge, MA: M.I.T. Press.",
                                                  "HGI_",
                                                  Properties.Resources.Harvard_General_Inquirer_IV___FULL));

            DictionaryList.Add(new DictionaryMetaObject("Hedges",
                                                  "Danescu-Niculescu-Mizil, C., Sudhof, M., Jurafsky, D., Leskovec, J., & Potts, C. (2013). A computational approach to politeness with application to social factors. Proceedings of the 51st Annual Meeting of the Association for Computational Linguistics (Volume 1: Long Papers), 250–259. Retrieved from https://www.aclweb.org/anthology/P13-1025",
                                                  "Hdg_",
                                                  Properties.Resources.Hedges));

            DictionaryList.Add(new DictionaryMetaObject("Hogenraad's Anticipation Sentiment Lexicon",
                                                  "Hogenraad, R. (2019). Fear in the West: A sentiment analysis using a computer-readable “Fear Index.” Quality & Quantity, 53(3), 1239–1261. https://doi.org/10.1007/s11135-018-0813-7",
                                                  "HASL_",
                                                  Properties.Resources.Hogenraad_Anticipation_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Hogenraad's Metamorphosis Lexicon",
                                                  "https://archive.org/details/aeTRANSFIGdlc",
                                                  "HML_",
                                                  Properties.Resources.Hogenraad_Metamorphosis_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Home Perceptions Dictionary",
                                                  "Saxbe, D. E., & Repetti, R. (2010). No Place Like Home: Home Tours Correlate With Daily Patterns of Mood and Cortisol. Personality and Social Psychology Bulletin, 36(1), 71–81. https://doi.org/10.1177/0146167209352864",
                                                  "HPD_",
                                                  Properties.Resources.Home_Perceptions_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Honor Dictionary",
                                                  "Gelfand, M. J., Severance, L., Lee, T., Bruss, C. B., Lun, J., Abdel‐Latif, A., … Ahmed, S. M. (2015). Culture and getting to yes: The linguistic signature of creative agreements in the United States and Egypt. Journal of Organizational Behavior, 36(7), 967–989. https://doi.org/10.1002/job.2026",
                                                  "HD_",
                                                  Properties.Resources.HonorDictionary));

            DictionaryList.Add(new DictionaryMetaObject("NRC Colour Lexicon",
                                                  "Mohammad, S. (2011). Colourful Language: Measuring Word-Colour Associations. Proceedings of the 2nd Workshop on Cognitive Modeling and Computational Linguistics, 97–106. Retrieved from https://www.aclweb.org/anthology/W11-0611" + Environment.NewLine + Environment.NewLine +
                                                  "Mohammad, S. M. (2011). Even the Abstract Have Colour: Consensus in Word-colour Associations. Proceedings of the 49th Annual Meeting of the Association for Computational Linguistics: Human Language Technologies: Short Papers - Volume 2, 368–373. Retrieved from http://dl.acm.org/citation.cfm?id=2002736.2002812",
                                                  "NRC_Colour_",
                                                  Properties.Resources.NRC_Colour));

            DictionaryList.Add(new DictionaryMetaObject("NRC EmoLex v0.92 (English)",
                                                  "Mohammad, S. M., & Turney, P. D. (2010). Emotions Evoked by Common Words and Phrases: Using Mechanical Turk to Create an Emotion Lexicon. Proceedings of the NAACL HLT 2010 Workshop on Computational Approaches to Analysis and Generation of Emotion in Text, 26–34. Retrieved from http://dl.acm.org/citation.cfm?id=1860631.1860635" + Environment.NewLine + Environment.NewLine +
                                                  "Mohammad, S. M., & Turney, P. D. (2013). Crowdsourcing a Word-Emotion Association Lexicon. Computational Intelligence, 29(3), 436–465. https://doi.org/10.1111/j.1467-8640.2012.00460.x",
                                                  "NRC_EmoLex_",
                                                  Properties.Resources.EmoLex092_English));

            DictionaryList.Add(new DictionaryMetaObject("Policy Position Dictionary",
                                                  "Laver, M., Benoit, K., & Garry, J. (2003). Extracting Policy Positions from Political Texts Using Words as Data. American Political Science Review, 97(2), 311–331. https://doi.org/10.1017/S0003055403000698",
                                                  "PPD_",
                                                  Properties.Resources.Laver_Garry_Dictionary_of_Policy_Position));

            DictionaryList.Add(new DictionaryMetaObject("Linguistic Category Model",
                                                  "Seih, Y.-T., Beier, S., & Pennebaker, J. W. (2017). Development and Examination of the Linguistic Category Model in a Computerized Text Analysis Method. Journal of Language and Social Psychology, 36(3), 343–355. https://doi.org/10.1177/0261927X16657855",
                                                  "LCM_",
                                                  Properties.Resources.LCM));

            DictionaryList.Add(new DictionaryMetaObject("Lexicoder Topic Dictionary",
                                                  "Quinn, A., Sevenans, J., & Soroka, S. (2013). Lexicoder Topic Dictionaries. Montreal, Canada: McGill University. Retrieved from http://www.lexicoder.com/download.html",
                                                  "LTD_",
                                                  Properties.Resources.Lexicoder_Topic_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("LUSI Action-Inaction Dictionary",
                                                  "https://www.depts.ttu.edu/psy/lusi/resources.php",
                                                  "LUSI_",
                                                  Properties.Resources.LUSI___Action_Inaction_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("LUSI Disengagement Dictionary",
                                                  "https://www.depts.ttu.edu/psy/lusi/resources.php",
                                                  "LUSI_",
                                                  Properties.Resources.LUSI___Disengagement_Dictionary___Modified));

            DictionaryList.Add(new DictionaryMetaObject("LUSI Netspeak Dictionary",
                                                  "https://www.depts.ttu.edu/psy/lusi/resources.php",
                                                  "LUSI_",
                                                  Properties.Resources.LUSI___Netspeak_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("LUSI Risky Sex Dictionary",
                                                  "https://www.depts.ttu.edu/psy/lusi/resources.php",
                                                  "LUSI_",
                                                  Properties.Resources.LUSI___Risky_Sex_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Invective Dictionary",
                                                  "Panter, A. T. (2017). Invective language in course evaluations [Dictionary]. Chapel Hill, NC: The University of North Carolina at Chapel Hill.",
                                                  "InvD_",
                                                  Properties.Resources.A__T__Panter___Invectives));

            DictionaryList.Add(new DictionaryMetaObject("Pornographic Language Dictionary",
                                                  "Arenas, E. S. (2018). Exploring Pornography in Widad Benmoussa’s Poetry Using LIWC and Corpus Tools. Sexuality & Culture, 22(4), 1094–1111. https://doi.org/10.1007/s12119-018-9513-7",
                                                  "PLD_",
                                                  Properties.Resources.Encarnacion_S__Arenas___LIWC___Essam_Porn_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Mind Perception Dictionary",
                                                  "Schweitzer, S., & Waytz, A. (2020). Language as a window into mind perception: How mental state language differentiates body and mind, human and nonhuman, and the self from others. Journal of Experimental Psychology: General. Advance online publication. https://doi.org/10.1037/xge0001013",
                                                  "MPD_",
                                                  Properties.Resources.Mind_Perception_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Mindfulness Dictionary",
                                                  "Collins, S. E., Chawla, N., Hsu, S. H., Grow, J., Otto, J. M., & Marlatt, G. A. (2009). Language-based Measures of Mindfulness: Initial Validity and Clinical Utility. Psychology of Addictive Behaviors : Journal of the Society of Psychologists in Addictive Behaviors, 23(4), 743–749. https://doi.org/10.1037/a0017579",
                                                  "MnD_",
                                                  Properties.Resources.Mindfulness_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Moral Foundations Dictionary",
                                                  "Graham, J., Haidt, J., & Nosek, B. A. (2009). Liberals and conservatives rely on different sets of moral foundations. Journal of Personality and Social Psychology, 96(5), 1029–1046. https://doi.org/10.1037/a0015141",
                                                  "MFD_",
                                                  Properties.Resources.Moral_Foundations_Dictionary__Fixed_Formatting_));

            DictionaryList.Add(new DictionaryMetaObject("Moral Foundations Dictionary v2.0",
                                                  "Frimer, J. A. (2019). Do liberals and conservatives use different moral languages? Two replications and six extensions of Graham, Haidt, and Nosek’s (2009) moral text analysis. Journal of Research in Personality, 103906. https://doi.org/10.1016/j.jrp.2019.103906" + Environment.NewLine + Environment.NewLine +
                                                  "http://www.jeremyfrimer.com/uploads/2/1/2/7/21278832/summary.pdf",
                                                  "MFD2_",
                                                  Properties.Resources.mfd2_0));

            DictionaryList.Add(new DictionaryMetaObject("Moral Justification Dictionary",
                                                  "Wheeler, M. A., & Laham, S. M. (2016). What we talk about when we talk about morality: Deontological, consequentialist, and emotive language use in justifications across foundation-specific moral violations. Personality and Social Psychology Bulletin, 42(9), 1206–1216. https://doi.org/10.1177/0146167216653374",
                                                  "MJD_",
                                                  Properties.Resources.Moral_Justification_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("MPQA Subjectivity Lexicon",
                                                  "Wilson, T., Wiebe, J., & Hoffmann, P. (2005). Recognizing Contextual Polarity in Phrase-level Sentiment Analysis. Proceedings of the Conference on Human Language Technology and Empirical Methods in Natural Language Processing, 347–354. https://doi.org/10.3115/1220575.1220619",
                                                  "MPQA_",
                                                  Properties.Resources.MPQA_Subjectivity_Lexicon));

            DictionaryList.Add(new DictionaryMetaObject("Opinion Lexicon",
                                                  "Hu, M., & Liu, B. (2004). Mining and Summarizing Customer Reviews. In Proceedings of the Tenth ACM SIGKDD International Conference on Knowledge Discovery and Data Mining (pp. 168–177). New York, NY, USA: ACM. https://doi.org/10.1145/1014052.1014073",
                                                  "OL_",
                                                  Properties.Resources.Opinion_Lexicon));

            DictionaryList.Add(new DictionaryMetaObject("Pain Dictionary",
                                                  "Wright, R. C., Junghaenel, D. U., Rivas, R., Hristidis, V., & Robbins, M. L. (2020). A new approach to capturing pain disclosure in daily life in-person and online. Journal of Health Psychology, 1359105320918322. https://doi.org/10.1177/1359105320918322",
                                                  "Pain_",
                                                  Properties.Resources.Pain_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("PANAS-X Dictionary",
                                                  "Watson, D., & Clark, L. A. (1994). The PANAS-X: Manual for the positive and negative affect schedule-expanded form. Iowa City, IA: The University of Iowa.",
                                                  "PXD_",
                                                  Properties.Resources.PANASX));

            DictionaryList.Add(new DictionaryMetaObject("Personal Values Dictionary",
                                                  "Ponizovskiy, V., Ardag, M., Grigoryan, L., Boyd, R., Dobewall, H., & Holtz, P. (in press). Development and validation of the Personal Values Dictionary: A theory-driven tool for investigating references to basic human values in text. European Journal of Personality. https://doi.org/10.1002/per.2294",
                                                  "PVD_",
                                                  Properties.Resources.Personal_Values_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Privacy Dictionary",
                                                  "Vasalou, A., Gill, A. J., Mazanderani, F., Papoutsi, C., & Joinson, A. (2011). Privacy dictionary: A new resource for the automated content analysis of privacy. Journal of the American Society for Information Science and Technology, 62(11), 2095–2105. https://doi.org/10.1002/asi.21610",
                                                  "PRV_",
                                                  Properties.Resources.Privacy_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Prorefugee Dictionary",
                                                  "Smith, L. G. E., McGarty, C., & Thomas, E. F. (2018). After Aylan Kurdi: How Tweeting About Death, Threat, and Harm Predict Increased Expressions of Solidarity With Refugees Over Time. Psychological Science, 29(4), 623–634. https://doi.org/10.1177/0956797617741107",
                                                  "PRD_",
                                                  Properties.Resources.Prorefugee_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Prosocial Dictionary",
                                                  "Frimer, J. A., Aquino, K., Gebauer, J. E., Zhu, L., & Oakes, H. (2015). A decline in prosocial language helps explain public disapproval of the US Congress. Proceedings of the National Academy of Sciences, 112(21), 6591–6594. https://doi.org/10.1073/pnas.1500355112",
                                                  "PrSc_",
                                                  Properties.Resources.Prosocial_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Psychodynamic Conflict",
                                                  "Dent, L., & Christian, C. (2019). The shifting prevalence of conflict in psychoanalytic literature: A brief report of a corpus-based text analysis. Psychoanalytic Psychology, 36(2), 184–188. https://doi.org/10.1037/pap0000220",
                                                  "PC_",
                                                  Properties.Resources.PsychodynamicConflict));

            DictionaryList.Add(new DictionaryMetaObject("Regressive Imagery Dictionary",
                                                  "Martindale, C. (1975). The grammar of altered states of consciousness: A semiotic reinterpretation of aspects of psychoanalytic theory. Psychoanalysis and Contemporary Thought, 4, 331–354." + Environment.NewLine + Environment.NewLine +
                                                  "Martindale, C. (1975). Romantic progression: The psychology of literary history. Washington, D.C.: Hemisphere." + Environment.NewLine + Environment.NewLine +
                                                  "Martindale, C. (1990). The clockwork muse: The predictability of artistic change. New York: Basic Books." + Environment.NewLine + Environment.NewLine +
                                                  "Martindale, C. (2007). Creativity, primordial cognition, and personality. Personality and Individual Differences, 43(7), 1777–1785. http://dx.doi.org/10.1016/j.paid.2007.05.014",
                                                  "RID_",
                                                  Properties.Resources.Regressive_Imagery_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Regulatory Mode Dictionary",
                                                  "Kanze, D., Conley, M. A., & Higgins, E. T. (2019). The motivation of mission statements: How regulatory mode influences workplace discrimination. Organizational Behavior and Human Decision Processes. https://doi.org/10.1016/j.obhdp.2019.04.002",
                                                  "RM_",
                                                  Properties.Resources.Regulatory_Mode_Kanze_Conley_Higgins));

            DictionaryList.Add(new DictionaryMetaObject("Schwartz Values Dictionary",
                                                  "Jones, K. L., Noorbaloochi, S., Jost, J. T., Bonneau, R., Nagler, J., & Tucker, J. A. (2018). Liberal and Conservative Values: What We Can Learn From Congressional Tweets. Political Psychology, 39(2), 423–443. https://doi.org/10.1111/pops.12415",
                                                  "SVS_",
                                                  Properties.Resources.SchwartzValuesDict));

            DictionaryList.Add(new DictionaryMetaObject("Self-Determination Theory Dictionary",
                                                  "Oliver, E. J., Markland, D., Hardy, J., & Petherick, C. M. (2008). The effects of autonomy-supportive versus controlling environments on self-talk. Motivation and Emotion, 32(3), 200–212. https://doi.org/10.1007/s11031-008-9097-x",
                                                  "SDT_",
                                                  Properties.Resources.SDTheory_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Security Lexicon",
                                                  "Baele, S. J., & Sterck, O. C. (2015). Diagnosing the Securitisation of Immigration at the EU Level: A New Method for Stronger Empirical Claims. Political Studies, 63(5), 1120–1139. https://doi.org/10.1111/1467-9248.12147",
                                                  "SecLex_",
                                                  Properties.Resources.Security_Lexicon));

            DictionaryList.Add(new DictionaryMetaObject("Self-care Dictionary",
                                                  "Wang, X., Parameswaran, S., Bagul, D., & Kishore, R. (2017). Does Online Social Support Work in Stigmatized Chronic Diseases? A Study of the Impacts of Different Facets of Informational and Emotional Support on Self-Care Behavior in an HIV Online Forum. ICIS 2017 Proceedings. Retrieved from https://aisel.aisnet.org/icis2017/General/Presentations/22" + Environment.NewLine + Environment.NewLine +
                                                  "Wang, X., Parameswaran, S., Bagul, D. M., & Kishore, R. (2018). Can online social support be detrimental in stigmatized chronic diseases? A quadratic model of the effects of informational and emotional support on self-care behavior of HIV patients. Journal of the American Medical Informatics Association: JAMIA, 25(8), 931–944. https://doi.org/10.1093/jamia/ocy012",
                                                  "SC_",
                                                  Properties.Resources.Self_Care_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Self-Transcendent Emotion Dictionary",
                                                  "Ji, Q., & Raney, A. A. (2020). Developing and validating the self-transcendent emotion dictionary for text analysis. PLOS ONE, 15(9), e0239050. https://doi.org/10.1371/journal.pone.0239050",
                                                  "STED_",
                                                  Properties.Resources.Ji_Raney_Self_Transcendent_Emotion_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Situational Eight Dictionary",
                                                  "Serfass, D. G., & Sherman, R. A. (2015). Situations in 140 characters: Assessing real-world situations on Twitter. PLoS ONE, 10(11). https://doi.org/10.1371/journal.pone.0143051",
                                                  "S8_",
                                                  Properties.Resources.Situational8));

            DictionaryList.Add(new DictionaryMetaObject("Social Ties Dictionary",
                                                  "Pressman, S. D., & Cohen, S. (2007). Use of social words in autobiographies and longevity. Psychosomatic Medicine, 69(3), 262–269.",
                                                  "ST_",
                                                  Properties.Resources.Social_Ties_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Stress Dictionary",
                                                  "Wang, W., Hernandez, I., Newman, D. A., He, J., & Bian, J. (2016). Twitter analysis: Studying US weekly trends in work stress and emotion. Applied Psychology, 65(2), 355–378. https://doi.org/10.1111/apps.12065",
                                                  "Str_",
                                                  Properties.Resources.Wang_et_al___2016____Stress_Dictionary));

            DictionaryList.Add(new DictionaryMetaObject("Value Lexicon",
                                                  "Bardi, A., Calogero, R. M., & Mullen, B. (2008). A new archival approach to the study of values and value--Behavior relations: Validation of the value lexicon. Journal of Applied Psychology, 93(3), 483–497. https://doi.org/10.1037/0021-9010.93.3.483",
                                                  "VLB_",
                                                  Properties.Resources.ValueLexiconBardi));

            DictionaryList.Add(new DictionaryMetaObject("Well-being Dictionary",
                                                  "Ratner, K., Burrow, A. L., Burd, K. A., & Hill, P. (2017). On the conflation of purpose and meaning in life: A qualitative study of high school and college student conceptions. https://doi.org/10.17605/OSF.IO/X9Q6N",
                                                  "WBD_",
                                                  Properties.Resources.Well_being_Dictionary));

            //This dictionary is kind of a mess. Documentation doesn't really provide enough info to make is usable.
            DictionaryList.Add(new DictionaryMetaObject("Values Lexicon v1.0",
                                                  "Wilson, S. R., Shen, Y., & Mihalcea, R. (2018). Building and Validating Hierarchical Lexicons with a Case Study on Personal Values. In S. Staab, O. Koltsova, & D. I. Ignatov (Eds.), Social Informatics (pp. 455–470). Springer International Publishing.",
                                                  "ValLex_",
                                                  Properties.Resources.ValuesLexiconWilson));

            DictionaryList.Add(new DictionaryMetaObject("Whirlall Dictionary",
                                                  "Thomas, C. B., & Duszynski, K. R. (1985). Are words of the Rorschach predictors of disease and death? The case of “whirling.” Psychosomatic Medicine, 47(2), 201–211.",
                                                  "Whl_",
                                                  Properties.Resources.Whirlall_Words));




            foreach(DictionaryMetaObject dict in DictionaryList)
            {
                ListOfBuiltInDictionaries.Add(dict.DictionaryName);
            }

        }




        public Payload FinishUp(Payload Input)
        {
            return (Input);
        }





        #region Import/Export Settings
        public void ImportSettings(Dictionary<string, string> SettingsDict)
        {
            RawFreqs = Boolean.Parse(SettingsDict["RawFreqs"]);

            foreach(DictionaryMetaObject dict in DictionaryList)
            {
                if (SettingsDict.ContainsKey(XmlConvert.EncodeName(dict.DictionaryName)))
                {
                    dict.UseDictionary = Boolean.Parse(SettingsDict[XmlConvert.EncodeName(dict.DictionaryName)]);
                }
                else
                {
                    dict.UseDictionary = false;
                }
            }

        }


        public Dictionary<string, string> ExportSettings(bool suppressWarnings)
        {
            Dictionary<string, string> SettingsDict = new Dictionary<string, string>();
            SettingsDict.Add("RawFreqs", RawFreqs.ToString());
            bool UsingCustomDictionary = false;

            foreach(DictionaryMetaObject dict in DictionaryList)
            {
                if (ListOfBuiltInDictionaries.Contains(dict.DictionaryName))
                {
                    SettingsDict.Add(XmlConvert.EncodeName(dict.DictionaryName), dict.UseDictionary.ToString());
                }
                else
                {
                    //we only show this message if the user has loaded in a custom dictionary
                    if (!UsingCustomDictionary)
                    {
                        UsingCustomDictionary = true;
                        if (!suppressWarnings) MessageBox.Show("Currently, the \"" + PluginName + "\" plugin does not store custom user dictionaries when exported as part of a pipeline. When you load this pipeline later, you will need to reload any custom dictionaries that you are currently using with this plugin. This feature may be added to a later version of the plugin.", "Pipeline Save Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
               
            }
            
            return (SettingsDict);

            
        }
        #endregion






    }
}

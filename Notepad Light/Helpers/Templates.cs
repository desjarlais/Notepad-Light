using System.Text;

namespace Notepad_Light.Helpers
{
    /// <summary>
    /// the purpose of the class is to have an in memory representation of the template files
    /// then, when it comes time to add or edit the template text, we can avoid file i/o
    /// </summary>
    public static class Templates
    {
        static StringBuilder sbTemplate1 = new StringBuilder();
        static StringBuilder sbTemplate2 = new StringBuilder();
        static StringBuilder sbTemplate3 = new StringBuilder();
        static StringBuilder sbTemplate4 = new StringBuilder();
        static StringBuilder sbTemplate5 = new StringBuilder();

        public static StringBuilder GetTemplate1()
        {
            return sbTemplate1;
        }

        public static StringBuilder GetTemplate2()
        {
            return sbTemplate2;
        }

        public static StringBuilder GetTemplate3()
        {
            return sbTemplate3;
        }

        public static StringBuilder GetTemplate4()
        {
            return sbTemplate4;
        }

        public static StringBuilder GetTemplate5()
        {
            return sbTemplate5;
        }

        public static void SetTemplate1(string input)
        {
            sbTemplate1.Clear();
            sbTemplate1.Append(input);
        }

        public static void SetTemplate2(string input)
        {
            sbTemplate2.Clear();
            sbTemplate2.Append(input);
        }

        public static void SetTemplate3(string input)
        {
            sbTemplate3.Clear();
            sbTemplate3.Append(input);
        }

        public static void SetTemplate4(string input)
        {
            sbTemplate4.Clear();
            sbTemplate4.Append(input);
        }

        public static void SetTemplate5(string input)
        {
            sbTemplate5.Clear();
            sbTemplate5.Append(input);
        }

        public static void WriteTemplatesToFile()
        {
            File.WriteAllText(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template1 + Strings.txtExt, sbTemplate1.ToString());
            File.WriteAllText(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template2 + Strings.txtExt, sbTemplate2.ToString());
            File.WriteAllText(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template3 + Strings.txtExt, sbTemplate3.ToString());
            File.WriteAllText(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template4 + Strings.txtExt, sbTemplate4.ToString());
            File.WriteAllText(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template5 + Strings.txtExt, sbTemplate5.ToString());
        }

        public static void UpdateTemplatesFromFiles()
        {
            int templateNum = 0;

            try
            {
                sbTemplate1.Append(File.ReadAllText(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template1 + Strings.txtExt));
                templateNum++;
                sbTemplate2.Append(File.ReadAllText(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template2 + Strings.txtExt));
                templateNum++;
                sbTemplate3.Append(File.ReadAllText(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template3 + Strings.txtExt));
                templateNum++;
                sbTemplate4.Append(File.ReadAllText(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template4 + Strings.txtExt));
                templateNum++;
                sbTemplate5.Append(File.ReadAllText(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template5 + Strings.txtExt));
            }
            catch (FileNotFoundException)
            {
                switch (templateNum)
                {
                    case 0:
                        File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template1 + Strings.txtExt);
                        break;
                    case 1:
                        File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template2 + Strings.txtExt);
                        break;
                    case 2:
                        File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template3 + Strings.txtExt);
                        break;
                    case 3:
                        File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template4 + Strings.txtExt);
                        break;
                    case 4:
                        File.Create(Strings.appFolderTemplatesFullPath + Properties.Settings.Default.Template5 + Strings.txtExt);
                        break;
                }
            }
        }
    }
}

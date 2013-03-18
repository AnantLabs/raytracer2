using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace _3dEditor
{
    public partial class Help : Form
    {
        private List<TabControl> tabs;
        public Help()
        {
            InitializeComponent();

            tabs = new List<TabControl>();
            tabs.Add(tabMenu);
            tabs.Add(tabBoard);
            tabs.Add(tabScene);
            tabs.Add(tabProperties);
            tabs.Add(tabAbout);

            foreach (TabControl tab in tabs)
            {
                tab.Dock = DockStyle.Fill;
            }

            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = String.Format("Product: {0}", AssemblyProduct);
            this.labelVersion.Text = String.Format("Version: {0}", AssemblyVersion);
            this.labelCompanyName.Text = String.Format("Author: {0}", AssemblyCompany);
            this.textBoxDescription.Text = AssemblyDescription;

            this.labelReleaseDate.Text = String.Format("Release Date: {0}", "April 2013");
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void ShowTab(TabControl tab)
        {
            foreach (TabControl t in tabs)
            {
                t.Visible = false;
            }
            tab.BringToFront();
            tab.Visible = true;
        }

        private void onShowMenu(object sender, EventArgs e)
        {
            ShowTab(tabMenu);
        }

        private void onShowBoard(object sender, EventArgs e)
        {
            ShowTab(tabBoard);
        }

        private void onShowScene(object sender, EventArgs e)
        {
            ShowTab(tabScene);
        }

        private void onShowProperties(object sender, EventArgs e)
        {
            ShowTab(tabProperties);
        }

        private void onAbout(object sender, EventArgs e)
        {
            ShowTab(tabAbout);
        }
    }
}

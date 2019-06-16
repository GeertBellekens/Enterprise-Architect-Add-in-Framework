using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using EADatabaseTransformer;
using EAWrappers = TSF.UmlToolingFramework.Wrappers.EA;


namespace EAAddinTester
{
    static class EAAddinTesterProgram
    {
        // the addins we are testing
        private static List<EAAddinFramework.EAAddinBase> addins = new List<EAAddinFramework.EAAddinBase>();

        // reference to currently opened EA repository
        internal static EA.Repository eaRepository;
        // the tester form
        private static EAAddinTesterForm form;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            addAddIns();
            eaRepository = getOpenedModel();
            if (eaRepository != null)
            {
                initializeAddins();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                form = new EAAddinTesterForm();
                Application.Run(form);
            }
        }
        private static void addAddIns()
        {
            //addins.Add(new MyAddin.MyAddinClass());
            //addins.Add(new TSF.UmlToolingFramework.EANavigator.EAAddin());
            //addins.Add(new EAWorksetSharing.EAWorksetSharingAddin());
            //addins.Add(new EAScriptAddin.EAScriptAddinAddinClass());
            //addins.Add(new ECDMMessageComposer.ECDMMessageComposerAddin());
            //addins.Add(new EADatabaseTransformer.EADatabaseTransformerAddin{debugMode = true});
            //addins.Add(new GlossaryManager.GlossaryManagerAddin());
            addins.Add(new EAJSON.EAJSONAddin());
        }
        private static void initializeAddins()
        {
            foreach (EAAddinFramework.EAAddinBase addin in addins)
            {
                addin.EA_FileOpen(eaRepository);
                addin.EA_OnPostInitialized(eaRepository);
            }
        }
        internal static void SetMenu(string location, ToolStripMenuItem addinMenu, string menuName)
        {
            // first remove dropdownItems
            addinMenu.DropDownItems.Clear();
            // then assign new items
            foreach (EAAddinFramework.EAAddinBase addin in addins)
            {
                SetMenu(location, addinMenu, menuName, addin);
            }
        }
        /// <summary>
        /// gets the menu items from the addin
        /// </summary>
        /// <param name="location">the location in EA</param>
        /// <param name="addinMenu">the menu where to add the items</param>
        /// <param name="menuName">the name of the menu</param>
        internal static void SetMenu(string location, ToolStripMenuItem addinMenu, string menuName, EAAddinFramework.EAAddinBase addin)
        {

            object menuItemsObject = addin.EA_GetMenuItems(eaRepository, location, menuName);
            string[] menuItems = null;
            // check if menuItemsObject is an array of strings
            if (menuItemsObject is string[])
            {
                menuItems = (string[])menuItemsObject;
            }
            else
            //must be a string then
            {
                menuItems = new string[1];
                menuItems[0] = (string)menuItemsObject;
            }

            foreach (string menuItem in menuItems)
            {
                if (menuItem != null)
                {
                    // if the menuItem starts with a "-" then it has submenu items
                    if (menuItem.StartsWith("-"))
                    {
                        // remove the "-";
                        string menuItemName = menuItem;
                        menuItemName = menuItem.Substring(1);
                        // add the menu item
                        addinMenu.DropDownItems.Add(menuItemName);
                        //get the newly added item
                        ToolStripMenuItem newMenuItem = (ToolStripMenuItem)addinMenu.DropDownItems[addinMenu.DropDownItems.Count - 1];
                        //add the eventhandler for its subItems
                        newMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(form.addInsToolStripMenuItem_DropDownItemClicked);
                        // add its submenu items
                        SetMenu(location, newMenuItem, menuItem, addin);
                    }
                    // else it is a leaf menu item
                    else
                    {
                        // add the menu item
                        addinMenu.DropDownItems.Add(menuItem);
                        // get the newly added item
                        ToolStripMenuItem newMenuItem = (ToolStripMenuItem)addinMenu.DropDownItems[addinMenu.DropDownItems.Count - 1];
                        //set its state, only leaf items get their state set.
                        bool enabledValue = true;
                        bool checkedValue = false;
                        addin.EA_GetMenuState(eaRepository, location, newMenuItem.OwnerItem.Text, newMenuItem.Text, ref enabledValue, ref checkedValue);
                        newMenuItem.Enabled = enabledValue;
                        newMenuItem.Checked = checkedValue;
                    }
                }
            }

        }
        /// <summary>
        /// Menu is clicked, forward to addin
        /// </summary>
        /// <param name="location">the location within EA</param>
        /// <param name="menuName">the name of the menu</param>
        /// <param name="itemName">the name of the clicked item</param>
        internal static void clickMenu(string location, string menuName, string itemName)
        {
            foreach (EAAddinFramework.EAAddinBase addin in addins)
            {
                addin.EA_MenuClick(eaRepository, location, menuName, itemName);
            }
        }
        /// <summary>
        /// generic test method. To be filled in with whatever needs to be tested.
        /// </summary>
        internal static void myTest()
        {
            EAWrappers.Model model = new EAWrappers.Model();
            string fqn = model.selectedItem.fqn;
        }
        /// <summary>
        /// Gets the Repository object from the currently running instance of EA.
        /// If multiple instances are running it returns the first one opened.
        /// </summary>
        /// <returns>Repository object for the running instance of EA</returns>
        private static EA.Repository getOpenedModel()
        {
            try
            {

                return ((EA.App)Marshal.GetActiveObject("EA.App")).Repository;

            }
            catch (COMException)
            {
                DialogResult result = MessageBox.Show("Could not find running instance of EA.\nStart EA and try again"
                                   , "EA not running", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Retry)
                {
                    return getOpenedModel();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

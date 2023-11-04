using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using EADatabaseTransformer;
using EAWrappers = TSF.UmlToolingFramework.Wrappers.EA;
using EAScriptAddin;
using EAAddinFramework.EASpecific;
using System.Reflection;
using EAAddinFramework.Utilities;

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
            mainEAScriptAddinSettingForm();
        }

        private static void mainEAScriptAddinSettingForm()
        {
            List<MethodInfo> operations = new List<MethodInfo>();
            List<ScriptFunction> functions = new List<ScriptFunction>();
            List<Script> scripts = new List<Script>();
            EAScriptAddinAddinClass scriptAddin = new EAScriptAddinAddinClass();
            EAScriptAddinSettingForm form = new EAScriptAddinSettingForm(operations, functions, scripts, scriptAddin);
            Application.Run(form);
        }

        private static void mainEAAddinTesterForm()
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
            addins.Add(new SAP2EAImporter.SAP2EAImporterAddin());
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
        internal static void myTest(string command, string arguments)
        {
            //var progresBarWindows = new ProgressBarWindow();
            //progresBarWindows.execute(command, arguments, "executing command title", "currently running this command", false);
            EAWrappers.Model model = new EAWrappers.Model();
            var sqlQuery = "select top 1000 * from t_object o order by o.Object_ID desc";
            Logger.log("Before DatasetFromQuery");
            var dataset1 = model.getDataSetFromQuery(sqlQuery, false);
            Logger.log("After DatasetFromQuery");
            var dummy = model.connection;//to force initialisation of the database connections
            Logger.log ("Before DatasetFromQuery2");
            var dataset2 = model.getDataSetFromQuery2(sqlQuery, false);
            Logger.log ("After DatasetFromQuery2");
            //var selectedPackage = model.selectedTreePackage as EAWrappers.Package;
            //var outputName = "EATester";
            ////test regular getting all elements
            ////EAOutputLogger.clearLog(model, outputName);
            ////EAOutputLogger.log(model, outputName, $"starting regular test for package '{selectedPackage?.name}'", 0);
            ////var elements = selectedPackage.getAllOwnedElements();
            ////EAOutputLogger.log(model, outputName, $"found {elements.Count} new elements", 0);
            //var connector = model.getRelationByGUID("{B34E3FAE-B047-46be-B4BD-EFA5D1DD1B27}");
            //var sourceEnd = connector.sourceEnd;
            //foreach(var tag in sourceEnd.taggedValues)
            //{
            //    EAOutputLogger.log(model, outputName, $"found {tag.name} with value {tag.tagValue} ", 0);
            //    tag.tagValue = "12";
            //    tag.save();
            //}

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

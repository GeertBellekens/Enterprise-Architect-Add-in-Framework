using System;
using System.Windows.Forms;

namespace MyAddin
{
	public class MyAddinClass : EAAddinFramework.EAAddinBase
    {
        // define menu constants
        const string menuName = "-&MyAddin";
        const string menuHello = "&Say Hello";
        const string menuGoodbye = "&Say Goodbye";
        
        // remember if we have to say hello or goodbye
        private bool shouldWeSayHello = true;
        
        /// <summary>
        /// constructor where we set the menuheader and menuOptions
        /// </summary>
		public MyAddinClass():base()
		{
			this.menuHeader = menuName;
			this.menuOptions = new string[]{menuHello,menuGoodbye};
		}

        /// <summary>
        /// Called once Menu has been opened to see what menu items should active.
        /// </summary>
        /// <param name="Repository">the repository</param>
        /// <param name="Location">the location of the menu</param>
        /// <param name="MenuName">the name of the menu</param>
        /// <param name="ItemName">the name of the menu item</param>
        /// <param name="IsEnabled">boolean indicating whethe the menu item is enabled</param>
        /// <param name="IsChecked">boolean indicating whether the menu is checked</param>
        public override void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(Repository))
            {
                switch (ItemName)
                {
                    // define the state of the hello menu option
                    case menuHello:
                        IsEnabled = shouldWeSayHello;
                        break;
                    // define the state of the goodbye menu option
                    case menuGoodbye:
                        IsEnabled = !shouldWeSayHello;
                        break;
                    // there shouldn't be any other, but just in case disable it.
                    default:
                        IsEnabled = false;
                        break;
                }
            }
            else
            {
                // If no open project, disable all menu options
                IsEnabled = false;
            }
        }

        /// <summary>
        /// Called when user makes a selection in the menu.
        /// This is your main exit point to the rest of your Add-in
        /// </summary>
        /// <param name="Repository">the repository</param>
        /// <param name="Location">the location of the menu</param>
        /// <param name="MenuName">the name of the menu</param>
        /// <param name="ItemName">the name of the selected menu item</param>
        public override void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            switch (ItemName)
            {
                // user has clicked the menuHello menu option
                case menuHello:
                    this.sayHello();
                    break;
                // user has clicked the menuGoodbye menu option
                case menuGoodbye:
                    this.sayGoodbye();
                    break;
            }
        }

        /// <summary>
        /// Say Hello to the world
        /// </summary>
        private void sayHello()
        {
            MessageBox.Show("Hello World");
            this.shouldWeSayHello = false;
        }

        /// <summary>
        /// Say Goodbye to the world
        /// </summary>
        private void sayGoodbye()
        {
            MessageBox.Show("Goodbye World");
            this.shouldWeSayHello = true;
        }
    }
}

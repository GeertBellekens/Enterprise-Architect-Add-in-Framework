using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAAddinFramework.Utilities
{
    [TypeDescriptionProvider(typeof(AbstractControlDescriptionProvider<AddinSettingsFormBase, Form>))]
    public abstract class AddinSettingsFormBase: Form
    {
        public AddinSettings settings { get; set; }
        protected AddinSettingsFormBase(AddinSettings settings)
        {
            this.settings = settings;
        }
        /// <summary>
        /// refreshes the contents of the settings form
        /// </summary>
        public abstract void refreshContents();
        
    }
    /// <summary>
    /// this class is needed to be able to be able to design forms inheriting from my abstract form
    /// from https://stackoverflow.com/questions/1620847/how-can-i-get-visual-studio-2008-windows-forms-designer-to-render-a-form-that-im
    /// </summary>
    /// <typeparam name="TAbstract"></typeparam>
    /// <typeparam name="TBase"></typeparam>
    public class AbstractControlDescriptionProvider<TAbstract, TBase> : TypeDescriptionProvider
    {
        public AbstractControlDescriptionProvider()
            : base(TypeDescriptor.GetProvider(typeof(TAbstract)))
        {
        }

        public override Type GetReflectionType(Type objectType, object instance)
        {
            if (objectType == typeof(TAbstract))
                return typeof(TBase);

            return base.GetReflectionType(objectType, instance);
        }

        public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
        {
            if (objectType == typeof(TAbstract))
                objectType = typeof(TBase);

            return base.CreateInstance(provider, objectType, argTypes, args);
        }
    }
}

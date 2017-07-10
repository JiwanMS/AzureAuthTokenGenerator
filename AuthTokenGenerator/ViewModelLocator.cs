namespace AuthTokenGenerator
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Ioc;

    using Microsoft.Practices.ServiceLocation;

    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<EnvViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EnvViewModel Env
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EnvViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}

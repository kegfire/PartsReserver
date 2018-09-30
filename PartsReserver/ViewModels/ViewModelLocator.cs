using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace PartsReserver.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
			SimpleIoc.Default.Register<MainPageViewModel>();
	        SimpleIoc.Default.Register<SettingsViewModel>();
		}

		public MainPageViewModel MainPage => ServiceLocator.Current.GetInstance<MainPageViewModel>();

	    public SettingsViewModel SettingsPage => ServiceLocator.Current.GetInstance<SettingsViewModel>();


		public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
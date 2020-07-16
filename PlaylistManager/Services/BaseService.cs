using PlaylistManager.Utilities;

namespace PlaylistManager.Services
{
    abstract public class BaseService
    {

        protected Settings _settings;

        public BaseService()
        {
            ISettingsFileAccess fileAccess = new SettingsFileAccess();
            SettingsFacade facade = new SettingsFacade(fileAccess);
            _settings = facade.GetSettings();
        }

    }
}

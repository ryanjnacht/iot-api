using System.Linq;
using iot_api.Repository;

namespace iot_api.Security
{
    public static class AccessKeyHelper
    {
        private static bool UseAccessKeys =>
            Configuration.Configuration.SecurityEnabled && AccessKeyRepository.Get().Any();

        public static bool CanAdminAccessKeys(string accessKey)
        {
            if (!UseAccessKeys)
                return true;

            if (string.IsNullOrEmpty(accessKey))
                return false;

            var accessKeyObj = AccessKeyRepository.Get(accessKey);
            return accessKeyObj != null && accessKeyObj.IsAdmin();
        }

        public static bool CanAccessDevices(string accessKey)
        {
            if (!UseAccessKeys)
                return true;

            if (string.IsNullOrEmpty(accessKey))
                return false;

            var accessKeyObj = AccessKeyRepository.Get(accessKey);
            return accessKeyObj != null && accessKeyObj.IsAdmin();
        }

        public static bool CanAccessDevice(string accessKey, string deviceId)
        {
            if (!UseAccessKeys)
                return true;

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(deviceId))
                return false;

            var accessKeyObj = AccessKeyRepository.Get(accessKey);
            if (accessKeyObj == null)
                return false;

            return accessKeyObj.CanAccessDevice(deviceId);
        }

        public static bool CanAccessWorkflows(string accessKey)
        {
            if (!UseAccessKeys)
                return true;

            if (string.IsNullOrEmpty(accessKey))
                return false;

            var accessKeyObj = AccessKeyRepository.Get(accessKey);
            return accessKeyObj != null && accessKeyObj.IsAdmin();
        }

        public static bool CanAccessWorkflow(string accessKey, string workflowId)
        {
            if (!UseAccessKeys)
                return true;

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(workflowId))
                return false;

            var accessKeyObj = AccessKeyRepository.Get(accessKey);
            if (accessKeyObj == null)
                return false;

            return accessKeyObj.CanAccessWorkflow(workflowId);
        }

        public static bool CanAdminRules(string accessKey)
        {
            if (!UseAccessKeys)
                return true;

            if (string.IsNullOrEmpty(accessKey))
                return false;

            var accessKeyObj = AccessKeyRepository.Get(accessKey);
            return accessKeyObj != null && accessKeyObj.IsAdmin();
        }

        public static bool CanAdminSchedules(string accessKey)
        {
            if (!UseAccessKeys)
                return true;

            if (string.IsNullOrEmpty(accessKey))
                return false;

            var accessKeyObj = AccessKeyRepository.Get(accessKey);
            return accessKeyObj != null && accessKeyObj.IsAdmin();
        }
    }
}
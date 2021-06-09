using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Logging.Web
{
    public class LoggerContext : Common.Models.LoggerContext
    {
        // Mahesh : Expose only this library to the web project.

        private readonly ContextHelper _contextHelper = new ContextHelper(); // Todo: Inject if required.

        override public IDictionary<string, string> GetAllData()
        {
            var mandatoryFields = _contextHelper.GetMandatoryFields();
            var customFields = base.GetAllData();

            return MergeAllFields(mandatoryFields, customFields);
        }

        private IDictionary<string, string> MergeAllFields(Dictionary<string, string> mandatoryFields, IDictionary<string, string> customFields)
        {
            var allLoggingFields = new Dictionary<string, string>(mandatoryFields);
            foreach(var customField in customFields)
            {
                if (allLoggingFields.ContainsKey(customField.Key))
                {
                    allLoggingFields[customField.Key] = customField.Value;
                }
                else
                {
                    allLoggingFields.Add(customField.Key, customField.Value);
                }
            }
            return allLoggingFields;
        }
    }
}

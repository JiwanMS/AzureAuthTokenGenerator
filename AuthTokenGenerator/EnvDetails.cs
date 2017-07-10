using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTokenGenerator
{
    public class EnvironmentDataSection : ConfigurationSection
    {
        public const string SectionName = "EnvironmentDataSection";

        private const string EndpointCollectionName = "environments";

        private EnvironmentDataSection() { }

        //[ConfigurationProperty("environment")]
        //public EnvironmentElement environment { get { return (EnvironmentElement)this["environment"]; } }

        [ConfigurationProperty(EndpointCollectionName)]
        [ConfigurationCollection(typeof(EnvironmentElementCollection), AddItemName = "environment")]
        public EnvironmentElementCollection Environments { get { return (EnvironmentElementCollection)base[EndpointCollectionName]; } }
    }

    public class EnvironmentElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EnvironmentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EnvironmentElement)element).Name;
        }
    }

    public class EnvironmentElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or Sets the Name
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets or Sets the Tenant
        /// </summary>
        [ConfigurationProperty("tenant", IsRequired = true)]
        public string Tenant {
            get { return (string)this["tenant"]; }
            set { this["tenant"] = value; }
        }

        /// <summary>
        /// Gets or Sets the AadInstance
        /// </summary>
        [ConfigurationProperty("aadinstance", IsRequired = true)]
        public string AadInstance {
            get { return (string)this["aadinstance"]; }
            set { this["aadinstance"] = value; }
        }

        /// <summary>
        /// Gets or Sets the ClientId
        /// </summary>
        [ConfigurationProperty("clientid", IsRequired = true)]
        public string ClientId {
            get { return (string)this["clientid"]; }
            set { this["clientid"] = value; }
        }

        /// <summary>
        /// Gets or Sets the ClientSecret
        /// </summary>
        [ConfigurationProperty("clientsecret", IsRequired = true)]
        public string ClientSecret {
            get { return (string)this["clientsecret"]; }
            set { this["clientsecret"] = value; }
        }

        /// <summary>
        /// Gets or Sets the ApiResourceId
        /// </summary>
        [ConfigurationProperty("apiresourceid", IsRequired = true)]
        public string ApiResourceId {
            get { return (string)this["apiresourceid"]; }
            set { this["apiresourceid"] = value; }
        }
    }

    public class GetEnvDetails
    {
        private static List<EnvironmentElement> EnvDetails = new List<EnvironmentElement>();

        public static List<EnvironmentElement> LoadFromAppConfig()
        {
            // Grab the Environments listed in the App.config and add them to our list.
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            EnvironmentDataSection gui = (EnvironmentDataSection)config.Sections["EnvironmentDataSection"];
            foreach (var item in gui.Environments)
            {
                EnvDetails.Add((EnvironmentElement)item);
            }
            return EnvDetails;
        }
    }
}

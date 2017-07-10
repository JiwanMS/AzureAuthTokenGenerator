using System.Collections.Specialized;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AuthTokenGenerator
{
    public class EnvViewModel : ViewModelBase
    {
        private ObservableCollection<EnvironmentElement> environments;

        private EnvironmentElement selectedEnvironment;

        private ICommand _saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(() => this.SaveObject());
                }
                return _saveCommand;
            }
        }

        private void SaveObject()
        {
            try
            {
                var azureAuthResult = AzureAuthenticationHelper.ObtainAuthToken(this.ClientId, this.ClientSecret, this.ApiResourceId,
                    this.Tenant, this.AadInstance);
                this.ExpiresOn = azureAuthResult.ExpiresOn.ToLocalTime().ToString();
                this.Token = azureAuthResult.AuthToken;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public EnvironmentElement SelectedEnvironment
        {
            get
            {
                return this.selectedEnvironment;
            }
            set
            {
                this.selectedEnvironment = value;
                this.RaisePropertyChanged(() => this.SelectedEnvironment);
                this.RaisePropertyChanged(() => this.Tenant);
                this.RaisePropertyChanged(() => this.AadInstance);
                this.RaisePropertyChanged(() => this.ClientId);
                this.RaisePropertyChanged(() => this.ClientSecret);
                this.RaisePropertyChanged(() => this.ApiResourceId);
                this.ExpiresOn = string.Empty;
                this.Token = string.Empty;
            }
        }

        
        public string Tenant
        {
            get { return SelectedEnvironment.Tenant; }
        }

        
        public string AadInstance
        {
            get { return SelectedEnvironment.AadInstance; }
        }

        
        public string ClientId
        {
            get { return SelectedEnvironment.ClientId; }
        }

        
        public string ClientSecret
        {
            get { return SelectedEnvironment.ClientSecret; }
        }

        
        public string ApiResourceId
        {
            get { return SelectedEnvironment.ApiResourceId; }
        }

        private string expiresOn;

        private string token;

        public string ExpiresOn
        {
            get { return expiresOn; }
            set
            {
                this.expiresOn = value;
                this.RaisePropertyChanged(() => this.ExpiresOn);
            }
        }

        public string Token
        {
            get { return token; }
            set
            {
                this.token = value;
                this.RaisePropertyChanged(() => this.Token);
            }
        }

        public ObservableCollection<EnvironmentElement> Environments
        {
            get
            {
                if (this.environments == null || this.environments.Count == 0)
                {
                    environments = new ObservableCollection<EnvironmentElement>();
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    EnvironmentDataSection gui = (EnvironmentDataSection)config.Sections["EnvironmentDataSection"];
                    foreach (var item in gui.Environments)
                    {
                        environments.Add((EnvironmentElement)item);
                    }
                }
                return this.environments;
            }

            set
            {
                this.environments = value;
                this.RaisePropertyChanged(() => this.environments);
            }
        }

        public EnvViewModel()
        {
            environments = new ObservableCollection<EnvironmentElement>();
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            EnvironmentDataSection gui = (EnvironmentDataSection)config.Sections["EnvironmentDataSection"];
            foreach (var item in gui.Environments)
            {
                environments.Add((EnvironmentElement)item);
            }
            SelectedEnvironment = environments[0];
        }

        private ICommand _validateTokenCommand;

        public ICommand ValidateTokenCommand
        {
            get
            {
                if (_validateTokenCommand == null)
                {
                    _validateTokenCommand = new RelayCommand(() => this.ValidateAuthToken());
                }
                return _validateTokenCommand;
            }
        }

        private void ValidateAuthToken()
        {
            try
            {
                var azureAuthResult = AuthTokenValidator.Authenticate(this.TokenToValidate, this.Authority, this.Audience, this.selectedRegion.Value);
                if (azureAuthResult)
                {
                    
                    MessageBox.Show("Token is Valid");
                }
                else
                {
                    MessageBox.Show("Invalid Token");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Invalid Token. Details : " + ex.Message);
            }
            catch (SignatureVerificationFailedException ex)
            {
                MessageBox.Show("Invalid Token. Details : " + ex.Message);
            }
            catch (SecurityTokenExpiredException ex)
            {
                MessageBox.Show("Invalid Token. Details : " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Token Validation failed. Exception Details : " + ex.Message);
            }
        }
        private string tokenToValidate;
        public string TokenToValidate
        {
            get { return tokenToValidate; }
            set
            {
                this.tokenToValidate = value;
                this.RaisePropertyChanged(() => this.TokenToValidate);
            }
        }

        private string audience;
        public string Audience
        {
            get { return audience; }
            set
            {
                this.audience = value;
                this.RaisePropertyChanged(() => this.Audience);
            }
        }

        private string authority;
        public string Authority
        {
            get { return authority; }
            set
            {
                this.authority = value;
                this.RaisePropertyChanged(() => this.Authority);
            }
        }

        private KeyValuePair<string, string> selectedRegion;

        public KeyValuePair<string, string> SelectedRegion
        {
            get
            {
                if (selectedRegion.Equals(default(KeyValuePair<string, string>)))
                {
                    selectedRegion = this.region.FirstOrDefault();
                }
                return selectedRegion;
            }
            set { this.selectedRegion = value; }
        }

        private Dictionary<string, string> region;
        public Dictionary<string, string> Region
        {
            get
            {
                if (region == null || region.Count == 0 )
                {
                    region = new Dictionary<string, string>
                    {
                        {"Global Azure", "CN=accounts.accesscontrol.windows.net"},
                        {"Azure China", "CN=accounts.accesscontrol.chinacloudapi.cn"}
                    };
                }
                return region;
            }
        }
    }
}

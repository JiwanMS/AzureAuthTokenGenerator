Azure Auth Token Generator

This tool helps you generate Authentication token from Azure AD using Client ID and Secret. Below Diagram explains how the Azure AD authentication works.
 

The tool offers multiple environment which can be used to store multiple set of keys for different applications. The tool also offers to validate a token with a Resource ID. 

To Configure the tool modify the below entry in App.Config(AuthTokenGenerator.exe.config) according to your AD application. 

<environment name="EnvironmentName" tenant="tenant name" aadinstance="aadinstance name" clientid="client id" clientsecret="client secret" apiresourceid="resource id"/>

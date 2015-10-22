# PowerShell.REST.API
[![Build status](https://ci.appveyor.com/api/projects/status/rifttomp0tb8bjxa?svg=true)](https://ci.appveyor.com/project/tonybaloney/powershell-rest-api)

Turn any PowerShell script into a HTTP REST API!

## Overview

This project is a HTTP service written in C#.NET using the Microsoft OWIN libraries.

The concept is to take an existing PowerShell script, with parameters and expose it as a HTTP/REST method.

The web service configures the web methods as boot based on the configuration of the script repository.

It hosts a PowerShell runspace pool to load, run and check PowerShell scripts, capturing errors and stack traces to the logs
and parsing complex PowerShell response objects back into JSON.

It also supports async jobs to be run as seperate threads, with the job results to be stored on disk.

## How it works

This project implements a OWIN WebAPI HTTP service with a single "generic" API controller. The API controller consults the configuration collection of the endpoint
to identify which PowerShell script needs to be run for each HTTP request.
It hosts a PowerShell session to import and run the script, whilst monitoring the script process for faults. It converts the response of the script to a temporary JObject
and then returns the response data.

It also converts any GET or POST parameters to the web method to a parameter collection and passes them to the PowerShell process.

## Running
### run on the command line
```cmd
DynamicPowerShellApi.Host.exe --console
```
### run as a service
```cmd
DynamicPowerShellApi.Host.exe --service
```
### install the service
```cmd
DynamicPowerShellApi.Host.exe --install-service --service-user "UserABC" --service-password "Password123"
```

## Configuration

### The main service configuration file
The file DynamicPowerShellApi.Host.exe.config is the main configuration file. It contains the setup for security, logging and the methods themselves.


```xml
<WebApiConfiguration HostAddress="http://localhost:9000">
		<Jobs JobStorePath="c:\temp\" />
		<Authentication Enabled="false" StoreName="My" StoreLocation="LocalMachine" Thumbprint="E6B6364C75ED8B6495A42D543AC728B4C2263082" Audience="http://aperture.identity/connectors" />
		<Apis>
			<WebApi Name="Example">
				<WebMethods>
					<WebMethod Name="GetMessage" AsJob="true" PowerShellPath="Example.ps1">
						<Parameters>
							<Parameter Name="message" Type="string" />
						</Parameters>
					</WebMethod>
				</WebMethods>
			</WebApi>
		</Apis>
	</WebApiConfiguration>
```

### Configuring the HTTP listeners URI

#### Running on a specific IP (IPv4)
```xml
<WebApiConfiguration HostAddress="http://12.32.12.42:9000">
```

#### Running on any IP (IPv4 and IPv6)
```xml
<WebApiConfiguration HostAddress="http://+:9000">
```

## Adding a web API

If you wanted to offer a script HTTP GET:/foo/bar, POST:/foo/baz

First, add a WebApi element with the name __foo__

```xml
    <Apis>
        ...
		<WebApi Name="foo">
```

Then for your script, bar.ps1, by convention each script should pipe the return object through [ConvertTo-Json](https://technet.microsoft.com/en-us/library/hh849922.aspx), this is because PowerShell's dynamic objects can contain circular references and cause JSON convertors to crash.

Take your named parameters, for example `$message`

```powershell
params(
    [string] $message
)
$message | ConvertTo-Json -Compress
```

Now, add the method to the configuration file by adding an `WebMethod` Element

* `Name` The name of the method, which matches the URL pattern /{WebApi:Name}/{WebMethod:Name}?params
* `AsJob` Whether to run the script synchronously (__false__) or async (__true__)
* `PowerShellPath` The script path relative to the ScriptRepository directory.

Then, add a `Parameter` Element to the `Parameters` collection for each parameter, either POST or GET.

```xml
    <Apis>
        ...
		<WebApi Name="foo">
            <WebMethods>
                <WebMethod Name="bar" AsJob="false" PowerShellPath="bar.ps1">
                    <Parameters>
                        <Parameter Name="message" Type="string" />
                    </Parameters>
                </WebMethod>
            </WebMethods>
```


## Authentication

By default, authentication is __disabled__ for testing purposes. The primary authentication option is JSON Web-Token (JWT)

You can re-enable it by setting Enabled to __"true"__ then configure the following values to enable JWT auth.

* `StoreName` - The Windows certificate store name, e.g. My, Root, Trust
* `StoreLocation` - The location store, e.g. LocalMachine, CurrentUser
* `Thumbprint` - The thumbprint of the SSL certificate
* `Audience` - The expected JWT audience for inbound tokens [Help](https://tools.ietf.org/html/rfc7519#section-4.1.3)

```xml
<Authentication Enabled="false" StoreName="My" StoreLocation="LocalMachine" Thumbprint="E6B6364C75ED8B6495A42D543AC728B4C2263082" Audience="http://dimensiondata.com/auth/connectors" />
```

### Adding another authentication option

If you want to use another authentication option, you can leverage OWIN middleware to plug and play OAuth, certificates or any of the other supported auth methods.

In DynamicPowerShellApi.Owin/Startup.cs replace the existing auth configuration with your choice, e.g. OAuth

```csharp
    // If the config file specifies authentication, load up the certificates and use the JWT middleware.
    if (WebApiConfiguration.Instance.Authentication.Enabled)
    {
        appBuilder.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
        {
            AccessTokenFormat = "eg.."
            ...
        });
       
    }
```
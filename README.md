# SharpWcf
Easy multi-service WCF configurator from code or files

## Example of configuration. 

Let's imagine you have two (or more) services:

```
public class ServiceA : IServiceA
{
    public void Operation1()
    {
    }
}
```

and some other service:

```
public class ServiceB : IServiceB
{
    public void Operation2()
    {            
    }
}
```

Now example for hosting:

Use old style *App.config* settings to setup behaviors and bindings:

```
<configuration>
  <system.serviceModel>

    <behaviors>
      <serviceBehaviors>
        <behavior name="ServiceBehavior">
          <serviceMetadata httpGetEnabled="true" />          
        </behavior>
      </serviceBehaviors>
    </behaviors>
    
    <bindings>
      <netTcpBinding>
        <binding name="TcpBinding" />
      </netTcpBinding>
      <webHttpBinding>
        <binding name="WebBinding" />
      </webHttpBinding>
    </bindings>
    
  </system.serviceModel>
</configuration>
```

and put address-related data in *JSON* configuration:

```
{
  "Services": [
    {
      "BaseAddresses": [
        "net.tcp://localhost:20001/*",
        "http://localhost:20002/*"
      ],
      "Behavior": "ServiceBehavior",      
      "Endpoints": [
        {
          "Address": "/",
          "Binding": "System.ServiceModel.NetTcpBinding, System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
          "BindingConfiguration": "TcpBinding"
        },
        {
          "Address": "/",
          "Binding": "System.ServiceModel.WebHttpBinding, System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
          "BindingConfiguration": "WebBinding"
        }
      ]
    }
  ]
}
```
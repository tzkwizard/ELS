<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="EventHubProcessor" generation="1" functional="0" release="0" Id="fd3ce42b-c446-4ee9-ac18-8a679a98f833" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="EventHubProcessorGroup" generation="1" functional="0" release="0">
      <settings>
        <aCS name="EventRole:AzureStorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:AzureStorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="EventRole:Collection" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:Collection" />
          </maps>
        </aCS>
        <aCS name="EventRole:consumerGroupName" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:consumerGroupName" />
          </maps>
        </aCS>
        <aCS name="EventRole:Database" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:Database" />
          </maps>
        </aCS>
        <aCS name="EventRole:DocumentDBAuthorizationKey" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:DocumentDBAuthorizationKey" />
          </maps>
        </aCS>
        <aCS name="EventRole:DocumentDBUrl" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:DocumentDBUrl" />
          </maps>
        </aCS>
        <aCS name="EventRole:eventHubName" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:eventHubName" />
          </maps>
        </aCS>
        <aCS name="EventRole:Microsoft.ServiceBus.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:Microsoft.ServiceBus.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="EventRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="EventRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <maps>
        <map name="MapEventRole:AzureStorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/AzureStorageConnectionString" />
          </setting>
        </map>
        <map name="MapEventRole:Collection" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Collection" />
          </setting>
        </map>
        <map name="MapEventRole:consumerGroupName" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/consumerGroupName" />
          </setting>
        </map>
        <map name="MapEventRole:Database" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Database" />
          </setting>
        </map>
        <map name="MapEventRole:DocumentDBAuthorizationKey" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/DocumentDBAuthorizationKey" />
          </setting>
        </map>
        <map name="MapEventRole:DocumentDBUrl" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/DocumentDBUrl" />
          </setting>
        </map>
        <map name="MapEventRole:eventHubName" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/eventHubName" />
          </setting>
        </map>
        <map name="MapEventRole:Microsoft.ServiceBus.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.ServiceBus.ConnectionString" />
          </setting>
        </map>
        <map name="MapEventRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapEventRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="EventRole" generation="1" functional="0" release="0" software="C:\Users\24666\Documents\Visual Studio 2013\Projects\self\ELS\EventHubProcessor\csx\Release\roles\EventRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="AzureStorageConnectionString" defaultValue="" />
              <aCS name="Collection" defaultValue="" />
              <aCS name="consumerGroupName" defaultValue="" />
              <aCS name="Database" defaultValue="" />
              <aCS name="DocumentDBAuthorizationKey" defaultValue="" />
              <aCS name="DocumentDBUrl" defaultValue="" />
              <aCS name="eventHubName" defaultValue="" />
              <aCS name="Microsoft.ServiceBus.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;EventRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;EventRole&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="EventRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="EventRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="EventRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
</serviceModel>
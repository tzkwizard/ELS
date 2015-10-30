<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="EventHubProcessor" generation="1" functional="0" release="0" Id="aef36744-55b2-4e5c-8d3a-c5c19bed2b72" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="EventHubProcessorGroup" generation="1" functional="0" release="0">
      <settings>
        <aCS name="EventRole:AzureStorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:AzureStorageConnectionString" />
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
        <aCS name="EventRole:DBSelfLink" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:DBSelfLink" />
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
        <aCS name="EventRole:MasterCollection" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:MasterCollection" />
          </maps>
        </aCS>
        <aCS name="EventRole:Microsoft.ServiceBus.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:Microsoft.ServiceBus.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="EventRole:Microsoft.WindowsAzure.Plugins.Caching.CacheSizePercentage" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:Microsoft.WindowsAzure.Plugins.Caching.CacheSizePercentage" />
          </maps>
        </aCS>
        <aCS name="EventRole:Microsoft.WindowsAzure.Plugins.Caching.ConfigStoreConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:Microsoft.WindowsAzure.Plugins.Caching.ConfigStoreConnectionString" />
          </maps>
        </aCS>
        <aCS name="EventRole:Microsoft.WindowsAzure.Plugins.Caching.DiagnosticLevel" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:Microsoft.WindowsAzure.Plugins.Caching.DiagnosticLevel" />
          </maps>
        </aCS>
        <aCS name="EventRole:Microsoft.WindowsAzure.Plugins.Caching.NamedCaches" defaultValue="">
          <maps>
            <mapMoniker name="/EventHubProcessor/EventHubProcessorGroup/MapEventRole:Microsoft.WindowsAzure.Plugins.Caching.NamedCaches" />
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
      <channels>
        <sFSwitchChannel name="SW:EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheArbitrationPort">
          <toPorts>
            <inPortMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.WindowsAzure.Plugins.Caching.cacheArbitrationPort" />
          </toPorts>
        </sFSwitchChannel>
        <sFSwitchChannel name="SW:EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheClusterPort">
          <toPorts>
            <inPortMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.WindowsAzure.Plugins.Caching.cacheClusterPort" />
          </toPorts>
        </sFSwitchChannel>
        <sFSwitchChannel name="SW:EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheReplicationPort">
          <toPorts>
            <inPortMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.WindowsAzure.Plugins.Caching.cacheReplicationPort" />
          </toPorts>
        </sFSwitchChannel>
        <sFSwitchChannel name="SW:EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheServicePortInternal">
          <toPorts>
            <inPortMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.WindowsAzure.Plugins.Caching.cacheServicePortInternal" />
          </toPorts>
        </sFSwitchChannel>
        <sFSwitchChannel name="SW:EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheSocketPort">
          <toPorts>
            <inPortMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.WindowsAzure.Plugins.Caching.cacheSocketPort" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapEventRole:AzureStorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/AzureStorageConnectionString" />
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
        <map name="MapEventRole:DBSelfLink" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/DBSelfLink" />
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
        <map name="MapEventRole:MasterCollection" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/MasterCollection" />
          </setting>
        </map>
        <map name="MapEventRole:Microsoft.ServiceBus.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.ServiceBus.ConnectionString" />
          </setting>
        </map>
        <map name="MapEventRole:Microsoft.WindowsAzure.Plugins.Caching.CacheSizePercentage" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.WindowsAzure.Plugins.Caching.CacheSizePercentage" />
          </setting>
        </map>
        <map name="MapEventRole:Microsoft.WindowsAzure.Plugins.Caching.ConfigStoreConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.WindowsAzure.Plugins.Caching.ConfigStoreConnectionString" />
          </setting>
        </map>
        <map name="MapEventRole:Microsoft.WindowsAzure.Plugins.Caching.DiagnosticLevel" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.WindowsAzure.Plugins.Caching.DiagnosticLevel" />
          </setting>
        </map>
        <map name="MapEventRole:Microsoft.WindowsAzure.Plugins.Caching.NamedCaches" kind="Identity">
          <setting>
            <aCSMoniker name="/EventHubProcessor/EventHubProcessorGroup/EventRole/Microsoft.WindowsAzure.Plugins.Caching.NamedCaches" />
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
          <role name="EventRole" generation="1" functional="0" release="0" software="C:\Users\24666\Documents\Visual Studio 2013\Projects\self\ELS\EventHubProcessor\csx\Rc\roles\EventRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Microsoft.WindowsAzure.Plugins.Caching.cacheArbitrationPort" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.Caching.cacheClusterPort" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.Caching.cacheReplicationPort" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.Caching.cacheServicePortInternal" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.Caching.cacheSocketPort" protocol="tcp" />
              <outPort name="EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheArbitrationPort" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/EventHubProcessor/EventHubProcessorGroup/SW:EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheArbitrationPort" />
                </outToChannel>
              </outPort>
              <outPort name="EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheClusterPort" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/EventHubProcessor/EventHubProcessorGroup/SW:EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheClusterPort" />
                </outToChannel>
              </outPort>
              <outPort name="EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheReplicationPort" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/EventHubProcessor/EventHubProcessorGroup/SW:EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheReplicationPort" />
                </outToChannel>
              </outPort>
              <outPort name="EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheServicePortInternal" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/EventHubProcessor/EventHubProcessorGroup/SW:EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheServicePortInternal" />
                </outToChannel>
              </outPort>
              <outPort name="EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheSocketPort" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/EventHubProcessor/EventHubProcessorGroup/SW:EventRole:Microsoft.WindowsAzure.Plugins.Caching.cacheSocketPort" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="AzureStorageConnectionString" defaultValue="" />
              <aCS name="consumerGroupName" defaultValue="" />
              <aCS name="Database" defaultValue="" />
              <aCS name="DBSelfLink" defaultValue="" />
              <aCS name="DocumentDBAuthorizationKey" defaultValue="" />
              <aCS name="DocumentDBUrl" defaultValue="" />
              <aCS name="eventHubName" defaultValue="" />
              <aCS name="MasterCollection" defaultValue="" />
              <aCS name="Microsoft.ServiceBus.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Caching.CacheSizePercentage" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Caching.ConfigStoreConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Caching.DiagnosticLevel" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Caching.NamedCaches" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;EventRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;EventRole&quot;&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.Caching.cacheArbitrationPort&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.Caching.cacheClusterPort&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.Caching.cacheReplicationPort&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.Caching.cacheServicePortInternal&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.Caching.cacheSocketPort&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[20000,20000,20000]" defaultSticky="true" kind="Directory" />
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
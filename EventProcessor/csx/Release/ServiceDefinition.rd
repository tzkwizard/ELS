<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="EventProcessor" generation="1" functional="0" release="0" Id="7f062a71-1764-4cbc-96b3-339b7779e863" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="EventProcessorGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/EventProcessor/EventProcessorGroup/LB:ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapCertificate|ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:AzureStorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:AzureStorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:Collection" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:Collection" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:consumerGroupName" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:consumerGroupName" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:Database" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:Database" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:DocumentDBAuthorizationKey" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:DocumentDBAuthorizationKey" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:DocumentDBUrl" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:DocumentDBUrl" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:eventHubName" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:eventHubName" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:Microsoft.ServiceBus.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:Microsoft.ServiceBus.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </maps>
        </aCS>
        <aCS name="ReceiverRole:numberOfPartitions" defaultValue="">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRole:numberOfPartitions" />
          </maps>
        </aCS>
        <aCS name="ReceiverRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/EventProcessor/EventProcessorGroup/MapReceiverRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput">
          <toPorts>
            <inPortMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp">
          <toPorts>
            <inPortMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapCertificate|ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" kind="Identity">
          <certificate>
            <certificateMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </certificate>
        </map>
        <map name="MapReceiverRole:AzureStorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/AzureStorageConnectionString" />
          </setting>
        </map>
        <map name="MapReceiverRole:Collection" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Collection" />
          </setting>
        </map>
        <map name="MapReceiverRole:consumerGroupName" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/consumerGroupName" />
          </setting>
        </map>
        <map name="MapReceiverRole:Database" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Database" />
          </setting>
        </map>
        <map name="MapReceiverRole:DocumentDBAuthorizationKey" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/DocumentDBAuthorizationKey" />
          </setting>
        </map>
        <map name="MapReceiverRole:DocumentDBUrl" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/DocumentDBUrl" />
          </setting>
        </map>
        <map name="MapReceiverRole:eventHubName" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/eventHubName" />
          </setting>
        </map>
        <map name="MapReceiverRole:Microsoft.ServiceBus.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Microsoft.ServiceBus.ConnectionString" />
          </setting>
        </map>
        <map name="MapReceiverRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </setting>
        </map>
        <map name="MapReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </setting>
        </map>
        <map name="MapReceiverRole:numberOfPartitions" kind="Identity">
          <setting>
            <aCSMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/numberOfPartitions" />
          </setting>
        </map>
        <map name="MapReceiverRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="ReceiverRole" generation="1" functional="0" release="0" software="C:\Users\24666\Documents\Visual Studio 2013\Projects\self\ELS\EventProcessor\csx\Release\roles\ReceiverRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp" portRanges="3389" />
              <outPort name="ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/EventProcessor/EventProcessorGroup/SW:ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
            </componentports>
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
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="" />
              <aCS name="numberOfPartitions" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;ReceiverRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;ReceiverRole&quot;&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="ReceiverRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="ReceiverRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="ReceiverRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="420344f5-5247-46c6-b9fd-121391c9f90a" ref="Microsoft.RedDog.Contract\ServiceContract\EventProcessorContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="83bc2029-fb51-46e2-bd25-47cc5935e551" ref="Microsoft.RedDog.Contract\Interface\ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/EventProcessor/EventProcessorGroup/ReceiverRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>
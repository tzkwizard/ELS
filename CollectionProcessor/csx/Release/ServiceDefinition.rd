<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="CollectionProcessor" generation="1" functional="0" release="0" Id="fc6bb944-cfbf-4d5f-aaa2-7e187963c890" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="CollectionProcessorGroup" generation="1" functional="0" release="0">
      <settings>
        <aCS name="CheckRole:AuthorizationKey" defaultValue="">
          <maps>
            <mapMoniker name="/CollectionProcessor/CollectionProcessorGroup/MapCheckRole:AuthorizationKey" />
          </maps>
        </aCS>
        <aCS name="CheckRole:Database" defaultValue="">
          <maps>
            <mapMoniker name="/CollectionProcessor/CollectionProcessorGroup/MapCheckRole:Database" />
          </maps>
        </aCS>
        <aCS name="CheckRole:EndpointUrl" defaultValue="">
          <maps>
            <mapMoniker name="/CollectionProcessor/CollectionProcessorGroup/MapCheckRole:EndpointUrl" />
          </maps>
        </aCS>
        <aCS name="CheckRole:MasterCollection" defaultValue="">
          <maps>
            <mapMoniker name="/CollectionProcessor/CollectionProcessorGroup/MapCheckRole:MasterCollection" />
          </maps>
        </aCS>
        <aCS name="CheckRole:Microsoft.ServiceBus.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/CollectionProcessor/CollectionProcessorGroup/MapCheckRole:Microsoft.ServiceBus.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="CheckRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/CollectionProcessor/CollectionProcessorGroup/MapCheckRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="CheckRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/CollectionProcessor/CollectionProcessorGroup/MapCheckRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <maps>
        <map name="MapCheckRole:AuthorizationKey" kind="Identity">
          <setting>
            <aCSMoniker name="/CollectionProcessor/CollectionProcessorGroup/CheckRole/AuthorizationKey" />
          </setting>
        </map>
        <map name="MapCheckRole:Database" kind="Identity">
          <setting>
            <aCSMoniker name="/CollectionProcessor/CollectionProcessorGroup/CheckRole/Database" />
          </setting>
        </map>
        <map name="MapCheckRole:EndpointUrl" kind="Identity">
          <setting>
            <aCSMoniker name="/CollectionProcessor/CollectionProcessorGroup/CheckRole/EndpointUrl" />
          </setting>
        </map>
        <map name="MapCheckRole:MasterCollection" kind="Identity">
          <setting>
            <aCSMoniker name="/CollectionProcessor/CollectionProcessorGroup/CheckRole/MasterCollection" />
          </setting>
        </map>
        <map name="MapCheckRole:Microsoft.ServiceBus.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/CollectionProcessor/CollectionProcessorGroup/CheckRole/Microsoft.ServiceBus.ConnectionString" />
          </setting>
        </map>
        <map name="MapCheckRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/CollectionProcessor/CollectionProcessorGroup/CheckRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapCheckRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/CollectionProcessor/CollectionProcessorGroup/CheckRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="CheckRole" generation="1" functional="0" release="0" software="C:\Users\24666\Documents\Visual Studio 2013\Projects\self\ELS\CollectionProcessor\csx\Release\roles\CheckRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="AuthorizationKey" defaultValue="" />
              <aCS name="Database" defaultValue="" />
              <aCS name="EndpointUrl" defaultValue="" />
              <aCS name="MasterCollection" defaultValue="" />
              <aCS name="Microsoft.ServiceBus.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;CheckRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;CheckRole&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/CollectionProcessor/CollectionProcessorGroup/CheckRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/CollectionProcessor/CollectionProcessorGroup/CheckRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/CollectionProcessor/CollectionProcessorGroup/CheckRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="CheckRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="CheckRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="CheckRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
</serviceModel>
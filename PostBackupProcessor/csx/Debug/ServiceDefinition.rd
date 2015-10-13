<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="PostBackupProcessor" generation="1" functional="0" release="0" Id="0da53766-52f2-4976-9afa-01a66155d8d4" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="PostBackupProcessorGroup" generation="1" functional="0" release="0">
      <settings>
        <aCS name="StorageRole:AuthorizationKey" defaultValue="">
          <maps>
            <mapMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/MapStorageRole:AuthorizationKey" />
          </maps>
        </aCS>
        <aCS name="StorageRole:Database" defaultValue="">
          <maps>
            <mapMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/MapStorageRole:Database" />
          </maps>
        </aCS>
        <aCS name="StorageRole:EndpointUrl" defaultValue="">
          <maps>
            <mapMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/MapStorageRole:EndpointUrl" />
          </maps>
        </aCS>
        <aCS name="StorageRole:MasterCollection" defaultValue="">
          <maps>
            <mapMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/MapStorageRole:MasterCollection" />
          </maps>
        </aCS>
        <aCS name="StorageRole:Microsoft.Storage.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/MapStorageRole:Microsoft.Storage.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="StorageRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/MapStorageRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="StorageRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/MapStorageRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <maps>
        <map name="MapStorageRole:AuthorizationKey" kind="Identity">
          <setting>
            <aCSMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/StorageRole/AuthorizationKey" />
          </setting>
        </map>
        <map name="MapStorageRole:Database" kind="Identity">
          <setting>
            <aCSMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/StorageRole/Database" />
          </setting>
        </map>
        <map name="MapStorageRole:EndpointUrl" kind="Identity">
          <setting>
            <aCSMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/StorageRole/EndpointUrl" />
          </setting>
        </map>
        <map name="MapStorageRole:MasterCollection" kind="Identity">
          <setting>
            <aCSMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/StorageRole/MasterCollection" />
          </setting>
        </map>
        <map name="MapStorageRole:Microsoft.Storage.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/StorageRole/Microsoft.Storage.ConnectionString" />
          </setting>
        </map>
        <map name="MapStorageRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/StorageRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapStorageRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/StorageRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="StorageRole" generation="1" functional="0" release="0" software="C:\Users\24666\Documents\Visual Studio 2013\Projects\self\ELS\PostBackupProcessor\csx\Debug\roles\StorageRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="AuthorizationKey" defaultValue="" />
              <aCS name="Database" defaultValue="" />
              <aCS name="EndpointUrl" defaultValue="" />
              <aCS name="MasterCollection" defaultValue="" />
              <aCS name="Microsoft.Storage.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;StorageRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;StorageRole&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/StorageRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/StorageRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/PostBackupProcessor/PostBackupProcessorGroup/StorageRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="StorageRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="StorageRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="StorageRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
</serviceModel>
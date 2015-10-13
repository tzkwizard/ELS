<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="ChatBackupProcessor" generation="1" functional="0" release="0" Id="3294719c-6656-4487-afdc-e06b8646e541" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="ChatBackupProcessorGroup" generation="1" functional="0" release="0">
      <settings>
        <aCS name="FirebaseRole:Firebasenode" defaultValue="">
          <maps>
            <mapMoniker name="/ChatBackupProcessor/ChatBackupProcessorGroup/MapFirebaseRole:Firebasenode" />
          </maps>
        </aCS>
        <aCS name="FirebaseRole:Microsoft.Storage.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/ChatBackupProcessor/ChatBackupProcessorGroup/MapFirebaseRole:Microsoft.Storage.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="FirebaseRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/ChatBackupProcessor/ChatBackupProcessorGroup/MapFirebaseRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="FirebaseRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/ChatBackupProcessor/ChatBackupProcessorGroup/MapFirebaseRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <maps>
        <map name="MapFirebaseRole:Firebasenode" kind="Identity">
          <setting>
            <aCSMoniker name="/ChatBackupProcessor/ChatBackupProcessorGroup/FirebaseRole/Firebasenode" />
          </setting>
        </map>
        <map name="MapFirebaseRole:Microsoft.Storage.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/ChatBackupProcessor/ChatBackupProcessorGroup/FirebaseRole/Microsoft.Storage.ConnectionString" />
          </setting>
        </map>
        <map name="MapFirebaseRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/ChatBackupProcessor/ChatBackupProcessorGroup/FirebaseRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapFirebaseRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/ChatBackupProcessor/ChatBackupProcessorGroup/FirebaseRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="FirebaseRole" generation="1" functional="0" release="0" software="C:\Users\24666\Documents\Visual Studio 2013\Projects\self\ELS\ChatBackupProcessor\csx\Debug\roles\FirebaseRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Firebasenode" defaultValue="" />
              <aCS name="Microsoft.Storage.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;FirebaseRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FirebaseRole&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/ChatBackupProcessor/ChatBackupProcessorGroup/FirebaseRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/ChatBackupProcessor/ChatBackupProcessorGroup/FirebaseRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/ChatBackupProcessor/ChatBackupProcessorGroup/FirebaseRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="FirebaseRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="FirebaseRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="FirebaseRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
</serviceModel>
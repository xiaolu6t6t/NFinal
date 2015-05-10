<?xml version="1.0" encoding="UTF-8"?>
<!--
  有关如何配置 ASP.NET 应用程序的详细信息，请访问
  http://go.microsoft.com/fwlink/?LinkId=169433
  -->
<configuration>
  <configSections>
    <section name="RewriterConfig" type="NFinal.URLRewriter.Config.RewriterConfigSerializerSectionHandler" />
  </configSections>
  <connectionStrings>
    <!--<add name="tiaojiu" connectionString="Server=localhost;Database=tiaojiu;Uid=root;Pwd=hayiaf102030;" providerName="MySql.Data.MySqlClient" />-->
  </connectionStrings>
  <system.serviceModel>
    <serviceHostingEnvironment aspNetCompatibilityEnabled="false" />
  </system.serviceModel>
  <appSettings>
    <add key="Apps" value="App" />
    <add key="AutoGeneration" value="false" />
  </appSettings>
  <RewriterConfig>
    <Rules>
      <RewriterRule>
        <LookFor>~/Index.html</LookFor>
        <SendTo>~/App/IndexController/Index.htm</SendTo>
      </RewriterRule>
    </Rules>
  </RewriterConfig>
  <system.webServer>
    <handlers>
      <add name="NFinalHandlerFactory" verb="*" path="*.htm" type="NFinal.Handler.HandlerFactory" preCondition="integratedMode" />
    </handlers>
    <modules>
      <remove name="RoleManager" />
      <remove name="Profile" />
      <remove name="FormsAuthentication" />
      <remove name="FileAuthorization" />
      <remove name="AnonymousIdentification" />
      <remove name="OutputCache" />
      <remove name="ScriptModule-4.0" />
      <remove name="ServiceModel-4.0" />
      <remove name="Session" />
      <remove name="UrlAuthorization" />
      <remove name="UrlMappingsModule" />
      <remove name="UrlRoutingModule-4.0" />
      <remove name="WindowsAuthentication" />
      <add name="ModuleRewriter" type="NFinal.URLRewriter.ModuleRewriter" />
    </modules>
  </system.webServer>
</configuration>
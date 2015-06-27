<?xml version="1.0" encoding="utf-8" ?>
<!--
  有关如何配置 ASP.NET 应用程序的详细信息，请访问
  http://go.microsoft.com/fwlink/?LinkId=169433
  -->
<configuration>
  <configSections>
    <section name="RewriterConfig" type="NFinal.URLRewriter.Config.RewriterConfigSerializerSectionHandler"/>
  </configSections>
  <connectionStrings>
    <add name="Common" connectionString="Data Source=|DataDirectory|\Common.db;Pooling=true;FailIfMissing=false" providerName="System.Data.SQLite"/>
  </connectionStrings>
  <appSettings>
    <add key="Apps" value=""/>
    <add key="AutoGeneration" value="false"/>
    <add key="vs:EnableBrowserLink" value="false"/>
  </appSettings>
  <RewriterConfig>
    <Rules>
      <RewriterRule>
        <LookFor>~/Index.html</LookFor>
        <SendTo>~/{$app}/IndexController/Index.htm</SendTo>
      </RewriterRule>
    </Rules>
  </RewriterConfig>
  <system.web>
    <!--IIS8下请删除此配置节:begin-->
    <httpHandlers>
      <add verb="*" path="*.htm" type="NFinal.Handler.HandlerFactory"/>
    </httpHandlers>
    <httpModules>
      <add name="ModuleRewriter" type="NFinal.URLRewriter.ModuleRewriter"/>
    </httpModules>
    <!--IIS8下请删除此配置节:end-->
    <compilation debug="true"/>
  </system.web>
  <!--IIS6下请删除此配置节:begin-->
  <system.webServer>
    <urlCompression doStaticCompression="false" doDynamicCompression="false"/>
    <validation validateIntegratedModeConfiguration="false"/>
    <handlers>
      <add name="NFinalHandlerFactory" verb="*" path="*.htm" type="NFinal.Handler.HandlerFactory"/>
    </handlers>
    <modules>
      <add name="ModuleRewriter" type="NFinal.URLRewriter.ModuleRewriter"/>
    </modules>
  </system.webServer>
  <!--IIS6下请删除此配置节:end-->
</configuration>
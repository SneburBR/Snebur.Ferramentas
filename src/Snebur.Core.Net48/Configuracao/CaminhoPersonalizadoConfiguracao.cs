using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace Snebur.Aplicacao.Configuracao
{

    public class CaminhoPadraoApplicationSettings<TSetting> :
                       CaminhoPadraoApplicationSettings<TSetting, ResolverFaltaApplicationSettingsPadrao>

                        where TSetting : ApplicationSettingsBase
    {
    }

    public class CaminhoPadraoApplicationSettings<TSetting, TResolverFaltaApplicationSettings> :
                       BaseCaminhoPadraoApplicationSettings, IContextoConfiugracao
                       where TSetting : ApplicationSettingsBase
                       where TResolverFaltaApplicationSettings : IResolverFaltaApplicationSettings
    {

        private Dictionary<string, SettingStruct> Dicionario { get; set; }
        private TResolverFaltaApplicationSettings Resolver { get; }

        public CaminhoPadraoApplicationSettings()
        {
            this.Dicionario = new Dictionary<string, SettingStruct>();
            this.Resolver = Activator.CreateInstance<TResolverFaltaApplicationSettings>();
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(this.ApplicationName, config);
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            if (!this._carregado)
            {
                this._carregado = true;
                this.CarregarValores();
            }
            var values = new SettingsPropertyValueCollection();
            foreach (SettingsProperty setting in collection)
            {
                var value = new SettingsPropertyValue(setting);
                value.IsDirty = false;

                var tipo = Type.GetType(setting.PropertyType.FullName);

                if (this.Dicionario.ContainsKey(setting.Name))
                {
                    value.SerializedValue = this.Dicionario[setting.Name].value;
                    value.PropertyValue = Convert.ChangeType(this.Dicionario[setting.Name].value, tipo);
                }
                else //apenas quando não existe a configuração
                {
                    var valor = this.RetornarValor(setting.Name, setting.DefaultValue, tipo);
                    value.SerializedValue = valor;
                    value.PropertyValue = Convert.ChangeType(valor, tipo);
                }
                values.Add(value);
            }
            this.Resolver?.Dispose();
            return values;
        }

        private object RetornarValor(string settingName, object valorPadrao, Type tipo)
        {
            try
            {
                return this.Resolver?.RetornarValor(settingName, valorPadrao, tipo) ?? valorPadrao;
            }
            catch
            {
                return valorPadrao;
            }
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            foreach (SettingsPropertyValue value in collection)
            {
                var setting = new SettingStruct()
                {
                    value = (value.PropertyValue == null ? String.Empty : value.PropertyValue.ToString()),
                    name = value.Name,
                    serializeAs = value.Property.SerializeAs.ToString()
                };

                if (!this.Dicionario.ContainsKey(value.Name))
                {
                    this.Dicionario.Add(value.Name, setting);
                }
                else
                {
                    this.Dicionario[value.Name] = setting;
                }
            }
            this.SalvarArquivoConfigruacao();
        }

        private static object _bloqueio = new object();

        private void CarregarValores()
        {
            lock (_bloqueio)
            {
                if (!File.Exists(this.CaminhoArquivoApplicationSettings) || this.IsArquivoXmlConfiguracaoComrrompido())
                {
                    this.ResolverFaltaArquivoApplicationSettings();
                }
                try
                {
                    var configXml = XDocument.Load(this.CaminhoArquivoApplicationSettings);
                    var settingElements = configXml.Element(CONFIG).Element(USER_SETTINGS).Element(typeof(TSetting).FullName).Elements(SETTING);

                    foreach (var element in settingElements)
                    {
                        var novoConfiguracao = new SettingStruct()
                        {
                            name = element.Attribute(NAME) == null ? String.Empty : element.Attribute(NAME).Value,
                            serializeAs = element.Attribute(SERIALIZE_AS) == null ? "String" : element.Attribute(SERIALIZE_AS).Value,
                            value = element.Value ?? String.Empty
                        };
                        this.Dicionario.Add(element.Attribute(NAME).Value, novoConfiguracao);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private bool IsArquivoXmlConfiguracaoComrrompido()
        {
            var arquivo = new FileInfo(this.CaminhoArquivoApplicationSettings);
            if (arquivo.Exists && arquivo.Length == 0)
            {
                Thread.Sleep(1000);
                ArquivoUtil.DeletarArquivo(this.CaminhoArquivoApplicationSettings, false, true);
                return true;
            }
            try
            {
                using (var fs = StreamUtil.OpenRead(this.CaminhoArquivoApplicationSettings))
                {
                    var configXml = XDocument.Load(this.CaminhoArquivoApplicationSettings);
                    var settingElements = configXml.Element(CONFIG).Element(USER_SETTINGS).Element(typeof(TSetting).FullName).Elements(SETTING);
                    return settingElements.Count() == 0;
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(1000);
                ArquivoUtil.DeletarArquivo(this.CaminhoArquivoApplicationSettings, false, true);
                LogUtil.ErroAsync(new Exception($"Arquivo configuracao.xml comrrompido {this.CaminhoArquivoApplicationSettings} ", ex));
                return true;
            }
        }

        internal protected void CriarArquivoConfiguracaoVazio()
        {
            var doc = new XDocument();
            var declaration = new XDeclaration("1.0", "utf-8", "true");
            var config = new XElement(CONFIG);
            var userSettings = new XElement(USER_SETTINGS);
            var group = new XElement(typeof(TSetting).FullName);
            userSettings.Add(group);
            config.Add(userSettings);
            doc.Add(config);
            doc.Declaration = declaration;
            doc.Save(this.CaminhoArquivoApplicationSettings);
        }

        private void SalvarArquivoConfigruacao()
        {
            var import = XDocument.Load(this.CaminhoArquivoApplicationSettings);
            var settingsSection = import.Element(CONFIG).Element(USER_SETTINGS).Element(typeof(TSetting).FullName);
            foreach (var entry in this.Dicionario)
            {
                var setting = settingsSection.Elements().FirstOrDefault(e => e.Attribute(NAME).Value == entry.Key);
                if (setting == null)
                {
                    var newSetting = new XElement(SETTING);
                    newSetting.Add(new XAttribute(NAME, entry.Value.name));
                    newSetting.Add(new XAttribute(SERIALIZE_AS, entry.Value.serializeAs));
                    newSetting.Value = (entry.Value.value ?? String.Empty);
                    settingsSection.Add(newSetting);
                }
                else //update the value if it exists.
                {
                    setting.Value = (entry.Value.value ?? String.Empty);
                }
            }
            import.Save(this.CaminhoArquivoApplicationSettings);
        }

        private void ResolverFaltaArquivoApplicationSettings()
        {
            if (!File.Exists(this.CaminhoArquivoApplicationSettings))
            {
                if (typeof(TResolverFaltaApplicationSettings) != typeof(ResolverFaltaApplicationSettingsPadrao))
                {
                    try
                    {
                        var instanciaResolverAplicacao = Activator.CreateInstance<TResolverFaltaApplicationSettings>();
                        instanciaResolverAplicacao.Resolver(this);
                    }
                    catch { }
                }
            }
            if (!File.Exists(this.CaminhoArquivoApplicationSettings))
            {
                try
                {
                    var resolverArquivoMaisRecente = new ResolverFaltaApplicationSettingsPadrao();
                    resolverArquivoMaisRecente.Resolver(this);
                }
                catch { }
            }
            if (!File.Exists(this.CaminhoArquivoApplicationSettings))
            {
                this.CriarArquivoConfiguracaoVazio();
            }
        }
        #region  IContextoConfiugracao 

        void IContextoConfiugracao.CriarArquivoConfiguracaoVazio()
        {
            this.CriarArquivoConfiguracaoVazio();
        }
        #endregion
    }
}
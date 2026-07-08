using System;
using System.Linq;

using SDM.Abstractions.Install.DevPack;

using Skyline.AppInstaller;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net;
using Skyline.DataMiner.Net.AppPackages;
using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
using Skyline.DataMiner.Net.Apps.Modules;
using Skyline.DataMiner.Net.Messages.SLDataGateway;
using Skyline.DataMiner.Net.Sections;

/// <summary>
/// DataMiner Script Class.
/// </summary>
internal class Script
{
	/// <summary>
	/// The script entry point.
	/// </summary>
	/// <param name="engine">Provides access to the Automation engine.</param>
	/// <param name="context">Provides access to the installation context.</param>
	[AutomationEntryPoint(AutomationEntryPointType.Types.InstallAppPackage)]
	public void Install(IEngine engine, AppInstallContext context)
	{
		try
		{
			engine.Timeout = new TimeSpan(0, 10, 0);
			engine.GenerateInformation("Starting installation");
			var installer = new AppInstaller(Engine.SLNetRaw, context);
			installer.InstallDefaultContent();

			////string setupContentPath = installer.GetSetupContentDirectory();

			// Install the SDM.Abstractions DevPack
			var devPackInstaller = new DevPackInstaller(installer, engine);
			devPackInstaller.DeployAllDevPacks();

			// Set abstraction version in the registry
			var moduleId = "(slc)standard_data_model";
			var moduleHelper = new ModuleSettingsHelper(engine.SendSLNetMessages);
			var registration = moduleHelper.ModuleSettings.Read(ModuleSettingsExposers.ModuleId.Equal(moduleId));
			if (registration is null || !registration.Any())
			{
				// Stnadalone deployment, no need to set the version in the registry
				return;
			}

			var id = new Guid("e41ec7db-088f-4b2a-9ac1-b1c6694ab33b");
			var sectionId = new SectionDefinitionID(new Guid("f16970b4-c311-4900-8dfd-50a6879b8f7b")) { ModuleId = moduleId };
			var fieldId = new FieldDescriptorID(new Guid("8b532bb2-974a-40da-929c-e7a84592046a"));

			var helper = new DomHelper(engine.SendSLNetMessages, moduleId);
			var instance = helper.DomInstances.Read(DomInstanceExposers.Id.Equal(id)).FirstOrDefault();
			if (instance is null)
			{
				installer.Log($"No instance found for {id}, skipping setting the abstraction version in the registry");
				return;
			}

			instance.AddOrUpdateFieldValue<string>(
				sectionId,
				fieldId,
				context.AppInfo.Version);
			helper.DomInstances.Update(instance);
			installer.Log($"Set abstraction version to {context.AppInfo.Version} in the SDM Registry");
		}
		catch (Exception e)
		{
			engine.ExitFail($"Exception encountered during installation: {e}");
		}
	}
}
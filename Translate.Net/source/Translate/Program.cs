﻿#region License block : MPL 1.1/GPL 2.0/LGPL 2.1
/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is the FreeCL.Net library.
 *
 * The Initial Developer of the Original Code is 
 *  Oleksii Prudkyi (Oleksii.Prudkyi@gmail.com).
 * Portions created by the Initial Developer are Copyright (C) 2007-2008
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s):
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */
#endregion

using System;
using System.Windows.Forms;
using FreeCL.Forms;
using Application = FreeCL.Forms.Application;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FreeCL.RTL;

namespace Translate
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="args")]
		[SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId="FreeCL.Forms.OptionsControlsManager.RegisterOptionControl(System.Type,System.String,System.Int32,System.String,System.Int32)")]
		private static void Main(string[] args)
		{
			//if(MonoHelper.IsUnix)			
			//	Environment.SetEnvironmentVariable ("MONO_BROWSER_ENGINE", "webkit");

			if(CommandLineHelper.IsCommandSwitchSet("gen_list"))
			{
				try
				{
					WebUI.ResultsWebServer.UnhandledException += OnUnhandledExceptionEvent; 
					WebUI.ResultsWebServer.Start();
				
					ServicesListHtmlGenerator.Generate();
				}
				catch(Exception e)
				{
					Application.OnThreadException(e);
				}
				return;
			}
			
			try
			{
		
				Application.ExceptionsLog += GlobalEventsLogException;
				ResultsCache.UseCache = true;
				GuessResultsCache.UseCache = true;
				
				UpdatesManager.Init();
				Application.ShowSplashForm(null);
				
				Application.OptionsForm = new OptionsForm();
				Application.BaseOptions = new TranslateOptions();
				Application.AboutForm = new AboutForm();
				
				OptionsControlsManager.RegisterOptionControl(typeof(StartupOptionsControl), "General", 0, "Startup", 1);
				OptionsControlsManager.RegisterOptionControl(typeof(TrayOptionsControl), "General", 0, "Tray", 2);
				OptionsControlsManager.RegisterOptionControl(typeof(ResultWindowOptionControl), "General", 0, "Result view", 3);
				OptionsControlsManager.RegisterOptionControl(typeof(GuesserOptionsControl), "General", 0, "Language detection", 4);
				
				if(!MonoHelper.IsUnix) //monobug - don't work with DataSource
					OptionsControlsManager.RegisterOptionControl(typeof(KeyboardLayoutControl), "General", 0, "Keyboard layouts", 5);
				
				
				OptionsControlsManager.RegisterOptionControl(typeof(UpdatesOptionsControl), "General", 0, "Updates", 6);
				OptionsControlsManager.RegisterOptionControl(typeof(HotkeysOptionsControl), "General", 0, "Hotkeys", 7);
				
				OptionsControlsManager.RegisterOptionControl(typeof(InputFontOptionsControl), "Fonts", 1, "Input text", 0);
				OptionsControlsManager.RegisterOptionControl(typeof(ResultViewFontOptionsControl), "Fonts", 1, "Result view", 1);
				OptionsControlsManager.RegisterOptionControl(typeof(ToolbarsFontOptionsControl), "Fonts", 1, "Toolbars", 2);
				
				OptionsControlsManager.RegisterOptionControl(typeof(GeneralNetworkOptionsControl), "Network", 2, "Common", 0);
				
				OptionsControlsManager.RegisterOptionControl(typeof(CustomProfilesControl), "Profiles", 3, "List", 4);
				OptionsControlsManager.RegisterOptionControl(typeof(AnalyticsOptionsControl), "Usage information collecting", 4, "Usage information collecting", 4);
				
				Application.Initialize();
				
				if(TranslateOptions.Instance.SingleInstance && ProcessHelper.IsOtherInstanceAlreadyStarted())
				{
					ProcessHelper.ActivateOtherInstance(ApplicationInfo.ProductName);
					return;
				}	
				
				UpdatesManager.CheckNewVersion();
				
				WebUI.ResultsWebServer.UnhandledException += OnUnhandledExceptionEvent; 
				try
				{
					WebUI.ResultsWebServer.Start();
				}
				catch(System.Reflection.TargetInvocationException e)
				{
					if(e.InnerException != null && e.InnerException.GetType() == typeof(System.Security.VerificationException))
					{
						MessageBox.Show(LangPack.TranslateString("Running from network share not supported.\nTool can work only when started from local drive under appropriate user's rights."),
							Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					else 
						throw;
				}

				AnalyticsMonitor.Start();
				Application.Run(new TranslateMainForm());
				WebUI.ResultsWebServer.Stop();
			}
			catch(Exception e)
			{
				Application.OnThreadException(e);
			}
		}
		
		static void OnUnhandledExceptionEvent(object sender, UnhandledExceptionEventArgs e)
		{
			Application.OnThreadException((Exception)e.ExceptionObject);
		}
		
		static void GlobalEventsLogException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			AnalyticsMonitor.OnThreadException(e.Exception);
		}
		
		
	}
}

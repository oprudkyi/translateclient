#region License block : MPL 1.1/GPL 2.0/LGPL 2.1
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
 * Portions created by the Initial Developer are Copyright (C) 2008
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
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using FreeCL.Forms;

namespace Translate
{
	/// <summary>
	/// Description of HotkeysOptrionsControl.
	/// </summary>
	public partial class HotkeysOptionsControl : FreeCL.Forms.BaseOptionsControl
	{
		public HotkeysOptionsControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			RegisterLanguageEvent(OnLanguageChanged);
		}
		
		void OnLanguageChanged()
		{
			cbControlCC.Text = TranslateString("Activate on Ctrl+C+C hotkey");
			cbControlInsIns.Text = TranslateString("Activate on Ctrl+Ins+Ins hotkey");
			cbTranslateOnHotkey.Text = TranslateString("Translate when activated by hotkey");
		}
		
		HookOptions current;
		public override void Init()
		{
			current = TranslateOptions.Instance.HookOptions;
			cbControlCC.Checked = current.ControlCC;
			cbControlInsIns.Checked = current.ControlInsIns;
			cbTranslateOnHotkey.Checked = current.TranslateOnHotkey;
		}
		
		public override void Apply()
		{
			current.ControlCC = cbControlCC.Checked;
			current.ControlInsIns = cbControlInsIns.Checked;
			current.TranslateOnHotkey = cbTranslateOnHotkey.Checked;
			KeyboardHook.Init();
		}
		
		public override bool IsChanged()
		{
			return 
				current.ControlCC != cbControlCC.Checked ||
				current.ControlInsIns != cbControlInsIns.Checked ||
				current.TranslateOnHotkey != cbTranslateOnHotkey.Checked;
			
		}
		
	}
}

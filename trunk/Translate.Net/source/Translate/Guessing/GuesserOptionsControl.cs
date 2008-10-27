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
using System.Drawing;
using System.Windows.Forms;

namespace Translate
{
	/// <summary>
	/// Description of GuesserOptionsControl.
	/// </summary>
	public partial class GuesserOptionsControl : FreeCL.Forms.BaseOptionsControl
	{
		public GuesserOptionsControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			RegisterLanguageEvent(OnLanguageChanged);
		}
		
		void OnLanguageChanged()
		{
			cbLanguageDetection.Text = TranslateString("Language detection");
			toolTip.SetToolTip(cbLanguageDetection, TranslateString("Detect language of source text"));
			gbIntillegentSwitching.Text = TranslateString("Intelligent switching of profiles and directions");

			cbSwitchDirectionBasedOnLayout.Text = TranslateString("Based on keyboard layout");
			toolTip.SetToolTip(cbSwitchDirectionBasedOnLayout, TranslateString("Intelligent switching of profiles and translation directions based on keyboard layout"));
			
			cbSwitchDirectionBasedOnLanguage.Text = TranslateString("Based on detected language");
			toolTip.SetToolTip(cbSwitchDirectionBasedOnLanguage, TranslateString("Intelligent switching of profiles and translation directions based on detected language"));
		}
		
		GuessingOptions current;
		public override void Init()
		{
			current = TranslateOptions.Instance.GuessingOptions;
			cbLanguageDetection.Checked = current.Enabled;
			cbSwitchDirectionBasedOnLayout.Checked = current.SwitchDirectionBasedOnLayout;
			cbSwitchDirectionBasedOnLanguage.Checked = current.SwitchDirectionBasedOnLanguage;
		}
		
		public override void Apply()
		{
			current.Enabled = cbLanguageDetection.Checked;
			current.SwitchDirectionBasedOnLayout = cbSwitchDirectionBasedOnLayout.Checked;
			current.SwitchDirectionBasedOnLanguage = cbSwitchDirectionBasedOnLanguage.Checked;
			
			(TranslateMainForm.Instance as TranslateMainForm).OnGuessingOptionChanhing();
		}
		
		public override bool IsChanged()
		{
			return current.Enabled != cbLanguageDetection.Checked || 
				current.SwitchDirectionBasedOnLayout != cbSwitchDirectionBasedOnLayout.Checked ||
				current.SwitchDirectionBasedOnLanguage != cbSwitchDirectionBasedOnLanguage.Checked;
		}		
	}
}

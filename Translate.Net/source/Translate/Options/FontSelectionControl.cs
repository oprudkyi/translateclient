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
using System.Drawing.Text;
using FreeCL.RTL;
using System.Collections.Generic;

namespace Translate
{
	/// <summary>
	/// Description of FontSelectionControl.
	/// </summary>
	public partial class FontSelectionControl : FreeCL.Forms.BaseUserControl
	{
		public FontSelectionControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			for (int i = 6; i <= 24; ++i) 
			{
				cbFontSize.Items.Add(i);
			}		
			InstalledFontCollection installedFontCollection = new InstalledFontCollection();
			List<string> fonts = new List<string>();
			foreach (FontFamily fontFamily in installedFontCollection.Families)
			{
				if (fontFamily.IsStyleAvailable(FontStyle.Regular) && 
						fontFamily.IsStyleAvailable(FontStyle.Bold)  && 
						fontFamily.IsStyleAvailable(FontStyle.Italic)
					)
					fonts.Add(fontFamily.Name);
			}
			fonts.Sort();
			cbFontName.Items.AddRange(fonts.ToArray());
			
			
			RegisterLanguageEvent(OnLanguageChanged);
		}
		
		void OnLanguageChanged()
		{
			lName.Text = LangPack.TranslateString("Name") + " :";
			lSize.Text = LangPack.TranslateString("Size") + " :";
			cbSystem.Text = LangPack.TranslateString("Default font");
			toolTip.SetToolTip(cbSystem, 
				LangPack.TranslateString("Name") + " :" + defaultFont.Name + ", " +
				LangPack.TranslateString("Size") + " :" + defaultFont.Size.ToString()
				);
		}
		
		public Font CurrentFont
		{
			get
			{
				return lTest.Font;
			}
			
			set
			{
				lTest.Font = value;
				cbFontName.SelectedItem = value.Name;
				cbFontSize.Text = value.Size.ToString();
			}
		}
		
		Font defaultFont = SystemFonts.DefaultFont;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Font DefaultFont {
			get { return defaultFont; }
			set { 
					defaultFont = value; 
					toolTip.SetToolTip(cbSystem, 
						LangPack.TranslateString("Name") + " :" + defaultFont.Name + ", " +
						LangPack.TranslateString("Size") + " :" + defaultFont.Size.ToString()
						);
				}
		}
		
		public void Init()
		{
			updating = true;
			cbSystem.Checked = FontSelectionControl.FontEquals(defaultFont, CurrentFont);
			updating = false;
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string DemoText
		{
			get
			{
				return lTest.Text;
			}
			
			set
			{
				lTest.Text = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SystemFont
		{
			get
			{
				return cbSystem.Checked;
			}
			
			set
			{
				cbSystem.Checked = value;
			}
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Font GetSelectedFont()
		{
			float fontSize = 10f;
			try 
			{
				fontSize = Math.Max(6, Single.Parse(cbFontSize.Text));
			} 
			catch (Exception) {}
			
			return new Font((string)cbFontName.SelectedItem,
			                fontSize);
		}		
		
		void UpdateFont()
		{
			Font currentFont = GetSelectedFont();
			lTest.Font = currentFont;
			updating = true;
			cbSystem.Checked = FontSelectionControl.FontEquals(defaultFont,currentFont);
			updating = false;
		}
		
		void CbFontNameTextChanged(object sender, EventArgs e)
		{
			UpdateFont();
		}
		
		void CbFontSizeTextChanged(object sender, EventArgs e)
		{
			UpdateFont();
		}
		
		void CbFontNameSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateFont();
		}
		
		bool updating = false;
		void CbSystemCheckedChanged(object sender, EventArgs e)
		{
			if(cbSystem.Checked)
			{
				if(!updating)
				{
					CurrentFont = defaultFont;
				}	
				cbFontName.Enabled = false;
				cbFontSize.Enabled = false;
			}
			else
			{
				cbFontName.Enabled = true;
				cbFontSize.Enabled = true;
			}
		}
		
		public static bool FontEquals(Font x, Font y)
		{
			return x.Name == y.Name && x.SizeInPoints == y.SizeInPoints;
		}
	}
}

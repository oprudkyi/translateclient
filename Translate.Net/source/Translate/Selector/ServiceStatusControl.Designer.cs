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

namespace Translate
{
	partial class ServiceStatusControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				FreeCL.RTL.WebBrowserHelper.ResetBatch(wbStatus);
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.wbStatus = new System.Windows.Forms.WebBrowser();
			this.gEvents = new FreeCL.UI.GlobalEvents(this.components);
			this.SuspendLayout();
			// 
			// wbStatus
			// 
			this.wbStatus.AllowWebBrowserDrop = false;
			this.wbStatus.IsWebBrowserContextMenuEnabled = false;
			this.wbStatus.Location = new System.Drawing.Point(-2, -2);
			this.wbStatus.Margin = new System.Windows.Forms.Padding(0);
			this.wbStatus.MinimumSize = new System.Drawing.Size(20, 20);
			this.wbStatus.Name = "wbStatus";
			this.wbStatus.ScrollBarsEnabled = false;
			this.wbStatus.Size = new System.Drawing.Size(192, 126);
			this.wbStatus.TabIndex = 0;
			this.wbStatus.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WbStatusNavigating);
			this.wbStatus.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WbStatusDocumentCompleted);
			// 
			// gEvents
			// 
			this.gEvents.Idle += new System.EventHandler(this.GEventsIdle);
			// 
			// ServiceStatusControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.wbStatus);
			this.Name = "ServiceStatusControl";
			this.Size = new System.Drawing.Size(192, 126);
			this.Load += new System.EventHandler(this.ServiceStatusControlLoad);
			this.Resize += new System.EventHandler(this.ServiceStatusControlResize);
			this.ResumeLayout(false);
		}
		private FreeCL.UI.GlobalEvents gEvents;
		private System.Windows.Forms.WebBrowser wbStatus;
	}
}

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
 * Portions created by the Initial Developer are Copyright (C) 2005-2008
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FreeCL.RTL
{
	/// <summary>
	/// Description of Memory.	
	/// </summary>
	public static class Memory
	{
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId="0#")]
		public static void DisposeAndNull(ref System.ComponentModel.Container obj)
		{
			if(obj != null)
			{
				obj.Dispose();
				obj = null;
			}
		}
		

		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId="0#")]		
		public static void DisposeAndNull(ref IDisposable obj)
		{
			if(obj != null)
			{
				obj.Dispose();
				obj = null;
			}
		}


		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId="0#")]
		public static void DisposeAndNull(ref System.ComponentModel.IContainer obj)
		{
			if(obj != null)
			{
				obj.Dispose();
				obj = null;
			}
		}

		public static void Dispose(System.Windows.Forms.Control.ControlCollection controls)
		{
			if(controls == null) return; 
			
			while(controls.Count > 0)
			{
				System.Windows.Forms.Control ctrl = controls[0];
				if(ctrl != null)
				{
					ctrl.Dispose(); 
				}
					
			}
		}

		public static void Dispose(System.ComponentModel.IContainer container)
		{
			if(container == null) return; 
			
			if(!MonoHelper.IsUnix)
			{
				while(container.Components.Count > 0)
				{
					object ctrl = container.Components[0];
					if(ctrl != null)
					{
						Console.WriteLine(ctrl.ToString());
						DisposeAndNull(ref ctrl);
					}
				}
			}
			else
			{  //TODO: here a bug in mono, because dsposing of childs of IContainer don't remove items from it  
				for(int i = 0; i < container.Components.Count; i++)
				{
					object ctrl = container.Components[i];
					if(ctrl != null)
					{
						DisposeAndNull(ref ctrl);
					}
				}
			}
		}

		public static void Dispose(System.Windows.Forms.Form[] forms)
		{
			if(forms == null) return; 
		
			for(int i = 0; i < forms.Length; i++)
			{
				System.Windows.Forms.Form ctrl = forms[i];
				if(ctrl != null)
				{
					ctrl.Dispose(); 
				}
					
			}
		}
		
		public static void DisposeChilds(System.Windows.Forms.Control ctrl)
		{
			if(ctrl == null) return;
			System.Windows.Forms.Form frm = ctrl as System.Windows.Forms.Form;
			if(frm != null)	
			{
				Dispose(frm.MdiChildren); 
			}
			Dispose(ctrl.Controls);
		}
		


		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId="0#")]
		[SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate")]		
		public static void DisposeAndNull(ref object obj)
		{
			if(obj != null && obj is IDisposable)
			{
				(obj as IDisposable).Dispose();
				obj = null;
			}
		}
	}
}


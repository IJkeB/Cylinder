/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2015
 *	
 *	"GVar.cs"
 * 
 *	This script is a data class for project-wide variables.
 * 
 */

using UnityEngine;

namespace AC
{

	/**
	 * A data class for global and local variables.
	 * Variables are created in the Variables Manager asset file, and copied to the RuntimeVariables component when the game begins.
	 */
	[System.Serializable]
	public class GVar
	{

		/** It's editor name. */
		public string label;
		/** It's internal ID number. */
		public int id;
		/** It's variable type. */
		public VariableType type;
		/** It's value, if an integer, popup or boolean. If a boolean, 0 = False, and 1 = True. */
		public int val;
		/** It's value, if a float. */
		public float floatVal;
		/** It's value, if a string. */
		public string textVal;
		/** An array of labels, if a popup. */
		public string[] popUps;
		/** What it links to.  A Variable can link to Options Data, or a PlayMaker Global Variable. */
		public VarLink link = VarLink.None;
		/** If linked to a PlayMaker Global Variable, the name of the PM variable. */
		public string pmVar;
		/** If True and linked to a PlayMaker Global Variable, then PM will be referred to for the initial value. */
		public bool updateLinkOnStart = false;
		/** True if the variable is currently selected in the Variable Manager. */
		public bool isEditing = false;

		private float backupFloatVal;
		private int backupVal;


		public GVar ()
		{}
		
		
		/**
		 * The main Constructor.
		 * An array of ID numbers is required, to ensure it's own ID is unique.
		 */
		public GVar (int[] idArray)
		{
			val = 0;
			floatVal = 0f;
			textVal = "";
			type = VariableType.Boolean;
			id = 0;
			link = VarLink.None;
			pmVar = "";
			popUps = null;
			updateLinkOnStart = false;
			backupVal = 0;
			backupFloatVal = 0f;
			
			// Update id based on array
			foreach (int _id in idArray)
			{
				if (id == _id)
				{
					id ++;
				}
			}
			
			label = "Variable " + (id + 1).ToString ();
		}
		

		/**
		 * A Constructor that copies all values from another variable.
		 * This way ensures that no connection remains to the asset file.
		 */
		public GVar (GVar assetVar)
		{
			val = assetVar.val;
			floatVal = assetVar.floatVal;
			textVal = assetVar.textVal;
			type = assetVar.type;
			id = assetVar.id;
			label = assetVar.label;
			link = assetVar.link;
			pmVar = assetVar.pmVar;
			popUps = assetVar.popUps;
			updateLinkOnStart = assetVar.updateLinkOnStart;
			backupVal = assetVar.val;
			backupFloatVal = assetVar.floatVal;
		}
		
		
		/**
		 * Sets its value to that of it's linked PlayMaker variable (if appropriate).
		 */
		public void Download ()
		{
			if (link == VarLink.PlaymakerGlobalVariable && pmVar != "")
			{
				if (!PlayMakerIntegration.IsDefinePresent ())
				{
					return;
				}
				
				if (type == VariableType.Integer || type == VariableType.PopUp)
				{
					val = PlayMakerIntegration.GetGlobalInt (pmVar);
				}
				else if (type == VariableType.Boolean)
				{
					bool _val = PlayMakerIntegration.GetGlobalBool (pmVar);
					if (_val)
					{
						val = 1;
					}
					else
					{
						val = 0;
					}
				}
				else if (type == VariableType.String)
				{
					textVal = PlayMakerIntegration.GetGlobalString (pmVar);
				}
				else if (type == VariableType.Float)
				{
					floatVal = PlayMakerIntegration.GetGlobalFloat (pmVar);
				}
			}
		}
		
		
		/**
		 * Sets the value of it's linked Options Data or PlayMaker variable to its value (if appropriate).
		 */
		public void Upload ()
		{
			if (link == VarLink.PlaymakerGlobalVariable && pmVar != "")
			{
				if (!PlayMakerIntegration.IsDefinePresent ())
				{
					return;
				}
				
				if (type == VariableType.Integer || type == VariableType.PopUp)
				{
					PlayMakerIntegration.SetGlobalInt (pmVar, val);
				}
				else if (type == VariableType.Boolean)
				{
					if (val == 1)
					{
						PlayMakerIntegration.SetGlobalBool (pmVar, true);
					}
					else
					{
						PlayMakerIntegration.SetGlobalBool (pmVar, false);
					}
				}
				else if (type == VariableType.String)
				{
					PlayMakerIntegration.SetGlobalString (pmVar, textVal);
				}
				else if (type == VariableType.Float)
				{
					PlayMakerIntegration.SetGlobalFloat (pmVar, floatVal);
				}
			}
			else if (link == VarLink.OptionsData)
			{
				Options.SavePrefs ();
			}
		}
		

		/**
		 * Backs up it's value.
		 * Necessary when skipping ActionLists that involve checking variable values.
		 */
		public void BackupValue ()
		{
			backupVal = val;
			backupFloatVal = floatVal;
		}
		
		
		/**
		 * Restores it's value from backup. 
		 * Necessary when skipping ActionLists that involve checking variable values.
		 */
		public void RestoreBackupValue ()
		{
			val = backupVal;
			floatVal = backupFloatVal;
		}
		
		
		/**
		 * Sets the value if it's type is String.
		 */
		public void SetStringValue (string newValue)
		{
			textVal = newValue;
		}
		
		
		/**
		 * <summary>Sets the value if it's type is Float.</summary>
		 * <param name = "newValue">The new float value</param>
		 * <param name = "setVarMethod">How the new value affects the old (replaces, increases by, or randomises)</param>
		 */
		public void SetFloatValue (float newValue, SetVarMethod setVarMethod = SetVarMethod.SetValue)
		{
			if (setVarMethod == SetVarMethod.IncreaseByValue)
			{
				floatVal += newValue;
			}
			else if (setVarMethod == SetVarMethod.SetAsRandom)
			{
				floatVal = Random.Range (0f, newValue);
			}
			else
			{
				floatVal = newValue;
			}
		}
		

		/**
		 * <summary>Sets the value if it's type is Integer, Boolean or PopUp.</summary>
		 * <param name = "newValue">The new integer value</param>
		 * <param name = "setVarMethod">How the new value affects the old (replaces, increases by, or randomises)</param>
		 */
		public void SetValue (int newValue, SetVarMethod setVarMethod = SetVarMethod.SetValue)
		{
			if (setVarMethod == SetVarMethod.IncreaseByValue)
			{
				val += newValue;
			}
			else if (setVarMethod == SetVarMethod.SetAsRandom)
			{
				val = Random.Range (0, newValue);
			}
			else
			{
				val = newValue;
			}
			
			if (type == VariableType.Boolean)
			{
				if (val > 0)
				{
					val = 1;
				}
				else
				{
					val = 0;
				}
			}
			else if (type == VariableType.PopUp)
			{
				if (val < 0)
				{
					val = 0;
				}
				else if (val >= popUps.Length)
				{
					val = popUps.Length - 1;
				}
			}
		}
		
		
		/**
		 * <summary>Returns the variable's value.</summary>
		 * <returns>The value, as a formatted string.</returns>
		 */
		public string GetValue ()
		{
			if (type == VariableType.Integer)
			{
				return val.ToString ();
			}
			else if (type == VariableType.PopUp)
			{
				if (popUps == null || popUps.Length == 0) return "";
				val = Mathf.Max (0, val);
				val = Mathf.Min (val, popUps.Length-1);
				return popUps [val];
			}
			else if (type == VariableType.String)
			{
				return textVal;
			}
			else if (type == VariableType.Float)
			{
				return floatVal.ToString ();
			}
			else
			{
				if (val == 0)
				{
					return "False";
				}
				else
				{
					return "True";
				}
			}
		}
		
	}

}
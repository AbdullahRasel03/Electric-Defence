using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace BuildReportTool.Window.Screen
{
	public class BuildStepsScreen : BaseScreen
	{
		public override string Name { get { return "Build Steps";  } }

		public override void RefreshData(BuildInfo buildReport, AssetDependencies assetDependencies, TextureData textureData,
			UnityBuildReport unityBuildReport)
		{
			if (unityBuildReport != null)
			{
				SelectStep(-1, unityBuildReport.BuildProcessSteps);
			}
		}

		Vector2 _scrollPos;
		Vector2 _logMessagesScrollPos;
		Texture _indentLine;

		int _selectedStepIdx = -1;
		int _selectedLogStepIdx = -1;
		int _selectedLogIdx = -1;

		Texture2D _logIcon;
		Texture2D _warnIcon;
		Texture2D _errorIcon;

		readonly GUIContent _logFilterLabel = new GUIContent("0");
		readonly GUIContent _warnFilterLabel = new GUIContent("0");
		readonly GUIContent _errorFilterLabel = new GUIContent("0");

		Rect _stepsViewRect;

		bool _showLogMessagesCollapsed;
		bool _showLogMessages = true;
		bool _showWarnMessages = true;
		bool _showErrorMessages = true;

		struct LogMsgIdx
		{
			public int StepIdx;
			public int LogIdx;
		}

		static LogMsgIdx MakeLogMsgIdx(int stepIdx, int logIdx)
		{
			LogMsgIdx newEntry;
			newEntry.StepIdx = stepIdx;
			newEntry.LogIdx = logIdx;
			return newEntry;
		}

		readonly Dictionary<LogMsgIdx, Rect> _logRects = new Dictionary<LogMsgIdx, Rect>();

		public override void DrawGUI(Rect position, BuildInfo buildReportToDisplay, AssetDependencies assetDependencies,
			TextureData textureData, UnityBuildReport unityBuildReport, out bool requestRepaint)
		{
			requestRepaint = false;
			if (unityBuildReport == null)
			{
				return;
			}

			if (_logIcon == null)
			{
				var logIcons = GUI.skin.FindStyle("LogMessageIcons");
				if (logIcons != null)
				{
					_logIcon = logIcons.normal.background;
					_warnIcon = logIcons.hover.background;
					_errorIcon = logIcons.active.background;

					_logFilterLabel.image = _logIcon;
					_warnFilterLabel.image = _warnIcon;
					_errorFilterLabel.image = _errorIcon;
				}
			}

			if (_indentLine == null)
			{
				_indentLine = GUI.skin.GetStyle("IndentStyle1").normal.background;
			}

			var steps = unityBuildReport.BuildProcessSteps;
			if (steps == null)
			{
				return;
			}

			// --------------------------------

			#region Steps
			GUILayout.BeginHorizontal();

			#region Column 1
			GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);
			GUILayout.Label("Step", BuildReportTool.Window.Settings.LIST_COLUMN_HEADER_STYLE_NAME);
			_scrollPos = GUILayout.BeginScrollView(_scrollPos,
				BuildReportTool.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME,
				BuildReportTool.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME, BRT_BuildReportWindow.LayoutNone);

			bool useAlt = true;
			for (int i = 0; i < steps.Length; ++i)
			{
				string styleToUse = useAlt
					                    ? BuildReportTool.Window.Settings.LIST_SMALL_ALT_STYLE_NAME
					                    : BuildReportTool.Window.Settings.LIST_SMALL_STYLE_NAME;
				if (i == _selectedStepIdx)
				{
					styleToUse = BuildReportTool.Window.Settings.LIST_SMALL_SELECTED_NAME;
				}

				GUILayout.BeginHorizontal(styleToUse);
				GUILayout.Space(steps[i].Depth * 20);
				if (GUILayout.Button(steps[i].Name, styleToUse, BRT_BuildReportWindow.LayoutListHeight))
				{
					SelectStep(i, steps);
				}
				GUILayout.EndHorizontal();
				if (Event.current.type == EventType.Repaint)
				{
					Rect labelRect = GUILayoutUtility.GetLastRect();

					var prevColor = GUI.color;
					GUI.color = new Color(0, 0, 0, 0.5f);
					for (int indentN = 0, indentLen = steps[i].Depth;
					     indentN < indentLen;
					     ++indentN)
					{
						var indentRect = new Rect((indentN * 20), labelRect.y, 20, labelRect.height);
						GUI.DrawTexture(indentRect, _indentLine, ScaleMode.ScaleAndCrop);
					}

					GUI.color = prevColor;
				}

				useAlt = !useAlt;
			}

			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			#endregion

			#region Column 2
			GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);
			GUILayout.Label("Warning Count", BuildReportTool.Window.Settings.LIST_COLUMN_HEADER_STYLE_NAME);
			_scrollPos = GUILayout.BeginScrollView(_scrollPos,
				BuildReportTool.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME,
				BuildReportTool.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME, BRT_BuildReportWindow.LayoutNone);
			useAlt = true;
			for (int i = 0; i < steps.Length; ++i)
			{
				string styleToUse = useAlt
					                    ? BuildReportTool.Window.Settings.LIST_SMALL_ALT_STYLE_NAME
					                    : BuildReportTool.Window.Settings.LIST_SMALL_STYLE_NAME;
				if (i == _selectedStepIdx)
				{
					styleToUse = BuildReportTool.Window.Settings.LIST_SMALL_SELECTED_NAME;
				}

				if (steps[i].WarningCount > 0)
				{
					if (GUILayout.Button(steps[i].WarningCount.ToString(), styleToUse, BRT_BuildReportWindow.LayoutListHeight))
					{
						SelectStep(i, steps);
					}
				}
				else
				{
					GUILayout.Label(GUIContent.none, styleToUse, BRT_BuildReportWindow.LayoutListHeight);
				}
				useAlt = !useAlt;
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			#endregion

			#region Column 3
			GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);
			GUILayout.Label("Error Count", BuildReportTool.Window.Settings.LIST_COLUMN_HEADER_STYLE_NAME);
			_scrollPos = GUILayout.BeginScrollView(_scrollPos,
				BuildReportTool.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME,
				BuildReportTool.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME, BRT_BuildReportWindow.LayoutNone);
			useAlt = true;
			for (int i = 0; i < steps.Length; ++i)
			{
				string styleToUse = useAlt
					                    ? BuildReportTool.Window.Settings.LIST_SMALL_ALT_STYLE_NAME
					                    : BuildReportTool.Window.Settings.LIST_SMALL_STYLE_NAME;
				if (i == _selectedStepIdx)
				{
					styleToUse = BuildReportTool.Window.Settings.LIST_SMALL_SELECTED_NAME;
				}

				if (steps[i].ErrorCount > 0)
				{
					if (GUILayout.Button(steps[i].ErrorCount.ToString(), styleToUse, BRT_BuildReportWindow.LayoutListHeight))
					{
						SelectStep(i, steps);
					}
				}
				else
				{
					GUILayout.Label(GUIContent.none, styleToUse, BRT_BuildReportWindow.LayoutListHeight);
				}
				useAlt = !useAlt;
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			#endregion

			#region Last Column
			GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);
			GUILayout.Label("Duration", BuildReportTool.Window.Settings.LIST_COLUMN_HEADER_STYLE_NAME);
			_scrollPos = GUILayout.BeginScrollView(_scrollPos,
				BuildReportTool.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME,
				"verticalscrollbar", BRT_BuildReportWindow.LayoutNone);


			useAlt = true;
			for (int i = 0; i < steps.Length; ++i)
			{
				string styleToUse = useAlt
					                    ? BuildReportTool.Window.Settings.LIST_SMALL_ALT_STYLE_NAME
					                    : BuildReportTool.Window.Settings.LIST_SMALL_STYLE_NAME;
				if (i == _selectedStepIdx)
				{
					styleToUse = BuildReportTool.Window.Settings.LIST_SMALL_SELECTED_NAME;
				}

				string duration;
				if (i == 0)
				{
					duration = unityBuildReport.TotalBuildTime.ToReadableString();
				}
				else
				{
					duration = steps[i].Duration.ToReadableString();
				}

				GUILayout.Label(duration, styleToUse, BRT_BuildReportWindow.LayoutListHeight);

				useAlt = !useAlt;
			}

			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			#endregion

			GUILayout.EndHorizontal();
			if (Event.current.type == EventType.Repaint)
			{
				_stepsViewRect = GUILayoutUtility.GetLastRect();
			}
			#endregion

			// --------------------------------

			#region Logs
			GUILayout.BeginHorizontal();

			GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

			#region Logs Toolbar
			GUILayout.BeginHorizontal(BuildReportTool.Window.Settings.TOP_BAR_BG_STYLE_NAME, BRT_BuildReportWindow.LayoutHeight18);
			GUILayout.Space(10);
			string logMessagesTitle;
			bool hasStepSelected = _selectedStepIdx != -1 &&
			                       steps[_selectedStepIdx].BuildLogMessages != null &&
			                       steps[_selectedStepIdx].BuildLogMessages.Length > 0;
			if (hasStepSelected)
			{
				logMessagesTitle = string.Format("Log Messages of: <i>{0}</i>", steps[_selectedStepIdx].Name);
			}
			else
			{
				logMessagesTitle = "Log Messages (Total)";
			}
			GUILayout.Label(logMessagesTitle, BuildReportTool.Window.Settings.TOP_BAR_LABEL_STYLE_NAME, BRT_BuildReportWindow.LayoutNoExpandWidth);
			GUILayout.FlexibleSpace();
			_showLogMessagesCollapsed = GUILayout.Toggle(_showLogMessagesCollapsed, "Collapse",
				BuildReportTool.Window.Settings.TOP_BAR_BTN_STYLE_NAME, BRT_BuildReportWindow.LayoutNoExpandWidth);
			GUILayout.Space(8);
			_showLogMessages = GUILayout.Toggle(_showLogMessages, _logFilterLabel,
				BuildReportTool.Window.Settings.TOP_BAR_BTN_STYLE_NAME, BRT_BuildReportWindow.LayoutNoExpandWidth);
			_showWarnMessages = GUILayout.Toggle(_showWarnMessages, _warnFilterLabel,
				BuildReportTool.Window.Settings.TOP_BAR_BTN_STYLE_NAME, BRT_BuildReportWindow.LayoutNoExpandWidth);
			_showErrorMessages = GUILayout.Toggle(_showErrorMessages, _errorFilterLabel,
				BuildReportTool.Window.Settings.TOP_BAR_BTN_STYLE_NAME, BRT_BuildReportWindow.LayoutNoExpandWidth);
			GUILayout.Space(8);
			GUILayout.EndHorizontal();
			#endregion

			_logMessagesScrollPos = GUILayout.BeginScrollView(_logMessagesScrollPos,
				BuildReportTool.Window.Settings.HIDDEN_SCROLLBAR_STYLE_NAME,
				"verticalscrollbar", BRT_BuildReportWindow.LayoutNone);

			if (hasStepSelected)
			{
				BuildLogMessage[] messages;
				if (_showLogMessagesCollapsed)
				{
					messages = steps[_selectedStepIdx].CollapsedBuildLogMessages;
				}
				else
				{
					messages = steps[_selectedStepIdx].BuildLogMessages;
				}

				useAlt = true;
				DrawMessages(messages, _selectedStepIdx, 0, ref useAlt, ref requestRepaint);
			}
			else
			{
				useAlt = true;
				for (int s = 0; s < steps.Length; ++s)
				{
					var step = steps[s];

					BuildLogMessage[] messages;
					if (_showLogMessagesCollapsed)
					{
						messages = step.CollapsedBuildLogMessages;
					}
					else
					{
						messages = step.BuildLogMessages;
					}

					if (messages == null || messages.Length == 0)
					{
						continue;
					}

					string styleToUse = useAlt
						                    ? BuildReportTool.Window.Settings.LIST_SMALL_ALT_STYLE_NAME
						                    : BuildReportTool.Window.Settings.LIST_SMALL_STYLE_NAME;

					GUILayout.BeginHorizontal(styleToUse);
					GUILayout.Space(8);
					GUILayout.Button(step.Name, styleToUse, BRT_BuildReportWindow.LayoutHeight25);
					GUILayout.EndHorizontal();

					useAlt = !useAlt;

					DrawMessages(messages, s, 20, ref useAlt, ref requestRepaint);
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();
			#endregion

			// if clicked on nothing interactable, then remove selection
			if (GUI.Button(_stepsViewRect, GUIContent.none, "HiddenScrollbar"))
         {
	         SelectStep(-1, steps);
         }
		}

		void DrawMessages(BuildLogMessage[] messages, int stepIdx, int leftIndent, ref bool useAlt, ref bool requestRepaint)
		{
			GUISkin nativeSkin =
				EditorGUIUtility.GetBuiltinSkin(EditorGUIUtility.isProSkin ? EditorSkin.Scene : EditorSkin.Inspector);
			var logCountStyle = nativeSkin.FindStyle("CN CountBadge");

			for (int m = 0; m < messages.Length; ++m)
			{
				var logTypeIcon = GetLogIcon(messages[m].LogType);
				if (logTypeIcon == _logIcon && !_showLogMessages)
				{
					continue;
				}
				if (logTypeIcon == _warnIcon && !_showWarnMessages)
				{
					continue;
				}
				if (logTypeIcon == _errorIcon && !_showErrorMessages)
				{
					continue;
				}

				string logStyleToUse = useAlt
					                    ? BuildReportTool.Window.Settings.LIST_NORMAL_ALT_STYLE_NAME
					                    : BuildReportTool.Window.Settings.LIST_NORMAL_STYLE_NAME;
				string logMessageStyleToUse = "Text";
				if (stepIdx == _selectedLogStepIdx && m == _selectedLogIdx)
				{
					logStyleToUse = BuildReportTool.Window.Settings.LIST_NORMAL_SELECTED_NAME;
					logMessageStyleToUse = "TextSelected";
				}

				GUILayout.BeginHorizontal(logStyleToUse, BRT_BuildReportWindow.LayoutMinHeight30);
				GUILayout.Space(leftIndent);
				GUILayout.Label(logTypeIcon, "DrawTexture", BRT_BuildReportWindow.Layout20x16);
				GUILayout.Label(messages[m].Message, logMessageStyleToUse);

				if (_showLogMessagesCollapsed && messages[m].Count > 0)
				{
					GUILayout.FlexibleSpace();
					GUILayout.Label(messages[m].Count.ToString(), logCountStyle, BRT_BuildReportWindow.LayoutNoExpandWidth);
				}

				GUILayout.EndHorizontal();

				var logMsgIdx = MakeLogMsgIdx(stepIdx, m);
				if (Event.current.type == EventType.Repaint)
				{
					if (_logRects.ContainsKey(logMsgIdx))
					{
						_logRects[logMsgIdx] = GUILayoutUtility.GetLastRect();
					}
					else
					{
						_logRects.Add(logMsgIdx, GUILayoutUtility.GetLastRect());
					}
				}

				if (_logRects.ContainsKey(logMsgIdx) &&
				    _logRects[logMsgIdx].Contains(Event.current.mousePosition) &&
				    Event.current.type == EventType.MouseDown)
				{
					requestRepaint = true;
					SelectLogMessage(stepIdx, m);

					if (Event.current.clickCount == 2)
					{
						if (messages[m].Message.StartsWith("Script attached to '"))
						{
							SearchPrefabFromLog(messages[m].Message);
						}
						else
						{
							OpenScriptFromLog(messages[m].Message);
						}
					}
				}
				useAlt = !useAlt;
			}
		}

		void SelectStep(int stepIdx, BuildProcessStep[] steps)
		{
			_selectedStepIdx = stepIdx;

			// count info, warn, and error messages
			int infoCount = 0;
			int warnCount = 0;
			int errorCount = 0;
			if (_selectedStepIdx > -1 && steps[_selectedStepIdx].BuildLogMessages != null && steps[_selectedStepIdx].BuildLogMessages.Length > 0)
			{
				CountLogTypes(steps[_selectedStepIdx].BuildLogMessages, ref infoCount, ref warnCount, ref errorCount);
			}
			else
			{
				for (int i = 0; i < steps.Length; ++i)
				{
					CountLogTypes(steps[i].BuildLogMessages, ref infoCount, ref warnCount, ref errorCount);
				}
			}

			_logFilterLabel.text = infoCount.ToString();
			_warnFilterLabel.text = warnCount.ToString();
			_errorFilterLabel.text = errorCount.ToString();
		}

		static void CountLogTypes(BuildLogMessage[] messages, ref int infoCount, ref int warnCount, ref int errorCount)
		{
			if (messages == null)
			{
				return;
			}

			for (int i = 0; i < messages.Length; ++i)
			{
				var logType = GetLogType(messages[i].LogType);

				switch (logType)
				{
					case LogType.Info:
						++infoCount;
						break;
					case LogType.Warn:
						++warnCount;
						break;
					case LogType.Error:
						++errorCount;
						break;
				}
			}
		}

		void SelectLogMessage(int stepIdx, int logMessageIdx)
		{
			_selectedLogStepIdx = stepIdx;
			_selectedLogIdx = logMessageIdx;

		}

		enum LogType
		{
			Info,
			Warn,
			Error,
		}

		static LogType GetLogType(string logType)
		{
			if (logType.Contains("Warn"))
			{
				return LogType.Warn;
			}
			else if (logType.Contains("Log"))
			{
				return LogType.Info;
			}
			else
			{
				return LogType.Error;
			}
		}

		Texture2D GetLogIcon(string logType)
		{
			if (logType.Contains("Warn"))
			{
				return _warnIcon;
			}
			else if (logType.Contains("Log"))
			{
				return _logIcon;
			}
			else
			{
				return _errorIcon;
			}
		}

		static void OpenScriptFromLog(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}

			int lineNumIdx = message.IndexOf(".cs(", StringComparison.OrdinalIgnoreCase);
			if (lineNumIdx < 0)
			{
				return;
			}
			lineNumIdx += 4;
			int lineNumEndIdx = message.IndexOf(",", lineNumIdx, StringComparison.OrdinalIgnoreCase);

			string filename = message.Substring(0, lineNumIdx - 1);
			string lineNumText = message.Substring(lineNumIdx, lineNumEndIdx - lineNumIdx);
			//Debug.Log(string.Format("filename: {0} lineNumText: {1}", filename, lineNumText));

			int line = int.Parse(lineNumText);
			UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filename, line);
		}


		static void SearchPrefabFromLog(string message)
		{
			if (!message.StartsWith("Script attached to '"))
			{
				return;
			}

			int lastQuote = message.IndexOf("'", 20, StringComparison.OrdinalIgnoreCase);
			if (lastQuote > -1)
			{
				string prefabName = message.Substring(20, lastQuote - 20);
				//Debug.Log(prefabName);
				SearchPrefab(prefabName);
			}
		}

		/// <summary>
		/// <see cref="UnityEditor.ProjectBrowser"/>
		/// </summary>
		static readonly System.Type ProjectBrowserType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");

		/// <summary>
		/// <see cref="UnityEditor.ProjectBrowser.SetSearch(string)"/>
		/// </summary>
		static readonly System.Reflection.MethodInfo ProjectBrowserSetSearchMethod = ProjectBrowserType.GetMethod("SetSearch",
			System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
			null, CallingConventions.Any, new[]{typeof(string)}, null);

		/// <summary>
		/// <see cref="UnityEditor.ProjectBrowser.SelectAll()"/>
		/// </summary>
		static readonly System.Reflection.MethodInfo ProjectBrowserSelectAllMethod = ProjectBrowserType.GetMethod("SelectAll",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

		static void SearchPrefab(string prefabName)
		{
			if (ProjectBrowserType == null)
			{
				return;
			}

			var projectWindow = UnityEditor.EditorWindow.GetWindow(ProjectBrowserType, false, "Project", true);
			if (projectWindow == null)
			{
				return;
			}

			if (ProjectBrowserSetSearchMethod == null)
			{
				return;
			}
			ProjectBrowserSetSearchMethod.Invoke(projectWindow, new object[] { prefabName });

			if (ProjectBrowserSelectAllMethod != null)
			{
				ProjectBrowserSelectAllMethod.Invoke(projectWindow, null);
			}
		}
	}
}
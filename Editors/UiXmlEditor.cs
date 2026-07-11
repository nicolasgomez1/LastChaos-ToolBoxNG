using System.Diagnostics;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LastChaos_ToolBoxNG
{
	public class UiXmlEditor : Form
	{
		private static readonly HashSet<string> ItemCollectionNormalIds = new(StringComparer.OrdinalIgnoreCase)
		{
			"cb_condition",
			"text_condition_title",
			"text_search_name",
			"btn_search",
			"edit_search",
			"tab_theme_and_search",
			"tab_summary_and_theme",
			"text_title"
		};

		private static readonly HashSet<string> ItemCollectionEquipmentIds = new(StringComparer.OrdinalIgnoreCase)
		{
			"text_equipment_title",
			"base_equipment_panel"
		};

		private readonly Main pMain;
		private readonly ComboBox cbFiles = new();
		private readonly Button btnOpen = new();
		private readonly Button btnReload = new();
		private readonly Button btnSave = new();
		private readonly CheckBox cbSavePopup = new();
		private readonly Button btnOpenFolder = new();
		private readonly Button btnOpenText = new();
		private readonly CheckBox cbShowHidden = new();
		private readonly CheckBox cbEquipmentPreview = new();
		private readonly CheckBox cbShowTextureNames = new();
		private readonly ComboBox cbDepthFilter = new();
		private readonly NumericUpDown nudZoom = new();
		private readonly TreeView tvElements = new();
		private readonly UiCanvas canvas;
		private readonly Label lblStatus = new();
		private readonly TextBox tbTag = new();
		private readonly TextBox tbId = new();
		private readonly TextBox tbStr = new();
		private readonly TextBox tbTex = new();
		private readonly TextBox tbColor = new();
		private readonly TextBox tbNewAttribute = new();
		private readonly ComboBox cbAnchorH = new();
		private readonly ComboBox cbAnchorV = new();
		private readonly ComboBox cbTextAlign = new();
		private readonly Button btnPickTexture = new();
		private readonly Button btnPickColor = new();
		private readonly Panel pnlColorPreview = new();
		private readonly Label lblFriendlyHelp = new();
		private readonly CheckBox cbQuickHide = new();
		private readonly Button btnQuickDuplicate = new();
		private readonly Button btnQuickDelete = new();
		private readonly NumericUpDown nudX = new();
		private readonly NumericUpDown nudY = new();
		private readonly NumericUpDown nudW = new();
		private readonly NumericUpDown nudH = new();
		private readonly NumericUpDown nudL = new();
		private readonly NumericUpDown nudT = new();
		private readonly NumericUpDown nudR = new();
		private readonly NumericUpDown nudB = new();
		private readonly CheckBox cbHide = new();
		private readonly ComboBox cbAddType = new();
		private readonly Button btnAddChild = new();
		private readonly Button btnDuplicate = new();
		private readonly Button btnDelete = new();
		private readonly Button btnAddAttribute = new();
		private readonly Button btnRemoveAttribute = new();
		private readonly DataGridView dgvAttributes = new();
		private readonly Dictionary<XElement, TreeNode> treeNodesByElement = new();

		private XDocument? document;
		private UiNode? rootNode;
		private UiNode? selectedNode;
		private string? currentFile;
		private bool loadingControls;
		private bool dirty;
		private int canvasDepthFilter = -1;
		private DateTime lastSavedAt = DateTime.MinValue;

		public UiXmlEditor(Main mainForm)
		{
			pMain = mainForm;
			canvas = new UiCanvas(this);

			Text = "UI XML Editor";
			Name = "UiXmlEditor";
			MinimumSize = new Size(1160, 900);
			Size = new Size(1360, 980);
			StartPosition = FormStartPosition.CenterParent;
			KeyPreview = true;

			BuildInterface();

			Load += (_, _) => LoadFileList();
			FormClosing += UiXmlEditor_FormClosing;
		}

		private void BuildInterface()
		{
			TableLayoutPanel root = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 3,
				Padding = new Padding(8)
			};
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
			root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
			Controls.Add(root);

			FlowLayoutPanel toolbar = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			root.Controls.Add(toolbar, 0, 0);

			toolbar.Controls.Add(new Label { Text = "XML:", AutoSize = true, Margin = new Padding(0, 9, 5, 0) });
			cbFiles.DropDownStyle = ComboBoxStyle.DropDownList;
			cbFiles.Width = 360;
			cbFiles.Margin = new Padding(0, 5, 8, 0);
			cbFiles.SelectedIndexChanged += (_, _) =>
			{
				if (cbFiles.SelectedItem is UiFileItem item)
					TryLoadFile(item.Path);
			};
			toolbar.Controls.Add(cbFiles);

			SetupButton(btnOpen, "Open...", (_, _) => OpenXmlFile());
			SetupButton(btnReload, "Reload", (_, _) => ReloadCurrentFile());
			SetupButton(btnSave, "Save", (_, _) => SaveCurrentFile());
			SetupButton(btnOpenFolder, "Open XML folder", (_, _) => OpenXmlFolder());
			SetupButton(btnOpenText, "Open in text editor", (_, _) => OpenInTextEditor());
			toolbar.Controls.AddRange([btnOpen, btnReload, btnSave, btnOpenFolder, btnOpenText]);

			cbSavePopup.Text = "Popup after save";
			cbSavePopup.AutoSize = true;
			cbSavePopup.Checked = true;
			cbSavePopup.Margin = new Padding(8, 9, 8, 0);
			toolbar.Controls.Add(cbSavePopup);

			cbShowHidden.Text = "Show hidden";
			cbShowHidden.AutoSize = true;
			cbShowHidden.Checked = true;
			cbShowHidden.Margin = new Padding(12, 9, 8, 0);
			cbShowHidden.CheckedChanged += (_, _) => RefreshCanvasOnly();
			toolbar.Controls.Add(cbShowHidden);

			cbEquipmentPreview.Text = "Equipment preview";
			cbEquipmentPreview.AutoSize = true;
			cbEquipmentPreview.Margin = new Padding(0, 9, 10, 0);
			cbEquipmentPreview.CheckedChanged += (_, _) => RefreshCanvasOnly();
			toolbar.Controls.Add(cbEquipmentPreview);

			cbShowTextureNames.Text = "Show texture names";
			cbShowTextureNames.AutoSize = true;
			cbShowTextureNames.Margin = new Padding(0, 9, 10, 0);
			cbShowTextureNames.CheckedChanged += (_, _) => RefreshCanvasOnly();
			toolbar.Controls.Add(cbShowTextureNames);

			toolbar.Controls.Add(new Label { Text = "Depth:", AutoSize = true, Margin = new Padding(2, 9, 4, 0) });
			cbDepthFilter.DropDownStyle = ComboBoxStyle.DropDownList;
			cbDepthFilter.Width = 112;
			cbDepthFilter.Margin = new Padding(0, 5, 8, 0);
			cbDepthFilter.SelectedIndexChanged += (_, _) =>
			{
				if (loadingControls)
					return;

				canvasDepthFilter = cbDepthFilter.SelectedItem is DepthFilterItem item ? item.Depth : -1;
				RefreshCanvasOnly();
			};
			toolbar.Controls.Add(cbDepthFilter);

			toolbar.Controls.Add(new Label { Text = "Zoom:", AutoSize = true, Margin = new Padding(4, 9, 4, 0) });
			ConfigureNumeric(nudZoom, 25, 200, 100);
			nudZoom.Width = 58;
			nudZoom.Increment = 10;
			nudZoom.Margin = new Padding(0, 5, 0, 0);
			nudZoom.ValueChanged += (_, _) => RefreshCanvasOnly();
			toolbar.Controls.Add(nudZoom);

			SplitContainer split = new()
			{
				Dock = DockStyle.Fill,
				Orientation = Orientation.Vertical,
				SplitterDistance = 830
			};
			root.Controls.Add(split, 0, 1);

			TableLayoutPanel canvasPanel = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 2
			};
			canvasPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
			canvasPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			split.Panel1.Controls.Add(canvasPanel);

			canvasPanel.Controls.Add(new Label
			{
				Text = "Canvas: click a piece to edit it. Use Depth to isolate stacked pieces. Drag to move, drag the yellow corner to resize, arrow keys nudge by 1px, Shift+arrows nudge by 10px.",
				Dock = DockStyle.Fill,
				TextAlign = ContentAlignment.MiddleLeft
			}, 0, 0);
			canvasPanel.Controls.Add(canvas, 0, 1);

			SplitContainer right = new()
			{
				Dock = DockStyle.Fill,
				Orientation = Orientation.Horizontal
			};
			split.Panel2.Controls.Add(right);
			bool rightSplitterInitialized = false;
			right.SizeChanged += (_, _) =>
			{
				if (rightSplitterInitialized)
					return;

				rightSplitterInitialized = TrySetSplitterDistance(right, 220);
			};

			tvElements.Dock = DockStyle.Fill;
			tvElements.HideSelection = false;
			tvElements.AfterSelect += (_, e) =>
			{
				if (!loadingControls && e.Node?.Tag is UiNode node)
					SelectNode(node, false);
			};
			right.Panel1.Controls.Add(tvElements);

			TabControl tabs = new()
			{
				Dock = DockStyle.Fill
			};
			right.Panel2.Controls.Add(tabs);

			TabPage tabCommon = new("Beginner editor");
			TabPage tabAttributes = new("Advanced raw XML");
			tabs.TabPages.AddRange([tabCommon, tabAttributes]);
			BuildCommonTab(tabCommon);
			BuildAttributesTab(tabAttributes);

			lblStatus.Dock = DockStyle.Fill;
			lblStatus.TextAlign = ContentAlignment.MiddleLeft;
			lblStatus.ForeColor = Color.FromArgb(80, 80, 80);
			lblStatus.Text = "Choose a UI XML file.";
			root.Controls.Add(lblStatus, 0, 2);
		}

		private void BuildCommonTab(TabPage tab)
		{
			TableLayoutPanel outer = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 3,
				Padding = new Padding(8)
			};
			outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
			outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
			tab.Controls.Add(outer);

			FlowLayoutPanel quickActions = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			cbQuickHide.Text = "Hide selected piece";
			cbQuickHide.AutoSize = true;
			cbQuickHide.Margin = new Padding(0, 9, 10, 0);
			cbQuickHide.CheckedChanged += (_, _) => ApplyTextAttribute("hide", cbQuickHide.Checked ? "1" : "0");
			quickActions.Controls.Add(cbQuickHide);
			SetupButton(btnQuickDuplicate, "Duplicate selected piece", (_, _) => DuplicateSelectedElement());
			SetupButton(btnQuickDelete, "Delete selected piece", (_, _) => DeleteSelectedElement());
			quickActions.Controls.AddRange([btnQuickDuplicate, btnQuickDelete]);
			outer.Controls.Add(quickActions, 0, 0);

			TabControl beginnerTabs = new()
			{
				Dock = DockStyle.Fill
			};
			TabPage tabBasics = new("Basics");
			TabPage tabTexture = new("Texture & Color");
			TabPage tabText = new("Text");
			TabPage tabStructure = new("Add / Delete");
			beginnerTabs.TabPages.AddRange([tabBasics, tabTexture, tabText, tabStructure]);
			outer.Controls.Add(beginnerTabs, 0, 1);

			ConfigureText(tbTag, true);
			ConfigureText(tbId, false);
			ConfigureText(tbStr, false);
			ConfigureText(tbTex, false);
			ConfigureText(tbColor, false);

			tbId.TextChanged += (_, _) => ApplyTextAttribute("id", tbId.Text);
			tbStr.TextChanged += (_, _) => ApplyTextAttribute("str", tbStr.Text);
			tbTex.TextChanged += (_, _) => ApplyTextAttribute("tex", tbTex.Text);
			tbColor.TextChanged += (_, _) => ApplyVisualColor(tbColor.Text);

			ConfigureNumeric(nudX, -99999, 99999, 0);
			ConfigureNumeric(nudY, -99999, 99999, 0);
			ConfigureNumeric(nudW, 0, 99999, 0);
			ConfigureNumeric(nudH, 0, 99999, 0);
			ConfigureNumeric(nudL, -99999, 99999, 0);
			ConfigureNumeric(nudT, -99999, 99999, 0);
			ConfigureNumeric(nudR, -99999, 99999, 0);
			ConfigureNumeric(nudB, -99999, 99999, 0);

			nudX.ValueChanged += (_, _) => ApplyNumericAttribute("x", nudX);
			nudY.ValueChanged += (_, _) => ApplyNumericAttribute("y", nudY);
			nudW.ValueChanged += (_, _) => ApplyNumericAttribute("w", nudW);
			nudH.ValueChanged += (_, _) => ApplyNumericAttribute("h", nudH);
			nudL.ValueChanged += (_, _) => ApplyNumericAttribute("l", nudL);
			nudT.ValueChanged += (_, _) => ApplyNumericAttribute("t", nudT);
			nudR.ValueChanged += (_, _) => ApplyNumericAttribute("r", nudR);
			nudB.ValueChanged += (_, _) => ApplyNumericAttribute("b", nudB);

			cbHide.Text = "Hidden";
			cbHide.AutoSize = true;
			cbHide.CheckedChanged += (_, _) => ApplyTextAttribute("hide", cbHide.Checked ? "1" : "0");

			ConfigureCombo(cbAnchorH, ["Left / normal", "Centered in parent", "Right edge"]);
			ConfigureCombo(cbAnchorV, ["Top / normal", "Centered in parent", "Bottom edge"]);
			ConfigureCombo(cbTextAlign, ["Left text", "Centered text", "Right text"]);
			cbAnchorH.SelectedIndexChanged += (_, _) => ApplyComboAttribute("align_h", cbAnchorH);
			cbAnchorV.SelectedIndexChanged += (_, _) => ApplyComboAttribute("align_v", cbAnchorV);
			cbTextAlign.SelectedIndexChanged += (_, _) => ApplyComboAttribute("h_align", cbTextAlign);

			SetupButton(btnPickTexture, "Choose...", (_, _) => PickTextureFile());
			SetupButton(btnPickColor, "Pick color...", (_, _) => PickVisualColor());
			pnlColorPreview.BorderStyle = BorderStyle.FixedSingle;
			pnlColorPreview.Width = 32;
			pnlColorPreview.Height = 22;
			pnlColorPreview.Margin = new Padding(6, 4, 6, 0);

			TableLayoutPanel textureEditor = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 2,
				RowCount = 1
			};
			textureEditor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			textureEditor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 86));
			textureEditor.Controls.Add(tbTex, 0, 0);
			textureEditor.Controls.Add(btnPickTexture, 1, 0);

			FlowLayoutPanel colorEditor = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			tbColor.Width = 128;
			colorEditor.Controls.Add(tbColor);
			colorEditor.Controls.Add(pnlColorPreview);
			colorEditor.Controls.Add(btnPickColor);

			TableLayoutPanel basics = CreateEditorGrid(6);
			tabBasics.Controls.Add(basics);
			AddField(basics, 0, "Piece type", tbTag, 3);
			AddField(basics, 1, "Name used by code", tbId, 3);
			AddField(basics, 2, "Left position (X)", nudX);
			AddField(basics, 2, "Top position (Y)", nudY, 1, 2);
			AddField(basics, 3, "Width", nudW);
			AddField(basics, 3, "Height", nudH, 1, 2);
			AddField(basics, 4, "Visibility", cbHide, 3);
			AddHint(basics, 5, "Basics are the safe fields: name, position, size, and whether this piece starts hidden.");

			TableLayoutPanel texture = CreateEditorGrid(6);
			tabTexture.Controls.Add(texture);
			AddField(texture, 0, "Texture file", textureEditor, 3);
			AddField(texture, 1, "Texture cut left", nudL);
			AddField(texture, 1, "Texture cut top", nudT, 1, 2);
			AddField(texture, 2, "Texture cut right", nudR);
			AddField(texture, 2, "Texture cut bottom", nudB, 1, 2);
			AddField(texture, 3, "Text / tint color", colorEditor, 3);
			AddHint(texture, 4, "For an image background, this is the tab you want. Texture cut values choose the rectangle inside the .tex file.");
			AddHint(texture, 5, "For buttons, hover/pressed pictures are usually child <uv> rows in Advanced raw XML.");

			TableLayoutPanel text = CreateEditorGrid(5);
			tabText.Controls.Add(text);
			AddField(text, 0, "Shown text", tbStr, 3);
			AddField(text, 1, "Text alignment", cbTextAlign, 3);
			AddField(text, 2, "Horizontal anchor", cbAnchorH);
			AddField(text, 2, "Vertical anchor", cbAnchorV, 1, 2);
			AddHint(text, 3, "Anchors move the piece relative to its parent: left/top, centered, or right/bottom.");
			AddHint(text, 4, "If a field seems to do nothing, the game code may be overwriting the text at runtime.");

			FlowLayoutPanel buttons = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = true
			};
			cbAddType.DropDownStyle = ComboBoxStyle.DropDownList;
			cbAddType.Items.AddRange(["UIBase", "UIText", "UIImage", "UIButton", "UIIcon", "UIEdit"]);
			cbAddType.SelectedIndex = 0;
			cbAddType.Width = 100;
			cbAddType.Margin = new Padding(0, 4, 6, 0);
			buttons.Controls.Add(cbAddType);
			SetupButton(btnAddChild, "Add child", (_, _) => AddChildElement());
			SetupButton(btnDuplicate, "Duplicate", (_, _) => DuplicateSelectedElement());
			SetupButton(btnDelete, "Delete", (_, _) => DeleteSelectedElement());
			buttons.Controls.AddRange([btnAddChild, btnDuplicate, btnDelete]);

			TableLayoutPanel structure = CreateEditorGrid(4);
			tabStructure.Controls.Add(structure);
			AddField(structure, 0, "New child type", cbAddType, 3);
			structure.Controls.Add(buttons, 0, 1);
			structure.SetColumnSpan(buttons, 4);
			AddHint(structure, 2, "Use Add child when you need a new text/image/button inside the currently selected piece.");
			AddHint(structure, 3, "Prefer Hide selected piece before deleting, unless you are sure no client code uses that ID.");

			lblFriendlyHelp.Dock = DockStyle.Fill;
			lblFriendlyHelp.ForeColor = Color.FromArgb(60, 60, 60);
			lblFriendlyHelp.TextAlign = ContentAlignment.MiddleLeft;
			lblFriendlyHelp.Padding = new Padding(0, 4, 0, 0);
			outer.Controls.Add(lblFriendlyHelp, 0, 2);
		}

		private void BuildAttributesTab(TabPage tab)
		{
			TableLayoutPanel layout = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 3,
				Padding = new Padding(8)
			};
			layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
			layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
			layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
			tab.Controls.Add(layout);

			dgvAttributes.Dock = DockStyle.Fill;
			dgvAttributes.AllowUserToAddRows = true;
			dgvAttributes.AllowUserToDeleteRows = false;
			dgvAttributes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dgvAttributes.RowHeadersVisible = false;
			dgvAttributes.SelectionMode = DataGridViewSelectionMode.CellSelect;
			dgvAttributes.Columns.Add(new DataGridViewTextBoxColumn { Name = "attribute", HeaderText = "Raw XML field", FillWeight = 90 });
			dgvAttributes.Columns.Add(new DataGridViewTextBoxColumn { Name = "value", HeaderText = "Saved value", FillWeight = 160 });
			dgvAttributes.CellEndEdit += (_, e) => ApplyAttributeGridRow(e.RowIndex);
			dgvAttributes.UserDeletedRow += (_, _) => RebuildAfterAttributeEdit();
			layout.Controls.Add(dgvAttributes, 0, 0);

			FlowLayoutPanel attrButtons = new()
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};
			tbNewAttribute.Width = 150;
			tbNewAttribute.PlaceholderText = "new raw XML field";
			tbNewAttribute.Margin = new Padding(0, 5, 6, 0);
			attrButtons.Controls.Add(tbNewAttribute);
			SetupButton(btnAddAttribute, "Add attribute", (_, _) => AddAttribute());
			SetupButton(btnRemoveAttribute, "Remove selected", (_, _) => RemoveSelectedAttributes());
			attrButtons.Controls.AddRange([btnAddAttribute, btnRemoveAttribute]);
			layout.Controls.Add(attrButtons, 0, 1);

			Label hint = new()
			{
				Dock = DockStyle.Fill,
				Text = "Advanced tab: use this when the beginner editor does not expose a field. Button states often live in child rows named uv, none, check, back, thumb, button1, and button2.",
				ForeColor = Color.FromArgb(80, 80, 80),
				TextAlign = ContentAlignment.MiddleLeft
			};
			layout.Controls.Add(hint, 0, 2);
		}

		private void LoadFileList()
		{
			string? folder = GetXmlFolder();
			cbFiles.Items.Clear();

			if (folder == null)
			{
				SetStatus("Client Data\\Interface\\xml folder not found. Use Open... to choose a UI XML manually.");
				return;
			}

			foreach (string path in Directory.GetFiles(folder, "*.xml").OrderBy(Path.GetFileName))
				cbFiles.Items.Add(new UiFileItem(path));

			if (cbFiles.Items.Count == 0)
			{
				SetStatus($"No XML files found in {folder}.");
				return;
			}

			int preferred = 0;
			for (int i = 0; i < cbFiles.Items.Count; i++)
			{
				if (cbFiles.Items[i] is UiFileItem item &&
					string.Equals(Path.GetFileName(item.Path), "UIItemCollection.xml", StringComparison.OrdinalIgnoreCase))
				{
					preferred = i;
					break;
				}
			}

			cbFiles.SelectedIndex = preferred;
		}

		private string? GetXmlFolder()
		{
			List<string> candidates = new();

			if (!string.IsNullOrWhiteSpace(pMain.pSettings.ClientPath))
				candidates.Add(Path.Combine(pMain.pSettings.ClientPath, "Data", "Interface", "xml"));

			candidates.Add(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RezaRePack1776", "Client", "ClientEp4", "Data", "Interface", "xml")));
			candidates.Add(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "RezaRePack1776", "Client", "ClientEp4", "Data", "Interface", "xml")));

			return candidates.FirstOrDefault(Directory.Exists);
		}

		private void OpenXmlFile()
		{
			using OpenFileDialog dialog = new()
			{
				Filter = "Last Chaos UI XML (*.xml)|*.xml|All files (*.*)|*.*",
				InitialDirectory = GetXmlFolder() ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
			};

			if (dialog.ShowDialog(this) == DialogResult.OK)
				TryLoadFile(dialog.FileName);
		}

		private void ReloadCurrentFile()
		{
			if (currentFile != null)
				TryLoadFile(currentFile);
		}

		private void TryLoadFile(string path)
		{
			if (!PromptToSaveDirty())
				return;

			try
			{
				document = XDocument.Load(path, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
				currentFile = path;
				dirty = false;
				BuildModelAndTree(document.Root);
				SetStatus($"Loaded {Path.GetFileName(path)}.");
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Could not load UI XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
				SetStatus("Load failed.");
			}
		}

		private bool PromptToSaveDirty()
		{
			if (!dirty)
				return true;

			DialogResult result = MessageBox.Show(this, "Save changes before continuing?", "UI XML Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
			if (result == DialogResult.Cancel)
				return false;
			if (result == DialogResult.Yes)
				return SaveCurrentFile();
			return true;
		}

		private bool SaveCurrentFile()
		{
			if (document == null || currentFile == null)
				return false;

			try
			{
				string backup = currentFile + "." + DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture) + ".bak";
				File.Copy(currentFile, backup, false);

				XmlWriterSettings settings = new()
				{
					Encoding = new UTF8Encoding(false),
					Indent = true,
					OmitXmlDeclaration = document.Declaration == null
				};

				using XmlWriter writer = XmlWriter.Create(currentFile, settings);
				document.Save(writer);
				dirty = false;
				lastSavedAt = DateTime.Now;
				string message = $"Saved {Path.GetFileName(currentFile)} at {lastSavedAt:HH:mm:ss}. Backup: {Path.GetFileName(backup)}";
				SetStatus(message);
				pMain.Logger(LogTypes.Success, $"UI XML Editor > Saved {currentFile}", false);
				if (cbSavePopup.Checked)
					MessageBox.Show(this, message, "UI XML saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Could not save UI XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
				SetStatus("Save failed.");
				return false;
			}
		}

		private void OpenXmlFolder()
		{
			string? folder = currentFile != null ? Path.GetDirectoryName(currentFile) : GetXmlFolder();
			if (folder == null)
				return;

			Process.Start(new ProcessStartInfo(folder) { UseShellExecute = true });
		}

		private void OpenInTextEditor()
		{
			if (currentFile == null)
				return;

			try
			{
				string editor = pMain.pSettings.TextEditorPath;
				if (!string.IsNullOrWhiteSpace(editor) && File.Exists(editor))
				{
					Process.Start(new ProcessStartInfo(editor, $"\"{currentFile}\"") { UseShellExecute = true });
				}
				else
				{
					Process.Start(new ProcessStartInfo("notepad.exe", $"\"{currentFile}\"") { UseShellExecute = true });
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Could not open text editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BuildModelAndTree(XElement? elementToSelect)
		{
			loadingControls = true;
			try
			{
				rootNode = document?.Root == null ? null : BuildNode(document.Root, null);
				treeNodesByElement.Clear();
				tvElements.Nodes.Clear();

				if (rootNode != null)
				{
					TreeNode rootTreeNode = BuildTreeNode(rootNode);
					tvElements.Nodes.Add(rootTreeNode);
					rootTreeNode.Expand();
				}

				UiNode? nextSelected = elementToSelect == null ? rootNode : FindNodeByElement(rootNode, elementToSelect) ?? rootNode;
				selectedNode = nextSelected;

				if (selectedNode != null && treeNodesByElement.TryGetValue(selectedNode.Element, out TreeNode? treeNode))
					tvElements.SelectedNode = treeNode;
			}
			finally
			{
				loadingControls = false;
			}

			canvas.RootNode = rootNode;
			UpdateDepthFilterOptions();
			SelectNode(selectedNode, false);
		}

		private UiNode BuildNode(XElement element, UiNode? parent)
		{
			UiNode node = new(element, parent);
			RecalculateNode(node);

			foreach (XElement childElement in element.Elements())
			{
				UiNode child = BuildNode(childElement, node);
				node.Children.Add(child);
			}

			return node;
		}

		private void RecalculateLayout()
		{
			if (rootNode != null)
				RecalculateNodeRecursive(rootNode);
		}

		private void RecalculateNodeRecursive(UiNode node)
		{
			RecalculateNode(node);
			foreach (UiNode child in node.Children)
				RecalculateNodeRecursive(child);
		}

		private void RecalculateNode(UiNode node)
		{
			node.X = GetInt(node.Element, "x", 0);
			node.Y = GetInt(node.Element, "y", 0);
			node.W = Math.Max(0, GetInt(node.Element, "w", 0));
			node.H = Math.Max(0, GetInt(node.Element, "h", 0));
			node.Hidden = GetInt(node.Element, "hide", 0) != 0;

			if (node.Parent == null)
			{
				node.AbsX = node.X;
				node.AbsY = node.Y;
			}
			else
			{
				int alignH = GetInt(node.Element, "align_h", 0);
				int alignV = GetInt(node.Element, "align_v", 0);

				node.AbsX = node.Parent.AbsX + node.X;
				if (alignH == 1)
					node.AbsX = node.Parent.AbsX + (node.Parent.W - node.W) / 2 + node.X;
				else if (alignH == 2)
					node.AbsX = node.Parent.AbsX + node.Parent.W - node.W + node.X;

				node.AbsY = node.Parent.AbsY + node.Y;
				if (alignV == 1)
					node.AbsY = node.Parent.AbsY + (node.Parent.H - node.H) / 2 + node.Y;
				else if (alignV == 2)
					node.AbsY = node.Parent.AbsY + node.Parent.H - node.H + node.Y;
			}

			node.Bounds = new Rectangle(node.AbsX, node.AbsY, Math.Max(1, node.W), Math.Max(1, node.H));
		}

		private TreeNode BuildTreeNode(UiNode node)
		{
			TreeNode treeNode = new(GetNodeLabel(node))
			{
				Tag = node
			};
			treeNodesByElement[node.Element] = treeNode;

			foreach (UiNode child in node.Children)
				treeNode.Nodes.Add(BuildTreeNode(child));

			return treeNode;
		}

		private static string GetNodeLabel(UiNode node)
		{
			string id = GetString(node.Element, "id");
			string hidden = node.Hidden ? " hidden" : "";
			return string.IsNullOrWhiteSpace(id) ? $"{node.Element.Name.LocalName}{hidden}" : $"{node.Element.Name.LocalName} #{id}{hidden}";
		}

		private UiNode? FindNodeByElement(UiNode? node, XElement element)
		{
			if (node == null)
				return null;
			if (ReferenceEquals(node.Element, element))
				return node;

			foreach (UiNode child in node.Children)
			{
				UiNode? found = FindNodeByElement(child, element);
				if (found != null)
					return found;
			}

			return null;
		}

		internal void SelectNode(UiNode? node, bool updateTree)
		{
			selectedNode = node;
			canvas.SelectedNode = selectedNode;

			if (updateTree && selectedNode != null && treeNodesByElement.TryGetValue(selectedNode.Element, out TreeNode? treeNode))
			{
				loadingControls = true;
				tvElements.SelectedNode = treeNode;
				loadingControls = false;
			}

			PopulateCommonFields();
			PopulateAttributeGrid();
			canvas.Invalidate();
		}

		private void PopulateCommonFields()
		{
			loadingControls = true;
			try
			{
				bool hasNode = selectedNode != null;
				foreach (Control control in GetPropertyControls())
					control.Enabled = hasNode;

				if (!hasNode)
				{
					tbTag.Text = "";
					tbId.Text = "";
					tbStr.Text = "";
					tbTex.Text = "";
					tbColor.Text = "";
					SetComboIndex(cbAnchorH, 0);
					SetComboIndex(cbAnchorV, 0);
					SetComboIndex(cbTextAlign, 0);
					cbQuickHide.Checked = false;
					pnlColorPreview.BackColor = SystemColors.Control;
					lblFriendlyHelp.Text = "Select a UI piece to edit its position and appearance.";
					return;
				}

				XElement element = selectedNode!.Element;
				tbTag.Text = element.Name.LocalName;
				tbId.Text = GetString(element, "id");
				tbStr.Text = GetString(element, "str");
				tbTex.Text = GetString(element, "tex");
				tbColor.Text = GetString(element, GetVisualColorAttribute(element));
				cbHide.Checked = GetInt(element, "hide", 0) != 0;
				cbQuickHide.Checked = cbHide.Checked;
				SetComboIndex(cbAnchorH, GetInt(element, "align_h", 0));
				SetComboIndex(cbAnchorV, GetInt(element, "align_v", 0));
				SetComboIndex(cbTextAlign, GetInt(element, "h_align", 0));
				SetNumeric(nudX, selectedNode.X);
				SetNumeric(nudY, selectedNode.Y);
				SetNumeric(nudW, selectedNode.W);
				SetNumeric(nudH, selectedNode.H);
				SetNumeric(nudL, GetInt(element, "l", 0));
				SetNumeric(nudT, GetInt(element, "t", 0));
				SetNumeric(nudR, GetInt(element, "r", 0));
				SetNumeric(nudB, GetInt(element, "b", 0));
				UpdateColorPreview();
				lblFriendlyHelp.Text = GetFriendlyHelp(element);
			}
			finally
			{
				loadingControls = false;
			}
		}

		private IEnumerable<Control> GetPropertyControls()
		{
			yield return tbId;
			yield return tbStr;
			yield return tbTex;
			yield return tbColor;
			yield return cbAnchorH;
			yield return cbAnchorV;
			yield return cbTextAlign;
			yield return btnPickTexture;
			yield return btnPickColor;
			yield return cbQuickHide;
			yield return btnQuickDuplicate;
			yield return btnQuickDelete;
			yield return nudX;
			yield return nudY;
			yield return nudW;
			yield return nudH;
			yield return nudL;
			yield return nudT;
			yield return nudR;
			yield return nudB;
			yield return cbHide;
			yield return cbAddType;
			yield return btnAddChild;
			yield return btnDuplicate;
			yield return btnDelete;
			yield return dgvAttributes;
			yield return tbNewAttribute;
			yield return btnAddAttribute;
			yield return btnRemoveAttribute;
		}

		private void PopulateAttributeGrid()
		{
			loadingControls = true;
			try
			{
				dgvAttributes.Rows.Clear();
				if (selectedNode == null)
					return;

				foreach (XAttribute attribute in selectedNode.Element.Attributes())
				{
					int row = dgvAttributes.Rows.Add(attribute.Name.LocalName, attribute.Value);
					dgvAttributes.Rows[row].Tag = attribute.Name.LocalName;
				}
			}
			finally
			{
				loadingControls = false;
			}
		}

		private void ApplyTextAttribute(string attribute, string value)
		{
			if (loadingControls || selectedNode == null)
				return;

			SetAttribute(selectedNode.Element, attribute, value);
			MarkXmlChanged(selectedNode.Element, rebuildTree: attribute == "id" || attribute == "hide");
		}

		private void ApplyNumericAttribute(string attribute, NumericUpDown source)
		{
			if (loadingControls || selectedNode == null)
				return;

			SetAttribute(selectedNode.Element, attribute, ((int)source.Value).ToString(CultureInfo.InvariantCulture));
			MarkXmlChanged(selectedNode.Element, rebuildTree: attribute == "hide");
		}

		private void ApplyComboAttribute(string attribute, ComboBox source)
		{
			if (loadingControls || selectedNode == null || source.SelectedIndex < 0)
				return;

			SetAttribute(selectedNode.Element, attribute, source.SelectedIndex.ToString(CultureInfo.InvariantCulture));
			MarkXmlChanged(selectedNode.Element, rebuildTree: false);
		}

		private void ApplyVisualColor(string value)
		{
			if (loadingControls || selectedNode == null)
				return;

			SetAttribute(selectedNode.Element, GetVisualColorAttribute(selectedNode.Element), value);
			UpdateColorPreview();
			MarkXmlChanged(selectedNode.Element, rebuildTree: false);
		}

		private void PickTextureFile()
		{
			if (selectedNode == null)
				return;

			using OpenFileDialog dialog = new()
			{
				Filter = "Last Chaos textures (*.tex)|*.tex|All files (*.*)|*.*",
				InitialDirectory = GetTextureFolder() ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
			};

			if (dialog.ShowDialog(this) == DialogResult.OK)
				tbTex.Text = Path.GetFileName(dialog.FileName);
		}

		private void PickVisualColor()
		{
			if (selectedNode == null)
				return;

			using ColorDialog dialog = new()
			{
				FullOpen = true,
				Color = TryParseLastChaosColor(tbColor.Text, out Color currentColor) ? currentColor : Color.White
			};

			if (dialog.ShowDialog(this) == DialogResult.OK)
				tbColor.Text = ToLastChaosColor(dialog.Color, TryParseLastChaosColor(tbColor.Text, out Color oldColor) ? oldColor.A : 255);
		}

		private void ApplyAttributeGridRow(int rowIndex)
		{
			if (loadingControls || selectedNode == null || rowIndex < 0 || rowIndex >= dgvAttributes.Rows.Count)
				return;

			DataGridViewRow row = dgvAttributes.Rows[rowIndex];
			if (row.IsNewRow)
				return;

			string oldName = row.Tag as string ?? "";
			string newName = Convert.ToString(row.Cells["attribute"].Value, CultureInfo.InvariantCulture)?.Trim() ?? "";
			string value = Convert.ToString(row.Cells["value"].Value, CultureInfo.InvariantCulture) ?? "";

			if (string.IsNullOrWhiteSpace(newName))
				return;

			if (!string.IsNullOrWhiteSpace(oldName) && !string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
				selectedNode.Element.Attribute(oldName)?.Remove();

			SetAttribute(selectedNode.Element, newName, value);
			row.Tag = newName;
			RebuildAfterAttributeEdit();
		}

		private void RebuildAfterAttributeEdit()
		{
			if (selectedNode == null)
				return;

			XElement selectedElement = selectedNode.Element;
			MarkXmlChanged(selectedElement, rebuildTree: true);
		}

		private void AddAttribute()
		{
			if (selectedNode == null)
				return;

			string name = tbNewAttribute.Text.Trim();
			if (string.IsNullOrWhiteSpace(name))
				return;

			SetAttribute(selectedNode.Element, name, "");
			tbNewAttribute.Clear();
			MarkXmlChanged(selectedNode.Element, rebuildTree: true);
		}

		private void RemoveSelectedAttributes()
		{
			if (selectedNode == null)
				return;

			HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);
			foreach (DataGridViewCell cell in dgvAttributes.SelectedCells)
			{
				if (cell.OwningRow.Tag is string name)
					names.Add(name);
			}

			foreach (string name in names)
				selectedNode.Element.Attribute(name)?.Remove();

			MarkXmlChanged(selectedNode.Element, rebuildTree: true);
		}

		internal void SetSelectedGeometry(int x, int y, int w, int h, bool finalUpdate)
		{
			if (selectedNode == null)
				return;

			SetAttribute(selectedNode.Element, "x", x.ToString(CultureInfo.InvariantCulture));
			SetAttribute(selectedNode.Element, "y", y.ToString(CultureInfo.InvariantCulture));
			SetAttribute(selectedNode.Element, "w", Math.Max(0, w).ToString(CultureInfo.InvariantCulture));
			SetAttribute(selectedNode.Element, "h", Math.Max(0, h).ToString(CultureInfo.InvariantCulture));
			MarkXmlChanged(selectedNode.Element, rebuildTree: false, refreshAttributeGrid: finalUpdate);
		}

		internal void NudgeSelected(int dx, int dy)
		{
			if (selectedNode == null)
				return;

			SetSelectedGeometry(selectedNode.X + dx, selectedNode.Y + dy, selectedNode.W, selectedNode.H, true);
		}

		private void AddChildElement()
		{
			if (selectedNode == null || cbAddType.SelectedItem == null)
				return;

			string tag = cbAddType.SelectedItem.ToString() ?? "UIBase";
			XElement child = CreateElement(tag);
			selectedNode.Element.Add(child);
			MarkXmlChanged(child, rebuildTree: true);
			BuildModelAndTree(child);
		}

		private XElement CreateElement(string tag)
		{
			XElement element = new(tag);
			element.SetAttributeValue("id", MakeUniqueId(tag.ToLowerInvariant()));
			element.SetAttributeValue("x", "20");
			element.SetAttributeValue("y", "20");
			element.SetAttributeValue("w", tag == "UIText" ? "140" : "80");
			element.SetAttributeValue("h", tag == "UIText" ? "15" : "40");
			element.SetAttributeValue("hide", "0");
			element.SetAttributeValue("align_h", "0");
			element.SetAttributeValue("align_v", "0");
			element.SetAttributeValue("tooltip", "-1");
			element.SetAttributeValue("tooltip_width", "0");

			if (tag is "UIImage" or "UIButton")
			{
				element.SetAttributeValue("tex", "");
				element.SetAttributeValue("l", "0");
				element.SetAttributeValue("t", "0");
				element.SetAttributeValue("r", "80");
				element.SetAttributeValue("b", "40");
			}

			if (tag is "UIText" or "UIButton")
			{
				element.SetAttributeValue("str_idx", "-1");
				element.SetAttributeValue("str", tag == "UIText" ? "New text" : "");
				element.SetAttributeValue("color", "0xf2f2f2ff");
				element.SetAttributeValue("h_align", "1");
			}

			return element;
		}

		private string MakeUniqueId(string prefix)
		{
			HashSet<string> ids = new(EnumerateNodes(rootNode).Select(n => GetString(n.Element, "id")), StringComparer.OrdinalIgnoreCase);
			for (int i = 1; i < 10000; i++)
			{
				string candidate = $"{prefix}_{i:000}";
				if (!ids.Contains(candidate))
					return candidate;
			}
			return $"{prefix}_{DateTime.Now.Ticks}";
		}

		private void DuplicateSelectedElement()
		{
			if (selectedNode?.Parent == null)
				return;

			XElement clone = new(selectedNode.Element);
			string oldId = GetString(clone, "id");
			if (!string.IsNullOrWhiteSpace(oldId))
				clone.SetAttributeValue("id", MakeUniqueId(oldId + "_copy"));

			clone.SetAttributeValue("x", (selectedNode.X + 12).ToString(CultureInfo.InvariantCulture));
			clone.SetAttributeValue("y", (selectedNode.Y + 12).ToString(CultureInfo.InvariantCulture));
			selectedNode.Element.AddAfterSelf(clone);
			MarkXmlChanged(clone, rebuildTree: true);
			BuildModelAndTree(clone);
		}

		private void DeleteSelectedElement()
		{
			if (selectedNode?.Parent == null)
				return;

			XElement parent = selectedNode.Parent.Element;
			selectedNode.Element.Remove();
			MarkXmlChanged(parent, rebuildTree: true);
			BuildModelAndTree(parent);
		}

		private void MarkXmlChanged(XElement elementToSelect, bool rebuildTree, bool refreshAttributeGrid = true)
		{
			dirty = true;

			if (rebuildTree)
			{
				BuildModelAndTree(elementToSelect);
				return;
			}

			RecalculateLayout();
			PopulateCommonFields();
			if (refreshAttributeGrid)
				PopulateAttributeGrid();

			canvas.RootNode = rootNode;
			canvas.SelectedNode = selectedNode;
			canvas.Invalidate();
			SetStatus($"Modified {GetNodeLabel(selectedNode ?? rootNode!)}. Save when ready.");
		}

		private void RefreshCanvasOnly()
		{
			canvas.Invalidate();
		}

		private void UpdateDepthFilterOptions()
		{
			int previousDepth = canvasDepthFilter;

			loadingControls = true;
			try
			{
				cbDepthFilter.Items.Clear();
				cbDepthFilter.Items.Add(new DepthFilterItem(-1, "All depths"));

				if (rootNode != null)
				{
					foreach (var group in EnumerateNodes(rootNode).GroupBy(node => node.Depth).OrderBy(group => group.Key))
						cbDepthFilter.Items.Add(new DepthFilterItem(group.Key, $"Depth {group.Key} ({group.Count()})"));
				}

				int selectedIndex = 0;
				for (int i = 0; i < cbDepthFilter.Items.Count; i++)
				{
					if (cbDepthFilter.Items[i] is DepthFilterItem item && item.Depth == previousDepth)
					{
						selectedIndex = i;
						break;
					}
				}

				cbDepthFilter.SelectedIndex = selectedIndex;
				canvasDepthFilter = cbDepthFilter.SelectedItem is DepthFilterItem selected ? selected.Depth : -1;
				cbDepthFilter.Enabled = rootNode != null;
			}
			finally
			{
				loadingControls = false;
			}
		}

		internal bool ShouldDraw(UiNode node)
		{
			if (node.Parent == null)
				return true;

			if (canvasDepthFilter >= 0 && node.Depth != canvasDepthFilter)
				return false;

			if (IsEquipmentPreviewActive())
			{
				if (IsInSubtree(node, ItemCollectionNormalIds))
					return false;
				if (IsInSubtree(node, ItemCollectionEquipmentIds))
					return true;
			}

			return cbShowHidden.Checked || !node.Hidden;
		}

		internal bool ShouldForceVisible(UiNode node)
		{
			return IsEquipmentPreviewActive() && IsInSubtree(node, ItemCollectionEquipmentIds);
		}

		private bool IsEquipmentPreviewActive()
		{
			return cbEquipmentPreview.Checked &&
				currentFile != null &&
				string.Equals(Path.GetFileName(currentFile), "UIItemCollection.xml", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsInSubtree(UiNode node, HashSet<string> ids)
		{
			for (UiNode? current = node; current != null; current = current.Parent)
			{
				if (ids.Contains(GetString(current.Element, "id")))
					return true;
			}

			return false;
		}

		internal IEnumerable<UiNode> VisibleNodesBackToFront()
		{
			return EnumerateNodes(rootNode).Where(ShouldDraw).Reverse();
		}

		internal float Zoom => (float)nudZoom.Value / 100.0f;
		internal bool ShowTextureNames => cbShowTextureNames.Checked;

		private IEnumerable<UiNode> EnumerateNodes(UiNode? node)
		{
			if (node == null)
				yield break;

			yield return node;
			foreach (UiNode child in node.Children)
			{
				foreach (UiNode nested in EnumerateNodes(child))
					yield return nested;
			}
		}

		private void UiXmlEditor_FormClosing(object? sender, FormClosingEventArgs e)
		{
			if (!PromptToSaveDirty())
				e.Cancel = true;
		}

		private void SetStatus(string text)
		{
			lblStatus.Text = text;
			lblStatus.ForeColor = text.Contains("Saved", StringComparison.OrdinalIgnoreCase)
				? Color.FromArgb(0, 120, 0)
				: Color.FromArgb(80, 80, 80);
		}

		private static void SetupButton(Button button, string text, EventHandler onClick)
		{
			button.Text = text;
			button.AutoSize = true;
			button.Margin = new Padding(0, 4, 6, 0);
			button.Click += onClick;
		}

		private static void ConfigureText(TextBox textBox, bool readOnly)
		{
			textBox.Dock = DockStyle.Fill;
			textBox.ReadOnly = readOnly;
		}

		private static void ConfigureNumeric(NumericUpDown numeric, int min, int max, int value)
		{
			numeric.Minimum = min;
			numeric.Maximum = max;
			numeric.Value = value;
			numeric.Dock = DockStyle.Fill;
		}

		private static void ConfigureCombo(ComboBox combo, string[] items)
		{
			combo.Dock = DockStyle.Fill;
			combo.DropDownStyle = ComboBoxStyle.DropDownList;
			combo.Items.Clear();
			combo.Items.AddRange(items);
			if (combo.Items.Count > 0)
				combo.SelectedIndex = 0;
		}

		private static bool TrySetSplitterDistance(SplitContainer split, int desiredDistance)
		{
			if (split.Height <= split.SplitterWidth + split.Panel1MinSize + split.Panel2MinSize)
				return false;

			int maxDistance = split.Height - split.SplitterWidth - split.Panel2MinSize;
			int distance = Math.Min(maxDistance, Math.Max(split.Panel1MinSize, desiredDistance));
			try
			{
				split.SplitterDistance = distance;
				return true;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
		}

		private static TableLayoutPanel CreateEditorGrid(int rowCount)
		{
			TableLayoutPanel layout = new()
			{
				Dock = DockStyle.Fill,
				ColumnCount = 4,
				RowCount = rowCount,
				Padding = new Padding(8)
			};
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

			for (int i = 0; i < rowCount; ++i)
				layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

			return layout;
		}

		private static void AddField(TableLayoutPanel layout, int row, string label, Control control, int span = 1, int column = 0)
		{
			Label fieldLabel = new()
			{
				Text = label,
				Dock = DockStyle.Fill,
				TextAlign = ContentAlignment.MiddleLeft
			};
			layout.Controls.Add(fieldLabel, column, row);
			layout.Controls.Add(control, column + 1, row);
			if (span > 1)
				layout.SetColumnSpan(control, span);
		}

		private static void AddHint(TableLayoutPanel layout, int row, string text)
		{
			Label hint = new()
			{
				Dock = DockStyle.Fill,
				Text = text,
				ForeColor = Color.FromArgb(80, 80, 80),
				TextAlign = ContentAlignment.MiddleLeft
			};
			layout.Controls.Add(hint, 0, row);
			layout.SetColumnSpan(hint, 4);
		}

		private void SetComboIndex(ComboBox combo, int value)
		{
			if (combo.Items.Count == 0)
				return;

			combo.SelectedIndex = Math.Min(combo.Items.Count - 1, Math.Max(0, value));
		}

		private string? GetTextureFolder()
		{
			List<string> candidates = new();

			if (currentFile != null)
			{
				string? xmlFolder = Path.GetDirectoryName(currentFile);
				if (xmlFolder != null)
				{
					DirectoryInfo? interfaceFolder = Directory.GetParent(xmlFolder);
					if (interfaceFolder != null)
						candidates.Add(interfaceFolder.FullName);
				}
			}

			if (!string.IsNullOrWhiteSpace(pMain.pSettings.ClientPath))
			{
				candidates.Add(Path.Combine(pMain.pSettings.ClientPath, "Data", "Interface"));
				candidates.Add(Path.Combine(pMain.pSettings.ClientPath, "Data"));
			}

			candidates.Add(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RezaRePack1776", "Client", "ClientEp4", "Data", "Interface")));
			candidates.Add(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "RezaRePack1776", "Client", "ClientEp4", "Data", "Interface")));

			return candidates.FirstOrDefault(Directory.Exists);
		}

		private static string GetVisualColorAttribute(XElement element)
		{
			string tag = element.Name.LocalName.ToUpperInvariant();
			if (tag == "UIBUTTON")
				return "text_color";
			if (tag == "UITEXTBOX")
				return "text_col";
			if (tag == "UICHECK")
				return "color_on";

			if (element.Attribute("color") != null)
				return "color";
			if (element.Attribute("text_color") != null)
				return "text_color";
			if (element.Attribute("text_col") != null)
				return "text_col";
			if (element.Attribute("color_on") != null)
				return "color_on";

			return "color";
		}

		private void UpdateColorPreview()
		{
			pnlColorPreview.BackColor = TryParseLastChaosColor(tbColor.Text, out Color color) ? color : SystemColors.Control;
		}

		internal static bool TryParseLastChaosColor(string value, out Color color)
		{
			color = Color.Empty;
			string text = value.Trim();
			if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
				text = text[2..];

			if (text.Length == 0 || text.Length > 8)
				return false;

			text = text.PadLeft(8, '0');
			if (!uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint raw))
				return false;

			int r = (int)((raw >> 24) & 0xff);
			int g = (int)((raw >> 16) & 0xff);
			int b = (int)((raw >> 8) & 0xff);
			int a = (int)(raw & 0xff);
			color = Color.FromArgb(a, r, g, b);
			return true;
		}

		private static string ToLastChaosColor(Color color, int alpha)
		{
			alpha = Math.Min(255, Math.Max(0, alpha));
			return string.Format(CultureInfo.InvariantCulture, "0x{0:X2}{1:X2}{2:X2}{3:X2}", color.R, color.G, color.B, alpha);
		}

		private static string GetFriendlyHelp(XElement element)
		{
			string id = GetString(element, "id");
			string name = string.IsNullOrWhiteSpace(id) ? element.Name.LocalName : $"{element.Name.LocalName} #{id}";

			return element.Name.LocalName.ToUpperInvariant() switch
			{
				"WINDOW" => $"{name}: the whole window. Its size is the editing area the game opens.",
				"UIBASE" => $"{name}: an invisible container/group. It organizes children; add or edit UIImage children when you want visible panels.",
				"UIIMAGE" => $"{name}: visible art/panel. Change Texture file and Texture cut values to borrow a different piece of game artwork.",
				"UITEXT" => $"{name}: normal text. Edit Shown text, Text/tint color, and Text alignment.",
				"UITEXTBOX" => $"{name}: multi-line text. Text/tint color maps to the text_col XML field.",
				"UIBUTTON" => $"{name}: clickable button. Text color is here; normal/hover/pressed pictures are child <uv> rows in Advanced raw XML.",
				"UICHECK" => $"{name}: checkbox/tab button. The checked/unchecked pictures are child <check>/<none> rows in Advanced raw XML.",
				"UIARRAY" => $"{name}: repeating list. Edit the UIArrayItem template inside it; the game clones that template for each entry.",
				"UIARRAYITEM" => $"{name}: one repeated list item template. Changes here affect every generated row.",
				"UIICON" => $"{name}: game item/skill icon placeholder. The game fills the actual icon at runtime; usually edit only position and size.",
				"UISCROLLBAR" => $"{name}: scrollbar container. Its child back/button/thumb rows control the visible pieces.",
				_ => $"{name}: generic UI piece. Position, size, visibility, and raw XML attributes can be edited here."
			};
		}

		private static int GetInt(XElement element, string attribute, int fallback)
		{
			string? value = element.Attribute(attribute)?.Value;
			return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed) ? parsed : fallback;
		}

		private static string GetString(XElement element, string attribute)
		{
			return element.Attribute(attribute)?.Value ?? "";
		}

		private static void SetAttribute(XElement element, string attribute, string value)
		{
			element.SetAttributeValue(attribute, value);
		}

		private static void SetNumeric(NumericUpDown numeric, int value)
		{
			decimal clamped = Math.Min(numeric.Maximum, Math.Max(numeric.Minimum, value));
			numeric.Value = clamped;
		}

		private sealed class UiFileItem
		{
			public UiFileItem(string path)
			{
				Path = path;
			}

			public string Path { get; }

			public override string ToString()
			{
				return System.IO.Path.GetFileName(Path);
			}
		}

		private sealed class DepthFilterItem
		{
			public DepthFilterItem(int depth, string label)
			{
				Depth = depth;
				Label = label;
			}

			public int Depth { get; }
			private string Label { get; }

			public override string ToString()
			{
				return Label;
			}
		}
	}

	internal sealed class UiNode
	{
		public UiNode(XElement element, UiNode? parent)
		{
			Element = element;
			Parent = parent;
			Depth = parent == null ? 0 : parent.Depth + 1;
		}

		public XElement Element { get; }
		public UiNode? Parent { get; }
		public int Depth { get; }
		public List<UiNode> Children { get; } = new();
		public int X { get; set; }
		public int Y { get; set; }
		public int W { get; set; }
		public int H { get; set; }
		public int AbsX { get; set; }
		public int AbsY { get; set; }
		public bool Hidden { get; set; }
		public Rectangle Bounds { get; set; }
	}

	internal sealed class UiCanvas : Panel
	{
		private const int CanvasPad = 24;
		private const int HandleSize = 9;

		private readonly UiXmlEditor owner;
		private bool dragging;
		private bool resizing;
		private Point dragStart;
		private int startX;
		private int startY;
		private int startW;
		private int startH;

		public UiCanvas(UiXmlEditor editor)
		{
			owner = editor;
			DoubleBuffered = true;
			AutoScroll = true;
			BackColor = Color.FromArgb(36, 37, 42);
			TabStop = true;
			Dock = DockStyle.Fill;
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public UiNode? RootNode { get; set; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public UiNode? SelectedNode { get; set; }

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.SmoothingMode = SmoothingMode.None;
			e.Graphics.Clear(BackColor);

			if (RootNode == null)
			{
				TextRenderer.DrawText(e.Graphics, "Load a UI XML file to start.", Font, ClientRectangle, Color.Gainsboro, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
				return;
			}

			float zoom = owner.Zoom;
			AutoScrollMinSize = new Size((int)((RootNode.Bounds.Right + CanvasPad * 2) * zoom), (int)((RootNode.Bounds.Bottom + CanvasPad * 2) * zoom));

			e.Graphics.TranslateTransform(AutoScrollPosition.X + CanvasPad, AutoScrollPosition.Y + CanvasPad);
			e.Graphics.ScaleTransform(zoom, zoom);

			DrawGrid(e.Graphics, RootNode.Bounds);
			DrawNodeRecursive(e.Graphics, RootNode);
			DrawSelection(e.Graphics);
		}

		private void DrawGrid(Graphics graphics, Rectangle rootBounds)
		{
			using Pen gridPen = new(Color.FromArgb(52, 54, 61));
			for (int x = 0; x <= Math.Max(rootBounds.Right + 100, 900); x += 20)
				graphics.DrawLine(gridPen, x, 0, x, Math.Max(rootBounds.Bottom + 100, 700));
			for (int y = 0; y <= Math.Max(rootBounds.Bottom + 100, 700); y += 20)
				graphics.DrawLine(gridPen, 0, y, Math.Max(rootBounds.Right + 100, 900), y);
		}

		private void DrawNodeRecursive(Graphics graphics, UiNode node)
		{
			if (owner.ShouldDraw(node))
				DrawNode(graphics, node);

			foreach (UiNode child in node.Children)
				DrawNodeRecursive(graphics, child);
		}

		private void DrawNode(Graphics graphics, UiNode node)
		{
			Rectangle rect = node.Bounds;
			string tag = node.Element.Name.LocalName;
			Color color = GetPreviewColor(node);
			bool hidden = node.Hidden && !owner.ShouldForceVisible(node);

			using Brush fill = BuildFillBrush(rect, color, tag, hidden);
			using Pen pen = new(Color.FromArgb(hidden ? 90 : 210, color), node == SelectedNode ? 2f : 1f);
			if (hidden)
				pen.DashStyle = DashStyle.Dash;

			graphics.FillRectangle(fill, rect);
			graphics.DrawRectangle(pen, rect);

			string label = node.Element.Attribute("id")?.Value ?? tag;
			if (tag.Equals("UIText", StringComparison.OrdinalIgnoreCase))
			{
				string text = node.Element.Attribute("str")?.Value ?? "";
				if (!string.IsNullOrWhiteSpace(text))
					label = text;
			}
			else if (owner.ShowTextureNames)
			{
				string tex = node.Element.Attribute("tex")?.Value ?? "";
				if (!string.IsNullOrWhiteSpace(tex))
					label = $"{label}  [{tex} {GetTextureCutLabel(node)}]";
			}

			if (rect.Width >= 24 && rect.Height >= 10)
			{
				Rectangle labelRect = new(rect.X + 3, rect.Y + 2, Math.Max(1, rect.Width - 6), Math.Max(1, rect.Height - 4));
				Color textColor = GetReadableLabelColor(color, tag);
				TextRenderer.DrawText(graphics, label, SystemFonts.DefaultFont, labelRect, textColor, TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
			}
		}

		private static Brush BuildFillBrush(Rectangle rect, Color color, string tag, bool hidden)
		{
			int alpha = hidden ? 18 : 62;
			if (tag.Equals("UIImage", StringComparison.OrdinalIgnoreCase) || tag.Equals("UIButton", StringComparison.OrdinalIgnoreCase) || tag.Equals("UICheck", StringComparison.OrdinalIgnoreCase))
			{
				Color hatchBack = Color.FromArgb(hidden ? 15 : 44, color);
				Color hatchFore = Color.FromArgb(hidden ? 35 : 95, color);
				return new HatchBrush(HatchStyle.Percent20, hatchFore, hatchBack);
			}

			if (tag.Equals("UIBASE", StringComparison.OrdinalIgnoreCase))
				alpha = hidden ? 10 : 28;

			return new SolidBrush(Color.FromArgb(alpha, color));
		}

		private static Color GetPreviewColor(UiNode node)
		{
			string? colorText = node.Element.Attribute("color")?.Value
				?? node.Element.Attribute("text_color")?.Value
				?? node.Element.Attribute("text_col")?.Value
				?? node.Element.Attribute("color_on")?.Value;

			if (!string.IsNullOrWhiteSpace(colorText) && UiXmlEditor.TryParseLastChaosColor(colorText, out Color parsed))
				return parsed;

			return GetColor(node.Element.Name.LocalName);
		}

		private static Color GetReadableLabelColor(Color fillColor, string tag)
		{
			if (tag.Equals("UIText", StringComparison.OrdinalIgnoreCase))
				return fillColor.A < 45 ? Color.WhiteSmoke : Color.FromArgb(255, fillColor.R, fillColor.G, fillColor.B);

			double brightness = (fillColor.R * 0.299) + (fillColor.G * 0.587) + (fillColor.B * 0.114);
			return brightness > 150 ? Color.Black : Color.WhiteSmoke;
		}

		private static string GetTextureCutLabel(UiNode node)
		{
			string l = node.Element.Attribute("l")?.Value ?? "0";
			string t = node.Element.Attribute("t")?.Value ?? "0";
			string r = node.Element.Attribute("r")?.Value ?? "0";
			string b = node.Element.Attribute("b")?.Value ?? "0";
			return $"{l},{t}-{r},{b}";
		}

		private void DrawSelection(Graphics graphics)
		{
			if (SelectedNode == null || !owner.ShouldDraw(SelectedNode))
				return;

			Rectangle rect = SelectedNode.Bounds;
			using Pen pen = new(Color.Yellow, 2f);
			graphics.DrawRectangle(pen, rect);
			using SolidBrush brush = new(Color.Yellow);
			graphics.FillRectangle(brush, rect.Right - HandleSize, rect.Bottom - HandleSize, HandleSize, HandleSize);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Focus();

			if (e.Button != MouseButtons.Left)
				return;

			Point logical = ToLogical(e.Location);
			UiNode? hit = HitTest(logical);
			if (hit != null)
				owner.SelectNode(hit, true);
			else
				return;

			if (SelectedNode == null || !owner.ShouldDraw(SelectedNode))
				return;

			Rectangle handle = new(SelectedNode.Bounds.Right - HandleSize, SelectedNode.Bounds.Bottom - HandleSize, HandleSize + 3, HandleSize + 3);
			resizing = handle.Contains(logical);
			dragging = true;
			dragStart = logical;
			startX = SelectedNode.X;
			startY = SelectedNode.Y;
			startW = SelectedNode.W;
			startH = SelectedNode.H;
			Capture = true;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (!dragging || SelectedNode == null)
				return;

			Point logical = ToLogical(e.Location);
			int dx = logical.X - dragStart.X;
			int dy = logical.Y - dragStart.Y;

			if (resizing)
				owner.SetSelectedGeometry(startX, startY, Math.Max(1, startW + dx), Math.Max(1, startH + dy), false);
			else
				owner.SetSelectedGeometry(startX + dx, startY + dy, startW, startH, false);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (!dragging)
				return;

			dragging = false;
			resizing = false;
			Capture = false;
			if (SelectedNode != null)
				owner.SetSelectedGeometry(SelectedNode.X, SelectedNode.Y, SelectedNode.W, SelectedNode.H, true);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			int step = e.Shift ? 10 : 1;
			if (e.KeyCode == Keys.Left)
				owner.NudgeSelected(-step, 0);
			else if (e.KeyCode == Keys.Right)
				owner.NudgeSelected(step, 0);
			else if (e.KeyCode == Keys.Up)
				owner.NudgeSelected(0, -step);
			else if (e.KeyCode == Keys.Down)
				owner.NudgeSelected(0, step);
			else
				return;

			e.Handled = true;
		}

		private UiNode? HitTest(Point logical)
		{
			foreach (UiNode node in owner.VisibleNodesBackToFront())
			{
				if (node.Bounds.Contains(logical))
					return node;
			}

			return null;
		}

		private Point ToLogical(Point point)
		{
			float zoom = owner.Zoom;
			return new Point(
				(int)Math.Round((point.X - AutoScrollPosition.X - CanvasPad) / zoom),
				(int)Math.Round((point.Y - AutoScrollPosition.Y - CanvasPad) / zoom));
		}

		private static Color GetColor(string tag)
		{
			return tag.ToUpperInvariant() switch
			{
				"WINDOW" => Color.FromArgb(160, 160, 170),
				"UIBASE" => Color.FromArgb(82, 150, 230),
				"UIIMAGE" => Color.FromArgb(208, 139, 70),
				"UITEXT" => Color.FromArgb(78, 190, 118),
				"UITEXTBOX" => Color.FromArgb(88, 205, 155),
				"UIBUTTON" => Color.FromArgb(235, 187, 72),
				"UICHECK" => Color.FromArgb(235, 187, 72),
				"UIEDIT" => Color.FromArgb(176, 118, 225),
				"UITAB" => Color.FromArgb(112, 135, 160),
				"UITABPAGE" => Color.FromArgb(104, 122, 145),
				"UITREE" => Color.FromArgb(117, 165, 175),
				"UILIST" => Color.FromArgb(117, 165, 175),
				"UIARRAY" => Color.FromArgb(117, 165, 175),
				"UIICON" => Color.FromArgb(230, 102, 118),
				_ => Color.FromArgb(150, 150, 155)
			};
		}
	}
}

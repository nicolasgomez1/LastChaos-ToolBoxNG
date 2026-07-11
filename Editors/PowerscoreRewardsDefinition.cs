using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LastChaos_ToolBoxNG
{
	internal sealed class PowerscoreRewardTier
	{
		public int Id { get; set; }
		public int Powerscore { get; set; }
		public string Label { get; set; } = "Milestone";
		public int ItemIndex { get; set; }
		public string ItemName { get; set; } = "";
		public int Quantity { get; set; }

		public PowerscoreRewardTier Copy()
		{
			return new PowerscoreRewardTier
			{
				Id = Id,
				Powerscore = Powerscore,
				Label = Label,
				ItemIndex = ItemIndex,
				ItemName = ItemName,
				Quantity = Quantity
			};
		}
	}

	internal static class PowerscoreRewardsDefinition
	{
		private const string RootName = "powerscore_rewards";
		private const string TierName = "milestone";

		public static List<PowerscoreRewardTier> Load(string path)
		{
			XDocument document = XDocument.Load(path, LoadOptions.SetLineInfo);
			XElement root = document.Root ?? throw new InvalidDataException("The Powerscore reward definition has no root element.");
			if (!string.Equals(root.Name.LocalName, RootName, StringComparison.Ordinal))
				throw new InvalidDataException($"Expected <{RootName}> but found <{root.Name.LocalName}>.");

			List<PowerscoreRewardTier> tiers = [];
			foreach (XElement element in root.Elements(TierName))
			{
				PowerscoreRewardTier tier = new()
				{
					Id = ReadInt(element, "id"),
					Powerscore = ReadInt(element, "powerscore"),
					ItemIndex = ReadInt(element, "item_index"),
					Quantity = ReadInt(element, "quantity"),
					Label = ((string?)element.Attribute("label") ?? "Milestone").Trim()
				};
				tiers.Add(tier);
			}

			string? validation = Validate(tiers);
			if (validation != null)
				throw new InvalidDataException(validation);

			return tiers.OrderBy(t => t.Powerscore).ThenBy(t => t.Id).ToList();
		}

		public static string? Validate(IReadOnlyCollection<PowerscoreRewardTier> tiers)
		{
			if (tiers.Count == 0)
				return "At least one Powerscore reward tier is required.";

			HashSet<int> ids = [];
			HashSet<int> powerscores = [];
			foreach (PowerscoreRewardTier tier in tiers)
			{
				if (tier.Id <= 0)
					return "Every reward tier needs a positive milestone ID.";
				if (!ids.Add(tier.Id))
					return $"Milestone ID {tier.Id} is duplicated.";
				if (tier.Powerscore <= 0)
					return $"Milestone {tier.Id} needs a positive Powerscore threshold.";
				if (!powerscores.Add(tier.Powerscore))
					return $"Powerscore threshold {tier.Powerscore} is duplicated.";
				if (tier.ItemIndex <= 0)
					return $"Milestone {tier.Id} needs a positive reward item ID.";
				if (tier.Quantity <= 0)
					return $"Milestone {tier.Id} needs a positive reward quantity.";

				string label = (tier.Label ?? "").Trim();
				if (label.Length == 0)
					return $"Milestone {tier.Id} needs a label.";
				if (label.Length > 64)
					return $"Milestone {tier.Id} has a label longer than 64 characters.";
			}

			return null;
		}

		public static string Save(string path, IReadOnlyCollection<PowerscoreRewardTier> tiers)
		{
			string? validation = Validate(tiers);
			if (validation != null)
				throw new InvalidDataException(validation);

			List<PowerscoreRewardTier> ordered = tiers
				.Select(t => t.Copy())
				.OrderBy(t => t.Powerscore)
				.ThenBy(t => t.Id)
				.ToList();

			XElement root = new(RootName, new XAttribute("version", "1"));
			root.Add(new XComment(" Presentation data only. The server remains authoritative for progress and claims. "));
			foreach (PowerscoreRewardTier tier in ordered)
			{
				root.Add(new XElement(TierName,
					new XAttribute("id", tier.Id.ToString(CultureInfo.InvariantCulture)),
					new XAttribute("powerscore", tier.Powerscore.ToString(CultureInfo.InvariantCulture)),
					new XAttribute("item_index", tier.ItemIndex.ToString(CultureInfo.InvariantCulture)),
					new XAttribute("quantity", tier.Quantity.ToString(CultureInfo.InvariantCulture)),
					new XAttribute("label", tier.Label.Trim())));
			}

			XDocument document = new(new XDeclaration("1.0", "utf-8", null), root);
			string fullPath = Path.GetFullPath(path);
			string folder = Path.GetDirectoryName(fullPath) ?? throw new InvalidOperationException("The definition path has no parent folder.");
			Directory.CreateDirectory(folder);

			string backupPath = "";
			if (File.Exists(fullPath))
			{
				backupPath = CreateBackupPath(fullPath);
				File.Copy(fullPath, backupPath, false);
			}

			string temporaryPath = fullPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
			try
			{
				XmlWriterSettings settings = new()
				{
					Encoding = new UTF8Encoding(false),
					Indent = true,
					IndentChars = "    ",
					NewLineChars = Environment.NewLine,
					NewLineHandling = NewLineHandling.Replace,
					OmitXmlDeclaration = false
				};

				using (XmlWriter writer = XmlWriter.Create(temporaryPath, settings))
					document.Save(writer);

				File.Move(temporaryPath, fullPath, true);
			}
			finally
			{
				if (File.Exists(temporaryPath))
					File.Delete(temporaryPath);
			}

			return backupPath;
		}

		private static int ReadInt(XElement element, string attributeName)
		{
			string? value = (string?)element.Attribute(attributeName);
			if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
				return parsed;

			IXmlLineInfo lineInfo = element;
			string location = lineInfo.HasLineInfo() ? $" on line {lineInfo.LineNumber}" : "";
			throw new InvalidDataException($"Milestone{location} has an invalid or missing {attributeName} attribute.");
		}

		private static string CreateBackupPath(string path)
		{
			string stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff", CultureInfo.InvariantCulture);
			string backup = path + "." + stamp + ".bak";
			int suffix = 1;
			while (File.Exists(backup))
				backup = path + "." + stamp + $"-{suffix++}.bak";
			return backup;
		}
	}
}

public class Defs
{
	public const int MAX_MAKE_ITEM_MATERIAL = 10;      // Standard
	public const int MAX_MAKE_ITEM_NUM = 4;            // Standard
	public const int MAX_MAKE_ITEM_SET = 5;            // Standard
	public const int DEF_SMC_DEFAULT_LENGTH = 64;      // Standard
	public const int DEF_EFFECT_DEFAULT_LENGTH = 32;   // Standard
	public const int DEF_MAX_ORIGIN_OPTION = 10;       // Standard
	public const int LACARETTE_GIFTDATA_TOTAL = 10;    // Standard
	public const int DEF_CONDITION_ITEM_MAX = 5;       // Standard
	public const int SSKILL_MAX_LEVEL = 5;             // Standard
	public const int JUMPSIZE = 20;                    // Standard
	public const int DEF_SKILL_MAX_MAGIC = 3;          // Standard
	public const int DEF_SKILL_LEVEL_NEEDED_ITEM = 2;  // Standard
	public const int DEF_SKILL_LEVEL_LEARN_SKILL_NEEDED = 3;   // Standard
	public const int DEF_SKILL_LEVEL_LEARN_ITEM_NEEDED = 3;    // Standard
	public const int DEF_SKILL_LEVEL_APP_MAGIC = 3;    // Standard
	public const int DEF_SKILL_LEVEL_MAGIC = 3;        // Standard
	public const int DEF_TRADE_WEAPON_MAX = 13;        // NicolasG Custom
	public const int DEF_TRADE_ARMOR_MAX = 9;          // NicolasG Custom
	public const int DEF_TRADE_MAX_GROUPS = 9;         // NicolasG Custom
	public const int DEF_TRADE_GROUP_TYPES = 6;        // NicolasG Custom
	public const int DEF_SKILL_COL = 8;                // Standard
	public const int DEF_APET_NAME_LENGTH = 20;        // Standard
	public const int DEF_SMCFILE_LENGTH = 64;          // Standard
	public const int DEF_APET_ANI_LENGTH = 32;         // Standard
	public const int DEF_MAX_EVOLUTION = 4;            // Standard
	public const int DEF_MAX_ACCEXP = 1;               // Standard
	public const int DEF_RAREOPTION_MAX = 10;          // Standard
	public const int MAX_MAX_NEED_ITEM = 5;            // Standard
	public const int QUEST_MAX_CONDITION = 3;          // Standard
	public const int QUEST_MAX_PRIZE = 5;              // Standard
	public const int QUEST_MAX_OPTION_PRIZE = 7;       // Standard
	public const int QUEST_MAX_CONDITION_DATA = 4;     // Standard
	public const int DEF_MAX_NPC_FIRE_DELAY = 4;       // Standard
	public const int MAX_NPC_SKILL_SERVER = 4;			// Standard
	public const int MAX_NPC_SKILL_CLIENT = 2;         // Standard
	public const int DEF_SMC_LENGTH = 128;             // Standard
	public const int DEF_ANI_LENGTH = 64;              // Standard
	public const int DEF_MOB_FIRE_EFFECT = 3;          // Standard
	public const int DEF_OPTION_MAX_LEVEL = 36;        // "Standard"	// NOTE: Exist OPTION_MAX_LEVEL 7 and DEF_OPTION_MAX_LEVEL 36. Take care, they are not the same thing.
	public const int MAX_TITLE_EFFECT_LENGTH = 64;     // Standard
	public const int DEF_NICK_OPTION_MAX = 5;          // Standard
	public const int MAX_WEARING = 12;                 // Standard
	public const int MAX_SET_ITEM_OPTION = 11;         // Standard
	public const int DEF_NEED_ITEM_COUNT = 6;          // Standard
	public const int MAX_AREA_COUNT = 64;              // NicolasG Custom (I think original value is 300)
	public const int MAX_FACTORY_ITEM_STUFF = 5;       // Standard
	//public const int MAX_SEAL_TYPE_SKILL = 10;			// Standard
	public const int MAX_NPC_DROPITEM = 20;            // Standard
	public const int MAX_NPC_DROPJEWEL = 20;            // Standard
	public const int MAX_NPC_PRODUCT = 5;              // Standard
	public const int AT_LEVELMAX = 5;                  // Standard
	public const int AT_MASK = 0xF;
	public const int AT_LVVEC = 4;
	public const int AT_AD_MASK = 0xFF;
	public const int AT_ADVEC = 8;
	public const int MAX_CHANNELS = 4;                 // "Standard"
	public const int NAS_ITEM_DB_INDEX = 19;           // Standard
	public const float m_fZoomDetail = 1.0f;           // Standard
	public const float WorldRatio = 0.3333f;           // Standard (0.3333 for 512x512, 0.6666 for 1024x1024 worlds size)
	public const int MAX_MISSIONCASE_STEPS = 5;        // "Standard"
	public const int MAX_TITLE_OPTION = 5;             // Standard

	public class StringType
	{
		public string FileName;
		public string Clause;
		public string TableName;
		public List<string> Columns;
		public string Condition;

		public StringType(string strFileName, string strClause, string strTableName, List<string> columns, string condition)
		{
			FileName = strFileName;
			Clause = strClause;
			TableName = strTableName;
			Columns = columns;
			Condition = condition;
		}
	}

	public static readonly Dictionary<string, StringType> StringTypes = new Dictionary<string, StringType>
	{
		//["NPCSHOP"] = new StringTypeInfo("", "", "", [""], ""),
		//["NPCHELP"] = new StringTypeInfo("", "", "", [""], ""),
		["LACARETTE"] = new StringType("strLacarette", "SELECT", "t_lacarette", ["a_index", "a_name"], "WHERE a_enable=1"),
		["AFFINITY"] = new StringType("strAffinity", "SELECT", "t_affinity", ["a_index", "a_name"], "WHERE a_enable=1"),
		["COMBO"] = new StringType("strCombo", "SELECT", "t_missioncase", ["a_index", "a_name"], "WHERE a_enable=1"),
		["ACTION"] = new StringType("strAction", "SELECT", "t_action", ["a_index", "a_name", "a_client_description"], ""),
		["SPECIALSKILL"] = new StringType("strSSkill", "SELECT", "t_special_skill", ["a_index", "a_name", "a_desc"], "WHERE a_enable=1"),
		["SKILL"] = new StringType("strSkill", "SELECT", "t_skill", ["a_index", "a_name", "a_client_description", "a_client_tooltip"], "WHERE a_job>=0"),
		["QUEST"] = new StringType("strQuest", "SELECT", "t_quest", ["a_index", "a_name", "a_desc", "a_desc2", "a_desc3"], ""),
		["NPCNAME"] = new StringType("strNPCName", "SELECT DISTINCT", "t_npc", ["a_index", "a_name", "a_descr"], "WHERE a_enable=1"),
		["RAREOPTION"] = new StringType("strRareOption", "SELECT", "t_rareoption", ["a_index", "a_prefix"], ""),
		["RAREOPTION"] = new StringType("strRareOption", "SELECT", "t_rareoption", ["a_index", "a_prefix"], ""),
		["OPTION"] = new StringType("strOption", "SELECT", "t_option", ["a_index", "a_name"], ""),
		["ITEMCOLLECTION"] = new StringType("strItemCollection", "SELECT", "t_item_collection", ["a_theme", "a_theme_string", "a_desc_string"], ""),
		["SETITEM"] = new StringType("strSetItem", "SELECT", "t_set_item", ["a_set_idx", "a_set_name"], "WHERE a_enable=1"),
		["ITEM"] = new StringType("strItem", "SELECT", "t_item", ["a_index", "a_name", "a_descr"], "WHERE a_enable=1"),
		//["HELP1"] = new StringType("strHelp", "SELECT", "t_help1", ["a_index", "a_name", "a_desc"], ""),	// NOTE: Uncomment this line if need strHelp_X.lod files
		["STRING"] = new StringType("strClient", "SELECT", "t_string", ["a_index", "a_string"], "")
	};

	public static readonly Dictionary<string, List<string>> ItemTypesNSubTypes = new Dictionary<string, List<string>>
	{
		// Standards
		{
			"Weapon",
			[
				// Standards
				"(Knight) Single Sword",
				"(EX|Rogue) Crossbow",
				"(Arch|Mage) Staff",
				"(Titan) Bigsword",
				"(Titan) Axe",
				"(Arch|Mage) Short Staff (Wand)",
				"(Healer) Bow",
				"(EX|Rogue) Daggers",
				"Mining (Hammer)",
				"Gathering (Knife)",
				"Charge (Energy Collector)",
				"(Knight) Double Swords",
				"(Healer) Scepter",
				"(Sorcerer) Scythe",
				"(Sorcerer) Fallarm",
				"(Nightshadow) Soul"
			]
		},
		{
			"Armor",
			[
				// Standards
				"Helmet",
				"Shirt",
				"Pants",
				"Gloves",
				"Boots",
				"Shield",
				"Backpack | Wings",
				"Complete Costume (SUIT)"
			]
		},
		{
			"Once (Varied)",
			[
				// Standards
				"Teleporting (WARP)",
				"Production Manual",
				"Crafting Manual",
				"Box",
				"Potion Creation Manual",
				"Transformation Scroll",
				"Quest Scroll",
				"Changing Sutff (CASH)",
				"Summon",
				"Box or MonsterCombo (ETC)",
				"Attack Scroll (TARGET)",
				"Title",
				"Reward Package",
				"Jumping Potion",
				"Extend Chars Slot",
				"Server Trans",
				"Remote Express",
				"Jewel Pocket",
				"Chaos Jewel Pocket",
				"Cash Inventory",
				"Pet Stash",
				"GPS",
				"Holy Water",
				"Protect PvP"
			]
		},
		{
			"Shot",
			[
				// Standards
				"Bullet Attack",
				"Bullet Mana",
				"Bullet Arrow"
			]
		},
		{
			"Etc (Quest, Event, Upgrade)",
			[
				// Standards
				"Quest",
				"Event",
				"Skill Up",
				"Upgrade",
				"Material",
				"Gold (MONEY)",
				"Product",
				"Process",
				"Bloodseal (OPTION)",
				"Powder (SAMPLE)",
				"Event Item (TEXTURE)",
				"Castle Siege Concentration (MIX_TYPE1)",
				"Castle Siege Powder (MIX_TYPE2)",
				"Castle Siege Stone (MIX_TYPE3)",
				"APet Target (PET_AI)",
				"Quest Trigger",
				"Socket Jewel (JEWEL)",
				"Socket Upgrading (STABILIZER)",
				"Socket Creation (PROCESS_SCROLL)",
				"Mercenary (MONSTER_MERCENARY_CARD)",
				"Guild Mark",
				"Reformer",
				"Chaos Jewel",
				"Functions",
				"RvR Jewel"
			]
		},
		{
			"Accesory",
			[
				// Standards
				"Charm",
				"Magic Stone",
				"Light Stone",
				"Earing",
				"Ring",
				"Necklace",
				"Pet",
				"APet (ATTACK_PET)",
				"Artifact"
			]
		},
		{
			"Potion",
			[
				// Standards
				"Antidote/Cure (STATE)",
				"HP Recover (HP)",
				"MP Recover (MP)",
				"HP & MP (DUAL)",
				"Statistic (STAT)",
				"Steroid (ETC)",
				"Mineral (UP)",
				"Tears",
				"Exp Crystal (CRYSTAL)",
				"NPC Scroll (NPC_PORTAL)",
				"HP Recovery Speed Potion (HP_SPEEDUP)",
				"MP Recovery Speed Potion (MP_SPEEDUP)",
				"APet HP Recover (PET_HP)",
				"APet Speed Up (PET_SPEEDUP)",
				"Totem",
				"APet MP Recover (PET_MP)"
			]
		}
	};

	public static readonly string[] ItemWearingPositions =
	{
		// Standards
		"-1 - None",
		"0 - Helmet",
		"1 - Shirt",
		"2 - Weapon",
		"3 - Pants",
		"4 - Shield",
		"5 - Gloves",
		"6 - Boots",
		"7 - Accesory 1",
		"8 - Accesory 2",
		"9 - Accesory 3",
		"10 - Pet",
		"11 - Backpack | Wings"
	};

	public static readonly Dictionary<string, List<string>> SyndicateTypesNGrades = new Dictionary<string, List<string>>
	{
		// Standards
		{
			"None", []
		},
		{
			"Kailux",
			[
				// Standards
				"Squire",
				"Knight",
				"Gentor",
				"Honorise",
				"Barone",
				"Visconte",
				"Conte",
				"Marquise",
				"Duka",					// NOTE: probably not used
				"Principal"	// Principe	// NOTE: probably not used
			]
		},
		{
			"Dilamun",
			[
				// Standards
				"Neopyte",	// Neoptye
				"Zelator",
				"Theoricus",
				"Philosophus",
				"Adeptus",
				"Magus",		// NOTE: probably not used
				"Ipsissimus"	// NOTE: probably not used
			]
		}
	};

	public static readonly string[] ItemCastleTypes =
	{
		// Standards
		"Done",
		"All",
		"Merac",
		"Dratan"
	};

	public static readonly string[] ItemClass =
	{
		// Standards
		"Titan",
		"Knight",
		"Healer",
		"Mage",
		"Rogue",
		"Sorcerer",
		"Nightshadow",
		"Ex-Rogue",
		"Arch-Mage",
		"Pet",
		"APet",
		"Unknown"	// NOTE: Some apet items have 2048, are too much items to be a error, but flag only goest up to APET, i'm not sure of that.
	};

	public static readonly string[] ItemFlags =
	{
		// Standards
		"COUNT",
		"DROP",
		"UPGRADE",
		"EXCHANGE",
		"TRADE",
		"BORKEN",
		"MADE",
		"MIX",
		"CASH",
		"LORD",
		"NO_STASH",
		"CHANGE",
		"COMPOSITE",
		"DUPLICATE",
		"LENT",
		"RARE",
		"ABS",
		"NOTREFORM",
		"ZONEMOVE_DEL",
		"ORIGIN",
		"TRIGGER",
		"RAIDSPECIAL",
		"QUEST",
		"BOX",
		"NOTTRADEAGENT",
		"DURABILITY",
		"COSTUME2",
		"SOCKET",
		"SELLER",
		"CASTLLAN",
		"LETSPARTY",
		"NONRVR",
		"QUESTGIVE",
		"TOGGLE",
		"COMPOSE",
		"NOTSINGLE",
		"INVISIBLE_COSTUME",
		// NicolasG Custom
		"MONEY_TICKET",
		"PARTY_TELEPORT"
	};

	public static readonly string[] APetTypes =
	{
		// Standards
		"None",
		"Human",
		"Beast",
		"Demon"
	};

	public static readonly string[] JewelCompositePosition =
	{
		// Standards
		"Weapon",
		"Helmet",
		"Armor",
		"Pants",
		"Gloves",
		"Boots",
		"Shield",
		"Backpack | Wings",
		"Accesory",
		"All"
	};

	public static readonly string[] FortuneItemProbTypes =
	{
		// Standards
		"Prob",
		"Random"
	};

	public static readonly string[] OptionTypes =
	{
		// Standards
		"STR_UP",
		"DEX_UP",
		"INT_UP",
		"CON_UP",
		"HP_UP",
		"MP_UP",
		"DAMAGE_UP",
		"MELEE_DAMAGE_UP",
		"RANGE_DAMAGE_UP",
		"MELEE_HIT_UP",
		"RANGE_HIT_UP",
		"DEFENSE_UP",
		"MELEE_DEFENSE_UP",
		"RANGE_DEFENSE_UP",
		"MELEE_AVOID_UP",
		"RANGE_AVOID_UP",
		"MAGIC_UP",
		"MAGIC_HIT_UP",
		"RESIST_UP",
		"RESIST_AVOID_UP",
		"ALL_DAMAGE_UP",
		"ALL_HIT_UP",
		"ALL_DEFENSE_UP",
		"ALL_AVOID_UP",
		"NOT_USED_24",
		"NOT_USED_25",
		"ATTR_FIRE",
		"ATTR_WATER",
		"ATTR_WIND",
		"ATTR_EARTH",
		"ATTR_DARK",
		"ATTR_LIGHT",
		// 2009 Source (Missing in posterior versions)
		"ATT_WATER_DOWN (2009)",
		"ATT_WIND_DOWN (2009)",
		"ATT_EARTH_DOWN (2009)",
		"ALL_ATT_DOWN (2009)",
		// Standards
		"MIX_STR",
		"MIX_DEX",
		"MIX_INT",
		"MIX_CON",
		"MIX_ATTACK",
		"MIX_MAGIC",
		"MIX_DEFENSE",
		"MIX_RESIST",
		"MIX_STURN",
		"MIX_BLOOD",
		"MIX_MOVE",
		"MIX_POISON",
		"MIX_SLOW",
		"DOWN_LIMITLEVEL",
		"INCREASE_INVEN",
		"STEAL_MP",
		"STEAL_HP",
		"ATTACK_BLIND",
		"ATTACK_POISON",
		"ATTACK_CRITICAL",
		"RECOVER_HP",
		"RECOVER_MP",
		"DECREASE_SKILL_DELAY",
		"DECREASE_SKILL_MP",
		"RESIST_STONE",
		"RESIST_STURN",
		"RESIST_SILENT",
		"BLOCKING",
		"MOVESPEED",
		"FLYSPEED",
		"ATTACK_DEADLY",
		"STR_UP_RATE",
		"DEX_UP_RATE",
		"INT_UP_RATE",
		"CON_UP_RATE",
		"HP_UP_RATE",
		"MP_UP_RATE",
		"WEAPON_UP_RATE",
		"ARMOR_UP_RATE",
		"MELEE_HIT_UP_RATE",
		"MAGIC_HIT_UP_RATE",
		"MELEE_AVOID_RATE",
		"MAGIC_AVOID_RATE",
		"RECOVER_HP_RATE",
		"RECOVER_MP_RATE",
		"TEST_QUEST",
		"EXP_UP_RATE",
		"SP_UP_RATE",
		"APET_TRANS_END",
		"APET_ELEMENT_HPUP",
		"APET_ELEMENT_ATTUP",
		"ATT_FIRE_UP",
		"ATT_WATER_UP",
		"ATT_WIND_UP",
		"ATT_EARTH_UP",
		"ATT_DARK_UP",
		"ATT_LIGHT_UP",
		"DEF_FIRE_UP",
		"DEF_WATER_UP",
		"DEF_WIND_UP",
		"DEF_EARTH_UP",
		"DEF_DARK_UP",
		"DEF_LIGHT_UP",
		"ALL_STAT_UP",
		"PVP_DAMAGE_ABSOLB",
		"DEBUF_DECR_TIME",
		"RECOVER_HP_NOTRATE",
		"RECOVER_MP_NOTRATE",
		"INCREASE_STRONG",
		"INCREASE_HARD",
		"INCREASE_HP",
		"CLIENT_1",
		"CLIENT_2",
		"CLIENT_3",
		"CLIENT_4",
		"CLIENT_5",
		"CLIENT_6",
		// NicolasG Custom
		"SUSTAINER",
		"REGEN_EP"
	};

	public static readonly Dictionary<string, List<string>> MagicTypesAndSubTypes = new Dictionary<string, List<string>>
	{
		// Standards
		{
			"Stat",
			[
				// Standards
				"Attack",
				"Defense",
				"Magic",
				"Resist",
				"Hit Rate",
				"Avoid",
				"Critical",
				"Attack Speed",
				"Magic Speed",
				"Move Speed",
				"Recover HP",
				"Recover MP",
				"Max HP",
				"Max MP",
				"Deadldy",
				"Magic Hit Rate",
				"Magic Avoid",
				"Attack Distance",
				"Attack Melee",
				"Attack Range",
				"Hit Rate Skill",
				"Attack 80",
				"Max HP 450",
				"Skill Speed",
				"Valor",
				"Statpall",
				"Attack Per",
				"Defense Per",
				"Statpall Per",
				"STR",
				"DEX",
				"INT",
				"CON",
				"Hard",
				"Strong",
				"NPC Attack",
				"NPC Magic",
				"Skill Cool Time",
				"Decrase Mana Spend"
			]
		},
		{
			"Attribute",
			[
				// Standards
				"Neutral",
				"Fire",
				"Def Water | Att Ice",
				"Earth",
				"Def Wind | Att Storm",
				"Darkess",
				"Light",
				"Random"
			]
		},
		{
			"Assist",
			[
				// Standards
				"Poison",
				"Hold",
				"Confusion",
				"Stone",
				"Silent",
				"Blood",
				"Blind",
				"Sturn",
				"Sleep",
				"HP",
				"MP",
				"Move Speed",
				"HP Cancel",
				"MP Cancel",
				"Dizzy",
				"Invisible",
				"Sloth",
				"Fear",
				"Fake Death",
				"Perfect Body",
				"Frenzy",
				"Damage Link",
				"Berserk",
				"Despair",
				"Mana Screen",
				"Bless",
				"Safeguard",
				"Mantle",
				"Guard",
				"Charge Attack",
				"Charge Magic",
				"Disease",
				"Curse",
				"Confused",
				"Taming",
				"Freeze",
				"Inverse Damage",
				"HP Dot",
				"Rebirth",
				"Darkness Mode",
				"Aura Darkness",
				"Aura Weakness",
				"Aura Illusion",
				"Mercenary",
				"Soul Totem Buff",
				"Soul Totem Attack",
				"Trap",
				"Parasite",
				"Suicide",
				"Invincibilty",
				"GPS",
				"Attack Tower",
				"Artifact GPS",
				"Totem Item Buff",
				"Totem Item Attack"
			]
		},
		{
			"Attack",
			[
				// Standards
				"Normal",
				"Critical",
				"Drain",
				"One Shot Kill",
				"Deadly",
				"Hard"
			]
		},
		{
			"Recover",
			[
				// Standards
				"HP",
				"MP",
				"STM",
				"Faith",
				"EXP",
				"SP"
			]
		},
		{
			"Cure",
			[
				// Standards
				"Posion",
				"Hold",
				"Confusion",
				"Stone",
				"Silent",
				"Blood",
				"Rebirth",
				"Invisible",
				"Sturn",
				"Sloth",
				"Not Help",
				"Blind",
				"Disease",
				"Curse",
				"All",
				"Instant Death"
			]
		},
		{
			"Other",
			[
				// Standards
				"Instant Death",
				"Skill Cancel",
				"Tackle",
				"Tackle 2",
				"Reflex",
				"Death EXP Plus",
				"Death SP Plus",
				"Telekinesis",
				"Tount",
				"Summon",
				"Evocation",
				"Target Free",
				"Curse",
				"Peace",
				"Soul Drain",
				"Knockback",
				"Warp",
				"Fly",
				"EXP",
				"SP",
				"Item Drop",
				"Skill",
				"PK Disposition",
				"Affinity",
				"Affinity Quest",
				"Affinity Monster",
				"Affinity Item",
				"Quest Exp",
				"Guild Party Exp",
				"Guild Party Sp",
				"Summon NPC"
			]
		},
		{
			"Reduce",
			[
				// Standards
				"Melee",
				"Range",
				"Magic",
				"Skill"
			]
		},
		{
			"Immune",
			[
				// Standards
				"Blind"
			]
		},
		{
			"Castle",
			[
				// Standards
				"Melee",
				"Range",
				"Magic",
				"Max HP",
				"Defense",
				"Resist",
				"Tower Attack"
			]
		},
		{
			"Money",
			[
				// Standards
				"Buy",
				"Sell",
				"Nas"
			]
		}
	};

	public static readonly string[] MagicDamageTypes =
	{
		// Standards
		"Only Power",
		"Addition",
		"Rate"
	};

	public static readonly string[] MagicHitTypes =
	{
		// Standards
		"Constant",
		"Variable"
	};

	public static readonly string[] MagicParamTypes =
	{
		// Standards
		"None",
		"Strength",
		"Dexterity",
		"Intelligence",
		"Constitution"
	};

	public static readonly string[] IETCTypes =
	{
		// Standards
		"UPGRADE_GENERAL",
		"UPGRADE_SPECIAL",
		"UPGRADE_SPECIAL_SUPER",
		"UPGRADE_BOOSTER",
		"UPGRADE_LUCKY",
		"UPGRADE_PLATINUM",
		"UPGRADE_CHAOS",
		"UPGRADE_PURITY_RUNE",
		"UPGRADE_DEVIL_RUNE",
		"UPGRADE_CHAOS_RUNE",
		"UPGRADE_SUPER_RUNE",
		"UPGRADE_LUCKY_RUNE"
	};

	public static readonly string[] ItemGrades =
	{
		// Standards
		"NORMAL (-1)",	// Red
		"HERO",			// Blue
		"UNIQUE",		// Green
		"RARE",			// Yellow
		"MAGIC",		// Light Green
		"BASIC",		// Light Cyan
		"NOTOPEN",		// ¿White?
		"ORIGIN"		// Purple
	};

	public static readonly string[] RareOptionItemGrades = ItemGrades.SkipWhile(grade => grade != "HERO").TakeWhile(grade => grade != "NOTOPEN").ToArray();	// Standards

	public static readonly Dictionary<string, int> MoonStonesNamesNIDS = new Dictionary<string, int>
	{
		// Standards
		{ "Poor Moonstone", 2545 },
		{ "Average Moonstone", 2546 },
		{ "Moon Stone", 723 },
		{ "Good Moonstone", 2547 },
		{ "Perfect Moonstone", 2548 }
		//, { "Moonstone", 6092 }	// Extra unused moonstone for Gamigo
	};

	public static readonly Dictionary<int, List<string>> CharactersClassNJobsTypes = new Dictionary<int, List<string>>
	{
		// Standards
		{ 0, ["0 - Titan",       "1 - Highlander",   "2 - Warmaster"] },
		{ 1, ["0 - Knight",      "1 - Royal",        "2 - Templar"] },
		{ 2, ["0 - Healer",      "1 - Archer",       "2 - Cleric"] },
		{ 3, ["0 - Mage",        "1 - Wizard",       "2 - Witch"] },
		{ 4, ["0 - Rogue",       "1 - Assasin",      "2 - Ranger"] },
		{ 5, ["0 - Sorcerer",    "1 - Elementalist", "2 - Specialist"] },
		{ 6, ["0 - NightShadow", "1 - NightShadow",  "2 - NightShadow"] },
		{ 7, ["0 - Ex-Rogue",    "1 - Ex-Assasin",   "2 - Ex-Ranger"] },
		{ 8, ["0 - ArchMage",    "1 - Arch-Wizard",  "2 - Arch-Witch"] }
	};

	public static readonly string[] NPCFlags =
	{
		// Standards
		"SHOPPER",
		"FIRSTATTACK",
		"ATTACK",
		"MOVING",
		"PEACEFUL",
		"ZONEMOVER",
		"CASTLE_GUARD",
		"REFINER",
		"QUEST",
		"CASTLE_TOWER",
		"MINERAL",
		"CROPS",
		"ENERGY",
		"ETERNAL",
		"LORD_SYMBOL",
		"REMISSION",
		"EVENT",
		"GUARD",
		"KEEPER",
		"GUILD",
		"MBOSS",
		"BOSS",
		"RESETSTAT",
		"CHANGEWEAPON",
		"WARCASTLE",
		"DISPLAY_MAP",
		"QUEST_COLLECT",
		"PARTY_MOB",
		"RAID",
		"SUBCITY",
		"CHAOCITY",
		"HUNTERCITY",
		// NicolasG Custom
		"WORLD_BOSS_DROP"
	};

	// NOTE: WTF why these don't match...
	public static readonly string[] NPCFlags1_CLIENT =
	{
		// Standards
		"AUCTION",			// 0
		"COLLSION_ENABLE",	// 1
		"FACTORY",			// 2
		"TRIGGER_CHOICE",	// 3
		"TRIGGER_KILL?",	// 4
		"DONT_ATTACK",
		"AFFINITY",
		"SHADOW",
		"CRAFTING",
		"TOTEM_ITEM",
		"NO_SHADOW_CAST"
	};

	public static readonly string[] NPCFlags1 =
	{
		// Standards
		"TRADEAGENT",	// CLIENT: AUCTION
		"COLLSION",		// CLIENT: COLLSION_ENABLE
		"FACTORY",
		"TRIGGER_CHOICE",
		"TRIGGER_KILL",	// NOT EXIST IN CLIENT
		"NOT_NPCPORTAL",// NOT EXIST IN CLIENT
		"DONT_ATTACK",
		"AFFINITY",
		"SHADOW",
		"CRAFTING",
		"TOTEM_ITEM",
		"DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY", "DUMMY",
		"NO_SHADOW_CAST"	// ONLY EXIST IN CLIENT
	};

	public static readonly string[] AffinityFlags =
	{
		// Standards
		"CONTRACT",
		"DONATE",
		"SHOP",
		"PRESENT"
	};

	public static readonly string[] AffinityWorkTypes =
	{
		// Standards
		"ITEM",
		"MOB",
		"QUEST"
	};

	public static readonly string[] SpecialSkillTypes =
	{
		// Standards
		"MINING",
		"GATHERING",
		"CHARGE",
		"STONE",
		"PLANT",
		"ELEMENT",

		"MAKE_WEAPON",
		"MAKE_WEAR",
		"MAKE_G_B",
		"MAKE_ARMOR",
		"MAKE_H_S",
		"MAKE_POTINO",

		"PROCESS_NPC",
		"STAT_TRAINING"
	};

	public static readonly string[] NPCAIType =
	{
		// Standards
		"NORMAL",
		"TANKER",
		"DAMAGE_DEALER",
		"HEALER"
	};

	public static readonly string[] NPCAIFlag =
	{
		// "Standards"
		"AI Flag 0",
		"AI Flag 1",
		"AI Flag 2",
		"AI Flag 3"
		//,"AI Flag 4"	// NOTE: In NPCTool are 5, but in Server only 4
	};

	public static readonly string[] NPCAILeaderFlag =
	{
		// Standards
		"No",
		"Yes"
	};

	public static readonly string[] NPCAttackType =
	{
		// "Standards"
		//"MSG_DAMAGE_UNKNOWM",	// -1
		"Melee",	// ST_MELEE
		"Range",	// ST_RANGE
		"Magic"		// ST_MAGIC
		/*"MSG_DAMAGE_REFLEX",
		"MSG_DAMAGE_LINK",
		"MSG_DAMAGE_COMBO",*/

		/*#define ST_MELEE				0
		#define ST_RANGE				1
		#define ST_MAGIC				2
		#define ST_PASSIVE				3
		#define ST_PET_COMMAND			4
		#define ST_PET_SKILL_PASSIVE	5
		#define ST_PET_SKILL_ACTIVE		6
		#define ST_GUILD_SKILL_PASSIVE	7
		#define ST_SEAL					8
		#define ST_SUMMON_SKILL			9*/
	};

	public static readonly Dictionary<int, string> NationsIDNName = new Dictionary<int, string>
	{
		// Standards
		{ 0,  "KOR" },
		{ 1,  "TWN" },
		{ 2,  "TWN2" },
		{ 3,  "CHN" },
		{ 4,  "TLD" },
		{ 5,  "TLD_ENG" },
		{ 6,  "JPN" },
		{ 7,  "MAL" },
		{ 8,  "MAL_ENG" },
		{ 9,  "USA" },
		{ 10, "BRZ" },
		{ 11, "HBK" },
		{ 12, "HBK_ENG" },
		{ 13, "GER" },
		{ 14, "SPN" },
		{ 15, "FRC" },
		{ 16, "PL" },
		{ 17, "RUS" },
		{ 18, "TUR" },
		{ 19, "ITA" },
		{ 20, "MEX" },
		{ 21, "SPN_USA" },
		{ 22, "FRC_USA" },
		{ 23, "NLD" },
		{ 24, "UK" }
	};

	public class NationCharSetNPostfix
	{
		public string Charset;
		public string Postfix;

		public NationCharSetNPostfix(string strCharset, string strPostfix)
		{
			Charset = strCharset;
			Postfix = strPostfix;
		}
	};

	public static readonly Dictionary<string, NationCharSetNPostfix> NationsCharsetsNPostfix = new Dictionary<string, NationCharSetNPostfix>
	{
		["TWN"]		= new NationCharSetNPostfix("big5",		"..."),
		["CHN"]		= new NationCharSetNPostfix("gb2312",	"..."),
		["THAI"]	= new NationCharSetNPostfix("tis620",	"th"),
		["JPN"]		= new NationCharSetNPostfix("cp932",	"..."),
		["MAL"]		= new NationCharSetNPostfix("big5",		"..."),
		["USA"]		= new NationCharSetNPostfix("utf8",		"us"),
		["BRZ"]		= new NationCharSetNPostfix("latin1",	"br"),
		["HK"]		= new NationCharSetNPostfix("big5",		"..."),
		["GER"]		= new NationCharSetNPostfix("cp1250",	"de"),
		["SPN"]		= new NationCharSetNPostfix("latin1",	"es"),
		["FRC"]		= new NationCharSetNPostfix("latin1",	"fr"),
		["PLD"]		= new NationCharSetNPostfix("cp1250",	"pl"),
		["RUS"]		= new NationCharSetNPostfix("cp1251",	"rus"),
		["TUR"]		= new NationCharSetNPostfix("latin5",	"..."),
		["ITA"]		= new NationCharSetNPostfix("latin1",	"it"),
		["MEX"]		= new NationCharSetNPostfix("latin1",	"mx"),
		["NLD"]		= new NationCharSetNPostfix("euckr",	"..."),
		["UK"]		= new NationCharSetNPostfix("latin1",	"uk")
	};

	public static readonly Dictionary<string, string> LanguageCodes = new Dictionary<string, string> // ISO 639-1:2002
	{
		["GER"] = "de",
		["FRC"] = "fr",
		["ITA"] = "it",
		["PLD"] = "pl",
		["SPN"] = "es",
		//["USA"] = "en",
		["BRZ"] = "pt",
		["MEX"] = "es",
		["UK"] = "en",
		["RUS"] = "ru",
		["THAI"] = "th",
		["TUR"] = "tr"
	};

	public static readonly byte[] EncryptFileHeader = { 0xDE, 0xAD, 0xBE, 0xEF };

	public static readonly byte[] EncryptKey1 =
	{
		0x6B, 0x08, 0x12, 0x53,
		0x71, 0x58, 0x65, 0xCC,
		0xCA, 0xB7, 0xE9, 0x79,
		0xEC, 0x60, 0x40, 0xA2,
		0x4F, 0xD8, 0xCA, 0x83,
		0xDC, 0x58, 0x19, 0x04,
		0xBC, 0x11, 0xFB, 0xCD,
		0x8E, 0x7B, 0x13, 0xCE,
		0x6B, 0x08, 0x12, 0x53,
		0x71, 0x58, 0x65, 0xCC,
		0xCA, 0xB7, 0xE9, 0x79,
		0xEC, 0x60, 0x40, 0xA2,
		0x4F, 0xD8, 0xCA, 0x83,
		0xDC, 0x58, 0x19, 0x04,
		0xBC, 0x11, 0xFB, 0xCD,
		0x8E, 0x7B, 0x13, 0xCE
	};

	public static readonly byte[] EncryptKey2 =
	{
		0x1F, 0x96, 0x91, 0x7C,
		0x1A, 0x9E, 0x39, 0xE1,
		0xD7, 0x77, 0x69, 0xE7,
		0xC5, 0xB3, 0x81, 0x7C,
		0xC1, 0xCE, 0xDB, 0x6B,
		0x5B, 0xD6, 0x44, 0xF6,
		0xD8, 0x17, 0xA4, 0x59,
		0x82, 0x2B, 0x22, 0xFF
	};
}

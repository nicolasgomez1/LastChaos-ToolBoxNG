# LastChaos ToolBox <img align="left" src="https://user-images.githubusercontent.com/5092697/138568453-9cbbedb8-7889-4a9d-ac72-5d2dae9bae9f.png" width="100px">
It provides the basics for creating tools to manage Databases and perhaps files related to LastChaos.

# Concept of Global Tables
* The idea behind the project is to have a fast and efficient tool in terms of requests to the DataBase Server. With that in mind I designed a scheme in which there are __Global Tables__, these are populated for the first time by the Tool that requires the information, and later said information can be used by another tool, Avoiding constants requests each time some Tool open.

* When a Tool populates a __Global Table__, it is not necessarily populated with all the information available in the Database, but rather the system is designed so that different tools request only the columns necessary for the operation of said Tool. Being able to load different columns by different Tools without the information overlapping or replacing.

* Finally, when in a Tool the operator decides to apply changes made, an attempt is made to execute a Query either type UPDATE or INSERT, in case of success, changes are updated in the Global Tables as well.

# Hardcoded Definitions and Information related to Server/Client is stored in
* [Definitions.cs](Definitions.cs)

# Definitions to fast adaptation to each Source code
* Many Forms have definitions in their code. Many tools are adapted to my source code, but you can also disable these adaptations (or create the ones you need).

# Help Dialogs
* Pickers
1) [Flag Picker Implementation Example](Pickers/FlagPicker.cs)
2) [Icon Picker Implementation Example](Pickers/IconPicker.cs)
3) [Skill Picker Implementation Example](Pickers/SkillPicker.cs)
4) [Item Picker Implementation Example](Pickers/ItemPicker.cs)
5) [Title Picker Implementation Example](Pickers/RareOptionPicker.cs)
6) [String Picker Implementation Example](Pickers/StringPicker.cs)
7) [Option Picker Implementation Example](Pickers/OptionPicker.cs)
8) [Magic Picker Implementation Example](Pickers/MagicPicker.cs)
9) [Generic Type Picker Implementation Example](Pickers/GenericTypePicker.cs) (Some Items require a Type index (For example IETC_UPGRADE_GENERAL) in a_num_0, so I made this generic picker to select index from some list.)
10) [Special Skill Picker Implementation Example](Pickers/SpecialSkillPicker.cs)
11) [NPC Picker Implementation Example](Pickers/NPCPicker.cs)
12) [Quest Picker Implementation Example](Pickers/QuestPicker.cs)
13) [Zone Picker Implementation Example](Pickers/ZonePicker.cs)
14) [Title Picker Implementation Example](Pickers/TitlePicker.cs)

- NOTE: Picker Dialogs are intended to complement larger Tools, however, they have the ability to request information autonomously. This means that you can make the decision to load, for example, t_skill names to be able to display them in your Tool, which will increase loading times. On the other hand, you could not request this information and work only with IDS, but when invoking the Skill Picker Dialog, it will request data such as the name and description of the skills automatically if necessary.

* Others
1) [MessageBox With Captions & ProgressBar Implementation Example](MessageBoxes/MessageBox_Progress.cs)
2) [MessageBox with Text Input Implementation Example](MessageBoxes/MessageBox_Input.cs)
3) [MessageBox with ComboBox Implementation Example](MessageBoxes/MessageBox_ComboBox.cs)

# Helper Functions
1) [AskForIndex](Main.cs)
2) [EscapeChars](Main.cs)
3) [GetIcon](Main.cs)
4) [GetWorldMap](Main.cs)
5) [GetGoldColor](Main.cs)
6) [AT_MIX](Main.cs)
7) [GET_AT_VAR](Main.cs)
8) [GET_AT_LV](Main.cs)
9) [AT_ADMIX](Main.cs)
10) [GET_AT_DEF](Main.cs)
11) [GET_AT_ATT](Main.cs)

# Generic Data Loaders
1) [GenericLoadZoneDataAsync](Main.cs)
2) [GenericLoadStringDataAsync](Main.cs)
3) [GenericLoadSkillDataAsync](Main.cs)
4) [GenericLoadSkillLevelDataAsync](Main.cs)
5) [GenericLoadQuestDataAsync](Main.cs)
6) [GenericLoadItemDataAsync](Main.cs)
7) [GenericLoadNPCDataAsync](Main.cs)
8) [GenericLoadOptionDataAsync](Main.cs)

# Showcase
## Item Editor
![1](https://github.com/user-attachments/assets/fc7f7ec9-bc47-421d-ac2a-3ce9972322cb)

## Control Panel
![2](https://github.com/user-attachments/assets/861eb420-034f-4aa9-917e-f5bd76a6b6e7)

## Strings & Lods Exporter
![3](https://github.com/user-attachments/assets/cdcc24ea-4b86-4bc7-8a36-ca8cd1a77a2e)
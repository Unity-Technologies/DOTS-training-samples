# Terminology

**Asset** - A source file on disk, typically located in the Project’s Assets folder. This file is imported to a game-ready representation of your Asset internally which can contain multiple Objects (SubAssets.)

**Object** (SubAsset) - A single Unity serializable unit. Also known as a **SubAsset**. An imported Asset is made up of one or more Objects. 

**Includes** - The unique set of Objects from which an Asset is constructed.

**References** - The unique set of Objects that are needed (referenced) by the Includes of an Asset, but not included in the Asset.
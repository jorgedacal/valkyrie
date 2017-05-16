﻿using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Content;

// This class manages the Quest editor Interface
// FIXME: Rename, not a good name any more
public class QuestEditorData {

    private readonly StringKey COMPONENT_TO_DELETE = new StringKey("val", "COMPONENT_TO_DELETE");
    // When a selection list is raised it is stored here
    // This allows the return value to be fetched later
    public EditorSelectionList esl;
    // This is the currently selected component
    public EditorComponent selection;
    // The selection stack is used for the 'back' button
    public Stack<EditorComponent> selectionStack;

    // Start the editor
    public QuestEditorData()
    {
        Game.Get().qed = this;       
        selectionStack = new Stack<EditorComponent>();
        // Start at the quest component
        SelectQuest();
    }

    // Start the editor from old version
    public QuestEditorData(QuestEditorData old)
    {
        Game.Get().qed = this;
        selectionStack = new Stack<EditorComponent>();
        if (old == null || old.selection == null)
        {
            // Start at the quest component
            SelectQuest();
        }
        else
        {
            selectionStack = old.selectionStack;
            selectionStack.Push(old.selection);
            Back();
        }
    }

    // Update component selection
    // Used to save selection history
    public void NewSelection(EditorComponent c)
    {
        // selection starts at null
        if (selection != null)
        {
            selectionStack.Push(selection);
        }
        selection = c;
    }

    // Go back in the selection stack
    public static void Back()
    {
        Game game = Game.Get();
        // Check if there is anything to go back to
        if (game.qed.selectionStack.Count == 0)
        {
            // Reset on quest selection
            game.qed.selection = new EditorComponentQuest();
            return;
        }
        game.qed.selection = game.qed.selectionStack.Pop();
        // Quest is OK
        if (game.qed.selection is EditorComponentQuest)
        {
            game.qed.selection.Update();
        }
        else if (game.quest.qd.components.ContainsKey(game.qed.selection.name))
        {
            // Existing component
            game.qed.selection.Update();
        }
        else
        {
            // History item has been deleted/renamed, go back further
            Back();
        }
    }

    // Open component selection top level
    // Menu for selection of all component types, includes delete options
    public static void TypeSelect()
    {
        Game game = Game.Get();
        if (GameObject.FindGameObjectWithTag(Game.DIALOG) != null)
        {
            return;
        }

        // Border
        DialogBox db = new DialogBox(new Vector2(21, 0), new Vector2(18, 26), StringKey.NULL);
        db.AddBorder();

        // Heading
        db = new DialogBox(new Vector2(21, 0), new Vector2(17, 1), 
            new StringKey("val","SELECT",CommonStringKeys.TYPE)
            );

        // Buttons for each component type (and delete buttons)
        float offset = 2;
        TextButton tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.QUEST, delegate { SelectQuest(); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        offset += 2;
        tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.TILE, delegate { ListType("Tile"); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, offset), new Vector2(6, 1), CommonStringKeys.DELETE, delegate { game.qed.DeleteComponent("Tile"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        offset += 2;
        tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.TOKEN, delegate { ListType("Token"); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, offset), new Vector2(6, 1), CommonStringKeys.DELETE, delegate { game.qed.DeleteComponent("Token"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        offset += 2;
        tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.SPAWN, delegate { ListType("Spawn"); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, offset), new Vector2(6, 1), CommonStringKeys.DELETE, delegate { game.qed.DeleteComponent("Spawn"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        offset += 2;
        tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.EVENT, delegate { ListType("Event"); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, offset), new Vector2(6, 1), CommonStringKeys.DELETE, delegate { game.qed.DeleteComponent("Event"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        offset += 2;
        tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.CUSTOM_MONSTER, delegate { ListType("CustomMonster"); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, offset), new Vector2(6, 1), CommonStringKeys.DELETE, delegate { game.qed.DeleteComponent("CustomMonster"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        offset += 2;
        tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.ACTIVATION, delegate { ListType("Activation"); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, offset), new Vector2(6, 1), CommonStringKeys.DELETE, delegate { game.qed.DeleteComponent("Activation"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        offset += 2;
        tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.UI, delegate { ListType("UI"); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, offset), new Vector2(6, 1), CommonStringKeys.DELETE, delegate { game.qed.DeleteComponent("UI"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        if (game.gameType is D2EGameType)
        {
            offset += 2;
            tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.DOOR, delegate { ListType("Door"); });
            tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

            tb = new TextButton(new Vector2(32, offset), new Vector2(6, 1), CommonStringKeys.DELETE, delegate { game.qed.DeleteComponent("Door"); }, Color.red);
            tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

            offset += 2;
            tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.MPLACE, delegate { ListType("MPlace"); });
            tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

            tb = new TextButton(new Vector2(32, offset), new Vector2(6, 1), CommonStringKeys.DELETE, delegate { game.qed.DeleteComponent("MPlace"); }, Color.red);
            tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        }

        if (game.gameType is MoMGameType)
        {
            offset += 2;
            tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.PUZZLE, delegate { ListType("Puzzle"); });
            tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

            tb = new TextButton(new Vector2(32, offset), new Vector2(6, 1), CommonStringKeys.DELETE, delegate { game.qed.DeleteComponent("Puzzle"); }, Color.red);
            tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
        }
        offset += 2;
        tb = new TextButton(new Vector2(22, offset), new Vector2(9, 1), CommonStringKeys.QITEM, delegate { ListType("QItem"); });
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        tb = new TextButton(new Vector2(32, offset), new Vector2(6, 1), CommonStringKeys.DELETE, delegate { game.qed.DeleteComponent("QItem"); }, Color.red);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();

        offset += 2;
        tb = new TextButton(new Vector2(25.5f, offset), new Vector2(9, 1), CommonStringKeys.CANCEL, delegate { Cancel(); });
        tb.background.GetComponent<UnityEngine.UI.Image>().color = new Color(0.03f, 0.0f, 0f);
        tb.button.GetComponent<UnityEngine.UI.Text>().fontSize = UIScaler.GetSmallFont();
    }

    // Create selection list for type of component
    public static void ListType(string type)
    {
        Game game = Game.Get();

        List<EditorSelectionList.SelectionListEntry> list = new List<EditorSelectionList.SelectionListEntry>();
        // This magic string is picked up later for object creation
        list.Add(EditorSelectionList.SelectionListEntry.BuildNameKeyItem(
            new StringKey("val","NEW_X",type.ToUpper()).Translate(),"{NEW:" + type + "}"));
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Value.typeDynamic.Equals(type))
            {
                list.Add(new EditorSelectionList.SelectionListEntry(kv.Key));
            }
        }
        game.qed.esl = new EditorSelectionList(CommonStringKeys.SELECT_ITEM, list, delegate { game.qed.SelectComponent(); });
        game.qed.esl.SelectItem();
    }

    // Select a component from a list
    public void SelectComponent()
    {
        SelectComponent(esl.selection);
    }

    // Select a component for editing
    public static void SelectComponent(string name)
    {
        Game game = Game.Get();
        QuestEditorData qed = game.qed;

        // Quest is a special component
        if (name.Equals("Quest"))
        {
            SelectQuest();
            return;
        }
        // These are special strings for creating new objects
        if (name.Equals("{NEW:Tile}"))
        {
            qed.NewTile();
            return;
        }
        if (name.Equals("{NEW:Door}"))
        {
            qed.NewDoor();
            return;
        }
        if (name.Equals("{NEW:Token}"))
        {
            qed.NewToken();
            return;
        }
        if (name.Equals("{NEW:UI}"))
        {
            qed.NewUI();
            return;
        }
        if (name.Equals("{NEW:Spawn}"))
        {
            qed.NewSpawn();
            return;
        }
        if (name.Equals("{NEW:MPlace}"))
        {
            qed.NewMPlace();
            return;
        }
        if (name.Equals("{NEW:QItem}"))
        {
            qed.NewItem();
            return;
        }
        if (name.Equals("{NEW:CustomMonster}"))
        {
            qed.NewCustomMonster();
            return;
        }
        if (name.Equals("{NEW:Activation}"))
        {
            qed.NewActivation();
            return;
        }
        if (name.Equals("{NEW:Event}"))
        {
            qed.NewEvent();
            return;
        }
        if (name.Equals("{NEW:Puzzle}"))
        {
            qed.NewPuzzle();
            return;
        }
        // This may happen to due rename/delete
        if (!game.quest.qd.components.ContainsKey(name))
        {
            SelectQuest();
        }

        // Determine the component type and select
        if (game.quest.qd.components[name] is QuestData.Tile)
        {
            SelectAsTile(name);
            return;
        }

        if (game.quest.qd.components[name] is QuestData.Door)
        {
            SelectAsDoor(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.Token)
        {
            SelectAsToken(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.UI)
        {
            SelectAsUI(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.Spawn)
        {
            SelectAsSpawn(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.MPlace)
        {
            SelectAsMPlace(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.Puzzle)
        {
            SelectAsPuzzle(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.QItem)
        {
            SelectAsItem(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.CustomMonster)
        {
            SelectAsCustomMonster(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.Activation)
        {
            SelectAsActivation(name);
            return;
        }
        if (game.quest.qd.components[name] is QuestData.Event)
        {
            SelectAsEvent(name);
            return;
        }
    }

    public static void SelectQuest()
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentQuest());
    }

    public static void SelectAsTile(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentTile(name));
    }

    public static void SelectAsDoor(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentDoor(name));
    }

    public static void SelectAsToken(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentToken(name));
    }

    public static void SelectAsUI(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentUI(name));
    }

    public static void SelectAsEvent(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentEvent(name));
    }

    public static void SelectAsSpawn(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentSpawn(name));
    }

    public static void SelectAsMPlace(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentMPlace(name));
    }

    public static void SelectAsPuzzle(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentPuzzle(name));
    }

    public static void SelectAsItem(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentItem(name));
    }
    public static void SelectAsCustomMonster(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentCustomMonster(name));
    }
    public static void SelectAsActivation(string name)
    {
        Game game = Game.Get();
        game.qed.NewSelection(new EditorComponentActivation(name));
    }

    // Create a new tile, use next available number
    public void NewTile()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Tile" + index))
        {
            index++;
        }
        QuestData.Tile tile = new QuestData.Tile("Tile" + index);
        game.quest.qd.components.Add("Tile" + index, tile);

        CameraController cc = GameObject.FindObjectOfType<CameraController>();
        tile.location.x = game.gameType.TileRound() * Mathf.Round(cc.gameObject.transform.position.x / game.gameType.TileRound());
        tile.location.y = game.gameType.TileRound() * Mathf.Round(cc.gameObject.transform.position.y / game.gameType.TileRound());

        game.quest.Add("Tile" + index);
        SelectComponent("Tile" + index);
    }

    public void NewDoor()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Door" + index))
        {
            index++;
        }
        QuestData.Door door = new QuestData.Door("Door" + index);
        game.quest.qd.components.Add("Door" + index, door);

        CameraController cc = GameObject.FindObjectOfType<CameraController>();
        door.location.x = game.gameType.SelectionRound() * Mathf.Round(cc.gameObject.transform.position.x / game.gameType.SelectionRound());
        door.location.y = game.gameType.SelectionRound() * Mathf.Round(cc.gameObject.transform.position.y / game.gameType.SelectionRound());

        game.quest.Add("Door" + index);
        SelectComponent("Door" + index);
    }

    public void NewToken()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Token" + index))
        {
            index++;
        }
        QuestData.Token token = new QuestData.Token("Token" + index);
        game.quest.qd.components.Add("Token" + index, token);

        CameraController cc = GameObject.FindObjectOfType<CameraController>();
        token.location.x = game.gameType.SelectionRound() * Mathf.Round(cc.gameObject.transform.position.x / game.gameType.SelectionRound());
        token.location.y = game.gameType.SelectionRound() * Mathf.Round(cc.gameObject.transform.position.y / game.gameType.SelectionRound());

        game.quest.Add("Token" + index);
        SelectComponent("Token" + index);
    }

    public void NewUI()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("UI" + index))
        {
            index++;
        }
        QuestData.UI ui = new QuestData.UI("UI" + index);
        game.quest.qd.components.Add("UI" + index, ui);
        SelectComponent("UI" + index);
    }

    public void NewSpawn()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Spawn" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("Spawn" + index, new QuestData.Spawn("Spawn" + index));
        SelectComponent("Spawn" + index);
    }

    public void NewMPlace()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("MPlace" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("MPlace" + index, new QuestData.MPlace("MPlace" + index));
        SelectComponent("MPlace" + index);
    }

    public void NewEvent()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Event" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("Event" + index, new QuestData.Event("Event" + index));
        SelectComponent("Event" + index);
    }

    public void NewPuzzle()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Puzzle" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("Puzzle" + index, new QuestData.Puzzle("Puzzle" + index));
        SelectComponent("Puzzle" + index);
    }
    
    public void NewItem()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("QItem" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("QItem" + index, new QuestData.QItem("QItem" + index));
        SelectComponent("QItem" + index);
    }

    public void NewCustomMonster()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("CustomMonster" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("CustomMonster" + index, new QuestData.CustomMonster("CustomMonster" + index));
        SelectComponent("CustomMonster" + index);
    }

    public void NewActivation()
    {
        Game game = Game.Get();
        int index = 0;

        while (game.quest.qd.components.ContainsKey("Activation" + index))
        {
            index++;
        }
        game.quest.qd.components.Add("Activation" + index, new QuestData.Activation("Activation" + index));
        SelectComponent("Activation" + index);
    }

    // Delete a component by type
    public void DeleteComponent(string type)
    {
        Game game = Game.Get();

        List<EditorSelectionList.SelectionListEntry> toDelete = new List<EditorSelectionList.SelectionListEntry>();

        // List all components of this type
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            if (kv.Key.IndexOf(type) == 0)
            {
                toDelete.Add(new EditorSelectionList.SelectionListEntry(kv.Key));
                // Disabled elements in selection list
                toDelete.Add(new EditorSelectionList.SelectionListEntry(null));
            }
        }
        // Create list for user
        esl = new EditorSelectionList(COMPONENT_TO_DELETE, toDelete, delegate { SelectToDelete(); });
        esl.SelectItem();
    }

    // Delete a component
    public void DeleteComponent()
    {
        Game game = Game.Get();

        List<EditorSelectionList.SelectionListEntry> toDelete = new List<EditorSelectionList.SelectionListEntry>();

        // List all components
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            toDelete.Add(new EditorSelectionList.SelectionListEntry(kv.Key));
            // Disabled elements in selection list
            toDelete.Add(new EditorSelectionList.SelectionListEntry(null));
        }
        // Create list for user
        esl = new EditorSelectionList(COMPONENT_TO_DELETE, toDelete, delegate { SelectToDelete(); });
        esl.SelectItem();
    }

    // Item selected from list for deletion
    public void SelectToDelete()
    {
        Game game = Game.Get();
        Destroyer.Dialog();

        if (esl.selection.Length == 0) return;

        // Remove all references to the deleted component
        foreach (KeyValuePair<string, QuestData.QuestComponent> kv in game.quest.qd.components)
        {
            kv.Value.RemoveReference(esl.selection);
        }

        // Remove the component
        if (game.quest.qd.components.ContainsKey(esl.selection))
        {
            game.quest.qd.components.Remove(esl.selection);
        }

        LocalizationRead.scenarioDict.RemoveKeyPrefix(esl.selection + ".");

        // Clean up the current quest environment
        game.quest.Remove(esl.selection);

        // If we deleted the selected item, go back to the last item
        if (selection.name.Equals(esl.selection))
        {
            Back();
        }
    }

    // Cancel a component selection, clean up
    public static void Cancel()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(Game.DIALOG))
            Object.Destroy(go);
    }

    // This is called game
    public void MouseDown()
    {
        selection.MouseDown();
    }
}

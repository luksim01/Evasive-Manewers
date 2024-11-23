%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &1
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 12004, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_PixelRect:
    serializedVersion: 2
    x: 37
    y: 209
    width: 985
    height: 550
  m_ShowMode: 0
  m_Title: Console
  m_RootView: {fileID: 4}
  m_MinSize: {x: 100, y: 121}
  m_MaxSize: {x: 4000, y: 4021}
  m_Maximized: 0
--- !u!114 &2
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12004, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_PixelRect:
    serializedVersion: 2
    x: -23
    y: 53
    width: 1440
    height: 775
  m_ShowMode: 4
  m_Title: Inspector
  m_RootView: {fileID: 7}
  m_MinSize: {x: 875, y: 300}
  m_MaxSize: {x: 10000, y: 10000}
  m_Maximized: 0
--- !u!114 &3
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 12006, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: ConsoleWindow
  m_EditorClassIdentifier: 
  m_Children: []
  m_Position:
    serializedVersion: 2
    x: 0
    y: 0
    width: 985
    height: 550
  m_MinSize: {x: 100, y: 121}
  m_MaxSize: {x: 4000, y: 4021}
  m_ActualView: {fileID: 18}
  m_Panes:
  - {fileID: 18}
  m_Selected: 0
  m_LastSelected: 0
--- !u!114 &4
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 12010, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Children:
  - {fileID: 3}
  m_Position:
    serializedVersion: 2
    x: 0
    y: 0
    width: 985
    height: 550
  m_MinSize: {x: 100, y: 121}
  m_MaxSize: {x: 4000, y: 4021}
  vertical: 0
  controlID: 34980
--- !u!114 &5
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 12006, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: TestRunnerWindow
  m_EditorClassIdentifier: 
  m_Children: []
  m_Position:
    serializedVersion: 2
    x: 0
    y: 387.5
    width: 340.5
    height: 337.5
  m_MinSize: {x: 101, y: 121}
  m_MaxSize: {x: 4001, y: 4021}
  m_ActualView: {fileID: 19}
  m_Panes:
  - {fileID: 19}
  m_Selected: 0
  m_LastSelected: 0
--- !u!114 &6
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 12010, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Children:
  - {fileID: 17}
  - {fileID: 5}
  m_Position:
    serializedVersion: 2
    x: 1099.5
    y: 0
    width: 340.5
    height: 725
  m_MinSize: {x: 100, y: 100}
  m_MaxSize: {x: 8096, y: 16192}
  vertical: 1
  controlID: 175
--- !u!114 &7
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12008, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Children:
  - {fileID: 8}
  - {fileID: 10}
  - {fileID: 9}
  m_Position:
    serializedVersion: 2
    x: 0
    y: 0
    width: 1440
    height: 775
  m_MinSize: {x: 875, y: 300}
  m_MaxSize: {x: 10000, y: 10000}
  m_UseTopView: 1
  m_TopViewHeight: 30
  m_UseBottomView: 1
  m_BottomViewHeight: 20
--- !u!114 &8
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12011, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Children: []
  m_Position:
    serializedVersion: 2
    x: 0
    y: 0
    width: 1440
    height: 30
  m_MinSize: {x: 0, y: 0}
  m_MaxSize: {x: 0, y: 0}
  m_LastLoadedLayoutName: 
--- !u!114 &9
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12042, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Children: []
  m_Position:
    serializedVersion: 2
    x: 0
    y: 755
    width: 1440
    height: 20
  m_MinSize: {x: 0, y: 0}
  m_MaxSize: {x: 0, y: 0}
--- !u!114 &10
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12010, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Children:
  - {fileID: 11}
  - {fileID: 14}
  - {fileID: 6}
  m_Position:
    serializedVersion: 2
    x: 0
    y: 30
    width: 1440
    height: 725
  m_MinSize: {x: 300, y: 100}
  m_MaxSize: {x: 24288, y: 16192}
  vertical: 0
  controlID: 174
--- !u!114 &11
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12010, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Children:
  - {fileID: 12}
  - {fileID: 13}
  m_Position:
    serializedVersion: 2
    x: 0
    y: 0
    width: 746.5
    height: 725
  m_MinSize: {x: 100, y: 100}
  m_MaxSize: {x: 8096, y: 16192}
  vertical: 1
  controlID: 60
--- !u!114 &12
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12006, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: SceneView
  m_EditorClassIdentifier: 
  m_Children: []
  m_Position:
    serializedVersion: 2
    x: 0
    y: 0
    width: 746.5
    height: 349
  m_MinSize: {x: 201, y: 221}
  m_MaxSize: {x: 4001, y: 4021}
  m_ActualView: {fileID: 20}
  m_Panes:
  - {fileID: 20}
  m_Selected: 0
  m_LastSelected: 0
--- !u!114 &13
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12006, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Children: []
  m_Position:
    serializedVersion: 2
    x: 0
    y: 349
    width: 746.5
    height: 376
  m_MinSize: {x: 201, y: 221}
  m_MaxSize: {x: 4001, y: 4021}
  m_ActualView: {fileID: 21}
  m_Panes:
  - {fileID: 21}
  m_Selected: 0
  m_LastSelected: 0
--- !u!114 &14
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 12010, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Children:
  - {fileID: 15}
  - {fileID: 16}
  m_Position:
    serializedVersion: 2
    x: 746.5
    y: 0
    width: 353
    height: 725
  m_MinSize: {x: 100, y: 100}
  m_MaxSize: {x: 8096, y: 16192}
  vertical: 1
  controlID: 83
--- !u!114 &15
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12006, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Children: []
  m_Position:
    serializedVersion: 2
    x: 0
    y: 0
    width: 353
    height: 385.5
  m_MinSize: {x: 202, y: 221}
  m_MaxSize: {x: 4002, y: 4021}
  m_ActualView: {fileID: 22}
  m_Panes:
  - {fileID: 22}
  m_Selected: 0
  m_LastSelected: 0
--- !u!114 &16
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 12006, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: ProjectBrowser
  m_EditorClassIdentifier: 
  m_Children: []
  m_Position:
    serializedVersion: 2
    x: 0
    y: 385.5
    width: 353
    height: 339.5
  m_MinSize: {x: 232, y: 271}
  m_MaxSize: {x: 10002, y: 10021}
  m_ActualView: {fileID: 23}
  m_Panes:
  - {fileID: 23}
  m_Selected: 0
  m_LastSelected: 0
--- !u!114 &17
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12006, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Children: []
  m_Position:
    serializedVersion: 2
    x: 0
    y: 0
    width: 340.5
    height: 387.5
  m_MinSize: {x: 275, y: 50}
  m_MaxSize: {x: 4000, y: 4000}
  m_ActualView: {fileID: 24}
  m_Panes:
  - {fileID: 24}
  m_Selected: 0
  m_LastSelected: 0
--- !u!114 &18
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 12003, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_MinSize: {x: 100, y: 100}
  m_MaxSize: {x: 4000, y: 4000}
  m_TitleContent:
    m_Text: Console
    m_Image: {fileID: -4950941429401207979, guid: 0000000000000000d000000000000000, type: 0}
    m_Tooltip: 
  m_Pos:
    serializedVersion: 2
    x: 37
    y: 209
    width: 985
    height: 529
  m_SerializedDataModeController:
    m_DataMode: 0
    m_PreferredDataMode: 0
    m_SupportedDataModes: 
    isAutomatic: 1
  m_ViewDataDictionary: {fileID: 0}
  m_OverlayCanvas:
    m_LastAppliedPresetName: Default
    m_SaveData: []
    m_OverlaysVisible: 1
--- !u!114 &19
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 13401, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_MinSize: {x: 100, y: 100}
  m_MaxSize: {x: 4000, y: 4000}
  m_TitleContent:
    m_Text: Test Runner
    m_Image: {fileID: 0}
    m_Tooltip: 
  m_Pos:
    serializedVersion: 2
    x: 1076.5
    y: 470.5
    width: 339.5
    height: 316.5
  m_SerializedDataModeController:
    m_DataMode: 0
    m_PreferredDataMode: 0
    m_SupportedDataModes: 
    isAutomatic: 1
  m_ViewDataDictionary: {fileID: 0}
  m_OverlayCanvas:
    m_LastAppliedPresetName: Default
    m_SaveData: []
    m_OverlaysVisible: 1
  m_Spl:
    ID: 212
    splitterInitialOffset: 145.5
    currentActiveSplitter: -1
    realSizes:
    - 79
    - 167
    relativeSizes:
    - 0.32142857
    - 0.67857146
    minSizes:
    - 32
    - 32
    maxSizes:
    - 0
    - 0
    lastTotalSize: 246
    splitSize: 6
    xOffset: 0
    m_Version: 1
    oldRealSizes: 
    oldMinSizes: 
    oldMaxSizes: 
    oldSplitSize: 0
  m_TestTypeToolbarIndex: 0
  m_PlayModeTestListGUI:
    m_Window: {fileID: 19}
    m_NewResultList:
    - id: 1018
      uniqueId: '[Evasive Manewers][suite]'
      name: Evasive Manewers
      fullName: Evasive Manewers
      resultStatus: 0
      duration: 1.7572685
      messages: One or more child tests had errors
      output: 
      stacktrace: 
      notRunnable: 0
      ignoredOrSkipped: 0
      description: 
      isSuite: 1
      categories: []
      parentId: 
      parentUniqueId: 
    - id: 1003
      uniqueId: '[PlayMode][/Users/lukas/Documents/Games Technologies/Applied Games
        Technologies/Evasive-Manewers/Library/ScriptAssemblies/PlayMode.dll][suite]'
      name: PlayMode.dll
      fullName: /Users/lukas/Documents/Games Technologies/Applied Games Technologies/Evasive-Manewers/Library/ScriptAssemblies/PlayMode.dll
      resultStatus: 0
      duration: 1.7406491
      messages: One or more child tests had errors
      output: 
      stacktrace: 
      notRunnable: 0
      ignoredOrSkipped: 0
      description: 
      isSuite: 1
      categories: []
      parentId: 1000
      parentUniqueId: '[Evasive Manewers][suite]'
    - id: 1001
      uniqueId: PlayMode.dll/[PlayMode][PlaceholderTests][suite]
      name: PlaceholderTests
      fullName: PlaceholderTests
      resultStatus: 0
      duration: 1.7329552
      messages: One or more child tests had errors
      output: 
      stacktrace: 
      notRunnable: 0
      ignoredOrSkipped: 0
      description: 
      isSuite: 1
      categories: []
      parentId: 1004
      parentUniqueId: '[PlayMode][/Users/lukas/Documents/Games Technologies/Applied
        Games Technologies/Evasive-Manewers/Library/ScriptAssemblies/PlayMode.dll][suite]'
    - id: 1002
      uniqueId: PlayMode.dll/PlaceholderTests/[PlayMode][PlaceholderTests.PlayerControllerMoveBoundaryTest]
      name: PlayerControllerMoveBoundaryTest
      fullName: PlaceholderTests.PlayerControllerMoveBoundaryTest
      resultStatus: 0
      duration: 1.692553
      messages: "  Expected: greater than or equal to 7.0f\n  But was:  0.0f\n"
      output: '(PlayerControllerMoveBoundaryTest) sheepdog position x:0

'
      stacktrace: 'at PlaceholderTests+<PlayerControllerMoveBoundaryTest>d__2.MoveNext
        () [0x00123] in /Users/lukas/Documents/Games Technologies/Applied Games Technologies/Evasive-Manewers/Assets/Tests/PlayMode/PlaceholderTests.cs:151

        at
        UnityEngine.TestTools.TestEnumerator+<Execute>d__7.MoveNext () [0x0003a]
        in ./Library/PackageCache/com.unity.test-framework@1.1.33/UnityEngine.TestRunner/NUnitExtensions/Attributes/TestEnumerator.cs:46

'
      notRunnable: 0
      ignoredOrSkipped: 0
      description: 
      isSuite: 0
      categories:
      - Uncategorized
      parentId: 1001
      parentUniqueId: PlayMode.dll/[PlayMode][PlaceholderTests][suite]
    m_ResultText: 
    m_ResultStacktrace: 'at PlaceholderTests+<PlayerControllerMoveBoundaryTest>d__2.MoveNext
      () [0x00123] in /Users/lukas/Documents/Games Technologies/Applied Games Technologies/Evasive-Manewers/Assets/Tests/PlayMode/PlaceholderTests.cs:151

      at
      UnityEngine.TestTools.TestEnumerator+<Execute>d__7.MoveNext () [0x0003a] in
      ./Library/PackageCache/com.unity.test-framework@1.1.33/UnityEngine.TestRunner/NUnitExtensions/Attributes/TestEnumerator.cs:46

'
    m_TestListState:
      scrollPos: {x: 0, y: 0}
      m_SelectedIDs: 2abd5713
      m_LastClickedID: 324517162
      m_ExpandedIDs: 856df0d8dbabf4df9827c7202a306b71c5b86176ffffff7f
      m_RenameOverlay:
        m_UserAcceptedRename: 0
        m_Name: 
        m_OriginalName: 
        m_EditFieldRect:
          serializedVersion: 2
          x: 0
          y: 0
          width: 0
          height: 0
        m_UserData: 0
        m_IsWaitingForDelay: 0
        m_IsRenaming: 0
        m_OriginalEventType: 11
        m_IsRenamingFilename: 0
        m_ClientGUIView: {fileID: 0}
      m_SearchString: 
    m_TestRunnerUIFilter:
      PassedHidden: 0
      FailedHidden: 0
      NotRunHidden: 0
      m_SearchString: 
      selectedCategoryMask: 0
      availableCategories:
      - Uncategorized
    m_SelectedOption: 0
  m_EditModeTestListGUI:
    m_Window: {fileID: 19}
    m_NewResultList:
    - id: 1000
      uniqueId: '[Evasive Manewers][suite]'
      name: Evasive Manewers
      fullName: Evasive Manewers
      resultStatus: 1
      duration: 0.0878664
      messages: 
      output: 
      stacktrace: 
      notRunnable: 0
      ignoredOrSkipped: 0
      description: 
      isSuite: 1
      categories: []
      parentId: 
      parentUniqueId: 
    - id: 1004
      uniqueId: '[EditMode][/Users/lukas/Documents/Games Technologies/Applied Games
        Technologies/Evasive-Manewers/Library/ScriptAssemblies/EditMode.dll][suite]'
      name: EditMode.dll
      fullName: /Users/lukas/Documents/Games Technologies/Applied Games Technologies/Evasive-Manewers/Library/ScriptAssemblies/EditMode.dll
      resultStatus: 1
      duration: 0.0328587
      messages: 
      output: 
      stacktrace: 
      notRunnable: 0
      ignoredOrSkipped: 0
      description: 
      isSuite: 1
      categories: []
      parentId: 1000
      parentUniqueId: '[Evasive Manewers][suite]'
    - id: 1001
      uniqueId: EditMode.dll/[EditMode][LogicTests][suite]
      name: LogicTests
      fullName: LogicTests
      resultStatus: 1
      duration: 0.0242892
      messages: 
      output: 
      stacktrace: 
      notRunnable: 0
      ignoredOrSkipped: 0
      description: 
      isSuite: 1
      categories: []
      parentId: 1003
      parentUniqueId: '[EditMode][/Users/lukas/Documents/Games Technologies/Applied
        Games Technologies/Evasive-Manewers/Library/ScriptAssemblies/EditMode.dll][suite]'
    - id: 1002
      uniqueId: EditMode.dll/LogicTests/[EditMode][LogicTests.StraySheepSpawnNonTriggerTest]
      name: StraySheepSpawnNonTriggerTest
      fullName: LogicTests.StraySheepSpawnNonTriggerTest
      resultStatus: 1
      duration: 0.010424
      messages: 
      output: 
      stacktrace: 
      notRunnable: 0
      ignoredOrSkipped: 0
      description: 
      isSuite: 0
      categories:
      - Uncategorized
      parentId: 1001
      parentUniqueId: EditMode.dll/[EditMode][LogicTests][suite]
    - id: 1003
      uniqueId: EditMode.dll/LogicTests/[EditMode][LogicTests.StraySheepSpawnTriggerTest]
      name: StraySheepSpawnTriggerTest
      fullName: LogicTests.StraySheepSpawnTriggerTest
      resultStatus: 1
      duration: 0.000429
      messages: 
      output: 
      stacktrace: 
      notRunnable: 0
      ignoredOrSkipped: 0
      description: 
      isSuite: 0
      categories:
      - Uncategorized
      parentId: 1001
      parentUniqueId: EditMode.dll/[EditMode][LogicTests][suite]
    m_ResultText: StraySheepSpawnNonTriggerTest (0.010s)
    m_ResultStacktrace: 
    m_TestListState:
      scrollPos: {x: 0, y: 0}
      m_SelectedIDs: b80bce1f
      m_LastClickedID: 533597112
      m_ExpandedIDs: 856df0d8382d7f0dc6b46e56ffffff7f
      m_RenameOverlay:
        m_UserAcceptedRename: 0
        m_Name: 
        m_OriginalName: 
        m_EditFieldRect:
          serializedVersion: 2
          x: 0
          y: 0
          width: 0
          height: 0
        m_UserData: 0
        m_IsWaitingForDelay: 0
        m_IsRenaming: 0
        m_OriginalEventType: 11
        m_IsRenamingFilename: 0
        m_ClientGUIView: {fileID: 0}
      m_SearchString: 
    m_TestRunnerUIFilter:
      PassedHidden: 0
      FailedHidden: 0
      NotRunHidden: 0
      m_SearchString: 
      selectedCategoryMask: 0
      availableCategories:
      - Uncategorized
--- !u!114 &20
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12013, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_MinSize: {x: 200, y: 200}
  m_MaxSize: {x: 4000, y: 4000}
  m_TitleContent:
    m_Text: Scene
    m_Image: {fileID: 8634526014445323508, guid: 0000000000000000d000000000000000, type: 0}
    m_Tooltip: 
  m_Pos:
    serializedVersion: 2
    x: -23
    y: 83
    width: 745.5
    height: 328
  m_SerializedDataModeController:
    m_DataMode: 0
    m_PreferredDataMode: 0
    m_SupportedDataModes: 
    isAutomatic: 1
  m_ViewDataDictionary: {fileID: 0}
  m_OverlayCanvas:
    m_LastAppliedPresetName: Default
    m_SaveData:
    - dockPosition: 0
      containerId: overlay-toolbar__top
      floating: 0
      collapsed: 0
      displayed: 1
      snapOffset: {x: -156, y: -26}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 3
      id: Tool Settings
      index: 0
      layout: 1
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 0
      containerId: overlay-toolbar__top
      floating: 0
      collapsed: 0
      displayed: 1
      snapOffset: {x: -141, y: -183.5}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 3
      id: unity-grid-and-snap-toolbar
      index: 1
      layout: 1
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-toolbar__top
      floating: 0
      collapsed: 0
      displayed: 1
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: unity-scene-view-toolbar
      index: 0
      layout: 1
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-toolbar__top
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 1
      id: unity-search-toolbar
      index: 1
      layout: 1
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 0
      containerId: overlay-container--left
      floating: 0
      collapsed: 0
      displayed: 1
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: unity-transform-toolbar
      index: 0
      layout: 2
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 0
      containerId: overlay-container--left
      floating: 0
      collapsed: 0
      displayed: 1
      snapOffset: {x: 0, y: 197}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: unity-component-tools
      index: 1
      layout: 2
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 0
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 1
      snapOffset: {x: 67.5, y: 86}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Orientation
      index: 0
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Light Settings
      index: 0
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Camera
      index: 1
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Cloth Constraints
      index: 1
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Cloth Collisions
      index: 2
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Navmesh Display
      index: 4
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Agent Display
      index: 5
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Obstacle Display
      index: 6
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Occlusion Culling
      index: 3
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Physics Debugger
      index: 4
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Scene Visibility
      index: 5
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Particles
      index: 6
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Tilemap
      index: 11
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 0, y: 0}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/Tilemap Palette Helper
      index: 12
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 0
      snapOffset: {x: 48, y: 48}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: Scene View/TrailRenderer
      index: 7
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    - dockPosition: 1
      containerId: overlay-container--right
      floating: 0
      collapsed: 0
      displayed: 1
      snapOffset: {x: 48, y: 10}
      snapOffsetDelta: {x: 0, y: 0}
      snapCorner: 0
      id: UnityEditor.SceneViewCameraOverlay
      index: 8
      layout: 4
      size: {x: 0, y: 0}
      sizeOverriden: 0
    m_OverlaysVisible: 1
  m_WindowGUID: dc19e29c0ee0f4a50825520d48faa5ff
  m_Gizmos: 1
  m_OverrideSceneCullingMask: 6917529027641081856
  m_SceneIsLit: 1
  m_SceneLighting: 1
  m_2DMode: 0
  m_isRotationLocked: 0
  m_PlayAudio: 0
  m_AudioPlay: 0
  m_Position:
    m_Target: {x: 0, y: 0, z: 0}
    speed: 2
    m_Value: {x: 0, y: 0, z: 0}
  m_RenderMode: 0
  m_CameraMode:
    drawMode: 0
    name: Shaded
    section: Shading Mode
  m_ValidateTrueMetals: 0
  m_DoValidateTrueMetals: 0
  m_SceneViewState:
    m_AlwaysRefresh: 0
    showFog: 1
    showSkybox: 1
    showFlares: 1
    showImageEffects: 1
    showParticleSystems: 1
    showVisualEffectGraphs: 1
    m_FxEnabled: 1
  m_Grid:
    xGrid:
      m_Fade:
        m_Target: 0
        speed: 2
        m_Value: 0
      m_Color: {r: 0.5, g: 0.5, b: 0.5, a: 0.4}
      m_Pivot: {x: 0, y: 0, z: 0}
      m_Size: {x: 1, y: 1}
    yGrid:
      m_Fade:
        m_Target: 1
        speed: 2
        m_Value: 1
      m_Color: {r: 0.5, g: 0.5, b: 0.5, a: 0.4}
      m_Pivot: {x: 0, y: 0, z: 0}
      m_Size: {x: 1, y: 1}
    zGrid:
      m_Fade:
        m_Target: 0
        speed: 2
        m_Value: 0
      m_Color: {r: 0.5, g: 0.5, b: 0.5, a: 0.4}
      m_Pivot: {x: 0, y: 0, z: 0}
      m_Size: {x: 1, y: 1}
    m_ShowGrid: 1
    m_GridAxis: 1
    m_gridOpacity: 0.5
  m_Rotation:
    m_Target: {x: 0.2137637, y: -0.3266245, z: 0.076097384, w: 0.91751415}
    speed: 2
    m_Value: {x: 0.21376355, y: -0.32662427, z: 0.07609733, w: 0.9175135}
  m_Size:
    m_Target: 11.970853
    speed: 2
    m_Value: 11.970853
  m_Ortho:
    m_Target: 1
    speed: 2
    m_Value: 1
  m_CameraSettings:
    m_Speed: 1.005
    m_SpeedNormalized: 0.5
    m_SpeedMin: 0.01
    m_SpeedMax: 2
    m_EasingEnabled: 1
    m_EasingDuration: 0.4
    m_AccelerationEnabled: 1
    m_FieldOfViewHorizontalOrVertical: 60
    m_NearClip: 0.03
    m_FarClip: 10000
    m_DynamicClip: 1
    m_OcclusionCulling: 0
  m_LastSceneViewRotation: {x: 0, y: 0, z: 0, w: 0}
  m_LastSceneViewOrtho: 0
  m_ReplacementShader: {fileID: 0}
  m_ReplacementString: 
  m_SceneVisActive: 1
  m_LastLockedObject: {fileID: 0}
  m_ViewIsLockedToObject: 0
--- !u!114 &21
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12015, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_MinSize: {x: 200, y: 200}
  m_MaxSize: {x: 4000, y: 4000}
  m_TitleContent:
    m_Text: Game
    m_Image: {fileID: 4621777727084837110, guid: 0000000000000000d000000000000000, type: 0}
    m_Tooltip: 
  m_Pos:
    serializedVersion: 2
    x: -23
    y: 432
    width: 745.5
    height: 355
  m_SerializedDataModeController:
    m_DataMode: 0
    m_PreferredDataMode: 0
    m_SupportedDataModes: 
    isAutomatic: 1
  m_ViewDataDictionary: {fileID: 0}
  m_OverlayCanvas:
    m_LastAppliedPresetName: Default
    m_SaveData: []
    m_OverlaysVisible: 1
  m_SerializedViewNames: []
  m_SerializedViewValues: []
  m_PlayModeViewName: GameView
  m_ShowGizmos: 0
  m_TargetDisplay: 0
  m_ClearColor: {r: 0, g: 0, b: 0, a: 0}
  m_TargetSize: {x: 1491, y: 668}
  m_TextureFilterMode: 0
  m_TextureHideFlags: 61
  m_RenderIMGUI: 1
  m_EnterPlayModeBehavior: 0
  m_UseMipMap: 0
  m_VSyncEnabled: 0
  m_Gizmos: 0
  m_Stats: 0
  m_SelectedSizes: 00000000000000000000000000000000000000000000000000000000000000000000000000000000
  m_ZoomArea:
    m_HRangeLocked: 0
    m_VRangeLocked: 0
    hZoomLockedByDefault: 0
    vZoomLockedByDefault: 0
    m_HBaseRangeMin: -372.75
    m_HBaseRangeMax: 372.75
    m_VBaseRangeMin: -167
    m_VBaseRangeMax: 167
    m_HAllowExceedBaseRangeMin: 1
    m_HAllowExceedBaseRangeMax: 1
    m_VAllowExceedBaseRangeMin: 1
    m_VAllowExceedBaseRangeMax: 1
    m_ScaleWithWindow: 0
    m_HSlider: 0
    m_VSlider: 0
    m_IgnoreScrollWheelUntilClicked: 0
    m_EnableMouseInput: 1
    m_EnableSliderZoomHorizontal: 0
    m_EnableSliderZoomVertical: 0
    m_UniformScale: 1
    m_UpDirection: 1
    m_DrawArea:
      serializedVersion: 2
      x: 0
      y: 21
      width: 745.5
      height: 334
    m_Scale: {x: 1, y: 1}
    m_Translation: {x: 372.75, y: 167}
    m_MarginLeft: 0
    m_MarginRight: 0
    m_MarginTop: 0
    m_MarginBottom: 0
    m_LastShownAreaInsideMargins:
      serializedVersion: 2
      x: -372.75
      y: -167
      width: 745.5
      height: 334
    m_MinimalGUI: 1
  m_defaultScale: 1
  m_LastWindowPixelSize: {x: 1491, y: 710}
  m_ClearInEditMode: 1
  m_NoCameraWarning: 1
  m_LowResolutionForAspectRatios: 00000000000000000000
  m_XRRenderMode: 0
  m_RenderTexture: {fileID: 0}
--- !u!114 &22
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12061, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_MinSize: {x: 200, y: 200}
  m_MaxSize: {x: 4000, y: 4000}
  m_TitleContent:
    m_Text: Hierarchy
    m_Image: {fileID: -3734745235275155857, guid: 0000000000000000d000000000000000, type: 0}
    m_Tooltip: 
  m_Pos:
    serializedVersion: 2
    x: 723.5
    y: 83
    width: 351
    height: 364.5
  m_SerializedDataModeController:
    m_DataMode: 0
    m_PreferredDataMode: 0
    m_SupportedDataModes: 
    isAutomatic: 1
  m_ViewDataDictionary: {fileID: 0}
  m_OverlayCanvas:
    m_LastAppliedPresetName: Default
    m_SaveData: []
    m_OverlaysVisible: 1
  m_SceneHierarchy:
    m_TreeViewState:
      scrollPos: {x: 0, y: 0}
      m_SelectedIDs: 
      m_LastClickedID: 0
      m_ExpandedIDs: 10e1f2ff68e1f2ffeee3f2ff2ce7f2ff86e7f2ff28eaf2ff3eedf2ff98edf2ff3cf0f2ff46f4f2ffa0f4f2ff72f7f2ff72faf2ffccfaf2ff5cfdf2ff7200f3ffcc00f3ff5c03f3ff7206f3ffcc06f3ff5c09f3ff5c0cf3ffb60cf3ff440ff3ff4412f3ff9e12f3ff3415f3ff261af3ff7e1af3ff061df3ff3c21f3ff9621f3ff2624f3ff9227f3ffea27f3ff702af3ffe62df3ff3e2ef3ffd830f3ffd437f3ff2c38f3ffb23af3ff3e40f3ff9840f3ff5843f3ff3248f3ff8c48f3ff1a4bf3ff684ef3ffc24ef3ff7451f3ff0255f3ff5c55f3ff4657f3ffaa5af3ff045bf3fff05cf3ff7e60f3ffd660f3ffae62f3ff846ef3ffdc6ef3ffac70f3ff4275f3ff907cf3ffe87cf3ffd07ff3ff4086f3ff9486f3ff3095f3ff8895f3ff5698f3ff90a0f3ffdea0f3ffaab7f3fff8b7f3fffcc5f3ff54c6f3ff64c8f3ff4ecaf3ffa6caf3ffb0ccf3ffa4d1f3fffcd1f3ffd2d3f3ffaad8f3ff04d9f3ffe2daf3ff4ae1f3ffa4e1f3ff82e3f3ff54eaf3ffaceaf3ff82ecf3ff42f0f3ff9cf0f3ff7af2f3ffb005f4ff0806f4fff007f4fffe0bf4ff560cf4ff4a0ef4ff5810f4ffb010f4ff9812f4ff1a29f4ff7229f4ffc82ef4ffaa31f4ff0232f4ff5434f4ffde40f4ff3641f4ffae43f4ff1247f4ff6a47f4ff8249f4ff6a4df4ffc24df4ffda4ff4ff1e5bf4ff765bf4ff945df4ff0466f4ff5c66f4ff6668f4ff2e6ef4ff866ef4ff9070f4ff5473f4ffac73f4ffb675f4ff9078f4ffe878f4fff27af4ffe27df4ff3a7ef4ff5880f4ffce83f4ff2684f4ff4486f4ff9a89f4fff489f4ff088cf4ff1e8ff4ff768ff4ff8091f4ff089ff4ff629ff4ff68a1f4fff6a4f4ff4ea5f4ff5ea7f4ff7aacf4ffd2acf4ffceaef4ffb4b1f4ff0cb2f4ff08b4f4ff58b7f4ffb0b7f4ffaeb9f4ffb06df5ff8a71f5ff8c3bf9ff2679fdffa47ffdff98c0ffff56ccfffff8fafffff4ffffff
      m_RenameOverlay:
        m_UserAcceptedRename: 0
        m_Name: 
        m_OriginalName: 
        m_EditFieldRect:
          serializedVersion: 2
          x: 0
          y: 0
          width: 0
          height: 0
        m_UserData: 0
        m_IsWaitingForDelay: 0
        m_IsRenaming: 0
        m_OriginalEventType: 11
        m_IsRenamingFilename: 0
        m_ClientGUIView: {fileID: 15}
      m_SearchString: 
    m_ExpandedScenes: []
    m_CurrenRootInstanceID: 0
    m_LockTracker:
      m_IsLocked: 0
    m_CurrentSortingName: TransformSorting
  m_WindowGUID: b13a7cd0719e84171872410714b7ebf0
--- !u!114 &23
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12014, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_MinSize: {x: 230, y: 250}
  m_MaxSize: {x: 10000, y: 10000}
  m_TitleContent:
    m_Text: Project
    m_Image: {fileID: -5179483145760003458, guid: 0000000000000000d000000000000000, type: 0}
    m_Tooltip: 
  m_Pos:
    serializedVersion: 2
    x: 723.5
    y: 468.5
    width: 351
    height: 318.5
  m_SerializedDataModeController:
    m_DataMode: 0
    m_PreferredDataMode: 0
    m_SupportedDataModes: 
    isAutomatic: 1
  m_ViewDataDictionary: {fileID: 0}
  m_OverlayCanvas:
    m_LastAppliedPresetName: Default
    m_SaveData: []
    m_OverlaysVisible: 1
  m_SearchFilter:
    m_NameFilter: 
    m_ClassNames: []
    m_AssetLabels: []
    m_AssetBundleNames: []
    m_ReferencingInstanceIDs: 
    m_SceneHandles: 
    m_ShowAllHits: 0
    m_SkipHidden: 0
    m_SearchArea: 1
    m_Folders:
    - Assets/Resources/Prefabs/Final/Animation Controllers
    m_Globs: []
    m_OriginalText: 
    m_ImportLogFlags: 0
    m_FilterByTypeIntersection: 0
  m_ViewMode: 1
  m_StartGridSize: 64
  m_LastFolders:
  - Assets/Resources/Prefabs/Final/Animation Controllers
  m_LastFoldersGridSize: -1
  m_LastProjectPath: /Users/lukas/Documents/Games Technologies/Applied Games Technologies/Evasive-Manewers
  m_LockTracker:
    m_IsLocked: 0
  m_FolderTreeState:
    scrollPos: {x: 0, y: 0}
    m_SelectedIDs: a06d0000
    m_LastClickedID: 28064
    m_ExpandedIDs: 00000000626d0000826d00009a6d0000e859020000ca9a3b
    m_RenameOverlay:
      m_UserAcceptedRename: 0
      m_Name: 
      m_OriginalName: 
      m_EditFieldRect:
        serializedVersion: 2
        x: 0
        y: 0
        width: 0
        height: 0
      m_UserData: 0
      m_IsWaitingForDelay: 0
      m_IsRenaming: 0
      m_OriginalEventType: 11
      m_IsRenamingFilename: 1
      m_ClientGUIView: {fileID: 0}
    m_SearchString: 
    m_CreateAssetUtility:
      m_EndAction: {fileID: 0}
      m_InstanceID: 0
      m_Path: 
      m_Icon: {fileID: 0}
      m_ResourceFile: 
  m_AssetTreeState:
    scrollPos: {x: 0, y: 0}
    m_SelectedIDs: 
    m_LastClickedID: 0
    m_ExpandedIDs: 00000000626d0000646d0000
    m_RenameOverlay:
      m_UserAcceptedRename: 0
      m_Name: 
      m_OriginalName: 
      m_EditFieldRect:
        serializedVersion: 2
        x: 0
        y: 0
        width: 0
        height: 0
      m_UserData: 0
      m_IsWaitingForDelay: 0
      m_IsRenaming: 0
      m_OriginalEventType: 11
      m_IsRenamingFilename: 1
      m_ClientGUIView: {fileID: 0}
    m_SearchString: 
    m_CreateAssetUtility:
      m_EndAction: {fileID: 0}
      m_InstanceID: 0
      m_Path: 
      m_Icon: {fileID: 0}
      m_ResourceFile: 
  m_ListAreaState:
    m_SelectedInstanceIDs: 
    m_LastClickedInstanceID: 0
    m_HadKeyboardFocusLastEvent: 1
    m_ExpandedInstanceIDs: 
    m_RenameOverlay:
      m_UserAcceptedRename: 0
      m_Name: 
      m_OriginalName: 
      m_EditFieldRect:
        serializedVersion: 2
        x: 0
        y: 0
        width: 0
        height: 0
      m_UserData: 0
      m_IsWaitingForDelay: 0
      m_IsRenaming: 0
      m_OriginalEventType: 11
      m_IsRenamingFilename: 1
      m_ClientGUIView: {fileID: 0}
    m_CreateAssetUtility:
      m_EndAction: {fileID: 0}
      m_InstanceID: 0
      m_Path: 
      m_Icon: {fileID: 0}
      m_ResourceFile: 
    m_NewAssetIndexInList: -1
    m_ScrollPosition: {x: 0, y: 14.25}
    m_GridSize: 64
  m_SkipHiddenPackages: 0
  m_DirectoriesAreaWidth: 163
--- !u!114 &24
MonoBehaviour:
  m_ObjectHideFlags: 52
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 1
  m_Script: {fileID: 12019, guid: 0000000000000000e000000000000000, type: 0}
  m_Name: 
  m_EditorClassIdentifier: 
  m_MinSize: {x: 275, y: 50}
  m_MaxSize: {x: 4000, y: 4000}
  m_TitleContent:
    m_Text: Inspector
    m_Image: {fileID: -440750813802333266, guid: 0000000000000000d000000000000000, type: 0}
    m_Tooltip: 
  m_Pos:
    serializedVersion: 2
    x: 1076.5
    y: 83
    width: 339.5
    height: 366.5
  m_SerializedDataModeController:
    m_DataMode: 0
    m_PreferredDataMode: 0
    m_SupportedDataModes: 
    isAutomatic: 1
  m_ViewDataDictionary: {fileID: 0}
  m_OverlayCanvas:
    m_LastAppliedPresetName: Default
    m_SaveData: []
    m_OverlaysVisible: 1
  m_ObjectsLockedBeforeSerialization: []
  m_InstanceIDsLockedBeforeSerialization: 
  m_PreviewResizer:
    m_CachedPref: -160
    m_ControlHash: -371814159
    m_PrefName: Preview_InspectorPreview
  m_LastInspectedObjectInstanceID: -1
  m_LastVerticalScrollValue: 0
  m_GlobalObjectId: 
  m_InspectorMode: 0
  m_LockTracker:
    m_IsLocked: 0
  m_PreviewWindow: {fileID: 0}

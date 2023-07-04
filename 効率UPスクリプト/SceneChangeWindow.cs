using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

//シーン切り替えEditor拡張
public class SceneChangeWindow : EditorWindow
{
    private string[] _sceneNameArray;       //ファイル名保存用
    [SerializeField]
    private string _createSceneName="NewScene";        //作成するシーンの名前用
    [SerializeField]
    private /*static*/ string _folderPath = "Assets/";      //シーンファイルの検索用パス
    [SerializeField]
    private /*static*/ string _folderPathCreateScene = "Assets/";      //作成したシーンの保存先用パス

    private Vector2 _scrollPos = Vector2.zero;      //スクロールバー用

    //シーン変更ウィンドウを開く
    [MenuItem("MyTool/SceneChangeWindow")]
    static void WindowOpen()
    {
        GetWindow<SceneChangeWindow>();
    }

    //ウィンドウの見た目を描画
    private void OnGUI()
    {
		DisplayPathField();
		EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        if(GUILayout.Button("フォルダから検索"))
        {
            _folderPath = DisplaySearch_PtahFolderPanel();
        }

        EditorGUILayout.EndHorizontal();
		LoadSceneFile();
        DisplaySceneButtons();

        DisplayCreateSaveScenePathField();

		EditorGUILayout.BeginHorizontal();

		GUILayout.FlexibleSpace();
		if(GUILayout.Button("フォルダから検索"))
		{
            _folderPathCreateScene=DisplaySearch_PtahFolderPanel();
		}
		EditorGUILayout.EndHorizontal();

        DisplayCreateSceneName();
        if(GUILayout.Button("+ Create Scene"))
        {
            CreateScene(_createSceneName, EditorApplication.NewScene);
        }
    }

    /// <summary>
    /// パスを入力するフィールドを表示
    /// </summary>
    private void DisplayPathField()
    {
        _folderPath = EditorGUILayout.TextField("ファイルパス", _folderPath);
    }

    /// <summary>
    ///ファイルパス検索
    /// </summary>
    private string DisplaySearch_PtahFolderPanel()
    {
        string selecteFullPath = EditorUtility.SaveFolderPanel("シーンフォルダを探す", Application.dataPath, "Assets");
        selecteFullPath = GetFullPathToAssetsPath(selecteFullPath);
        if(string.IsNullOrEmpty(selecteFullPath))
        {
            selecteFullPath = "Assets";
        }
        selecteFullPath += "/";
        return selecteFullPath;
    }

    /// <summary>
    /// シーンの保存先のパスを入力するフィールドを表示
    /// </summary>
    private void DisplayCreateSaveScenePathField()
    {
        _folderPathCreateScene = EditorGUILayout.TextField("シーンの保存先", _folderPathCreateScene);
    }

    /// <summary>
    /// 作成するシーンの名前を入力するフィールドを表示
    /// </summary>
    private void DisplayCreateSceneName()
	{
        _createSceneName = EditorGUILayout.TextField("作成するシーンの名前",_createSceneName);
	}

    /// <summary>
    /// シーンファイルのファイル名を取得
    /// </summary>
    private void LoadSceneFile()
    {
        //フォルダ内の全てのシーンファイルのパスを取得する
        _sceneNameArray = Directory.GetFiles(_folderPath, "*", SearchOption.TopDirectoryOnly).Where(s => !s.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)).ToArray();
        //シーンファイルパスからファイル名のみを取得する
        for(int i = 0; i < _sceneNameArray.Length; i++)
        {
            _sceneNameArray[i] = Path.GetFileName(_sceneNameArray[i]);
        }
    }

    /// <summary>
    /// 絶対パスを相対パスにする
    /// </summary>
    /// <param name="fullPath">絶対パス</param>
    /// <returns>相対パスを出力(string)</returns>
    private string GetFullPathToAssetsPath(string fullPath)
	{
        int startIndex = fullPath.IndexOf("Assets/", StringComparison.Ordinal);
        if(startIndex==-1)
		{
            startIndex = fullPath.IndexOf("Assets\\", StringComparison.Ordinal);
		}
        if(startIndex == -1)
            return "";

        string assetPath = fullPath.Substring(startIndex);
        return assetPath;
	}

    /// <summary>
    /// シーン変更ボタンを表示
    /// </summary>
    private void DisplaySceneButtons()
    {
        //スクロールバーを描画
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        //シーンのファイルの数ボタンを描画
        for(int i = 0; i < _sceneNameArray.Length; i++)
        {
			EditorGUILayout.BeginHorizontal();
			//ボタンを描画
			if(GUILayout.Button(_sceneNameArray[i],GUILayout.MaxWidth(500)))
            {
                OpenScene(_sceneNameArray[i]);
            }

			GUILayout.FlexibleSpace();

			if(GUILayout.Button("X", GUILayout.Width(20)))
			{
                //警告メッセージを出したい
				DeleteScene(_sceneNameArray[i]);
			}
			EditorGUILayout.EndHorizontal();
		}
        //ここまでスクロールバーを描画
        EditorGUILayout.EndScrollView();

  //      DisplayCreateScenePathField();
  //      DisplayCreateSceneName();
  //      if(GUILayout.Button("+ Create Scene"))
  //      {
		//	CreateScene(_createSceneName, EditorApplication.NewScene);
		//}
    }

    /// <summary>
    /// シーンを開く
    /// </summary>
    /// <param name="sceneName"></param>
    private void OpenScene(string sceneName)
    {
        //シーンに変更があった場合に保存をするのかを尋ねる
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(_folderPath + sceneName);
        }
    }

    private /*static*/ void CreateScene(string filenameWithoutExtension,Action newSceneCallback)
    {
        //シーンに変更があった場合に保存をするのかを尋ねる
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
            var filename = filenameWithoutExtension + ".unity";
            var path = GetPath() + "/" + filename;
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            newSceneCallback();

            EditorApplication.SaveScene(path);

		    //シーンをつくると同時にBuild Settingsに登録する
		    //var scenes = EditorBuildSettings.scenes;
		    //ArrayUtility.Add(ref scenes,new EditorBuildSettingsScene(path, true));
		    //EditorBuildSettings.scenes = scenes;
		}
	}

	private /*static*/ void DeleteScene(string sceneName)
	{
        if(EditorUtility.DisplayDialog("Delete selected asset?","けしちゃうの？","Delete","Cancel"))
		{
            var path = _folderPath + sceneName;
            AssetDatabase.DeleteAsset(path);
		}
	}

	private /*static*/ string GetPath()
    {
        //var instanceId = Selection.activeInstanceID;
        //var path = AssetDatabase.GetAssetPath(instanceId);
        //path = string.IsNullOrEmpty(path) ? path : _folderPathCreateScene;
        //path = _folderPathCreateScene;
        var path = _folderPathCreateScene;

        if(Directory.Exists(path))
        {
            return path;
        }
        if(File.Exists(path))
        {
            var parent = Directory.GetParent(path);
            var fullName = parent.FullName;
            var unixFileName = fullName.Replace("\\", "/");
            return FileUtil.GetProjectRelativePath(unixFileName);
        }
        return string.Empty;
    }
}
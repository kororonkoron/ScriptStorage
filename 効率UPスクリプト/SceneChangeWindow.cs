using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

//�V�[���؂�ւ�Editor�g��
public class SceneChangeWindow : EditorWindow
{
    private string[] _sceneNameArray;       //�t�@�C�����ۑ��p
    [SerializeField]
    private string _createSceneName="NewScene";        //�쐬����V�[���̖��O�p
    [SerializeField]
    private /*static*/ string _folderPath = "Assets/";      //�V�[���t�@�C���̌����p�p�X
    [SerializeField]
    private /*static*/ string _folderPathCreateScene = "Assets/";      //�쐬�����V�[���̕ۑ���p�p�X

    private Vector2 _scrollPos = Vector2.zero;      //�X�N���[���o�[�p

    //�V�[���ύX�E�B���h�E���J��
    [MenuItem("MyTool/SceneChangeWindow")]
    static void WindowOpen()
    {
        GetWindow<SceneChangeWindow>();
    }

    //�E�B���h�E�̌����ڂ�`��
    private void OnGUI()
    {
		DisplayPathField();
		EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        if(GUILayout.Button("�t�H���_���猟��"))
        {
            _folderPath = DisplaySearch_PtahFolderPanel();
        }

        EditorGUILayout.EndHorizontal();
		LoadSceneFile();
        DisplaySceneButtons();

        DisplayCreateSaveScenePathField();

		EditorGUILayout.BeginHorizontal();

		GUILayout.FlexibleSpace();
		if(GUILayout.Button("�t�H���_���猟��"))
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
    /// �p�X����͂���t�B�[���h��\��
    /// </summary>
    private void DisplayPathField()
    {
        _folderPath = EditorGUILayout.TextField("�t�@�C���p�X", _folderPath);
    }

    /// <summary>
    ///�t�@�C���p�X����
    /// </summary>
    private string DisplaySearch_PtahFolderPanel()
    {
        string selecteFullPath = EditorUtility.SaveFolderPanel("�V�[���t�H���_��T��", Application.dataPath, "Assets");
        selecteFullPath = GetFullPathToAssetsPath(selecteFullPath);
        if(string.IsNullOrEmpty(selecteFullPath))
        {
            selecteFullPath = "Assets";
        }
        selecteFullPath += "/";
        return selecteFullPath;
    }

    /// <summary>
    /// �V�[���̕ۑ���̃p�X����͂���t�B�[���h��\��
    /// </summary>
    private void DisplayCreateSaveScenePathField()
    {
        _folderPathCreateScene = EditorGUILayout.TextField("�V�[���̕ۑ���", _folderPathCreateScene);
    }

    /// <summary>
    /// �쐬����V�[���̖��O����͂���t�B�[���h��\��
    /// </summary>
    private void DisplayCreateSceneName()
	{
        _createSceneName = EditorGUILayout.TextField("�쐬����V�[���̖��O",_createSceneName);
	}

    /// <summary>
    /// �V�[���t�@�C���̃t�@�C�������擾
    /// </summary>
    private void LoadSceneFile()
    {
        //�t�H���_���̑S�ẴV�[���t�@�C���̃p�X���擾����
        _sceneNameArray = Directory.GetFiles(_folderPath, "*", SearchOption.TopDirectoryOnly).Where(s => !s.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)).ToArray();
        //�V�[���t�@�C���p�X����t�@�C�����݂̂��擾����
        for(int i = 0; i < _sceneNameArray.Length; i++)
        {
            _sceneNameArray[i] = Path.GetFileName(_sceneNameArray[i]);
        }
    }

    /// <summary>
    /// ��΃p�X�𑊑΃p�X�ɂ���
    /// </summary>
    /// <param name="fullPath">��΃p�X</param>
    /// <returns>���΃p�X���o��(string)</returns>
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
    /// �V�[���ύX�{�^����\��
    /// </summary>
    private void DisplaySceneButtons()
    {
        //�X�N���[���o�[��`��
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        //�V�[���̃t�@�C���̐��{�^����`��
        for(int i = 0; i < _sceneNameArray.Length; i++)
        {
			EditorGUILayout.BeginHorizontal();
			//�{�^����`��
			if(GUILayout.Button(_sceneNameArray[i],GUILayout.MaxWidth(500)))
            {
                OpenScene(_sceneNameArray[i]);
            }

			GUILayout.FlexibleSpace();

			if(GUILayout.Button("X", GUILayout.Width(20)))
			{
                //�x�����b�Z�[�W���o������
				DeleteScene(_sceneNameArray[i]);
			}
			EditorGUILayout.EndHorizontal();
		}
        //�����܂ŃX�N���[���o�[��`��
        EditorGUILayout.EndScrollView();

  //      DisplayCreateScenePathField();
  //      DisplayCreateSceneName();
  //      if(GUILayout.Button("+ Create Scene"))
  //      {
		//	CreateScene(_createSceneName, EditorApplication.NewScene);
		//}
    }

    /// <summary>
    /// �V�[�����J��
    /// </summary>
    /// <param name="sceneName"></param>
    private void OpenScene(string sceneName)
    {
        //�V�[���ɕύX���������ꍇ�ɕۑ�������̂���q�˂�
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(_folderPath + sceneName);
        }
    }

    private /*static*/ void CreateScene(string filenameWithoutExtension,Action newSceneCallback)
    {
        //�V�[���ɕύX���������ꍇ�ɕۑ�������̂���q�˂�
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
            var filename = filenameWithoutExtension + ".unity";
            var path = GetPath() + "/" + filename;
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            newSceneCallback();

            EditorApplication.SaveScene(path);

		    //�V�[��������Ɠ�����Build Settings�ɓo�^����
		    //var scenes = EditorBuildSettings.scenes;
		    //ArrayUtility.Add(ref scenes,new EditorBuildSettingsScene(path, true));
		    //EditorBuildSettings.scenes = scenes;
		}
	}

	private /*static*/ void DeleteScene(string sceneName)
	{
        if(EditorUtility.DisplayDialog("Delete selected asset?","�������Ⴄ�́H","Delete","Cancel"))
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
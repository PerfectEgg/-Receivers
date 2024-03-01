using UnityEditor;
using UnityEditor.SceneManagement;

public class LoadStartScene : EditorWindow
{
    [MenuItem("DearMyBrother/PlayStartScene %h")]

    public static void PlayStartScene()
    {
        if(EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        EditorSceneManager.OpenScene("Assets/Scenes/StartScene.unity");
        EditorApplication.isPlaying = true;
    }
}
